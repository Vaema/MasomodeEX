using MasomodeEX.Common.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX.Common.Globals
{
    public class MasoMutantMusic : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            //if (MasoModeUtils.checkIfMasoEX() && npc.type == MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type)
            //{
            //    string phase1 = "Assets/Sounds/Music/MutantPhase1";
            //    string phase2 = "Assets/Sounds/Music/MutantPhase2";

            //    npc.ModNPC.Music = MusicLoader.GetMusicSlot((Mod)(object)MasomodeEX.Instance, npc.localAI[3] > 1f ? phase1 : phase2);
            //}
            return true;
        }
    }
}