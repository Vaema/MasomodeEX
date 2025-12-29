using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using Luminance.Common.Utilities;
using MasomodeEX.Common.Utilities;
using MasomodeEX.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MasomodeEX.Common.Globals
{
    public class MasomodeEXGlobalProjectile : GlobalProjectile
    {
        public bool masobool;
        public int counter;

        public override bool InstancePerEntity => true;

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            switch (projectile.type)
            {
                case ProjectileID.Grenade:
                case ProjectileID.HappyBomb:
                case ProjectileID.BombSkeletronPrime:
                case ProjectileID.CannonballFriendly:
                case ProjectileID.CannonballHostile:
                case ProjectileID.StickyGrenade:
                case ProjectileID.BouncyGrenade:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int p = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ProjectileID.Bomb, 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                        if (p < Main.maxProjectiles)
                            Main.projectile[p].Kill();
                    }
                    break;
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            switch (projectile.type)
            {
                case ProjectileID.PinkLaser:
                case ProjectileID.DeathLaser:
                case ProjectileID.FlamesTrap:
                case ProjectileID.RocketSkeleton:
                case ProjectileID.VortexLightning:
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        var npc = Main.npc[i];
                        if (!npc.active || npc.type != NPCID.SkeletronPrime || npc.type != NPCID.TheDestroyer || npc.type != NPCID.Retinazer || npc.type != NPCID.Spazmatism)
                            continue;

                        target.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 180);
                    }
                    break;

                case ProjectileID.CursedFlameHostile:
                case ProjectileID.EyeFire:
                    target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 600), true, false);
                    target.AddBuff(BuffID.Weak, Main.rand.Next(7200), true, false);
                    target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), Main.rand.Next(60, 600));

                    goto case ProjectileID.PinkLaser;

                case ProjectileID.BombSkeletronPrime:
                    if (!target.HasBuff(ModContent.BuffType<FusedBuff>()))
                    {
                        target.AddBuff(ModContent.BuffType<FusedBuff>(), 600);
                    }
                    goto case ProjectileID.PinkLaser;

                case ProjectileID.GoldenShowerHostile:
                    target.AddBuff(BuffID.Ichor, Main.rand.Next(60, 600), true, false);
                    target.AddBuff(BuffID.Bleeding, Main.rand.Next(7200), true, false);
                    target.AddBuff(ModContent.BuffType<BloodthirstyBuff>(), Main.rand.Next(300));
                    break;

                case ProjectileID.StardustJellyfishSmall:
                case ProjectileID.NebulaLaser:
                    target.AddBuff(BuffID.VortexDebuff, Main.rand.Next(300), true, false);
                    break;

                case ProjectileID.PhantasmalBolt:
                    if (Main.rand.NextBool(10000))
                    {
                        target.immune = false;
                        target.immuneTime = 0;
                        target.Hurt(PlayerDeathReason.ByProjectile(target.whoAmI, projectile.whoAmI), 1000, -target.direction, false, false, -1, false, 0f, 0f, 4.5f);
                    }
                    break;

                case ProjectileID.PhantasmalDeathray:
                    target.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.MasomodeEX.PlayerDeathReasons.LordHitCap", target.name)), 19998.0, 0, false);
                    break;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<CosmosMoon>())
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
                    NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<CosmosChampion>());
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

        public override void AI(Projectile projectile)
        {
            if (projectile.Center.X > 0f && projectile.Center.X / 16f < Main.maxTilesX && projectile.Center.Y > 0f && projectile.Center.Y / 16f < Main.maxTilesY)
            {
                Tile tileSafely = Framing.GetTileSafely(projectile.Center);
                if (tileSafely.TileType == TileID.PlanteraBulb)
                    WorldGen.KillTile((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, false, false, false);
            }

            switch (projectile.type)
            {
                case ProjectileID.Bee:
                case ProjectileID.GiantBee:
                    {
                        if (!MasoModeUtils.AnyBossAlive())
                        {
                            break;
                        }
                        projectile.timeLeft = 0;
                        projectile.damage = 0;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            break;
                        }

                        int j = NPC.NewNPC(null, (int)projectile.Center.X, (int)projectile.Center.Y, Main.rand.NextBool(2) ? NPCID.Bee : NPCID.BeeSmall);
                        if (j < Main.maxNPCs)
                        {
                            Main.npc[j].velocity = projectile.velocity;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
                        }
                        break;
                    }

                case ProjectileID.EyeFire:
                    projectile.position += projectile.velocity / 2f;
                    break;

                case ProjectileID.Sharknado:
                case ProjectileID.Cthulunado:
                    if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && projectile.Distance(Main.LocalPlayer.Center) < 1000f)
                    {
                        Player obj = Main.LocalPlayer;
                        obj.position += Main.LocalPlayer.DirectionTo(projectile.Center) * 0.1f;
                    }
                    break;

                case ProjectileID.SandnadoHostile:
                    if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && projectile.Distance(Main.LocalPlayer.Center) < 1000f)
                    {
                        Player obj2 = Main.LocalPlayer;
                        obj2.position += Main.LocalPlayer.DirectionTo(projectile.Center) * 2f;
                    }
                    break;

                case ProjectileID.FallingStar:
                    projectile.hostile = true;
                    break;

                case ProjectileID.PhantasmalSphere:
                    if (projectile.velocity.Length() == 12f)
                    {
                        projectile.Kill();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(null, projectile.Center, projectile.velocity.RotatedBy(Math.PI * 2.0 / 15.0 * i, default), ModContent.ProjectileType<EyePhantom>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f, 0f);
                        }
                    }
                    if (++counter > 90)
                    {
                        counter = 0;
                        int p = Player.FindClosest(projectile.Center, 0, 0);
                        if (p != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(null, projectile.Center, projectile.DirectionTo(Main.player[p].Center) * 8f, ProjectileID.PhantasmalBolt, 30, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }
                    break;

                case ProjectileID.PhantasmalBolt:
                    if (projectile.timeLeft > 720)
                        projectile.timeLeft = 720;
                    break;

                case ProjectileID.PhantasmalDeathray:
                    if (!masobool)
                    {
                        masobool = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rotationDirection = (float)Math.PI / 160f;
                            Projectile.NewProjectile(null, projectile.Center, projectile.velocity, ModContent.ProjectileType<PhantasmalDeathrayML>(), 4999, 0f, Main.myPlayer, rotationDirection, projectile.ai[1], 0f);
                            Projectile.NewProjectile(null, projectile.Center, projectile.velocity, ModContent.ProjectileType<PhantasmalDeathrayML>(), 4999, 0f, Main.myPlayer, 0f - rotationDirection, projectile.ai[1], 0f);
                        }
                    }
                    break;

                case ProjectileID.MoonLeech:
                    if (!Main.player.IndexInRange((int)projectile.ai[1]))
                        return;

                    if (!Main.npc.IndexInRange((int)projectile.ai[0]))
                        return;

                    var target = Main.player[(int)projectile.ai[1]];
                    var npc = Main.npc[(int)projectile.ai[0]];
                    if (npc.type != NPCID.MoonLordHead)
                    {
                        npc = Main.npc.First(n => n.type == NPCID.MoonLordHead && n.active);
                    }

                    Vector2 offset = new(0, 216); // mouth pos
                    target.Center += target.DirectionTo(npc.Center + offset) * 5;
                    break;
            }
        }
    }
}