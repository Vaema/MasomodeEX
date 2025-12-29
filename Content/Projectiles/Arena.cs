using System;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Boss;

namespace MasomodeEX.Content.Projectiles
{
    public class Arena : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 2;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            int ai1 = (int)Projectile.ai[1];
            if (Projectile.ai[1] >= 0f && Projectile.ai[1] < 200f && Main.npc[ai1].active && (Main.npc[ai1].boss || Main.npc[ai1].type == NPCID.EaterofWorldsHead))
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Main.npc[ai1].Center;

                Player player = Main.player[Main.myPlayer];
                if (player.active && !player.dead)
                {
                    float distance = player.Distance(Projectile.Center);
                    if (Math.Abs(distance - 1200f) < 46f && player.hurtCooldowns[0] == 0 && Projectile.alpha == 0 && player.whoAmI == Main.npc[ai1].target)
                    {
                        int hitDirection = Projectile.Center.X > player.Center.X ? 1 : -1;
                        player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), Projectile.damage, hitDirection, false, false, 0, false, 0f, 0f, 4.5f);
                        player.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 200;

                        player.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 5400);
                        player.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);
                        player.AddBuff(ModContent.BuffType<MutantFangBuff>(), 300);
                    }

                    if (distance > 1200f && distance < 6000f)
                    {
                        if (distance > 2400f)
                        {
                            player.frozen = true;
                            player.controlHook = false;
                            player.controlUseItem = false;
                            player.velocity.X = 0f;
                            player.velocity.Y = -0.4f;
                            if (player.mount.Active)
                                player.mount.Dismount(player);
                        }

                        Vector2 movement = Projectile.Center - player.Center;
                        float difference = movement.Length() - 1200f;
                        movement.Normalize();
                        movement *= difference < 17f ? difference : 17f;
                        player.position += movement;

                        for (int i = 0; i < 20; i++)
                        {
                            int d = Dust.NewDust(player.position, player.width, player.height, DustID.IceTorch, 0f, 0f, 0, default, 2.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Dust obj = Main.dust[d];
                            obj.velocity *= 5f;
                        }
                    }
                }

                Projectile.timeLeft = 2;
                Projectile.scale = (1f - Projectile.alpha / 255f) * 2f;
                Projectile.ai[0] += (float)Math.PI / 140f;
                if (Projectile.ai[0] > (float)Math.PI)
                {
                    Projectile.ai[0] -= (float)Math.PI * 2f;
                    Projectile.netUpdate = true;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 1)
                        Projectile.frame = 0;
                }
            }
            else
                Projectile.Kill();
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            int frame = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y = frame * Projectile.frame;
            Rectangle rect = new(0, y, tex.Width, frame);
            Vector2 origin = rect.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);

            for (int x = 0; x < 32; x++)
            {
                Vector2 drawOffset = new Vector2(1200f * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0], default);
                drawOffset = drawOffset.RotatedBy((double)((float)Math.PI / 16f * x), default);

                for (int i = 0; i < 4; i++)
                {
                    color *= (4 - i) / 4f;
                    Vector2 value = Projectile.Center + drawOffset.RotatedBy((double)((float)Math.PI / 140f * -i), default);
                    Main.spriteBatch.Draw(tex, value - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, color, Projectile.rotation, origin, Projectile.scale, 0, 0f);
                }

                Main.spriteBatch.Draw(tex, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, color, Projectile.rotation, origin, Projectile.scale, 0, 0f);
            }

            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}