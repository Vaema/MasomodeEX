using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using MasomodeEX.Common.Utilities;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Souls;
using MasomodeEX.Content.Buffs;
using MasomodeEX.Content.Items;
using MasomodeEX.Common.Configs;

namespace MasomodeEX.Common.Globals
{
    internal class MasomodeEXPlayer : ModPlayer
    {
        public int hitcap = 25;

        public override void PreUpdate()
        {
            if (!MasoModeUtils.checkIfMasoEX())
                return;

            if (Player.lavaWet)
            {
                Player.AddBuff(BuffID.Burning, 2);
                if (Player.ZoneUnderworldHeight)
                    Player.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 2);
            }

            if (Player.wet)
                Player.AddBuff(ModContent.BuffType<LethargicBuff>(), 2);

            if (Player.honeyWet)
                Player.AddBuff(BuffID.Slow, 2);

            if (Player.adjLava)
                Player.AddBuff(BuffID.OnFire, 2);

            
            Tile currentTile = Framing.GetTileSafely(Player.Center);

            if (currentTile.WallType == WallID.GraniteUnsafe)
                Player.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 2);

            if (currentTile.WallType == WallID.MarbleUnsafe)
                Player.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 2);

            if (currentTile.WallType == WallID.LihzahrdBrickUnsafe)
                Player.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

            if ((currentTile.TileType == TileID.Trees || currentTile.TileType == TileID.PalmTree) && Player.hurtCooldowns[0] <= 0)
                Player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.MasomodeEX.PlayerDeathReasons.Tree", Player.name)), 20, 0);

            if (currentTile.TileType == TileID.DemonAltar && Player.hurtCooldowns[0] <= 0)
            {
                int def = Player.statDefense;
                float end = Player.endurance;
                Player.statDefense -= def;
                Player.endurance = 0f;
                Player.Hurt(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.MasomodeEX.PlayerDeathReasons.DemonAltar", Player.name)), Player.statLife / 2, 0);
                Player.statDefense += def;
                Player.endurance = end;
            }

            if (Player.ZoneOverworldHeight || Player.ZoneSkyHeight)
            {
                if (Main.dayTime)
                {
                    if (Main.eclipse)
                        Player.AddBuff(ModContent.BuffType<LivingWastelandBuff>(), 2);
                }
                else
                {
                    Player.AddBuff(currentTile.WallType == WallID.None ? 80 : 22, 2);

                    if (Main.bloodMoon)
                        Player.AddBuff(ModContent.BuffType<BloodthirstyBuff>(), 2);

                    if (Main.pumpkinMoon)
                        Player.AddBuff(ModContent.BuffType<RottingBuff>(), 2);

                    if (Main.snowMoon)
                        Player.AddBuff(BuffID.Frostburn, 2);
                }

                if (Main.raining && currentTile.WallType == WallID.None)
                {
                    if (Player.ZoneSnow)
                    {
                        Player.AddBuff(BuffID.Chilled, 2);

                        int debuff = Player.FindBuffIndex(BuffID.Chilled);
                        if (debuff != -1 && Player.buffTime[debuff] > 7200)
                        {
                            Player.ClearBuff(BuffID.Chilled);
                            Player.AddBuff(BuffID.Frozen, Main.GameModeInfo.DebuffTimeMultiplier > 1f ? 150 : 300);
                        }

                        Player.AddBuff(BuffID.Frostburn, 2);
                    }
                    else
                    {
                        Player.AddBuff(BuffID.Wet, 2);
                        Player.AddBuff(ModContent.BuffType<LightningRodBuff>(), 2);
                    }
                }
            }

            if (Player.ZoneSkyHeight && Player.breath > 0)
                Player.breath--;

            if (Player.ZoneUnderworldHeight && !Player.fireWalk && !Player.buffImmune[24])
                Player.AddBuff(BuffID.Burning, 2);

            if (Player.ZoneBeach)
            {
                if (Player.FargoSouls().MaxLifeReduction < 50)
                    Player.FargoSouls().MaxLifeReduction = 50;

                Player.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 2);

                if (Player.wet)
                    Player.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), 2);
            }
            else if (Player.ZoneDesert)
            {
                Player.AddBuff(BuffID.Weak, 2);

                if (Player.ZoneOverworldHeight && currentTile.WallType == WallID.None)
                    Player.AddBuff(BuffID.WindPushed, 2);
            }

            if (Player.ZoneJungle)
            {
                Player.AddBuff(ModContent.BuffType<SwarmingBuff>(), 2);

                if (Player.wet)
                {
                    Player.AddBuff(ModContent.BuffType<InfestedBuff>(), 2);

                    if (Main.hardMode)
                        Player.AddBuff(BuffID.Venom, 2);
                }
            }

            if (Player.ZoneSnow)
            {
                Player.AddBuff(BuffID.Chilled, 2);

                if (Player.wet)
                    Player.AddBuff(BuffID.Frostburn, 2);
            }

            if (Player.ZoneDungeon)
                Player.AddBuff(BuffID.WaterCandle, 2);

            if (Player.ZoneCorrupt)
            {
                Player.AddBuff(BuffID.Darkness, 2);

                if (Player.wet)
                    Player.AddBuff(BuffID.CursedInferno, 2);
            }

            if (Player.ZoneCrimson)
            {
                Player.AddBuff(BuffID.Bleeding, 2);

                if (Player.wet)
                    Player.AddBuff(BuffID.Ichor, 2);
            }
            
            if (Player.ZoneHallow)
            {
                Player.AddBuff(ModContent.BuffType<HallowIlluminatedBuff>(), 120);

                if (Player.wet)
                    Player.AddBuff(BuffID.Confused, 2);
            }

            if (Player.ZoneMeteor && !Player.fireWalk)
                Player.AddBuff(BuffID.OnFire, 2);

            if (Player.buffImmune[149] || Player.stickyBreak <= 0)
                return;

            Player.AddBuff(BuffID.Webbed, 30);
            Player.stickyBreak = 0;

            Vector2 vector = Collision.StickyTiles(Player.position, Player.velocity, Player.width, Player.height);
            if (vector.X == -1f || vector.Y == -1f)
                return;

            int vecX = (int)vector.X;
            int vecY = (int)vector.Y;
            WorldGen.KillTile(vecX, vecY);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (!Main.tile[vecX, vecY].HasTile)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, vecX, vecY);
            }
        }

        public override void UpdateDead()
        {
            if (MasoModeUtils.checkIfMasoEX())
                hitcap = 25;
        }

        public override void PostUpdateMiscEffects()
        {
            if (MasoModeUtils.checkIfMasoEX())
            {
                if (Framing.GetTileSafely(Player.Center).WallType == WallID.LihzahrdBrickUnsafe)
                {
                    Player.dangerSense = false;
                    Player.InfoAccMechShowWires = false;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (MasoModeUtils.checkIfMasoEX())
                info.Damage = (int)(info.Damage * 1.5);

            if (MasoModeUtils.checkIfMasoEX() && NPC.AnyNPCs(NPCID.MoonLordCore) && --hitcap <= 0)
            {
                Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.MasomodeEX.PlayerDeathReasons.LordHitCap", Player.name)), 19998.0, 0);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (MasoModeUtils.checkIfMasoEX() && damageSource.TryGetCausingEntity(out Entity entity))
            {
                NPC npc = (NPC)(object)(entity is NPC ? entity : null);
                if (npc != null && npc.type == ModContent.NPCType<MutantBoss>())
                    MasomodeEXWorld.MutantPlayerKills++;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (!MasoModeUtils.checkIfMasoEX())
                return;

            if (proj.type == ModContent.ProjectileType<MutantSpearAim>() || proj.type == ModContent.ProjectileType<MutantSpearDash>() || 
                proj.type == ModContent.ProjectileType<MutantSpearSpin>() || proj.type == ModContent.ProjectileType<MutantSpearThrown>())
            {
                Player.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), 60);
                Player.AddBuff(ModContent.BuffType<MutantJudgement>(), 3600);
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (MasoModeUtils.checkIfMasoEX() && Main.rand.NextBool(200))
                itemDrop = ModContent.ItemType<MutantSummon>();
        }

        public override void OnEnterWorld()
        {
            if (ModContent.GetInstance<MasoEXClientConfig>().GreetingMessage)
                Main.NewText(Language.GetTextValue("Mods.MasomodeEX.Messages.JoinWorld"), Color.Lime);
        }
    }
}