using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using MasomodeEX.Common.Globals;
using FargowiltasSouls;

namespace MasomodeEX.Content.Projectiles;

public class MoonLordText : ModProjectile
{
    public bool[] text = new bool[17];

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.hide = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        int ai0 = (int)Projectile.ai[0];
        if (ai0 <= -1 || ai0 >= 200 || !Main.npc[ai0].active || Main.npc[ai0].type != NPCID.MoonLordCore)
        {
            Projectile.Kill();
            return;
        }

        NPC npc = Main.npc[ai0];
        Projectile.Center = npc.Center;
        Projectile.timeLeft = 2;

        SpawnTalk();

        FightProgressTalk(npc);

        TargetStateTalk(npc);

        if (!text[16] && npc.GetGlobalNPC<MasomodeEXGlobalNPC>().masoBool[1])
        {
            for (int i = 0; i < text.Length; i++)
                text[i] = false;
            text[0] = true;
            text[16] = true;
        }
    }

    private void SpawnTalk()
    {
        if (!text[0])
        {
            if (!NPC.downedMoonlord)
            {
                Talk(ref text[0], "Spawn", 4);
            }
            else
            {
                Talk(ref text[0], "Rematch", 2);
            }
        }
    }

    private void FightProgressTalk(NPC npc)
    {
        if (!npc.HasValidTarget)
            return;

        switch (npc.GetLifePercent())
        {
            case < 0.001f:
                Talk(ref text[11], "FightProgress.0.01");
                break;

            case < 0.005f:
                Talk(ref text[10], "FightProgress.0.05");
                break;

            case < 0.01f:
                Talk(ref text[9], "FightProgress.0.1");
                break;

            case < 0.05f:
                Talk(ref text[8], "FightProgress.0.5");
                break;

            case < 0.1f:
                Talk(ref text[7], "FightProgress.1");
                break;

            case < 0.15f:
                Talk(ref text[6], "FightProgress.15");
                break;

            case < 0.25f:
                Talk(ref text[5], "FightProgress.Quarter");
                break;

            case < 0.5f:
                Talk(ref text[4], "FightProgress.Half");
                break;

            case < 0.75f:
                Talk(ref text[3], "FightProgress.MoreHalf");
                break;

            case < 1: // idk, this mfs talks these lines at same time
                Talk(ref text[1], "FightProgress.Full0");
                Talk(ref text[2], "FightProgress.Full1");
                break;
        }
    }

    private void TargetStateTalk(NPC npc)
    {
        if (!npc.HasValidTarget)
            return;

        var target = Main.player[npc.target];
        float perc = target.statLifeMax2 / target.statLife;

        switch (perc)
        {
            case < 0.25f:
                Talk(ref text[14], "PlayerState.Quarter");
                break;

            case < 0.5f:
                Talk(ref text[12], "PlayerState.Half0");
                break;
        }

        if (!text[13])
        {
            if (target.statLife < 200)
            {
                Talk(ref text[13], "PlayerState.Half1");
            }
        }

        if (!target.Alive() && !text[15])
        {
            bool eyes = NPC.AnyNPCs(NPCID.MoonLordFreeEye);
            Talk(ref text[15], "PlayerState.Death", talkSwitch: !eyes);

            if (eyes)
            {
                Talk(ref text[15], "PlayerState.FreeEyeDeath");
            }
        }
    }

    private void Talk(ref bool talked, string textKey, int randMax = -1, Color? color = null, bool talkSwitch = true)
    {
        if (!talked)
        {
            if (talkSwitch)
                talked = true;

            string random = randMax == -1 ? "" : $"{Main.rand.Next(randMax)}";
            string keyClear = "Mods.MasomodeEX.NPCYap.MoonLord." + textKey + random;
            color ??= Color.LimeGreen;

            FargoSoulsUtil.PrintLocalization(keyClear, (Color)color);
        }
    }
}
