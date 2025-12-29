using System.Collections.Generic;
using System.IO;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using MasomodeEX.Common.Utilities;

namespace MasomodeEX.Common.Globals
{
    internal class MasomodeEXWorld : ModSystem
    {
        public static bool MasoquistModeEX = true;
        public static int MutantSummons;
        public static int MutantDefeats;
        public static int MutantPlayerKills;
        public static bool ForcedBM = false;
        public static bool ForcedSE = false;

        public override void OnWorldLoad()
        {
            MutantSummons = 0;
            MutantDefeats = 0;
            MutantPlayerKills = 0;
            ForcedBM = false;
            ForcedSE = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<int> mutant = [MutantSummons, MutantDefeats, MutantPlayerKills];
            tag.Add("mutant", mutant);
            tag.Add("ForcedBM", ForcedBM);
            tag.Add("ForcedSE", ForcedSE);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("mutant"))
            {
                IList<int> list = tag.GetList<int>("mutant");
                MutantSummons = list[0];
                MutantDefeats = list[1];
                MutantPlayerKills = list[2];
            }

            if (tag.ContainsKey("ForcedBM"))
                ForcedBM = tag.GetBool("ForcedBM");
            if (tag.ContainsKey("ForcedSE"))
                ForcedSE = tag.GetBool("ForcedSE");
        }

        public override void NetReceive(BinaryReader reader)
        {
            MutantSummons = reader.ReadInt32();
            MutantDefeats = reader.ReadInt32();
            MutantPlayerKills = reader.ReadInt32();
            ForcedBM = reader.ReadBoolean();
            ForcedSE = reader.ReadBoolean();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(MutantSummons);
            writer.Write(MutantDefeats);
            writer.Write(MutantPlayerKills);
            writer.Write(ForcedBM);
            writer.Write(ForcedSE);
        }

        public override void PreUpdateWorld()
        {
            if (!MasoModeUtils.checkIfMasoEX())
                return;

            if (Main.dayTime && Main.time == 0.0)
            {
                if (Main.rand.NextBool(4))
                {
                    ForcedSE = true;
                }
                else
                    ForcedSE = false;
                ForcedBM = false;
            }

            if (!Main.dayTime && Main.time == 0.0)
            {
                if (WorldSavingSystem.DownedAnyBoss)
                {
                    if (Main.rand.NextBool(4))
                    {
                        ForcedBM = true;
                    }
                    else
                        ForcedBM = false;
                }
                ForcedSE = false;
            }

            if (!Main.dayTime && ForcedBM && WorldSavingSystem.DownedAnyBoss)
            {
                Main.bloodMoon = true;
                Main.eclipse = false;
            }
            else if (Main.hardMode && ForcedSE)
            {
                Main.eclipse = true;
                Main.bloodMoon = false;
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }
    }
}