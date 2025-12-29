using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MasomodeEX.Common.Globals
{
    class MasomodeEXGlobalBuff : GlobalBuff
    {
        //public override bool ReApply(int type, Player player, int time, int buffIndex)
        //{
        //    if (Main.debuff[type] && type != BuffID.PeaceCandle
        //        && player.buffTime[buffIndex] < 18000 + 60) //extra second to hopefully prevent duration flickering
        //        player.buffTime[buffIndex] += time;
        //    return false;
        //}

        public override void Update(int type, Player player, ref int buffIndex)
        {
            switch (type)
            {
                case BuffID.Rabies:
                    if (player.buffTime[buffIndex] > 1)
                    {
                        player.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), player.buffTime[buffIndex]);
                        player.buffTime[buffIndex] = 1;
                    }
                    break;

                case BuffID.Frozen:
                case BuffID.Stoned:
                    if (player.buffTime[buffIndex] > 3600)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("Mods.MasomodeEX.PlayerDeathReasons.Shatter", player.name)), 9999, 0);
                    }
                    break;

                case BuffID.MoonLeech:
                    player.buffType[buffIndex] = ModContent.BuffType<MutantNibbleBuff>();
                    break;

                default:
                    break;
            }
        }
    }
}
