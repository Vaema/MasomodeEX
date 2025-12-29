using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls;

namespace MasomodeEX.Content.Buffs
{
	public class MutantJudgement : ModBuff
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mutant Judgement");
            //Description.SetDefault("You have been deemed unworthy");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }

		public override void Update(Player player, ref int buffIndex)
        {
            player.immune = false;
            player.immuneTime = 0;
            player.hurtCooldowns[0] = 0;
            player.hurtCooldowns[1] = 0;
            player.moonLeech = true;
            player.chaosState = true;
            player.potionDelay = player.buffTime[buffIndex];
            player.FargoSouls().noDodge = true;
            player.FargoSouls().noSupersonic = true;
            player.FargoSouls().MutantPresence = true;
            player.FargoSouls().MutantNibble = true;
        }
    }
}
