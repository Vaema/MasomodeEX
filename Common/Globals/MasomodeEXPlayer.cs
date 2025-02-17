using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Player;
using Terraria.ID;
using MasomodeEX.Common.Utilities;
using FargowiltasSouls.Core.ModPlayers;

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
                Player.AddBuff(67, 2, true, false);
                if (Player.ZoneUnderworldHeight)
                    Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ShadowflameBuff").Type, 2, true, false);
            }

            if (Player.wet)
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("LethargicBuff").Type, 2, true, false);
            if (Player.honeyWet)
                Player.AddBuff(32, 2);
            if (Player.adjLava)
                Player.AddBuff(24, 2);

            Tile currentTile = Framing.GetTileSafely(Player.Center);
            if (currentTile.WallType == 180)
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("CrippledBuff").Type, 2, true, false);
            if (currentTile.WallType == 178)
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("ClippedWingsBuff").Type, 2);
            if (currentTile.WallType == 87)
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("LowGroundBuff").Type, 2);
            if ((currentTile.TileType == 5 || currentTile.TileType == 323) && Player.hurtCooldowns[0] <= 0)
                Player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.MasomodeEX.Death.Tree", Player.name)), 20, 0, false, false, 0, false, 0f, 0f, 4.5f);

            if (currentTile.TileType == 26 && Player.hurtCooldowns[0] <= 0)
            {
                int def = Player.statDefense;
                float end = Player.endurance;
                Player.statDefense -= def;
                Player.endurance = 0f;
                Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + " was slain."), Player.statLife / 2, 0, false, false, 0, false, 0f, 0f, 4.5f);
                Player.statDefense += def;
                Player.endurance = end;
            }

            if (Player.ZoneOverworldHeight || Player.ZoneSkyHeight)
            {
                if (Main.dayTime)
                {
                    if (Main.eclipse)
                        Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("LivingWastelandBuff").Type, 2);
                }
                else
                {
                    Player.AddBuff(currentTile.WallType == 0 ? 80 : 22, 2);
                    if (Main.bloodMoon)
                        Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("BloodthirstyBuff").Type, 2);
                    if (Main.pumpkinMoon)
                        Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("RottingBuff").Type, 2);
                    if (Main.snowMoon)
                        Player.AddBuff(44, 2);
                }

                if (Main.raining && currentTile.WallType == 0)
                {
                    if (Player.ZoneSnow)
                    {
                        Player.AddBuff(46, 2, true, false);
                        int debuff = Player.FindBuffIndex(46);
                        if (debuff != -1 && Player.buffTime[debuff] > 7200)
                        {
                            Player.ClearBuff(46);
                            Player.AddBuff(47, Main.GameModeInfo.DebuffTimeMultiplier > 1f ? 150 : 300);
                        }
                        Player.AddBuff(44, 2, true, false);
                    }
                    else
                    {
                        Player.AddBuff(103, 2, true, false);
                        Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("LightningRodBuff").Type, 2);
                    }
                }
            }

            if (Player.ZoneSkyHeight && Player.breath > 0)
                Player.breath--;

            if (Player.ZoneUnderworldHeight && !Player.fireWalk && !Player.buffImmune[24])
                Player.AddBuff(67, 2, true, false);

            if (Player.ZoneBeach)
            {
                if (Player.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction < 50)
                    Player.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction = 50;

                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("OceanicMaulBuff").Type, 2);
                if (Player.wet)
                    Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("MutantNibbleBuff").Type, 2);
            }
            else if (Player.ZoneDesert)
            {
                if (Player.ZoneOverworldHeight && currentTile.WallType == 0)
                    Player.AddBuff(194, 2, true, false);
                Player.AddBuff(33, 2, true, false);
            }

            if (Player.ZoneJungle)
            {
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("SwarmingBuff").Type, 2, true, false);
                if (Player.wet)
                {
                    Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("InfestedBuff").Type, 2, true, false);
                    if (Main.hardMode)
                        Player.AddBuff(70, 2, true, false);
                }
            }

            if (Player.ZoneSnow)
            {
                Player.AddBuff(46, 2, true, false);
                if (Player.wet)
                    Player.AddBuff(44, 2, true, false);
            }

            if (Player.ZoneDungeon)
                Player.AddBuff(86, 2, true, false);

            if (Player.ZoneCorrupt)
            {
                Player.AddBuff(22, 2, true, false);
                if (Player.wet)
                    Player.AddBuff(39, 2, true, false);
            }

            if (Player.ZoneCrimson)
            {
                Player.AddBuff(30, 2, true, false);
                if (Player.wet)
                    Player.AddBuff(69, 2, true, false);
            }
            
            if (Player.ZoneHallow)
            {
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("FlippedHallowBuff").Type, 120, true, false);
                if (Player.wet)
                    Player.AddBuff(31, 2, true, false);
            }

            if (Player.ZoneMeteor && !Player.fireWalk)
                Player.AddBuff(24, 2, true, false);

            if (Player.buffImmune[149] || Player.stickyBreak <= 0)
                return;

            Player.AddBuff(149, 30, true, false);
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
                if (Framing.GetTileSafely(Player.Center).WallType == 87)
                {
                    Player.dangerSense = false;
                    Player.InfoAccMechShowWires = false;
                }
            }
        }

        public override void OnHurt(HurtInfo info)
        {
            if (MasoModeUtils.checkIfMasoEX())
                info.Damage = (int)(info.Damage * 1.5);

            if (MasoModeUtils.checkIfMasoEX() && NPC.AnyNPCs(398) && --hitcap <= 0)
            {
                Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " got terminated."), 19998.0, 0, false);
                Main.NewText(Language.GetTextValue("Mods.MasomodeEX.Messages.FailedMLHits"), (Color?)Color.LimeGreen);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (MasoModeUtils.checkIfMasoEX() && damageSource.TryGetCausingEntity(out Entity entity))
            {
                NPC npc = (NPC)(object)(entity is NPC ? entity : null);
                if (npc != null && npc.type == Mod.Find<ModNPC>("MutantBoss").Type)
                    MasomodeEXWorld.MutantPlayerKills++;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref HurtModifiers modifiers)
        {
            if (MasoModeUtils.checkIfMasoEX() && (proj.type == MasomodeEX.Souls.Find<ModProjectile>("MutantSpearAim").Type || proj.type == MasomodeEX.Souls.Find<ModProjectile>("MutantSpearDash").Type || proj.type == MasomodeEX.Souls.Find<ModProjectile>("MutantSpearSpin").Type || proj.type == MasomodeEX.Souls.Find<ModProjectile>("MutantSpearThrown").Type))
            {
                Player.AddBuff(MasomodeEX.Souls.Find<ModBuff>("TimeFrozenBuff").Type, 60, true, false);
                Player.AddBuff(Mod.Find<ModBuff>("MutantJudgement").Type, 3600, true, false);
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (MasoModeUtils.checkIfMasoEX() && Main.rand.NextBool(200))
                itemDrop = Mod.Find<ModItem>("MutantSummon").Type;
        }

        public override void OnEnterWorld()
        {
            if (MasoModeUtils.checkIfMasoEX())
            {
                Main.NewText(Language.GetTextValue("Mods.MasomodeEX.Messages.JoinWorld"), (Color?)Color.Red);
                Main.NewText(Language.GetTextValue("Mods.MasomodeEX.Messages.JoinWorld1"), (Color?)Color.Red);
                Main.NewText(Language.GetTextValue("Mods.MasomodeEX.Messages.JoinWorld2"), (Color?)Color.Red);
            }
        }
    }
}