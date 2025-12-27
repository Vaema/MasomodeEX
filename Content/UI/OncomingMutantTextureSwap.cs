using FargowiltasSouls.Content.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Reflection;
using Terraria.ModLoader;

namespace MasomodeEX.Content.UI
{
    public class OncomingMutantTextureSwap : ILoadable
    {
        public static Asset<Texture2D> MasoEXTexture;
        public static Asset<Texture2D> NormalTexture;

        public void Load(Mod mod)
        {
            NormalTexture = FargoUIManager.OncomingMutantAuraTexture;
            MasoEXTexture = mod.Assets.Request<Texture2D>("Content/UI/OncomingMutantAura");

            var info = typeof(FargoUIManager).GetProperty("OncomingMutantAuraTexture", BindingFlags.Public | BindingFlags.Static);
            info.SetValue(null, MasoEXTexture);
        }

        public void Unload() 
        {
            var info = typeof(FargoUIManager).GetProperty("OncomingMutantAuraTexture", BindingFlags.Public | BindingFlags.Static);
            info.SetValue(null, NormalTexture);

            MasoEXTexture = null;
            NormalTexture = null;
        }
    }
}
