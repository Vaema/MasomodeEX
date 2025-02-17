using System;
using MasomodeEX.Common.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasomodeEX.Content.Projectiles
{
    public class EridanusMoon : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type == MasomodeEX.Souls.Find<ModProjectile>("CosmosMoon").Type)
            {
                if (projectile.localAI[0] == 0f)
                {
                    projectile.localAI[0] = 1f;
                    SoundEngine.PlaySound(SoundID.Item92, null, null);
                    projectile.rotation = projectile.ai[0];
                }

                if (projectile.localAI[0] == 2f)
                {
                    projectile.damage = 0;
                    projectile.velocity.Y += 0.2f;
                    projectile.tileCollide = true;
                }
                else
                {
                    NPC npc = MasoModeUtils.NPCExists(projectile.ai[1], MasomodeEX.Souls.Find<ModNPC>("CosmosChampion").Type);
                    if (npc != null)
                    {
                        projectile.timeLeft = 600;
                        float offset = Math.Abs(850f * (float)Math.Sin(npc.ai[2] * 2f * (float)Math.PI / 200f));
                        offset += 150f;
                        projectile.ai[0] += 0.02f;
                        projectile.Center = npc.Center + offset * projectile.ai[0].ToRotationVector2();
                    }
                    else
                    {
                        projectile.localAI[0] = 2f;
                        projectile.velocity = projectile.position - projectile.oldPos[1];
                    }
                }

                projectile.rotation += 0.04f;

                return false;
            }

            return true;
        }
    }
}