using System;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MasomodeEX.Content.NPCs.Eridanus
{
    public class EridanusMoon : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode)
                return true;

            if (npc.type == MasomodeEX.Souls.Find<ModNPC>("CosmosChampion").Type && npc.ai[1] == 0f && npc.ai[0] == -3f)
            {
                Player player = Main.player[npc.target];
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = 4;
                    float startRotation = npc.DirectionFrom(player.Center).ToRotation() + (float)Math.PI / 2f;
                    for (int i = 0; i < max; i++)
                        Projectile.NewProjectile(npc.GetSource_FromThis(null), npc.Center, Vector2.Zero, MasomodeEX.Souls.Find<ModProjectile>("CosmosMoon").Type, npc.damage, 0f, Main.myPlayer, (float)Math.PI * 2f / max * i + startRotation, npc.whoAmI, 0f);
                }
            }

            return true;
        }
    }
}