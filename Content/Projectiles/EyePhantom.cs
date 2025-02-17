using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace MasomodeEX.Content.Projectiles
{
    public class EyePhantom : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 2;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 720;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 200;
            CooldownSlot = 1;
        }

        public override bool CanHitPlayer(Player target) => target.hurtCooldowns[1] == 0;

        public override void AI()
        {
            if ((Projectile.ai[1] += 1f) > 30f)
            {
                if (Projectile.localAI[0] == 0f || Projectile.ai[0] < 0f)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.localAI[1] = Main.rand.Next(60);
                    Projectile.ai[0] = Player.FindClosest(Projectile.Center, 0, 0);
                }
                else
                {
                    int foundTarget = (int)Projectile.ai[0];
                    Player p = Main.player[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(p.Center) * 8f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.05f);
                }
            }

            if ((Projectile.localAI[1] += 1f) > 360f)
            {
                Projectile.localAI[1] = Main.rand.Next(60);
                int p2 = Player.FindClosest(Projectile.Center, 0, 0);
                if (p2 != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(null, Projectile.Center, Projectile.DirectionTo(Main.player[p2].Center) * 8f, 462, 30, 0f, Main.myPlayer, 0f, 0f, 0f);
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            Projectile.scale = 1f - Projectile.alpha / 255f;

            for (int i = 0; i < 2; i++)
            {
                float num = Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 vector2 = new Vector2(-Projectile.width * 0.65f * Projectile.scale, 0f).RotatedBy((double)num * MathHelper.TwoPi, default).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Vortex, (0f - Projectile.velocity.X) / 3f, (0f - Projectile.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2;
                Main.dust[index2].noGravity = true;
            }

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeleft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 208;
            Projectile.Center = Projectile.position;

            for (int index1 = 0; index1 < 3; index1++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].position = new Vector2(Projectile.width / 2, 0f).RotatedBy(MathHelper.TwoPi * Main.rand.NextDouble(), default) * (float)Main.rand.NextDouble() + Projectile.Center;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, default, 2.5f);
                Main.dust[dust2].position = new Vector2(Projectile.width / 2, 0f).RotatedBy(MathHelper.TwoPi * Main.rand.NextDouble(), default) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[dust2].noGravity = true;
                Main.dust[dust2].velocity *= 1f;

                int dust3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust3].position = new Vector2(Projectile.width / 2, 0f).RotatedBy(MathHelper.TwoPi * Main.rand.NextDouble(), default) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[dust3].velocity *= 1f;
                Main.dust[dust3].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            int num = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y = num * Projectile.frame;
            Rectangle rect = new(0, y, tex.Width, num);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size() / 2f, Projectile.scale, 0, 0f);
            return false;
        }
    }
}