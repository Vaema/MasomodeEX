using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using MasomodeEX.Common.Utilities;

namespace MasomodeEX.Common.Globals
{
    public class MasomodeEXGlobalProjectile : GlobalProjectile
    {
        public bool masobool;
        public int counter;

        public override bool InstancePerEntity => true;

        public override void AI(Projectile projectile)
        {
            if (projectile.Center.X > 0f && projectile.Center.X / 16f < Main.maxTilesX && projectile.Center.Y > 0f && projectile.Center.Y / 16f < Main.maxTilesY)
            {
                Tile tileSafely = Framing.GetTileSafely(projectile.Center);
                if (tileSafely.TileType == 238)
                    WorldGen.KillTile((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, false, false, false);
            }

            switch (projectile.type)
            {
                case 181:
                case 566:
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
                        int j = NPC.NewNPC(null, (int)projectile.Center.X, (int)projectile.Center.Y, Main.rand.NextBool(2) ? 210 : 211, 0, 0f, 0f, 0f, 0f, 255);
                        if (j < 200)
                        {
                            Main.npc[j].velocity = projectile.velocity;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
                        }
                        break;
                    }

                case 101:
                    projectile.position += projectile.velocity / 2f;
                    break;

                case 456:
                    if (projectile.Center == Main.player[(int)projectile.ai[1]].Center && Main.player[(int)projectile.ai[1]].active && !Main.player[(int)projectile.ai[1]].dead)
                    {
                        Player obj3 = Main.player[(int)projectile.ai[1]];
                        obj3.position += Main.player[(int)projectile.ai[1]].DirectionTo(Main.npc[(int)projectile.ai[0]].Center) * 5f;
                    }
                    projectile.position += projectile.velocity * 0.5f;
                    break;

                case 384:
                case 386:
                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && projectile.Distance(Main.player[Main.myPlayer].Center) < 1000f)
                    {
                        Player obj = Main.player[Main.myPlayer];
                        obj.position += Main.player[Main.myPlayer].DirectionTo(projectile.Center) * 0.1f;
                    }
                    break;

                case 657:
                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && projectile.Distance(Main.player[Main.myPlayer].Center) < 1000f)
                    {
                        Player obj2 = Main.player[Main.myPlayer];
                        obj2.position += Main.player[Main.myPlayer].DirectionTo(projectile.Center) * 2f;
                    }
                    break;

                case 12:
                    projectile.hostile = true;
                    break;

                case 454:
                    if (projectile.velocity.Length() == 12f)
                    {
                        projectile.Kill();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.NewProjectile(null, projectile.Center, projectile.velocity.RotatedBy(Math.PI * 2.0 / 15.0 * i, default), Mod.Find<ModProjectile>("EyePhantom").Type, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f, 0f);
                        }
                    }
                    if (++counter > 90)
                    {
                        counter = 0;
                        int p = Player.FindClosest(projectile.Center, 0, 0);
                        if (p != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(null, projectile.Center, projectile.DirectionTo(Main.player[p].Center) * 8f, 462, 30, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }
                    break;

                case 462:
                    if (projectile.timeLeft > 720)
                        projectile.timeLeft = 720;
                    break;

                case 455:
                    if (!masobool)
                    {
                        masobool = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rotationDirection = (float)Math.PI / 160f;
                            Projectile.NewProjectile(null, projectile.Center, projectile.velocity, MasomodeEX.Souls.Find<ModProjectile>("PhantasmalDeathrayML").Type, 4999, 0f, Main.myPlayer, rotationDirection, projectile.ai[1], 0f);
                            Projectile.NewProjectile(null, projectile.Center, projectile.velocity, MasomodeEX.Souls.Find<ModProjectile>("PhantasmalDeathrayML").Type, 4999, 0f, Main.myPlayer, 0f - rotationDirection, projectile.ai[1], 0f);
                        }
                    }
                    break;
            }
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            switch (projectile.type)
            {
                case 30:
                case 75:
                case 102:
                case 162:
                case 240:
                case 397:
                case 517:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int p = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, 28, 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                        if (p < 1000)
                            Main.projectile[p].Kill();
                    }
                    break;
            }
        }

        public virtual void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            switch (projectile.type)
            {
                case 84:
                case 100:
                case 188:
                case 303:
                case 580:
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].active && (Main.npc[i].type == NPCID.SkeletronPrime || Main.npc[i].type == NPCID.TheDestroyer || Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism))
                        {
                            target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWings").Type, 2, true, false);
                            break;
                        }
                    }
                    break;

                case 96:
                case 101:
                    target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                    target.AddBuff(33, Main.rand.Next(7200), true, false);
                    target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("Shadowflame").Type, Main.rand.Next(60, 600));
                    goto case 84;

                case 102:
                    if (!target.HasBuff(MasomodeEX.Souls.Find<ModBuff>("Fused").Type))
                        target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("Fused").Type, 600);
                    goto case 84;

                case 288:
                    target.AddBuff(69, Main.rand.Next(60, 600), true, false);
                    target.AddBuff(30, Main.rand.Next(7200), true, false);
                    target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("Bloodthirsty").Type, Main.rand.Next(300));
                    break;

                case 539:
                case 576:
                    target.AddBuff(164, Main.rand.Next(300), true, false);
                    break;

                case 462:
                    if (Main.rand.NextBool(10000))
                    {
                        target.immune = false;
                        target.immuneTime = 0;
                        target.Hurt(PlayerDeathReason.ByProjectile(target.whoAmI, projectile.whoAmI), 1000, -target.direction, false, false, -1, false, 0f, 0f, 4.5f);
                    }
                    break;

                case 455:
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " got terminated."), 19998.0, 0, false);
                    break;
            }
        }
    }
}