using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MasomodeEX.Common.Configs
{
    public class MasoEXClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header($"Misc")]
        [DefaultValue(true)]
        public bool GreetingMessage;
    }
}
