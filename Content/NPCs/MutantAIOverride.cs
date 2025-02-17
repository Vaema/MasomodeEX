using System;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Creative.CreativePowers;

namespace MasomodeEX.Content.NPCs
{
    [AutoloadBossHead]
    public static class MutantAIOverride
    {
        public static bool spawned;

        public static void SpawnSphereRing(NPC NPC, int max, float speed, int damage, float rotationModifier, float offset = 0f)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float rotation = (float)Math.PI * 2f / max;
                int type = ModContent.ProjectileType<MutantSphereRing>();
                for (int i = 0; i < max; i++)
                {
                    Vector2 vel = speed * Vector2.UnitY.RotatedBy((double)(rotation * i + offset), default);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * NPC.spriteDirection, speed, 0f);
                }
                SoundEngine.PlaySound(SoundID.Item84, null, null);
            }
        }

        public static bool AliveCheck(NPC NPC, Player p, bool forceDespawn = false)
        {
            if (WorldSavingSystem.SwarmActive || forceDespawn || (!p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f) && NPC.localAI[3] > 0f)
            {
                NPC.TargetClosest(true);
                p = Main.player[NPC.target];
                if (WorldSavingSystem.SwarmActive || forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                {
                    NPC.velocity.Y -= 1f;

                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;

                    if (NPC.timeLeft == 1)
                    {
                        if (NPC.position.Y < 0f)
                            NPC.position.Y = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC npc) && !NPC.AnyNPCs(npc.Type))
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            int i = NPC.NewNPC(NPC.GetSource_FromAI(null), (int)NPC.Center.X, (int)NPC.Center.Y, npc.Type, 0, 0f, 0f, 0f, 0f, 255);
                            if (i != 200)
                            {
                                Main.npc[i].homeless = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                    return false;
                }
            }

            if (NPC.timeLeft < 3600)
                NPC.timeLeft = 3600;

            if ((double)(p.Center.Y / 16f) > Main.worldSurface)
            {
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y -= 1f;
                if (NPC.velocity.Y < -32f)
                    NPC.velocity.Y = -32f;
                return false;
            }

            return true;
        }

        public static void Movement(NPC NPC, Vector2 target, float speed, bool fastX = true, bool obeySpeedCap = true)
        {
            float turnaroundModifier = 1f;
            float maxSpeed = 24f;
            if (WorldSavingSystem.MasochistModeReal)
            {
                speed *= 2f;
                turnaroundModifier *= 2f;
                maxSpeed *= 1.5f;
            }
            if (Math.Abs(NPC.Center.X - target.X) > 10f)
            {
                if (NPC.Center.X < target.X)
                {
                    NPC.velocity.X += speed;
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X += speed * (!fastX ? 1 : 2) * turnaroundModifier;
                }
                else
                {
                    NPC.velocity.X -= speed;
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X -= speed * (!fastX ? 1 : 2) * turnaroundModifier;
                }
            }
            if (NPC.Center.Y < target.Y)
            {
                NPC.velocity.Y += speed;
                if (NPC.velocity.Y < 0f)
                    NPC.velocity.Y += speed * 2f * turnaroundModifier;
            }
            else
            {
                NPC.velocity.Y -= speed;
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y -= speed * 2f * turnaroundModifier;
            }
            if (obeySpeedCap)
            {
                if (Math.Abs(NPC.velocity.X) > maxSpeed)
                    NPC.velocity.X = maxSpeed * Math.Sign(NPC.velocity.X);
                if (Math.Abs(NPC.velocity.Y) > maxSpeed)
                    NPC.velocity.Y = maxSpeed * Math.Sign(NPC.velocity.Y);
            }
        }

        public static void DramaticTransition(NPC NPC, bool fightIsOver, bool normalAnimation = true)
        {
            NPC.velocity = Vector2.Zero;

            if (fightIsOver)
            {
                Main.player[NPC.target].ClearBuff(ModContent.BuffType<MutantFangBuff>());
                Main.player[NPC.target].ClearBuff(ModContent.BuffType<AbomRebirthBuff>());
            }

            SoundStyle item = SoundID.Item27;
            item.Volume = 1.5f;
            SoundEngine.PlaySound(in item, (Vector2?)NPC.Center);

            if (normalAnimation && Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), 0, 0f, Main.myPlayer);

            float totalAmountToHeal = fightIsOver ? Main.player[NPC.target].statLifeMax2 / 4f : NPC.lifeMax - NPC.life + NPC.lifeMax * 0.1f;
            for (int i = 0; i < 40; i++)
            {
                int heal = (int)(Main.rand.NextFloat(0.9f, 1.1f) * totalAmountToHeal / 40f);
                Vector2 vel = normalAnimation ? Main.rand.NextFloat(2f, 18f) * -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) : 0.1f * -Vector2.UnitY.RotatedBy((double)((float)Math.PI / 20f * i), default);
                float ai0 = fightIsOver ? -Main.player[NPC.target].whoAmI - 1 : NPC.whoAmI;
                float ai1 = vel.Length() / Main.rand.Next(fightIsOver ? 90 : 150, 180);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, vel, ModContent.ProjectileType<MutantHeal>(), heal, 0f, Main.myPlayer, ai0, ai1);
            }
        }

        public static void EModeSpecialEffects(NPC NPC)
        {
            if (!WorldSavingSystem.EternityMode)
                return;

            if (Main.GameModeInfo.IsJourneyMode && CreativePowerManager.Instance.GetPower<FreezeTime>().Enabled)
                CreativePowerManager.Instance.GetPower<FreezeTime>().SetPowerInfo(false);

            if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss", default, Array.Empty<object>());

            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                if (WorldSavingSystem.MasochistModeReal && musicMod.Version >= Version.Parse("0.1.1"))
                    NPC.ModNPC.Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Storia");
                else
                    NPC.ModNPC.Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/rePrologue");
            }
        }

        public static void TryMasoP3Theme(NPC npc)
        {
            if (WorldSavingSystem.MasochistModeReal && ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod) && musicMod.Version >= Version.Parse("0.1.1.3"))
                npc.ModNPC.Music = MusicLoader.GetMusicSlot((Mod)(object)ModContent.GetInstance<MasomodeEX>(), "Assets/Music/MutantPhase2");
        }

        public static void FancyFireballs(NPC NPC, int repeats)
        {
            float modifier = 0f;
            for (int j = 0; j < repeats; j++)
                modifier = MathHelper.Lerp(modifier, 1f, 0.08f);

            float distance = 1600f * (1f - modifier);
            float rotation = (float)Math.PI * 2f * modifier;
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(NPC.Center + distance * Vector2.UnitX.RotatedBy((double)(rotation + (float)Math.PI / 3f * i), default), 0, 0, DustID.Vortex, NPC.velocity.X * 0.3f, NPC.velocity.Y * 0.3f, 0, Color.White, 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 6f - 4f * modifier;
            }
        }

        public static bool Phase3Transition(NPC NPC, Player player)
        {
            bool retval = true;
            NPC.localAI[3] = 3f;
            EModeSpecialEffects(NPC);

            if (NPC.buffType[0] != 0)
                NPC.DelBuff(0);

            if (NPC.ai[1] == 0f)
            {
                NPC.life = NPC.lifeMax;
                DramaticTransition(NPC, fightIsOver: true);
            }

            if (NPC.ai[1] < 60f && !Main.dedServ && Main.LocalPlayer.active)
                ScreenShakeSystem.StartShake(15f, (float)Math.PI * 2f, null, 7.5f);

            if (NPC.ai[1] == 360f)
                SoundEngine.PlaySound(SoundID.Roar, null, null);

            if ((NPC.ai[1] += 1f) > 480f)
            {
                retval = false;
                if (!AliveCheck(NPC, player))
                    return retval;

                Vector2 targetPos = player.Center;
                targetPos.Y -= 300f;
                Movement(NPC, targetPos, 1f, fastX: true, obeySpeedCap: false);
                if (NPC.Distance(targetPos) < 50f || NPC.ai[1] > 720f)
                {
                    NPC.netUpdate = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.localAI[0] = 0f;
                    NPC.ai[0] -= 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = NPC.DirectionFrom(player.Center).ToRotation();
                    NPC.ai[3] = (float)Math.PI / 20f;
                    SoundEngine.PlaySound(SoundID.Roar, null, null);
                    if (player.Center.X < NPC.Center.X)
                        NPC.ai[3] *= -1f;
                }
            }
            else
            {
                NPC.velocity *= 0.9f;

                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000f)
                {
                    Main.LocalPlayer.controlUseItem = false;
                    Main.LocalPlayer.controlUseTile = false;
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = 0;
                }

                if ((NPC.localAI[0] -= 1f) < 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(15);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(null), spawnPos, Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Dust obj = Main.dust[d];
                obj.velocity *= 4f;
            }

            return retval;
        }

        public static void VoidRaysP3(NPC NPC)
        {
            if ((NPC.ai[1] -= 1f) < 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float speed = WorldSavingSystem.MasochistModeReal && NPC.localAI[0] <= 40f ? 4f : 2f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, speed * Vector2.UnitX.RotatedBy(NPC.ai[2], default), ModContent.ProjectileType<MutantMark1>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f, 2), 0f, Main.myPlayer, 0f, 0f, 0f);
                }

                NPC.ai[1] = 1f;
                NPC.ai[2] += NPC.ai[3];

                if (NPC.localAI[0] < 30f)
                {
                    EModeSpecialEffects(NPC);
                    TryMasoP3Theme(NPC);
                }

                if (NPC.localAI[0]++ == 40f || NPC.localAI[0] == 80f || NPC.localAI[0] == 120f)
                {
                    NPC.netUpdate = true;
                    NPC.ai[2] -= NPC.ai[3] / (WorldSavingSystem.MasochistModeReal ? 3 : 2);
                }
                else if (NPC.localAI[0] >= (WorldSavingSystem.MasochistModeReal ? 160 : 120))
                {
                    NPC.netUpdate = true;
                    NPC.ai[0] -= 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        public static void OkuuSpheresP3(NPC NPC, Player player)
        {
            if (NPC.ai[2] == 0f)
            {
                if (!AliveCheck(NPC, player))
                    return;

                NPC.ai[2] = !Main.rand.NextBool() ? 1 : -1;
                NPC.ai[3] = Main.rand.NextFloat((float)Math.PI * 2f);
            }

            int endTime = 480;
            if (WorldSavingSystem.MasochistModeReal)
                endTime += 360;

            if ((NPC.ai[1] += 1f) > 10f && NPC.ai[3] > 60f && NPC.ai[3] < endTime - 120)
            {
                NPC.ai[1] = 0f;
                float rotation = MathHelper.ToRadians(45f) * (NPC.ai[3] - 60f) / 240f * NPC.ai[2];
                int max = WorldSavingSystem.MasochistModeReal ? 11 : 10;
                float speed = WorldSavingSystem.MasochistModeReal ? 11f : 10f;
                SpawnSphereRing(NPC, max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f, 2), -0.75f, rotation);
                SpawnSphereRing(NPC, max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f, 2), 0.75f, rotation);
            }

            if (NPC.ai[3] < 30f)
            {
                EModeSpecialEffects(NPC);
                TryMasoP3Theme(NPC);
            }

            if ((NPC.ai[3] += 1f) > endTime)
            {
                NPC.netUpdate = true;
                NPC.ai[0] -= 1f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Dust obj = Main.dust[d];
                obj.velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        public static void BoundaryBulletHellP3(NPC NPC, Player player)
        {
            if (NPC.localAI[0] == 0f)
            {
                if (!AliveCheck(NPC, player))
                    return;

                NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
            }

            if ((NPC.ai[1] += 1f) > 3f)
            {
                SoundEngine.PlaySound(SoundID.Item12);
                NPC.ai[1] = 0f;
                NPC.ai[2] += 0.0014959965f * NPC.ai[3] * NPC.localAI[0] * (WorldSavingSystem.MasochistModeReal ? 2f : 1f);
                if (NPC.ai[2] > (float)Math.PI)
                    NPC.ai[2] -= (float)Math.PI * 2f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = WorldSavingSystem.MasochistModeReal ? 10 : 8;
                    for (int j = 0; j < max; j++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, new Vector2(0f, -6f).RotatedBy((double)(NPC.ai[2] + (float)Math.PI * 2f / max * j), default), ModContent.ProjectileType<MutantEye>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f, 2), 0f, Main.myPlayer, 0f, 0f, 0f);
                }
            }

            if (NPC.ai[3] < 30f)
            {
                EModeSpecialEffects(NPC);
                TryMasoP3Theme(NPC);
            }

            int endTime = 360;
            if (WorldSavingSystem.MasochistModeReal)
                endTime += 360;

            if ((NPC.ai[3] += 1f) > endTime)
            {
                NPC.ai[0] -= 1f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.localAI[0] = 0f;
                NPC.netUpdate = true;
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Dust obj = Main.dust[d];
                obj.velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        public static void FinalSpark(NPC NPC, Player player)
        {
            if (NPC.localAI[2] > 30f)
            {
                NPC.localAI[2] += 1f;
                if (NPC.localAI[2] > 120f)
                    AliveCheck(NPC, player, true);
                return;
            }

            if ((NPC.localAI[0] -= 1f) < 0f)
            {
                NPC.localAI[0] = Main.rand.Next(30);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), spawnPos, Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                }
            }

            int ringTime = 120;
            if ((NPC.ai[1] += 1f) > ringTime)
            {
                NPC.ai[1] = 0f;
                EModeSpecialEffects(NPC);
                TryMasoP3Theme(NPC);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = 10;
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f, 2);
                    SpawnSphereRing(NPC, max, 6f, damage, 0.5f);
                    SpawnSphereRing(NPC, max, 6f, damage, -0.5f);
                }
            }

            if (NPC.ai[2] == 0f)
                NPC.localAI[1] = 1f;
            else if (NPC.ai[2] == 330f)
            {
                if (NPC.localAI[1] == 0f)
                {
                    NPC.localAI[1] = 1f;
                    NPC.ai[2] -= 780f;
                    NPC.ai[3] -= MathHelper.ToRadians(20f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3], default), ModContent.ProjectileType<MutantGiantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f, 2), 0f, Main.myPlayer, 0f, NPC.whoAmI, 0f);
                    NPC.netUpdate = true;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Roar, null, null);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            float offset = j - 0.5f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, (NPC.ai[3] + (float)Math.PI / 4f * offset).ToRotationVector2(), ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 13f, NPC.whoAmI, 0f);
                        }
                    }
                }
            }

            if (NPC.ai[2] < 420f)
            {
                if (NPC.localAI[1] == 0f || NPC.ai[2] > 330f)
                    NPC.ai[3] = NPC.DirectionFrom(player.Center).ToRotation();
            }
            else
            {
                if (!Main.dedServ && !Filters.Scene["FargowiltasSouls:FinalSpark"].IsActive())
                    Filters.Scene.Activate("FargowiltasSouls:FinalSpark", default, Array.Empty<object>());
                if (NPC.ai[1] % 3f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3], default), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer, Main.rand.NextFloat(0.5f, 1.25f) * (!Main.rand.NextBool() ? 1 : -1), Main.rand.Next(10, 60), 0f);
            }

            int endTime = 1020;
            if ((NPC.ai[2] += 1f) > endTime)
            {
                NPC.netUpdate = true;
                NPC.ai[0] -= 1f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.localAI[0] = 0f;
                NPC.localAI[1] = 0f;
                NPC.localAI[2] = 0f;
                FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI, false);
            }
            else if (NPC.ai[2] == 420f)
            {
                NPC.netUpdate = true;
                NPC.ai[3] += MathHelper.ToRadians(20f) * -1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3], default), ModContent.ProjectileType<MutantGiantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f, 2), 0f, Main.myPlayer, 0f, NPC.whoAmI, 0f);
            }
            else if (NPC.ai[2] < 300f && NPC.localAI[1] != 0f)
            {
                float num = 0.99f;
                if (NPC.ai[2] >= 60f)
                    num = 0.79f;
                if (NPC.ai[2] >= 120f)
                    num = 0.58f;
                if (NPC.ai[2] >= 180f)
                    num = 0.43f;
                if (NPC.ai[2] >= 240f)
                    num = 0.33f;

                for (int i = 0; i < 9; i++)
                {
                    if (Main.rand.NextFloat() >= num)
                    {
                        float f = Main.rand.NextFloat() * 6.283185f;
                        float num2 = Main.rand.NextFloat();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + f.ToRotationVector2() * (110f + 600f * num2), 229, (Vector2?)((f - 3.141593f).ToRotationVector2() * (14f + 8f * num2)), 0, default, 1f);
                        dust.scale = 0.9f;
                        dust.fadeIn = 1.15f + num2 * 0.3f;
                        dust.noGravity = true;
                    }
                }
            }

            SpinLaser(useMasoSpeed: false);
            if (AliveCheck(NPC, player))
                NPC.localAI[2] = 0f;
            else
                NPC.localAI[2] += 1f;

            NPC.velocity = Vector2.Zero;

            void SpinLaser(bool useMasoSpeed)
            {
                float newRotation = NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation();
                float difference = MathHelper.WrapAngle(newRotation - NPC.ai[3]);
                float rotationDirection = (float)Math.PI / 180f;
                rotationDirection *= useMasoSpeed ? 0.525f : 1f;
                NPC.ai[3] += Math.Min(rotationDirection, Math.Abs(difference)) * Math.Sign(difference);
                if (useMasoSpeed)
                    NPC.ai[3] = NPC.ai[3].AngleLerp(newRotation, 0.015f);
            }
        }

        public static void DyingDramaticPause(NPC NPC, Player player)
        {
            if (!AliveCheck(NPC, player))
                return;

            NPC.ai[3] -= (float)Math.PI / 360f;
            NPC.velocity = Vector2.Zero;

            if ((NPC.ai[1] += 1f) > 120f)
            {
                NPC.netUpdate = true;
                NPC.ai[0] -= 1f;
                NPC.ai[1] = 0f;
                NPC.ai[3] = -(float)Math.PI / 2f;
                NPC.netUpdate = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, Vector2.UnitY * -1f, ModContent.ProjectileType<MutantGiantDeathray2>(), 0, 0f, Main.myPlayer, 1f, NPC.whoAmI, 0f);
            }

            if ((NPC.localAI[0] -= 1f) < 0f)
            {
                NPC.localAI[0] = Main.rand.Next(15);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                    int type = ModContent.ProjectileType<PhantasmalBlast>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Dust obj = Main.dust[d];
                obj.velocity *= 4f;
            }
        }

        public static void DyingAnimationAndHandling(NPC NPC, Player player)
        {
            NPC.velocity = Vector2.Zero;

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0f, 0f, 0, default, 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 12f;
            }

            if ((NPC.localAI[0] -= 1f) < 0f)
            {
                NPC.localAI[0] = Main.rand.Next(5);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(240f, 240f);
                    int type = ModContent.ProjectileType<PhantasmalBlast>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(null), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                }
            }

            if ((NPC.ai[1] += 1f) % 3f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromThis(null), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3], default), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer, Main.rand.NextFloat(0.75f, 1.5f) * (!Main.rand.NextBool() ? 1 : -1), Main.rand.Next(10, 90), 0f);

            if (++NPC.alpha <= 255)
                return;

            NPC.alpha = 255;
            NPC.life = 0;
            NPC.dontTakeDamage = false;
            NPC.checkDead();

            if (Main.netMode == NetmodeID.MultiplayerClient || !ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC npc) || NPC.AnyNPCs(npc.Type))
                return;

            int j = NPC.NewNPC(NPC.GetSource_FromAI(null), (int)NPC.Center.X, (int)NPC.Center.Y, npc.Type, 0, 0f, 0f, 0f, 0f, 255);
            if (j != 200)
            {
                Main.npc[j].homeless = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
                }
            }
        }
    }
}