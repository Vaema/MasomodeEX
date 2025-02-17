using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Utils;

namespace MasomodeEX.Content.Projectiles;

public class PhantasmalDeathrayIce : ModProjectile
{
    public TileActionAttempt CutTilesAttempt;

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = true;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        Vector2? vector = null;

        if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            Projectile.velocity = -Vector2.UnitY;

        if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == NPCID.IceGolem)
        {
            Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
            
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Projectile.localAI[0] == 0f)
                SoundEngine.PlaySound(SoundID.Zombie104);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 240f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * (float)Math.PI / 240f) * 10f * 1f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            float rot = Projectile.velocity.ToRotation();
            rot += Projectile.ai[0];
            Projectile.rotation = rot - (float)Math.PI / 2f;
            Projectile.velocity = rot.ToRotationVector2();

            Vector2 samplingPoint = Projectile.Center;
            if (vector.HasValue)
                samplingPoint = vector.Value;

            float[] array = new float[(int)3f];
            Collision.LaserScan(samplingPoint, Projectile.velocity, Projectile.width * Projectile.scale, 3000f, array);
            float num = 0f;
            for (int i = 0; i < array.Length; i++)
                num += array[i];
            num /= 3f;

            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num, amount);
            Vector2 currentPos = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int j = 0; j < 2; j++)
            {
                float num2 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2)? -1f : 1f) * ((float)Math.PI / 2f);
                float num3 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector2 = new((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
                int dust = Dust.NewDust(currentPos, 0, 0, DustID.CopperCoin, vector2.X, vector2.Y, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 value = Projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int dust = Dust.NewDust(currentPos + value - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity.Y = 0f - Math.Abs(Main.dust[dust].velocity.Y);
            }
        }
        else
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.velocity == Vector2.Zero)
            return false;

        Texture2D head = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D body = ModContent.Request<Texture2D>("Projectiles/PhantasmalDeathrayIce2").Value;
        Texture2D tail = ModContent.Request<Texture2D>("Projectiles/PhantasmalDeathrayIce3").Value;

        float ai = Projectile.localAI[1];
        Color color = new Color(255, 255, 255, 0) * 0.9f;

        Projectile.Center += Projectile.velocity * Projectile.scale * head.Height / 2f;

        Vector2 pos = Projectile.Center - Main.screenPosition;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(head, pos, null, color, Projectile.rotation, head.Size() / 2f, Projectile.scale, 0, 0f);
        
        ai -= (head.Height / 2 + tail.Height) * Projectile.scale;
        if (ai > 0f)
        {
            float num = 0f;
            Rectangle rectangle = new(0, 16 * (Projectile.timeLeft / 3 % 5), tail.Width, 16);
            while (num + 1f < ai)
            {
                if (ai - num < rectangle.Height)
                    rectangle.Height = (int)(num - num);

                spriteBatch.Draw(body, Projectile.Center - Main.screenPosition, rectangle, color, Projectile.rotation, new Vector2(rectangle.Width / 2, 0f), Projectile.scale, 0, 0f);
                
                num += rectangle.Height * Projectile.scale;
                Projectile.Center += Projectile.velocity * rectangle.Height * Projectile.scale;

                rectangle.Y += 16;
                if (rectangle.Y + rectangle.Height > body.Height)
                    rectangle.Y = 0;
            }
        }

        spriteBatch.Draw(tail, pos, null, color, Projectile.rotation, tail.Frame().Top(), Projectile.scale, 0, 0f);
        
        return false;
    }

    public override void CutTiles()
    {
        DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
        if (CutTilesAttempt == null)
        {
            TileActionAttempt attempt = DelegateMethods.CutTiles;
            CutTilesAttempt = attempt;
        }
        PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, CutTilesAttempt);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (projHitbox.Intersects(targetHitbox))
            return true;

        float num = 0f;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num))
            return true;
        
        return false;
    }

    public virtual void OnHitPlayer(Player target, int damage, bool crit)
    {
        target.AddBuff(46, Main.rand.Next(3600));
        target.AddBuff(44, Main.rand.Next(300, 600));
        target.AddBuff(47, Main.rand.Next(180));
    }
}
