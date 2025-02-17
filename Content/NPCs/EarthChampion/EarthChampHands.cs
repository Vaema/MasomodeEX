using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using MasomodeEX.Common.Utilities;

namespace MasomodeEX.Content.NPCs.EarthChampion;

public class EarthChampHands : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        //IL_005f: Unknown result type (might be due to invalid IL or missing references)
        //IL_009b: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a1: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b0: Unknown result type (might be due to invalid IL or missing references)
        //IL_00ec: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f2: Unknown result type (might be due to invalid IL or missing references)
        if (!WorldSavingSystem.EternityMode)
        {
            return true;
        }
        if (npc.type == MasomodeEX.Souls.Find<ModNPC>("EarthChampion").Type && npc.ai[1] == 54f && npc.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
        {
            MasoModeUtils.NewNPCEasy(npc.GetSource_FromAI(null), npc.Center, MasomodeEX.Souls.Find<ModNPC>("EarthChampionHand").Type, npc.whoAmI, 0f, 0f, npc.whoAmI, 1f);
            MasoModeUtils.NewNPCEasy(npc.GetSource_FromAI(null), npc.Center, MasomodeEX.Souls.Find<ModNPC>("EarthChampionHand").Type, npc.whoAmI, 0f, 0f, npc.whoAmI, -1f);
        }
        return true;
    }
}
