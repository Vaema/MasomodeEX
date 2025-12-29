using FargowiltasSouls.Content.Bosses.MutantBoss;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX.Content.Items
{
    public class MutantSummon : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
        }

        public override void UpdateInventory(Player player)
        {
            if (--Item.stack < 1)
                Item.SetDefaults(0);

            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<MutantBoss>());
        }

        public override bool CanPickup(Player player) => true;

        public override bool OnPickup(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<MutantBoss>());
            return false;
        }
    }
}