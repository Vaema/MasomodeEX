using System;
using System.Collections.Generic;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using MasomodeEX.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.NPC;
using static Terraria.Player;

namespace MasomodeEX.Common.Globals;

public class MasomodeEXGlobalNPC : GlobalNPC
{
    public bool SpawnedMinions1;
    public bool[] masoBool = new bool[4];
    public int[] Counter = new int[3];
    public bool FirstTick;

    public override bool InstancePerEntity => true;

    public override void SetDefaults(NPC npc)
    {
        if (MasoModeUtils.checkIfMasoEX())
        {
            if (npc.type == NPCID.MoonLordLeechBlob)
            {
                npc.damage = 50;
            }
            if (!npc.friendly)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.5);
                npc.defense = (int)(npc.defense * 1.5);
                npc.knockBackResist *= 0.5f;
            }
            npc.buffImmune[24] = true;
            npc.buffImmune[68] = true;
            npc.GetGlobalNPC<EModeGlobalNPC>().isWaterEnemy = true;
            if (npc.type == ModContent.NPCType<MutantBoss>())
            {
                npc.ModNPC.Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/MutantPhase1");
                npc.ModNPC.SceneEffectPriority = (SceneEffectPriority)8;
            }
        }
    }

    public override bool PreAI(NPC npc)
    {
        if (MasoModeUtils.checkIfMasoEX())
        {
            OldEModeGlobalNPC fargoNPCOld = npc.GetGlobalNPC<OldEModeGlobalNPC>();
            if (npc.townNPC && Main.bloodMoon && ++Counter[0] > 15)
            {
                Counter[0] = 0;
                int p = Player.FindClosest(npc.Center, 0, 0);
                if (p > -1 && Main.player[p].active && npc.Distance(Main.player[p].Center) < 500f)
                {
                    switch (npc.type)
                    {
                        case 20:
                            npc.Transform(196);
                            break;
                        case 229:
                            npc.Transform(216);
                            break;
                        case 441:
                            npc.Transform(534);
                            break;
                        case 228:
                            npc.Transform(198);
                            break;
                        case 54:
                            npc.Transform(35);
                            break;
                        default:
                            if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type)
                                npc.Transform(MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                            else if (npc.type != MasomodeEX.Fargo.Find<ModNPC>("Abominationn").Type && npc.type != MasomodeEX.Fargo.Find<ModNPC>("Deviantt").Type)
                                npc.Transform(104);
                            break;
                    }
                }
            }
            switch (npc.type)
            {
                case 243:
                    {
                        if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) > 1000f && npc.Distance(Main.player[npc.target].Center) < 3000f)
                        {
                            Player obj11 = Main.player[npc.target];
                            obj11.position = obj11.position + npc.DirectionFrom(Main.player[npc.target].Center) * 20f;
                        }
                        for (int num32 = 0; num32 < 20; num32++)
                        {
                            Vector2 offset5 = default;
                            double angle4 = Main.rand.NextDouble() * 2.0 * Math.PI;
                            offset5.X += (float)(Math.Sin(angle4) * 1000.0);
                            offset5.Y += (float)(Math.Cos(angle4) * 1000.0);
                            Dust dust4 = Main.dust[Dust.NewDust(npc.Center + offset5 - new Vector2(4f, 4f), 0, 0, DustID.Ice, 0f, 0f, 100, Color.White, 1f)];
                            dust4.velocity = npc.velocity;
                            if (Main.rand.NextBool(3))
                            {
                                dust4.velocity += Vector2.Normalize(offset5) * 5f;
                            }
                            dust4.noGravity = true;
                        }
                        if (npc.life > 0)
                        {
                            int cap = npc.lifeMax / npc.life;
                            Counter[0] += Main.rand.Next(2 + cap) + 1;
                            if (Counter[0] >= Main.rand.Next(1400, 26000))
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                {
                                    Vector2 vector2 = new(npc.position.X + npc.width * 0.5f, npc.position.Y + 20f);
                                    vector2.X += 10 * npc.direction;
                                    float num33 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector2.X;
                                    float num34 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector2.Y;
                                    float num35 = num33 + Main.rand.Next(-40, 41);
                                    float num36 = num34 + Main.rand.Next(-40, 41);
                                    double num37 = (float)Math.Sqrt((double)num35 * (double)num35 + (double)num36 * (double)num36);
                                    float num38 = (float)(15.0 / num37);
                                    float SpeedX = num35 * num38;
                                    float SpeedY = num36 * num38;
                                    int Damage = 32;
                                    int Type = 257;
                                    vector2.X += SpeedX * 3f;
                                    vector2.Y += SpeedY * 3f;
                                    Projectile.NewProjectile(null, vector2.X, vector2.Y, SpeedX, SpeedY, Type, Damage, 0f, Main.myPlayer, 0f, 0f, 0f);
                                }
                            }
                        }
                        if (++Counter[1] > 240)
                        {
                            Counter[1] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(null, npc.Center, Vector2.UnitY, Mod.Find<ModProjectile>("PhantasmalDeathrayIce").Type, npc.damage / 4, 0f, Main.myPlayer, (float)Math.PI / 120f, npc.whoAmI, 0f);
                            }
                        }
                        break;
                    }
                case 181:
                    Aura(npc, 600f, 80, reverse: true, 199);
                    Aura(npc, 600f, 22, reverse: true, 199);
                    break;
                case 548:
                    if (npc.lifeMax > 100)
                    {
                        npc.lifeMax = 100;
                    }
                    if (npc.life > npc.lifeMax)
                    {
                        npc.life = npc.lifeMax;
                    }
                    break;
                case 576:
                case 577:
                    Aura(npc, 500f, 197, reverse: true, 188);
                    break;
                case 564:
                    Aura(npc, 900f, MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type, reverse: true, 254);
                    break;
                case 565:
                    Aura(npc, 600f, MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type, reverse: true, 254);
                    break;
                case 480:
                    Aura(npc, 400f, 156, reverse: false, 1);
                    break;
                case 48:
                    npc.noTileCollide = true;
                    break;
                case 422:
                case 493:
                case 507:
                case 517:
                    if (npc.position.Y < Main.maxTilesY * 16 / 2)
                        npc.position.Y += 4f / 15f;
                    break;
                case 63:
                case 64:
                case 103:
                case 242:
                case 256:
                    Aura(npc, 200f, 144, reverse: false, 226);
                    break;
                case 28:
                case 44:
                case 104:
                case 216:
                case 351:
                    npc.position.X += npc.velocity.X;
                    if (npc.velocity.Y < 0f)
                        npc.position.Y += npc.velocity.Y;
                    break;
                case 198:
                case 199:
                case 226:
                    if (!NPC.downedPlantBoss)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                        npc.Transform(MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                    }
                    break;
                case 327:
                    Aura(npc, 300f, MasomodeEX.Souls.Find<ModBuff>("MarkedforDeathBuff").Type);
                    break;
                case 328:
                    Aura(npc, 200f, MasomodeEX.Souls.Find<ModBuff>("LivingWastelandBuff").Type);
                    break;
                case 345:
                    Aura(npc, 300f, 47);
                    break;
                case 50:
                    {
                        Aura(npc, 600f, 137, reverse: true, 33);
                        for (int num = 0; num < 20; num++)
                        {
                            Vector2 offset = default;
                            double angle = Main.rand.NextDouble() * 2.0 * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 600.0);
                            offset.Y += (float)(Math.Cos(angle) * 600.0);
                            Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4f, 4f), 0, 0, DustID.Water, 0f, 0f, 0, Color.White, 2f)];
                            dust.velocity = npc.velocity;
                            if (Main.rand.NextBool(3))
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }
                        npc.position.X += npc.velocity.X;
                        if (masoBool[1])
                        {
                            if (npc.velocity.Y != 0f)
                                break;

                            masoBool[1] = false;
                            if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 dist = Main.player[npc.target].Center - npc.Center;
                                dist += Main.player[npc.target].velocity * 30f;
                                dist.X /= 60f;
                                dist.Y = dist.Y / 60f - 4.5f;
                                for (int num = 0; num < 10; num++)
                                    Projectile.NewProjectile(null, npc.Center, dist + Main.rand.NextVector2Square(-1f, 1f), MasomodeEX.Souls.Find<ModProjectile>("RainbowSlimeSpike").Type, npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                            }
                        }
                        
                        else if (npc.velocity.Y > 0f)
                            masoBool[1] = true;

                        break;
                    }
                case 4:
                    {
                        Aura(npc, 600f, MasomodeEX.Souls.Find<ModBuff>("BerserkedBuff").Type, reverse: true);
                        Aura(npc, 600f, 163, reverse: true, 256);
                        if (WorldGen.crimson)
                        {
                            if (++Counter[0] > 240)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && npc.life < npc.lifeMax * 0.6)
                                {
                                    Vector2 dist = Main.player[npc.target].Center - npc.Center;
                                    dist += Main.player[npc.target].velocity * 30f;
                                    dist.X /= 360f;
                                    dist.Y = dist.Y / 360f - 13.500001f;
                                    for (int num = 0; num < 10; num++)
                                        Projectile.NewProjectile(null, npc.Center, dist + Main.rand.NextVector2Square(-1f, 1f), 288, npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                                }
                            }
                        }
                        else
                        {
                            if (++Counter[1] > 60)
                            {
                                Counter[1] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && npc.life < npc.lifeMax * 0.6)
                                {
                                    int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 112, 0, 0f, 0f, 0f, 0f, 255);
                                    if (n != 200 && Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }

                            if (++Counter[0] > 3)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.life < npc.lifeMax * 0.6)
                                    Projectile.NewProjectile(null, npc.Center, npc.velocity.RotatedBy((double)MathHelper.ToRadians(Main.rand.NextFloat(-6f, 6f)), default) * 0.66f, 101, npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                            }
                        }

                        if (++Counter[2] <= 600)
                            break;
                        Counter[2] = 0;
                        if (!npc.HasValidTarget)
                            break;

                        Player player = Main.player[npc.target];
                        SoundEngine.PlaySound(SoundID.Zombie104, player.position);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = player.Center;
                            int direction = player.velocity.X != 0f ? Math.Sign(player.velocity.X) : player.direction;
                            spawnPos.X += 600 * direction;
                            spawnPos.Y -= 600f;
                            Vector2 speed = Vector2.UnitY;
                            for (int num31 = 0; num31 < 30; num31++)
                            {
                                Projectile.NewProjectile(null, spawnPos, speed, MasomodeEX.Souls.Find<ModProjectile>("BloodScythe").Type, npc.damage / 4, 1f, Main.myPlayer, 0f, 0f, 0f);
                                spawnPos.X += 72 * direction;
                                speed.Y += 0.15f;
                            }
                        }
                        break;
                    }
                case 13:
                case 14:
                    {
                        Aura(npc, 50f, MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, reverse: false, 27);
                        if (!npc.HasValidTarget)
                            break;

                        npc.position -= npc.velocity;
                        int cornerX = (int)npc.position.X / 16 - 1;
                        int cornerX2 = (int)(npc.position.X + npc.width) / 16 + 2;
                        int cornerY = (int)npc.position.Y / 16 - 1;
                        int cornerY2 = (int)(npc.position.Y + npc.height) / 16 + 2;
                        if (cornerX < 0)
                            cornerX = 0;
                        if (cornerX2 > Main.maxTilesX)
                            cornerX2 = Main.maxTilesX;
                        if (cornerY < 0)
                            cornerY = 0;
                        if (cornerY2 > Main.maxTilesY)
                            cornerY2 = Main.maxTilesY;

                        bool isOnSolidTile = false;
                        Vector2 tilePos = default;
                        for (int i = cornerX; i < cornerX2; i++)
                        {
                            for (int j = cornerY; j < cornerY2; j++)
                            {
                                Tile tile = Main.tile[i, j];
                                if (!(tile != null) || (!(tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType] && (!Main.tileSolidTop[tile.TileType] || tile.TileFrameY != 0)) && tile.LiquidAmount <= 64))
                                    continue;

                                tilePos = new(i * 16f, j * 16f);
                                if (npc.position.X + npc.width > tilePos.X && npc.position.X < tilePos.X + 16f && npc.position.Y + npc.height > tilePos.Y && npc.position.Y < tilePos.Y + 16f)
                                {
                                    isOnSolidTile = true;
                                    WorldGen.KillTile(i, j, false, false, false);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                                }
                            }
                        }

                        float num = Main.player[npc.target].Center.X;
                        float y = Main.player[npc.target].Center.Y;
                        float num2 = num - npc.Center.X;
                        float num3 = y - npc.Center.Y;
                        Math.Sqrt((double)num2 * (double)num2 + (double)num3 * (double)num3);
                        if (!isOnSolidTile)
                        {
                            npc.velocity.Y -= 0.15f;
                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 4.8f)
                            {
                                if (npc.velocity.X < 0f)
                                    npc.velocity.X += 0.1f;
                                else
                                    npc.velocity.X -= 0.1f;
                            }
                            else if (npc.velocity.Y == 12f)
                            {
                                if (npc.velocity.X < num2)
                                    npc.velocity.X -= 0.1f;
                                else if (npc.velocity.X > num2)
                                    npc.velocity.X += 0.1f;
                            }
                            else if (npc.velocity.Y > 4f)
                            {
                                if (npc.velocity.X < 0f)
                                    npc.velocity.X -= 0.09f;
                                else
                                    npc.velocity.X += 0.09f;
                            }
                        }

                        float num21 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                        float num22 = Math.Abs(num2);
                        float num23 = Math.Abs(num3);
                        float num25 = 12f / num21;
                        float num27 = num2 * num25;
                        float num28 = num3 * num25;
                        if ((npc.velocity.X > 0f && num27 > 0f || npc.velocity.X < 0f && num27 < 0f) && (npc.velocity.Y > 0f && num28 > 0f || npc.velocity.Y < 0f && num28 < 0f))
                        {
                            if (npc.velocity.X < num27)
                                npc.velocity.X += 0.15f;
                            else if (npc.velocity.X > num27)
                                npc.velocity.X -= 0.15f;
                            if (npc.velocity.Y < num28)
                                npc.velocity.Y += 0.15f;
                            else if (npc.velocity.Y > num28)
                                npc.velocity.Y -= 0.15f;
                        }
                        if (npc.velocity.X > 0f && num27 > 0f || npc.velocity.X < 0f && num27 < 0f || npc.velocity.Y > 0f && num28 > 0f || npc.velocity.Y < 0f && num28 < 0f)
                        {
                            if (npc.velocity.X < num27)
                                npc.velocity.X += 0.1f;
                            else if (npc.velocity.X > num27)
                                npc.velocity.X -= 0.1f;
                            if (npc.velocity.Y < num28)
                                npc.velocity.Y += 0.1f;
                            else if (npc.velocity.Y > num28)
                                npc.velocity.Y -= 0.1f;

                            if (Math.Abs(num28) < 2.4f && (npc.velocity.X > 0f && num27 < 0f || npc.velocity.X < 0f && num27 > 0f))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += 0.2f;
                                else
                                    npc.velocity.Y -= 0.2f;
                            }

                            if (Math.Abs(num27) < 2.4f && (npc.velocity.Y > 0f && num28 < 0f || npc.velocity.Y < 0f && num28 > 0f))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += 0.2f;
                                else
                                    npc.velocity.X -= 0.2f;
                            }
                        }
                        else if (num22 > num23)
                        {
                            if (npc.velocity.X < num27)
                                npc.velocity.X += 0.1f;
                            else if (npc.velocity.X > num27)
                                npc.velocity.X -= 0.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 6f)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += 0.1f;
                                else
                                    npc.velocity.Y -= 0.1f;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num28)
                                npc.velocity.Y += 0.1f;
                            else if (npc.velocity.Y > num28)
                                npc.velocity.Y -= 0.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 6f)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += 0.1f;
                                else
                                    npc.velocity.X -= 0.1f;
                            }
                        }

                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
                        npc.netUpdate = true;
                        npc.localAI[0] = 1f;
                        float ratio = npc.life / (float)npc.lifeMax;
                        if (ratio > 0.5f)
                            ratio = 0.5f;
                        npc.position += npc.velocity * (1.5f - ratio);
                        
                        break;
                    }
                case 266:
                    if (npc.buffType[0] != 0)
                        npc.DelBuff(0);
                    npc.knockBackResist = 0f;
                    break;
                case 35:
                    Aura(npc, 600f, MasomodeEX.Souls.Find<ModBuff>("LethargicBuff").Type, reverse: true, 60);
                    if (++Counter[0] > 300)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                        {
                            Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 3f;
                            for (int num = 0; num < 8; num++)
                                Projectile.NewProjectile(null, npc.Center, vel.RotatedBy(Math.PI / 4.0 * num, default), 270, npc.damage / 5, 0f, Main.myPlayer, -1f, 0f, 0f);
                        }
                    }

                    if (++Counter[1] > 360)
                    {
                        Counter[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 44);
                            if (n < 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n, 0f);
                        }
                    }

                    npc.localAI[2] += 1f;
                    npc.reflectsProjectiles = npc.ai[1] == 1f || npc.ai[1] == 2f;
                    
                    break;
                case 36:
                    Aura(npc, 140f, 160);
                    fargoNPCOld.Counter[0]--;
                    fargoNPCOld.Counter[1]--;
                    break;
                case 68:
                    if (npc.HasValidTarget)
                        npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 5f;
                    npc.reflectsProjectiles = true;
                    break;
                case 657:
                    TrySpawnMinions(ref SpawnedMinions1, 0.75);
                    break;
                case 222:
                    if (!masoBool[0] && npc.HasPlayerTarget)
                    {
                        masoBool[0] = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                        }
                    }

                    if (!masoBool[1] && npc.life < npc.lifeMax / 2 && npc.HasPlayerTarget)
                    {
                        masoBool[1] = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                            NPC.SpawnOnPlayer(npc.target, MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                        }
                    }
                    
                    if (--Counter[0] < 0)
                    {
                        Counter[0] = 60;
                        masoBool[2] = NPC.AnyNPCs(MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                        masoBool[2] = NPC.AnyNPCs(MasomodeEX.Souls.Find<ModNPC>("RoyalSubject").Type);
                    }

                    break;
                case 210:
                case 211:
                    switch (Main.rand.Next(21))
                    {
                        case 0:
                            npc.Transform(42);
                            break;
                        case 1:
                            npc.Transform(231);
                            break;
                        case 2:
                            npc.Transform(232);
                            break;
                        case 3:
                            npc.Transform(233);
                            break;
                        case 4:
                            npc.Transform(234);
                            break;
                        case 5:
                            npc.Transform(235);
                            break;
                        case 6:
                            npc.Transform(-56);
                            break;
                        case 7:
                            npc.Transform(-58);
                            break;
                        case 8:
                            npc.Transform(-60);
                            break;
                        case 9:
                            npc.Transform(-62);
                            break;
                        case 10:
                            npc.Transform(-64);
                            break;
                        case 11:
                            npc.Transform(-57);
                            break;
                        case 12:
                            npc.Transform(-59);
                            break;
                        case 13:
                            npc.Transform(-61);
                            break;
                        case 14:
                            npc.Transform(-63);
                            break;
                        case 15:
                            npc.Transform(-65);
                            break;
                        case 16:
                            npc.Transform(176);
                            break;
                        case 17:
                            npc.Transform(-20);
                            break;
                        case 18:
                            npc.Transform(-21);
                            break;
                        case 19:
                            npc.Transform(-19);
                            break;
                        case 20:
                            npc.Transform(-18);
                            break;
                    }
                    break;
                case 113:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int x = (int)npc.Center.X / 16;
                        int startY = (int)npc.Center.Y / 16 + ((int)npc.Center.Y / 16 - Main.maxTilesY);

                        for (int num = startY; num < Main.maxTilesY; num++)
                            WorldGen.PlaceWall(x, num, 77, false);

                        float velX = Math.Abs(npc.velocity.X);
                        if (velX > 16f)
                            velX += 16f;

                        while (velX > 16f)
                        {
                            x += !(npc.velocity.X > 0f) ? 1 : -1;
                            for (int num = startY; num < Main.maxTilesY; num++)
                                WorldGen.PlaceWall(x, num, 77);
                            velX -= 16f;
                        }
                    }

                    fargoNPCOld.masoBool[0] = true;
                    fargoNPCOld.Counter[0]++;
                    Main.player[Main.myPlayer].AddBuff(MasomodeEX.Souls.Find<ModBuff>("LowGroundBuff").Type, 2, true, false);
                    
                    break;
                case 114:
                    if (npc.ai[2] != 2f)
                        npc.ai[1] += 1f;
                    break;
                case 115:
                case 116:
                    Aura(npc, 100f, 67, reverse: false, 6);
                    break;
                case 125:
                    if (npc.ai[0] < 4f)
                        npc.ai[0] = 4f;

                    Aura(npc, 900f, 69, reverse: true, 90);
                    if (Counter[0]++ > 240)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                        {
                            Vector2 dist = Main.player[npc.target].Center - npc.Center;
                            dist.Normalize();
                            dist *= 10f;

                            for (int num = 0; num < 12; num++)
                                Projectile.NewProjectile(null, npc.Center, dist.RotatedBy(Math.PI / 6.0 * num, default), ModContent.ProjectileType<MechElectricOrb>(), npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                    }

                    if (++Counter[1] <= 150)
                        break;
                    Counter[1] = 0;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float retiSpeed = 13.08997f;
                        float retiAcc = retiSpeed * retiSpeed / 500f * 1f;
                        for (int num = 0; num < 4; num++)
                            Projectile.NewProjectile(null, npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2.0 * num, default) * retiSpeed, MasomodeEX.Souls.Find<ModProjectile>("MutantRetirang").Type, npc.damage / 4, 0f, Main.myPlayer, retiAcc, 240f, 0f);
                    }

                    break;
                case 126:
                    if (npc.ai[0] < 4f)
                        npc.ai[0] = 4f;

                    Aura(npc, 900f, 39, reverse: true, 89);
                    if (Counter[0]++ > 120)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                        {
                            Vector2 dist = Main.player[npc.target].Center - npc.Center;
                            dist.Normalize();
                            dist *= 14f;
                            for (int j = 0; j < 8; j++)
                                Projectile.NewProjectile(null, npc.Center, dist.RotatedBy(Math.PI / 4.0 * j, default), ModContent.ProjectileType<MechElectricOrbSpaz>(), npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                    }

                    if (++Counter[1] <= 150)
                        break;
                    Counter[1] = 0;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float spazSpeed = 13.08997f;
                        float spazAcc = spazSpeed * spazSpeed / 250f * -1f;
                        for (int k = 0; k < 4; k++)
                            Projectile.NewProjectile(null, npc.Center, Vector2.UnitX.RotatedBy(Math.PI / 2.0 * k + Math.PI / 4.0, default) * spazSpeed, MasomodeEX.Souls.Find<ModProjectile>("MutantSpazmarang").Type, npc.damage / 4, 0f, Main.myPlayer, spazAcc, 120f, 0f);
                    }

                    break;
                case 134:
                    {
                        if (++Counter[0] > 240)
                        {
                            fargoNPCOld.masoBool[0] = true;
                            Counter[0] = 0;
                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);

                            if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 15f;
                                int current = Projectile.NewProjectile(null, npc.Center, vel, MasomodeEX.Souls.Find<ModProjectile>("MutantDestroyerHead").Type, npc.damage / 4, 0f, Main.myPlayer, npc.target, 0f, 0f);
                                for (int num = 0; num < 18; num++)
                                    current = Projectile.NewProjectile(null, npc.Center, vel, MasomodeEX.Souls.Find<ModProjectile>("MutantDestroyerBody").Type, npc.damage / 4, 0f, Main.myPlayer, current, 0f, 0f);
                                int previous = current;
                                current = Projectile.NewProjectile(null, npc.Center, vel, MasomodeEX.Souls.Find<ModProjectile>("MutantDestroyerTail").Type, npc.damage / 4, 0f, Main.myPlayer, current, 0f, 0f);
                                Main.projectile[previous].localAI[1] = current;
                                Main.projectile[previous].netUpdate = true;
                            }
                        }

                        if (!fargoNPCOld.masoBool[0])
                            break;

                        int cornerX2 = (int)npc.position.X / 16 - 1;
                        int cornerX4 = (int)(npc.position.X + npc.width) / 16 + 2;
                        int cornerY2 = (int)npc.position.Y / 16 - 1;
                        int cornerY4 = (int)(npc.position.Y + npc.height) / 16 + 2;
                        if (cornerX2 < 0)
                            cornerX2 = 0;
                        if (cornerX4 > Main.maxTilesX)
                            cornerX4 = Main.maxTilesX;
                        if (cornerY2 < 0)
                            cornerY2 = 0;
                        if (cornerY4 > Main.maxTilesY)
                            cornerY4 = Main.maxTilesY;

                        Vector2 tilePos2 = default;
                        for (int x3 = cornerX2; x3 < cornerX4; x3++)
                        {
                            for (int y3 = cornerY2; y3 < cornerY4; y3++)
                            {
                                Tile tile = Main.tile[x3, y3];
                                if (!(tile != null) || (!tile.HasUnactuatedTile || !Main.tileSolid[tile.TileType] && (!Main.tileSolidTop[tile.TileType] || tile.TileFrameY != 0)) && tile.LiquidAmount <= 64)
                                    continue;
                                tilePos2 = new(x3 * 16f, y3 * 16f);
                                
                                if (npc.position.X + npc.width > tilePos2.X && npc.position.X < tilePos2.X + 16f && npc.position.Y + npc.height > tilePos2.Y && npc.position.Y < tilePos2.Y + 16f)
                                {
                                    WorldGen.KillTile(x3, y3, false, false, false);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x3, y3);
                                }
                            }
                        }

                        break;
                    }
                case 135:
                case 136:
                    if (npc.ai[2] == 0f || ++Counter[0] <= 420)
                        break;

                    Counter[0] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int num = 0; num < 6; num++)
                        {
                            int p = Projectile.NewProjectile(null, npc.Center, Vector2.Normalize(npc.position - Main.player[npc.target].position).RotatedBy(Math.PI / 4.0 * num, default) * 10f, ModContent.ProjectileType<MechElectricOrbHoming>(), npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                            Main.projectile[p].ai[2] = 1; // destroyer themed color
                        }
                    }

                    break;
                case 127:
                    Aura(npc, 100f, MasomodeEX.Souls.Find<ModBuff>("StunnedBuff").Type);
                    npc.reflectsProjectiles = npc.ai[1] == 1f || npc.ai[1] == 2f;
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 131, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 129, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 128, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 130, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }

                    if (!masoBool[1] && npc.ai[0] == 2f && ++Counter[0] > 300)
                    {
                        masoBool[1] = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 131, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 129, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 128, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 130, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }

                        npc.ai[3] = -2f;
                        npc.netUpdate = true;
                    }

                    if (++Counter[1] > 120)
                    {
                        Counter[1] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                            Projectile.NewProjectile(null, npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 18f, MasomodeEX.Souls.Find<ModProjectile>("MutantGuardian").Type, npc.damage / 3, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }

                    break;
                case 128:
                case 129:
                case 130:
                case 131:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        if (Main.npc[(int)npc.ai[1]].type == NPCID.SkeletronPrime && Main.npc[(int)npc.ai[1]].ai[0] == 2f && Main.npc[(int)npc.ai[1]].ai[3] == -1f)
                            masoBool[1] = true;
                    }
                    
                    if (masoBool[1] && !MasomodeEX.Instance.HyperLoaded)
                    {
                        masoBool[2] = !masoBool[2];
                        if (masoBool[2])
                        {
                            npc.position += npc.velocity * 1.5f;
                            npc.AI();
                        }
                    }
                    
                    if (++Counter[0] > 240)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                            Projectile.NewProjectile(null, npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 14f, ModContent.ProjectileType<MechElectricOrb>(), npc.damage / 4, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }

                    break;
                case 262:
                    {
                        npc.life += 5;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;

                        if (++Counter[0] > 75)
                        {
                            Counter[0] = Main.rand.Next(30);
                            CombatText.NewText(npc.Hitbox, CombatText.HealLife, 300, false, false);
                        }

                        if (npc.life >= npc.lifeMax / 2)
                            break;
                        
                        if (++Counter[1] > 300)
                        {
                            Counter[1] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int tentaclesToSpawn = 6;
                                for (int i = 0; i < 200; i++)
                                {
                                    if (Main.npc[i].active && Main.npc[i].type == NPCID.PlanterasTentacle && Main.npc[i].ai[3] == 0f)
                                        tentaclesToSpawn--;
                                }

                                for (int j = 0; j < tentaclesToSpawn; j++)
                                {
                                    int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 264, npc.whoAmI);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }

                        if (--Counter[2] < 0)
                        {
                            Counter[2] = 180;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(null, npc.Center, Vector2.Zero, MasomodeEX.Souls.Find<ModProjectile>("MutantMark2").Type, npc.damage / 4, 0f, Main.myPlayer, 30f, 150f, 0f);
                        }
                        break;
                    }
                case 264:
                    npc.position += npc.velocity;
                    Aura(npc, 200f, MasomodeEX.Souls.Find<ModBuff>("IvyVenomBuff").Type, reverse: false, 188);
                    break;
                case 245:
                    if (npc.dontTakeDamage)
                        break;

                    npc.position.X += npc.velocity.X * 0.5f;
                    if (npc.velocity.Y < 0f)
                    {
                        npc.position.Y += npc.velocity.Y * 0.5f;
                        if (npc.velocity.Y > -2f)
                            npc.velocity.Y = 20f;
                    }

                    if (npc.ai[0] == 0f)
                    {
                        if (npc.ai[1] > 0f && npc.ai[1] < 300f)
                            npc.ai[1] = 300f;
                        else if (npc.ai[1] < -5f)
                            npc.ai[1] = -5f;
                    }

                    break;
                case 246:
                case 247:
                case 248:
                    Aura(npc, 200f, MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type);
                    break;
                case 249:
                    Aura(npc, 200f, MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type);
                    break;
                case 370:
                    {
                        npc.position += npc.velocity;
                        if (--Counter[0] < 0)
                            Counter[0] = 300;

                        if (!fargoNPCOld.masoBool[3] || --Counter[1] >= 0)
                            break;
                        Counter[1] = 240;

                        if (Main.netMode == NetmodeID.MultiplayerClient || !npc.HasPlayerTarget)
                            break;

                        for (int i = -1; i <= 1; i += 2)
                        {
                            int max = 3;
                            for (int j = -max; j <= max; j++)
                            {
                                if (Math.Abs(j) == max)
                                {
                                    float spread = (float)Math.PI / 12f;
                                    Vector2 offset = npc.ai[2] == 0f ? Vector2.UnitY.RotatedBy((double)(spread * j), default) * -450f * i : Vector2.UnitX.RotatedBy((double)(spread * j), default) * 475f * i;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(null), npc.Center, Vector2.Zero, MasomodeEX.Souls.Find<ModProjectile>("MutantFishron").Type, npc.damage, 0f, Main.myPlayer, offset.X, offset.Y, 0f);
                                }
                            }
                        }
                        break;
                    }
                case 439:
                    Aura(npc, 600f, MasomodeEX.Souls.Find<ModBuff>("MarkedforDeathBuff").Type, reverse: false, 199);
                    Aura(npc, 600f, MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type);
                    if (++Counter[0] > 60)
                    {
                        Counter[0] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !NPC.AnyNPCs(454))
                        {
                            int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, 454, 0, 0f, 0f, 0f, 0f, 255);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }

                    if (masoBool[0])
                        break;
                    masoBool[0] = true;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, Mod.Find<ModNPC>("CultistIllusion").Type, npc.whoAmI, npc.whoAmI, -1f, 1f, 0f, 255);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, Mod.Find<ModNPC>("CultistIllusion").Type, npc.whoAmI, npc.whoAmI, 1f, -1f, 0f, 255);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, Mod.Find<ModNPC>("CultistIllusion").Type, npc.whoAmI, npc.whoAmI, 1f, 1f, 0f, 255);
                        if (n != 200 && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    break;
                case 401:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        Main.NewText("WHAT ARE YOU DOING TO ME!?! Healing Leech Clots, Recover me at once!", Color.LimeGreen);
                    }
                    break;
                case 398:
                    {
                        if (Main.LocalPlayer.active && Main.LocalPlayer.mount.Active)
                            Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);

                        if (!masoBool[0])
                        {
                            npc.TargetClosest(false);
                            masoBool[0] = true;
                            
                            if (NPC.CountNPCS(398) == 1)
                                Main.LocalPlayer.GetModPlayer<MasomodeEXPlayer>().hitcap = 25;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(null, npc.Center, Vector2.Zero, Mod.Find<ModProjectile>("MoonLordText").Type, 0, 0f, Main.myPlayer, npc.whoAmI, 0f, 0f);
                        }

                        if (!masoBool[1] && npc.ai[0] == 2f)
                        {
                            masoBool[1] = true;
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.life = npc.lifeMax;
                            npc.netUpdate = true;
                            Main.NewText("Hehehehehehe! I GUARANTEE YOU DON'T HAVE ENOUGH POWER TO DEFEAT ME!!!", Color.LimeGreen);
                            return false;
                        }

                        if (++Counter[0] > 90)
                        {
                            Counter[0] = 0;
                            if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(null, npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 8f, 462, 30, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }

                        npc.position += npc.velocity * (1f - npc.life / (float)npc.lifeMax);
                        
                        if (masoBool[1])
                            npc.position += npc.velocity;

                        break;
                    }
                case 400:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        Main.NewText(Main.rand.Next(5) switch
                        {
                            0 => "Guys, Will you assist me for a moment?",
                            1 => "I'm going to bring more of my servants to assist me!",
                            2 => "Go servants, what are you all waiting for?",
                            3 => "Servants! Get this little peasant out of my sight at once!",
                            _ => "Mwahahahahahahahahaha! Trap this tiny struggle at once!",
                        }, Color.LimeGreen);

                        if (NPC.CountNPCS(400) >= 3)
                            Main.NewText("Ahhhh, my eyes!", Color.LimeGreen);
                    }
                    if (++Counter[0] > 90)
                    {
                        Counter[0] = 0;
                        if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(null, npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 8f, 462, 30, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }
                    break;
                case 397:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, npc.ai[2] == 0f ? Mod.Find<ModNPC>("PhantomPortal").Type : Mod.Find<ModNPC>("PurityPortal").Type, npc.whoAmI, npc.whoAmI, 0f, 0f, 0f, 255);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }

                    if (npc.dontTakeDamage)
                    {
                        if (masoBool[1])
                        {
                            masoBool[1] = false;
                            Main.NewText(Main.rand.Next(3) switch
                            {
                                0 => "It looks like you can't defeat me!",
                                1 => "I see that you have annihilated my son!,You wimp!",
                                _ => "I still have plenty of gimmicks and tricks left!",
                            }, (Color?)Color.LimeGreen);
                        }
                    }
                    else
                        masoBool[1] = true;

                    break;
                case 396:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        _ = Main.netMode;
                    }

                    if (npc.dontTakeDamage)
                    {
                        if (masoBool[1])
                        {
                            masoBool[1] = false;
                            Main.NewText(Main.rand.Next(3) switch
                            {
                                0 => "It looks like you can't defeat me!",
                                1 => "I see that you have annihilated my son!,You wimp!",
                                _ => "I still have plenty of gimmicks and tricks left!",
                            }, Color.LimeGreen);
                        }
                    }
                    else
                        masoBool[1] = true;

                    break;
                case 551:
                    Aura(npc, 700f, 24, reverse: true, 6);
                    Aura(npc, 700f, 67, reverse: true);
                    if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].Distance(npc.Center) < 700f)
                    {
                        Main.player[Main.myPlayer].AddBuff(195, 2, true, false);
                        Main.player[Main.myPlayer].AddBuff(196, 2, true, false);
                    }
                    break;
                case 2:
                    if (!masoBool[0])
                    {
                        masoBool[0] = true;
                        if (Main.rand.NextBool(4))
                            npc.Transform(133);
                    }
                    break;
                case 477:
                case 479:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PurpleCrystalShard, 0f, 0f, 0, default, 1f);
                            Main.dust[d].scale += 1f;
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }

                        if (++Counter[0] > 6)
                        {
                            Counter[0] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(null, npc.Center, Main.rand.NextVector2Unit(0f, (float)Math.PI * 2f) * 12f, Mod.Find<ModProjectile>("MothDust").Type, npc.damage / 5, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                        break;
                    }
                case 290:
                    npc.reflectsProjectiles = true;
                    break;
                case 133:
                    npc.noTileCollide = true;
                    break;
                case 109:
                    {
                        if (npc.Distance(Main.player[Main.myPlayer].Center) > 500f)
                        {
                            Main.player[Main.myPlayer].AddBuff(MasomodeEX.Souls.Find<ModBuff>("AtrophiedBuff").Type, 2, true, false);
                            Main.player[Main.myPlayer].AddBuff(MasomodeEX.Souls.Find<ModBuff>("JammedBuff").Type, 2, true, false);
                            Main.player[Main.myPlayer].AddBuff(MasomodeEX.Souls.Find<ModBuff>("ReverseManaFlowBuff").Type, 2, true, false);
                            Main.player[Main.myPlayer].AddBuff(MasomodeEX.Souls.Find<ModBuff>("AntisocialBuff").Type, 2, true, false);
                        }

                        for (int l = 0; l < 20; l++)
                        {
                            int type = 127;
                            switch (Main.rand.Next(4))
                            {
                                case 1:
                                    type = 229;
                                    break;
                                case 2:
                                    type = 242;
                                    break;
                                case 3:
                                    type = 135;
                                    break;
                            }

                            Vector2 offset = default;
                            double angle = Main.rand.NextDouble() * 2.0 * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 450.0);
                            offset.Y += (float)(Math.Cos(angle) * 450.0);
                            Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4f, 4f), 0, 0, type, 0f, 0f, 100, Color.White, 1f)];
                            dust.velocity = npc.velocity;
                            if (Main.rand.NextBool(3))
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }

                        if (++Counter[0] <= 1100)
                            break;
                        
                        npc.life = 0;
                        SoundEngine.PlaySound(npc.DeathSound, (Vector2?)npc.Center, null);
                        npc.active = false;
                        bool bossAlive = false;

                        for (int m = 0; m < 200; m++)
                        {
                            if (Main.npc[m].boss && Main.npc[m].active)
                            {
                                bossAlive = true;
                                break;
                            }
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            break;

                        if (bossAlive)
                        {
                            Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 516, 100, 8f, Main.myPlayer, 0f, 0f, 0f);
                            break;
                        }

                        for (int n = 0; n < 100; n++)
                        {
                            int type2 = 28;
                            int damage = 100;
                            float knockback = 8f;
                            switch (Main.rand.Next(6))
                            {
                                case 1:
                                    type2 = 37;
                                    break;
                                case 2:
                                    type2 = 516;
                                    break;
                                case 3:
                                    type2 = 30;
                                    damage = 60;
                                    break;
                                case 4:
                                    type2 = 397;
                                    damage = 60;
                                    break;
                                case 5:
                                    type2 = 517;
                                    damage = 60;
                                    break;
                            }

                            int p = Projectile.NewProjectile(null, npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), Main.rand.Next(-1000, 1001) / 50f, Main.rand.Next(-2000, 101) / 50f, type2, damage, knockback, Main.myPlayer, 0f, 0f, 0f);
                            Main.projectile[p].timeLeft += Main.rand.Next(180);
                        }

                        break;
                    }
                case 491:
                    if (++Counter[0] <= 300)
                        break;
                    Counter[0] = 0;
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.Y -= Math.Abs(speed.X) * 0.2f;
                        speed.X += Main.rand.Next(-20, 21);
                        speed.Y += Main.rand.Next(-20, 21);
                        speed.Normalize();
                        speed *= 11f;
                        npc.localAI[2] = 0f;

                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 cannonSpeed = speed;
                            cannonSpeed.X += Main.rand.Next(-10, 11) * 0.3f;
                            cannonSpeed.Y += Main.rand.Next(-10, 11) * 0.3f;
                            Projectile.NewProjectile(null, npc.Center, cannonSpeed, 240, Main.expertMode ? 80 : 100, 0f, Main.myPlayer, 0f, 0f, 0f);
                        }
                    }

                    break;
            }

            if (npc.type == MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type && !MasomodeEX.Instance.HyperLoaded)
                npc.position += npc.velocity * 0.7f;
            
            if ((npc.type == MasomodeEX.Souls.Find<ModNPC>("CosmosChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("EarthChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("LifeChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("NatureChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("ShadowChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("SpiritChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("TerraChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("TimberChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("WillChampion").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("LesserFairy").Type || npc.type == MasomodeEX.Souls.Find<ModNPC>("LesserSquirrel").Type) && !MasomodeEX.Instance.HyperLoaded && ++Counter[0] > 3)
            {
                Counter[0] = 0;
                npc.position += npc.velocity * 1.2f;
                npc.AI();
            }
            
            return true;
        }

        return true;
        
        void TrySpawnMinions(ref bool check, double threshold)
        {
            if (!check && npc.life < npc.lifeMax * threshold)
            {
                check = true;
                for (int i = 0; i < 5; i++)
                    MasoModeUtils.NewNPCEasy(npc.GetSource_FromAI(null), npc.Center, MasomodeEX.Souls.Find<ModNPC>("GelatinSubject").Type, npc.whoAmI, 0f, 0f, 0f, 0f, npc.target, Main.rand.NextFloat(8f) * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(MathHelper.PiOver2));
            }
        }
    }

    public override void OnHitPlayer(NPC npc, Player target, HurtInfo hurtInfo)
    {
        if (!MasoModeUtils.checkIfMasoEX())
            return;

        if (npc.type != NPCID.DemonEye && npc.type != NPCID.DemonEyeOwl && npc.type != NPCID.DemonEyeSpaceship)
            target.AddBuff(156, 15);

        switch (npc.type)
        {
            case 48:
                if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoSoulsPlayer>().SecurityWallet)
                {
                    if (!StealFromInventory(target, ref Main.mouseItem))
                        StealFromInventory(target, ref target.inventory[target.selectedItem]);

                    byte extraTries = 30;
                    bool successfulSteal = StealFromInventory(target, ref target.inventory[Main.rand.Next(target.inventory.Length)]);
                    while (!successfulSteal && extraTries > 0)
                    {
                        extraTries--;
                        successfulSteal = StealFromInventory(target, ref target.inventory[Main.rand.Next(target.inventory.Length)]);
                    }
                }
                break;
            case 412:
            case 413:
            case 414:
            case 421:
            case 427:
                target.AddBuff(164, Main.rand.Next(300));
                break;
            case 418:
            case 419:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type, Main.rand.Next(300));
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("FlippedBuff").Type, Main.rand.Next(300));
                break;
            case 1:
                if (npc.netID == -4)
                {
                    target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("StunnedBuff").Type, 60);
                    target.velocity.X = target.Center.X < npc.Center.X ? -500f : 500f;
                    target.velocity.Y = -100f;
                }
                break;
            case 535:
                npc.Transform(50);
                break;
            case 2:
                target.AddBuff(156, Main.rand.Next(300));
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type, Main.rand.Next(300));
                npc.Transform(133);
                break;
            case 133:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type, Main.rand.Next(300));
                npc.Transform(4);
                break;
            case 94:
                npc.Transform(13);
                target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                target.AddBuff(33, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, Main.rand.Next(60, 600));
                break;
            case 268:
                npc.Transform(266);
                target.AddBuff(69, Main.rand.Next(60, 600), true, false);
                target.AddBuff(30, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("BloodthirstyBuff").Type, Main.rand.Next(300));
                break;
            case 170:
            case 171:
            case 180:
                npc.Transform(370);
                break;
            case 6:
                target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                target.AddBuff(33, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, Main.rand.Next(60, 600));
                if (target.statLife + hurtInfo.Damage < 100 && NPC.AnyNPCs(13))
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " was eaten alive by Eater of Souls."), 999.0, 0, false);
                npc.Transform(94);
                break;
            case 13:
                target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                target.AddBuff(33, 7200, true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, Main.rand.Next(60, 600));
                if (target.statLife + hurtInfo.Damage < 150)
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " was eaten alive by Eater of Worlds."), 999.0, 0, false);
                break;
            case 14:
            case 15:
                target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                target.AddBuff(33, 7200, true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, Main.rand.Next(60, 600));
                break;
            case 125:
            case 126:
            case 127:
            case 128:
            case 129:
            case 130:
            case 131:
            case 135:
            case 136:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type, 2);
                break;
            case 139:
                {
                    target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type, 2);
                    NPC.SpawnOnPlayer(target.whoAmI, 127);
                    NPC.SpawnOnPlayer(target.whoAmI, 125);
                    NPC.SpawnOnPlayer(target.whoAmI, 126);
                    NPC.SpawnOnPlayer(target.whoAmI, 134);
                    break;
                }
            case 134:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type, 2, true, false);
                if (target.statLife + hurtInfo.Damage < 300)
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " was eaten alive by The Destroyer."), 999.0, 0, false);
                break;
            case 348:
            case 349:
                if (target.Male)
                    target.KillMe(PlayerDeathReason.ByCustomReason(target.name + " got his nuts cracked."), 9999.0, 0, false);
                break;
            case 75:
            case 80:
            case 84:
            case 86:
            case 120:
            case 122:
            case 137:
            case 138:
            case 244:
            case 475:
            case 527:
            case 545:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("UnstableBuff").Type, Main.rand.Next(60, 300), true, false);
                target.AddBuff(31, Main.rand.Next(300, 1200), true, false);
                break;
            case -2:
            case -1:
            case 7:
            case 8:
            case 9:
            case 47:
            case 57:
            case 81:
            case 83:
            case 98:
            case 99:
            case 100:
            case 101:
            case 112:
            case 121:
            case 168:
            case 473:
            case 525:
            case 543:
                target.AddBuff(39, Main.rand.Next(60, 600), true, false);
                target.AddBuff(33, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, Main.rand.Next(60, 600), true, false);
                break;
            case 174:
            case 179:
            case 181:
            case 182:
            case 183:
            case 239:
            case 240:
            case 241:
            case 242:
            case 266:
            case 267:
            case 464:
            case 465:
            case 470:
            case 474:
            case 526:
            case 544:
                target.AddBuff(69, Main.rand.Next(60, 600), true, false);
                target.AddBuff(30, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("BloodthirstyBuff").Type, Main.rand.Next(300), true, false);
                break;
            case 173:
                target.AddBuff(69, Main.rand.Next(60, 600), true, false);
                target.AddBuff(30, Main.rand.Next(7200), true, false);
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("BloodthirstyBuff").Type, Main.rand.Next(300), true, false);
                npc.Transform(268);
                break;
            case 4:
            case 5:
                target.AddBuff(MasomodeEX.Souls.Find<ModBuff>("HexedBuff").Type, Main.rand.Next(300), true, false);
                break;
        }
    }

    public float ModifyHitByEither(NPC npc, int damage)
    {
        float newDamage = damage;
        switch (npc.type)
        {
            case 266:
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(2) & npc.HasPlayerTarget)
                {
                    Vector2 distance = Main.player[npc.target].Center - npc.Center;
                    if (Main.rand.NextBool(2))
                        distance.X *= -1f;
                    if (Main.rand.NextBool(2))
                        distance.Y *= -1f;
                    npc.Center = Main.player[npc.target].Center + distance;
                    npc.netUpdate = true;
                }
                break;
            case 222:
                if (masoBool[2])
                    newDamage = 0f;
                break;
            case 419:
                if (npc.ai[2] <= -6f)
                    newDamage = 0f;
                break;
            case 396:
            case 397:
            case 398:
                if (damage > npc.lifeMax / 10)
                {
                    newDamage = 0f;
                    Main.NewText("YOU THINK YOU CAN BUTCHER A GREAT LORD!?!", (Color?)Color.LimeGreen);
                }
                break;
        }
        return newDamage;
    }

    public override bool CheckDead(NPC npc)
    {
        switch (npc.type)
        {
            case 75:
            case 80:
            case 84:
            case 86:
            case 120:
            case 122:
            case 137:
            case 138:
            case 244:
            case 475:
            case 527:
            case 545:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 8; i++)
                        Projectile.NewProjectile(null, npc.Center, Vector2.UnitY.RotatedBy(Math.PI / 4.0 * i, default) * 4f, 146, 0, 0f, Main.myPlayer, 8f, 0f, 0f);
                }
                break;
            case -2:
            case -1:
            case 6:
            case 7:
            case 8:
            case 9:
            case 14:
            case 15:
            case 47:
            case 57:
            case 81:
            case 83:
            case 94:
            case 98:
            case 99:
            case 100:
            case 101:
            case 112:
            case 121:
            case 168:
            case 473:
            case 525:
            case 543:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int k = 0; k < 8; k++)
                        Projectile.NewProjectile(null, npc.Center, Vector2.UnitY.RotatedBy(Math.PI / 4.0 * k, default) * 4f, 147, 0, 0f, Main.myPlayer, 8f, 0f, 0f);
                }
                break;
            case 173:
            case 174:
            case 179:
            case 181:
            case 182:
            case 183:
            case 239:
            case 240:
            case 241:
            case 242:
            case 266:
            case 267:
            case 464:
            case 465:
            case 470:
            case 474:
            case 526:
            case 544:
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int j = 0; j < 8; j++)
                        Projectile.NewProjectile(null, npc.Center, Vector2.UnitY.RotatedBy(Math.PI / 4.0 * j, default) * 4f, 149, 0, 0f, Main.myPlayer, 8f, 0f, 0f);
                }
                break;
            case 23:
                if (!NPC.downedBoss2)
                {
                    SoundEngine.PlaySound(npc.DeathSound, (Vector2?)npc.Center, null);
                    npc.active = false;
                    return false;
                }
                break;
        }

        if (npc.townNPC)
        {
            if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Abominationn").Type)
            {
                int mutant = NPC.FindFirstNPC(MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type);
                if (mutant > -1 && Main.npc[mutant].active)
                {
                    Main.npc[mutant].Transform(MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText("Mutant has been enraged by the death of his brother!", 175, 75, byte.MaxValue);
                    else if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mutant has been enraged by the death of his brother!"), new Color(175, 75, 255), -1);
                }
            }
            else if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Deviantt").Type)
            {
                int mutant = NPC.FindFirstNPC(MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type);
                if (mutant > -1 && Main.npc[mutant].active)
                {
                    Main.npc[mutant].Transform(MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText("Mutant has been enraged by the death of his sister!", 175, 75, byte.MaxValue);
                    else if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mutant has been enraged by the death of his sister!"), new Color(175, 75, 255), -1);
                }
            }
            else if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type)
            {
                npc.active = true;
                npc.life = 1;
                npc.Transform(ModContent.NPCType<MutantBoss>());

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Mutant has been enraged!", 175, 75, byte.MaxValue);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mutant has been enraged!"), new Color(175, 75, 255), -1);
                
                return false;
            }
        }

        return true;
    }

    public override void OnKill(NPC npc)
    {
        if (MasoModeUtils.checkIfMasoEX() && npc.type == NPCID.MoonLordCore)
        {
            Main.NewText("Ahhhhh! It was a mistake to cum here!", Color.LimeGreen);
            Main.NewText("The enemy souls are possessed by ethereal spirits...", Color.LimeGreen);
        }
    }

    public override void ModifyHitByItem(NPC npc, Player player, Item item, ref HitModifiers modifiers)
    {
        if (WorldSavingSystem.EternityMode && Condition.ForTheWorthyWorld.IsMet())
            modifiers.FinalDamage.Base = ModifyHitByEither(npc, (int)modifiers.FinalDamage.Base);
    }

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref HitModifiers modifiers)
    {
        if (!WorldSavingSystem.EternityMode || !Condition.ForTheWorthyWorld.IsMet())
            return;

        modifiers.FinalDamage.Base = ModifyHitByEither(npc, (int)modifiers.FinalDamage.Base);

        if (!NPC.downedBoss3 && MasoModeUtils.AnyBossAlive() && projectile.type == 27)
            NPC.SpawnOnPlayer(projectile.owner, MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);

        if (npc.aiStyle == 37)
        {
            if (projectile.penetrate > 0)
                modifiers.FinalDamage.Base /= projectile.penetrate;
            else if (projectile.penetrate < 0)
                modifiers.FinalDamage.Base /= 5f;
        }
    }

    private void Aura(NPC npc, float distance, int buff, bool reverse = false, int dustid = 228)
    {
        if (!WorldSavingSystem.EternityMode || !Condition.ForTheWorthyWorld.IsMet())
            return;

        Player p = Main.player[Main.myPlayer];
        float range = npc.Distance(p.Center);
        bool num;
        
        if (!reverse)
            num = range < distance;
        else
        {
            if (!(range > distance))
                goto IL_0052;
            num = range < 3000f;
        }

        if (num)
            p.AddBuff(buff, 2, true, false);

        goto IL_0052;
        IL_0052:
        for (int i = 0; i < 20; i++)
        {
            Vector2 offset = default;
            double angle = Main.rand.NextDouble() * 2.0 * Math.PI;
            offset.X += (float)(Math.Sin(angle) * (double)distance);
            offset.Y += (float)(Math.Cos(angle) * (double)distance);
            Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4f, 4f), 0, 0, dustid, 0f, 0f, 100, Color.White, 1f)];
            dust.velocity = npc.velocity;
            if (Main.rand.NextBool(3))
                dust.velocity += Vector2.Normalize(offset) * (reverse ? 5f : -5f);
            dust.noGravity = true;
        }
    }

    public static void PrintAI(NPC npc)
    {
        if (MasoModeUtils.checkIfMasoEX())
            Main.NewText("ai: " + npc.ai[0] + " " + npc.ai[1] + " " + npc.ai[2] + " " + npc.ai[3] + ", local: " + npc.localAI[0] + " " + npc.localAI[1] + " " + npc.localAI[2] + " " + npc.localAI[2], byte.MaxValue, byte.MaxValue, byte.MaxValue);
    }

    private bool StealFromInventory(Player target, ref Item item)
    {
        if (MasoModeUtils.checkIfMasoEX() && !item.IsAir)
        {
            int i = Item.NewItem(null, (int)target.position.X, (int)target.position.Y, target.width, target.height, item.type, 1, false, 0, false, false);
            Main.item[i].netDefaults(item.netID);
            Main.item[i].Prefix(item.prefix);
            Main.item[i].stack = item.stack;
            Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
            Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
            Main.item[i].noGrabDelay = 100;
            Main.item[i].newAndShiny = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
            item = new Item();
            return true;
        }
        return false;
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (MasoModeUtils.checkIfMasoEX())
            spawnRate /= 2;
    }

    public override void OnChatButtonClicked(NPC npc, bool firstButton)
    {
        if (!MasoModeUtils.checkIfMasoEX() || !(npc.type == NPCID.Nurse && firstButton))
            return;

        for (int i = 0; i < 200; i++)
        {
            if (Main.npc[i].active && Main.npc[i].boss)
            {
                npc.StrikeInstantKill();
                SoundEngine.PlaySound(SoundID.Roar, (Vector2?)Main.player[Main.myPlayer].Center, null);
                Main.player[Main.myPlayer].AddBuff(Mod.Find<ModBuff>("MutantJudgement").Type, 3600, true, false);
                for (int j = 0; j < 10; j++)
                    NPC.SpawnOnPlayer(Main.myPlayer, MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                break;
            }
        }
    }

    public override void GetChat(NPC npc, ref string chat)
    {
        if (MasoModeUtils.checkIfMasoEX())
        {
            if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type && Main.rand.NextBool(3))
                chat = "What you're doing is a crime against the universe, mortal.";
            else if (npc.type == MasomodeEX.Fargo.Find<ModNPC>("Deviantt").Type && Main.rand.NextBool(3))
                chat = "Play a real game mode, would you? Not this... thing.";
        }
    }

    public override void OnCaughtBy(NPC npc, Player player, Item item, bool failed)
    {
        if (!MasoModeUtils.checkIfMasoEX() || npc.type != MasomodeEX.Fargo.Find<ModNPC>("Deviantt").Type)
            return;

        int mutant = NPC.FindFirstNPC(MasomodeEX.Fargo.Find<ModNPC>("Mutant").Type);
        if (mutant > -1 && Main.npc[mutant].active)
        {
            Main.npc[mutant].Transform(MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText("Mutant has been enraged by the abduction of his sister!", 175, 75, byte.MaxValue);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Mutant has been enraged by the abduction of his sister!"), new Color(175, 75, 255), -1);
        }
        else
            NPC.SpawnOnPlayer(player.whoAmI, MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
    }

    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (!MasoModeUtils.checkIfMasoEX())
            return;

        pool[MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type] = 0.0001f;
        if (!NPC.downedBoss3)
        {
            Tile tileSafely = Framing.GetTileSafely(spawnInfo.PlayerFloorX, spawnInfo.PlayerFloorY);
            ushort tileType = tileSafely.TileType;
            if (tileType == 41 || (uint)(tileType - 43) <= 1u)
                pool[68] = 1000f;
            tileSafely = Framing.GetTileSafely(spawnInfo.Player.Center);
            tileType = tileSafely.WallType;
            if ((uint)(tileType - 7) <= 2u || (uint)(tileType - 94) <= 5u)
                pool[68] = 1000f;
        }
    }
}
