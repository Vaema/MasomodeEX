using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Projectiles.Masomode;

namespace MasomodeEX.Common.Globals
{
    public class MasomodeEXGlobalItem : GlobalItem
    {
        public bool spawned;
        public int lavaCounter;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (!spawned)
            {
                spawned = true;

                if (item.type == ItemID.Heart && Main.rand.NextBool(4) && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(null, item.Center.X, item.Center.Y, Main.rand.Next(-30, 31) * 0.1f, Main.rand.Next(-40, -15) * 0.1f, ModContent.ProjectileType<FakeHeart>(), 40, 0f, Main.myPlayer);
            }

            if (item.lavaWet && ++lavaCounter > 5)
                item.active = false;
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type != ItemID.GuideVoodooDoll || !player.lavaWet)
                return;

            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if (guide != -1 && Main.npc[guide].active)
            {
                if (--item.stack <= 0)
                    item.SetDefaults(ItemID.None);

                Main.npc[guide].StrikeInstantKill();
                if (player.ZoneUnderworldHeight)
                    NPC.SpawnWOF(player.Center);
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (MasomodeEX.Instance.LuiLoaded)
            {
                if ((item.type == ModLoader.GetMod("Luiafk").Find<ModItem>("UnlimitedGrandDesign").Type || item.type == ModLoader.GetMod("Luiafk").Find<ModItem>("ComboRod").Type))
                    return false;
            }

            switch (item.type)
            {
                case ItemID.WireCutter:
                case ItemID.WireKite:
                    if (player.ZoneJungle && !NPC.downedGolemBoss)
                        return false;
                    break;

                default:
                    break;
            }

            return true;
        }
    }
}