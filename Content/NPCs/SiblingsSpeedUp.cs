using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX.Content.NPCs
{
    public class SiblingsSpeedUp : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int FUCKINGINT;

        public override bool PreAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<MutantBoss>())
            {
                if (npc.ai[1] % 12 == 0 && npc.ai[0] != -5 && FUCKINGINT < 1) // boost ai except for final spark
                {
                    FUCKINGINT++;
                    npc.AI();
                }

                else if (npc.ai[0] == -5 && npc.ai[1] % 60 == 0 && FUCKINGINT < 1) // slightly boost final spark
                {
                    FUCKINGINT++;
                    npc.AI();
                }

                if (npc.ai[0] != 13 || npc.ai[0] != 14 || npc.ai[0] != 15)
                    npc.Center = npc.Center + npc.velocity * 0.05f;
                else if (npc.ai[0] == 34)
                    npc.Center = npc.Center + npc.velocity * 0.008f;
                else
                    npc.Center = npc.Center + npc.velocity * 0.0001f;
            }

            if (npc.type == ModContent.NPCType<AbomBoss>())
            {
                if (npc.ai[1] % 20 == 0 && FUCKINGINT < 1)
                {
                    FUCKINGINT++;
                    npc.AI();
                }

                npc.Center = npc.Center + npc.velocity * 0.05f;
            }

            if (npc.type == ModContent.NPCType<DeviBoss>())
            {
                if (npc.ai[1] % 32 == 0 && FUCKINGINT < 1)
                {
                    FUCKINGINT++;
                    npc.AI();
                }

                npc.Center = npc.Center + npc.velocity * 0.03f;
            }

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (FUCKINGINT == 1)
            {
                FUCKINGINT = 0;
            }
            base.PostAI(npc);
        }
    }
}
