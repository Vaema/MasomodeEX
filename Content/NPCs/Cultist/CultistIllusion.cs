using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MasomodeEX.Content.NPCs.Cultist
{
    public class CultistIllusion : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_439";

        public override void SetStaticDefaults() => Main.npcFrameCount[Type] = Main.npcFrameCount[439];

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 50;
            NPC.damage = 50;
            NPC.defense = 42;
            NPC.lifeMax = 32000;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public virtual void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = 75;
            NPC.lifeMax = (int)(40000f * bossLifeScale);
        }

        public override void AI()
        {
            if (NPC.ai[0] < 0f || NPC.ai[0] >= 200f)
            {
                NPC.StrikeInstantKill();
                NPC.active = false;
                return;
            }

            NPC cultist = Main.npc[(int)NPC.ai[0]];
            if (!cultist.active || cultist.type != NPCID.CultistBoss)
            {
                NPC.StrikeInstantKill();
                NPC.active = false;

                for (int j = 0; j < 40; j++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 0, default, 1f);
                    Dust obj = Main.dust[d];
                    obj.velocity *= 2.5f;
                    Dust obj2 = Main.dust[d];
                    obj2.scale += 0.5f;
                }

                for (int i = 0; i < 20; i++)
                {
                    int d2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 2f);
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].noLight = true;
                    Dust obj3 = Main.dust[d2];
                    obj3.velocity *= 9f;
                }

                return;
            }

            NPC.target = cultist.target;
            NPC.damage = cultist.damage;
            NPC.defDamage = cultist.damage;
            NPC.alpha = cultist.alpha;

            if (NPC.HasPlayerTarget)
            {
                Vector2 dist = Main.player[NPC.target].Center - cultist.Center;
                NPC.Center = Main.player[NPC.target].Center;
                NPC.position.X += dist.X * NPC.ai[1];
                NPC.position.Y += dist.Y * NPC.ai[2];
                NPC.direction = NPC.spriteDirection = NPC.position.X < Main.player[NPC.target].position.X ? 1 : -1;

                if (cultist.ai[3] == -1f)
                    return;

                switch ((int)cultist.ai[0])
                {
                    case 2:
                        if (cultist.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            dist.Normalize();
                            dist = dist.RotatedByRandom(Math.PI / 12.0);
                            dist *= Main.rand.NextFloat(6f, 9f);
                            Projectile.NewProjectile(null, NPC.Center, dist, 348, NPC.damage / 3, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                        break;
                    case 3:
                        if (cultist.ai[1] != 3f || Main.netMode == NetmodeID.MultiplayerClient)
                            break;

                        int k = NPC.NewNPC(null, (int)NPC.Center.X, (int)NPC.Center.Y, 519, 0, 0f, 0f, 0f, 0f, 255);
                        if (k < 200)
                        {
                            Main.npc[k].velocity.X = Main.rand.Next(-10, 11);
                            Main.npc[k].velocity.Y = Main.rand.Next(-15, -4);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, k);
                        }
                        break;
                    case 4:
                        if (cultist.ai[1] == 19f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 dir = Main.player[NPC.target].Center - NPC.Center;
                            float ai1New = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4.0)) * 7f;
                            Projectile.NewProjectile(null, NPC.Center, vel, 466, NPC.damage / 15 * 6, 0f, Main.myPlayer, dir.ToRotation(), ai1New, 0f);
                        }
                        break;
                    case 7:
                        if (cultist.ai[1] == 3f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 speed = Vector2.UnitX.RotatedByRandom(Math.PI);
                            speed *= 6f;
                            Projectile.NewProjectile(null, NPC.Center, speed, 452, NPC.damage / 3, 0f, Main.myPlayer, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, NPC.Center, speed.RotatedBy(Math.PI * 2.0 / 3.0, default), 452, NPC.damage / 3, 0f, Main.myPlayer, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, NPC.Center, speed.RotatedBy(Math.PI * -2.0 / 3.0, default), 452, NPC.damage / 3, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                        break;
                    case 8:
                        if (cultist.ai[1] == 3f && NPC.active && NPC.type == NPCID.CultistBossClone)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, 575, NPC.damage / 15 * 6, 0f, Main.myPlayer, 0f, 0f, 0f);
                        break;
                    case 5:
                    case 6:
                        break;
                }
            }
            else
                NPC.Center = cultist.Center;
        }

        public override bool CheckActive() => false;

        public override bool PreKill() => false;

        public override void FindFrame(int frameHeight)
        {
            NPC cultist = Main.npc[(int)NPC.ai[0]];
            if (cultist.active && cultist.type == NPCID.CultistBoss)
                NPC.frame.Y = cultist.frame.Y;
        }
    }
}