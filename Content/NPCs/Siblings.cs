using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace MasomodeEX.Content.NPCs
{
    public class Siblings : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int step;
        public override void PostAI(NPC npc)
        {
            if (step >= 2)
            {
                step = 0;
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (step > 2)
            {
                return true;
            }

            if (npc.type == ModContent.NPCType<MutantBoss>())
                MutantBoost(npc);

            if (npc.type == ModContent.NPCType<AbomBoss>())
                AbomBoost(npc, 0.25f);

            if (npc.type == ModContent.NPCType<DeviBoss>())
                DeviBoost(npc, 0.1f);

            return true;
        }

        private void DeviBoost(NPC npc, float boostSpeed = 0.5f)
        {
            bool updateStep = Main.GameUpdateCount % 10 != 0;

            if (updateStep)
                return;

            step++;
            npc.AI();
            npc.Center += npc.velocity * boostSpeed;
        }

        private void AbomBoost(NPC npc, float boostSpeed = 0.5f)
        {
            bool updateStep = Main.GameUpdateCount % 8 != 0;
            const int laevateinnAttack = 15;
            const int verticalDiveAttack = 19;

            if (updateStep)
                return;

            if (npc.ai[0] == laevateinnAttack || npc.ai[0] == verticalDiveAttack)
                return;

            step++;
            npc.AI();
            npc.Center += npc.velocity * boostSpeed;
        }

        private void MutantBoost(NPC npc)
        {
            bool updateStep = Main.GameUpdateCount % 8 != 0;

            // p1
            const int voidRaysP1 = 8;
            const int BoundaryBulletHellAndSwordP1 = 9;

            // p2
            const int Phase2Transition = 10;
            const int ApproachForNextAttackP2 = 11;
            const int VoidRaysP2 = 12;
            const int EOCStarSickles = 20;
            const int MechRayFan = 27;
            const int Nuke = 34;
            const int PrepareSlimeRain = 36;
            const int OkuuSpheresP2 = 40;
            const int TwinRangsAndCrystals = 43;
            const int EmpressSwordWave = 44;
            const int PrepareMutantSword = 45;

            // final spark
            const int Phase3Transition = -1;
            const int BoundaryBulletHellP3 = -4;
            const int FinalSpark = -5;

            if (updateStep)
                return;

            switch (npc.ai[0])
            {
                // don't boost
                case voidRaysP1 or BoundaryBulletHellAndSwordP1 
                or Phase2Transition or EOCStarSickles 
                or Nuke or PrepareSlimeRain 
                or VoidRaysP2 or OkuuSpheresP2 
                or EmpressSwordWave or ApproachForNextAttackP2
                or MechRayFan or PrepareMutantSword
                or TwinRangsAndCrystals:
                    return;

                case FinalSpark:
                    ManageFinalSpark(npc);
                    return;

                case Phase3Transition or BoundaryBulletHellP3 or < FinalSpark:
                    return;
            }

            step++;
            npc.AI();

            void ManageFinalSpark(NPC npc)
            {
                float newRotation = npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation();
                float difference = MathHelper.WrapAngle(newRotation - npc.ai[3]);
                float rotationDirection = 2f * (float)Math.PI * 1f / 6f / 60f;
                rotationDirection *= 1.5f;
                float change = Math.Min(rotationDirection, Math.Abs(difference)) * Math.Sign(difference);

                npc.ai[3] += change;
            }
        }
    }
}
