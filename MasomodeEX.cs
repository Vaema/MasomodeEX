using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX
{
    public class MasomodeEX : Mod
    {
        internal static Mod Souls;
        internal static Mod Fargo;
        internal bool HyperLoaded;
        internal bool VeinMinerLoaded;
        internal bool LuiLoaded;

        internal static List<int> DebuffIDs;

        internal static MasomodeEX Instance { get; private set; }

        public override void Load()
        {
            Instance = this;

            Main.getGoodWorld = true;
            
            WorldGen.noTrapsWorldGen = true;
            WorldGen.notTheBees = true;
            WorldGen.getGoodWorldGen = true;
            WorldGen.tempTenthAnniversaryWorldGen = true;
            WorldGen.dontStarveWorldGen = true;
            WorldGen.tempRemixWorldGen = true;
            WorldGen.everythingWorldGen = true;
        }

        public override void Unload()
        {
            Instance = null;
            DebuffIDs?.Clear();
        }

        public override void PostSetupContent()
        {
            Souls = ModLoader.GetMod("FargowiltasSouls");
            Fargo = ModLoader.GetMod("Fargowiltas");
            HyperLoaded = ModLoader.HasMod("HyperMode");
            VeinMinerLoaded = ModLoader.HasMod("OreExcavator");
            LuiLoaded = ModLoader.HasMod("miningcracks_take_on_luiafk");
            DebuffIDs =
            [
                20,
                22,
                23,
                24,
                36,
                39,
                44,
                46,
                47,
                67,
                68,
                69,
                70,
                80,
                88,
                94,
                103,
                137,
                144,
                145,
                148,
                149,
                156,
                156,
                163,
                164,
                195,
                196,
                156,
                199,
                Souls.Find<ModBuff>("AntisocialBuff").Type,
                Souls.Find<ModBuff>("AtrophiedBuff").Type,
                Souls.Find<ModBuff>("BerserkedBuff").Type,
                Souls.Find<ModBuff>("BloodthirstyBuff").Type,
                Souls.Find<ModBuff>("ClippedWingsBuff").Type,
                Souls.Find<ModBuff>("CrippledBuff").Type,
                Souls.Find<ModBuff>("CurseoftheMoonBuff").Type,
                Souls.Find<ModBuff>("DefenselessBuff").Type,
                Souls.Find<ModBuff>("FlamesoftheUniverseBuff").Type,
                Souls.Find<ModBuff>("FlippedBuff").Type,
                Souls.Find<ModBuff>("HallowIlluminatedBuff").Type,
                Souls.Find<ModBuff>("FusedBuff").Type,
                Souls.Find<ModBuff>("GodEaterBuff").Type,
                Souls.Find<ModBuff>("GuiltyBuff").Type,
                Souls.Find<ModBuff>("HexedBuff").Type,
                Souls.Find<ModBuff>("InfestedBuff").Type,
                Souls.Find<ModBuff>("IvyVenomBuff").Type,
                Souls.Find<ModBuff>("JammedBuff").Type,
                Souls.Find<ModBuff>("LethargicBuff").Type,
                Souls.Find<ModBuff>("LightningRodBuff").Type,
                Souls.Find<ModBuff>("LivingWastelandBuff").Type,
                Souls.Find<ModBuff>("LovestruckBuff").Type,
                Souls.Find<ModBuff>("MarkedforDeathBuff").Type,
                Souls.Find<ModBuff>("MidasBuff").Type,
                Souls.Find<ModBuff>("MutantNibbleBuff").Type,
                Souls.Find<ModBuff>("NullificationCurseBuff").Type,
                Souls.Find<ModBuff>("OiledBuff").Type,
                Souls.Find<ModBuff>("OceanicMaulBuff").Type,
                Souls.Find<ModBuff>("PurifiedBuff").Type,
                Souls.Find<ModBuff>("ReverseManaFlowBuff").Type,
                Souls.Find<ModBuff>("RottingBuff").Type,
                Souls.Find<ModBuff>("ShadowflameBuff").Type,
                Souls.Find<ModBuff>("SqueakyToyBuff").Type,
                Souls.Find<ModBuff>("StunnedBuff").Type,
                Souls.Find<ModBuff>("SwarmingBuff").Type,
                Souls.Find<ModBuff>("UnstableBuff").Type,
                Souls.Find<ModBuff>("MutantFangBuff").Type,
                Souls.Find<ModBuff>("MutantPresenceBuff").Type,
                Souls.Find<ModBuff>("TimeFrozenBuff").Type
            ];
        }
    }
}