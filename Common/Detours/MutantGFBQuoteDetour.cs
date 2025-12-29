using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX.Common.Detours
{
    public class MutantGFBQuoteDetour : ILoadable
    {
        private Hook GFBQuoteHook;

        public void Load(Mod mod)
        {
            var gfbQuote = typeof(MutantBoss).GetMethod("EdgyBossText", BindingFlags.NonPublic | BindingFlags.Instance);

            GFBQuoteHook = new Hook(gfbQuote, GFBQuote);
            GFBQuoteHook.Apply();
        }

        public void Unload() { } // tmod doing that for us

        private delegate void orig_GFBQuote(MutantBoss self, string text);
        private void GFBQuote(orig_GFBQuote orig, MutantBoss self, string text)
        {
            if (!Main.zenithWorld)
            {
                Color color = Color.Cyan;
                FargoSoulsUtil.PrintText(text, color);
                CombatText.NewText(self.NPC.Hitbox, color, text, true);
            }
            else
            {
                orig(self, text);
            }
        }
    }
}
