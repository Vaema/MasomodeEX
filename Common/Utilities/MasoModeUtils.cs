using System.Linq;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;

namespace MasomodeEX.Common.Utilities
{
    public static class MasoModeUtils
    {
        public static bool checkIfMasoEX()
        {
            if (!WorldSavingSystem.MasochistModeReal && !Condition.ForTheWorthyWorld.IsMet() && !Condition.NotZenithWorld.IsMet())
                return false;

            return true;
        }

        public static int NewNPCEasy(IEntitySource source, Vector2 spawnPos, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return 200;

            int i = NPC.NewNPC(source, (int)spawnPos.X, (int)spawnPos.Y, type, start, ai0, ai1, ai2, ai3, target);
            if (i != Main.npc.Length - 1)
            {
                if (velocity != default)
                    Main.npc[i].velocity = velocity;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
            }

            return i;
        }

        public static bool AnyBossAlive()
        {
            int boss = FargoSoulsGlobalNPC.boss;
            if (boss == -1)
                return false;
            if (Main.npc[boss].active && (Main.npc[boss].boss || Main.npc[boss].type == NPCID.EaterofWorldsHead))
                return true;

            FargoSoulsGlobalNPC.boss = -1;

            return false;
        }

        public static NPC NPCExists(int whoAmI, params int[] types)
        {
            if (whoAmI <= -1 || whoAmI >= 200 || !Main.npc[whoAmI].active || types.Length != 0 && !types.Contains(Main.npc[whoAmI].type))
                return null;

            return Main.npc[whoAmI];
        }

        public static NPC NPCExists(float whoAmI, params int[] types) => NPCExists((int)whoAmI, types);

        public static void Talk(string message, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(message, color);
            else
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
        }
    }
}