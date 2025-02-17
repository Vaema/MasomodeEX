using Terraria.ModLoader;

namespace MasomodeEX.Common.Globals
{
    public class OldEModeGlobalNPC : GlobalNPC
    {
        public bool[] masoBool = new bool[4];
        public int[] Counter = new int[4];

        public override bool InstancePerEntity => true;
    }
}