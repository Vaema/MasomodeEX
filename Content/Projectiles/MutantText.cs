using FargowiltasSouls;
using MasomodeEX.Common.Globals;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MasomodeEX.Content.Projectiles;

public class MutantText : ModProjectile
{
    public bool[] p1 = new bool[8];
    public bool[] p2 = new bool[16];
    public string currentText = "";
    public int delay;
    public string[] spam =
    [
        "THE ABYSS CONSUMES ALL!",
        "RAGNAROK WILL END YOUR CHILDISH RAMPAGE!",
        "YOU WILL BE BURNED AS THE SUNKEN SEA WAS!",
        "THE TRUE FURY OF THE TYRANT WILL ANNIHILATE YOUR TINY POWER!",
        "YOU ARE NOTHING COMPARED TO THE ERODED SPIRITS!",
        "I BRING FORTH THE END UPON THE FOOLISH, THE UNWORTHY!",
        "YOU WANT TO DEFEAT ME?",
        "MAYBE IN TWO ETERNITIES!",
        "DIE, FOOLISH TERRARIAN!",
        "THEY SAID THERE WAS 3 END BRINGERS...",
        "BUT I AM THE FOURTH, A BREAKER OF REALITY!",
        "HELL DOESN’T ACCEPT SCUM LIKE YOU, SO SUFFER FOREVER IN MY ENDLESS ONSLAUGHT!",
        "MY INFINITE POWER!!",
        "THE POTENTIAL OF ETERNITIES STRETCHED TO THE ABSOLUTE MAXIMUM APOTHEOSES!",
        "YOUR UNHOLY SOUL SHALL BE CONSUMED BY DEPTHS LOWER THAN THE DEEPEST REACHES OF HELL!",
        "I CONTROL THE POWER THAT HAS REACHED FROM THE FAR ENDS OF THE UNIVERSE!",
        "UNITING DIMENSIONS, MANIPULATING TERRARIA, SLAYING MASOCHIST, AND JUDGING HEAVENS!!!",
        "FOR CENTURIES I HAVE TRAINED FOR ONE GOAL ONLY:",
        "PURGE THE WORLD OF THE UNWORTHY, SLAY THE WEAK, AND BRING FORTH TRUE POWER.",
        "IN THE HIGHEST REACHES OF HEAVEN, MY BROTHER RULES OVER THE SKY!",
        "SOON, ALL OF TERRARIA WILL BE PURGED OF THE UNWORTHY AND A NEW AGE WILL START!",
        "A NEW AGE OF AWESOME!",
        "A GOLDEN AGE WHERE ONLY ABSOLUTE BEINGS EXIST!",
        "DEATH, INFERNO, TIDE; I AM THE OMEGA AND THE ALPHA, THE BEGINNING, AND THE END!",
        "ALMIGHTY POWER; REVELATIONS.",
        "ABSOLUTE BEING, ABSOLUTE PURITY.",
        "WITHIN THE FOOLISH BANTERINGS OF THE MORTAL WORLD I HAVE ACHIEVED POWER!",
        "POWER THAT WAS ONCE BANISHED TO THE EDGE OF THE GALAXY!",
        "I BRING FORTH CALAMITIES, CATASTROPHES, AND CATACLYSM;",
        "ELDRITCH POWERS DERIVED FROM THE ABSOLUTE WORD OF FATE.",
        "FEEL MY UBIQUITOUS WRATH DRIVE YOU INTO THE GROUND!",
        "JUST AS A WORLD SHAPER DRIVES HIS WORLD INTO REALITY!",
        "THE SHARPSHOOTER’S EYE PALES IN COMPARISON OF MY PERCEPTION OF REALITY!",
        "THE BERSERKER'S RAGE IS NAUGHT BUT A BUNNY'S BEFORE MINE!",
        "THE OLYMPIANS ARE MERE LESSER GODS COMPARED TO MY IMMEASURABLE MIGHT!",
        "THE ARCH WIZARD'S A POSER, A HACK, A PARLOUR TRICK TOTING JOKER!",
        "A MASTERY OF FLIGHT AND THE IRON WILL OF A COLOSSUS ARE BOTH ELEMENTARY CONCEPTS!",
        "A CONJURER IS BUT A PEDDLING MAGICIAN!",
        "A TRAWLER IS BUT A SLIVER COMPARED TO MY LIFE MASTERY!",
        "SUPERSONIC SPEED, LIGHTSPEED TIME!",
        "GLORIOUS LIGHT SHALL ELIMINATE YOU, YOU FOOLISH BUFFOON!",
        "WHAT ARE YOUR TRUE INTENTIONS?",
        "WHY DO YOU REALLY EVEN NEED THIS POWER?",
        "WHAT IS THE POINT IN ALL OF THIS!?",
        "TO THINK YOU WERE SATISFIED WITH THE PROSPERITY OF THIS LAND!",
        "SAFETY AMONGST THE TOWN, PROTECTION OF THE EVIL THREATS, BUT NO!",
        "YOU WANTED MORE!",
        "YOU JUST WANTED TO SPITE ME, DIDN’T YOU!?",
        "ENOUGH OF THIS!",
        "I CAN’T KEEP GOING MUCH LONGER!",
        "YOU CANNOT KILL ME, THIS IS JUST THE ACT OF AN INSIGNIFICANT LUNATIC!",
        "I WILL SOON RETURN FOR ANOTHER BATTLE!",
        "THIS IS ONLY THE BEGINNING!",
        "DO YOU HONESTLY THINK YOU CAN SURVIVE ANY LONGER?",
        "POWER IS IN THE EYE OF THE BEHOLDER!",
        "YOU ARE NOT DESERVING OF TRUE DIVINITY!",
        "I SHOULD KNOW FROM THE COUNTLESS YOU HAVE SLAUGHTERED!",
        "YOUR MOTIVATION IS UNFOUNDED!",
        "ALL YOU SEEK IS THE DESTRUCTION OF ANYONE WHO POSES A THREAT TO YOU!",
        "THAT’S WHY YOU LIMP ON THIS MOIST ROCK!",
        "SEARCHING FOR STICKS AND PEBBLES TO ELIMINATE THE FEARS YOU CANNOT TRULY OVERCOME!",
        "IT REALLY SURPRISES ME THAT IT TOOK YOU AWHILE TO REACH THIS POINT!",
        "ESPECIALLY GIVEN HOW WELL YOU LEECH OFF YOUR OPPONENTS AFTER BARELY SCRAPING BY!",
        "THAT’S HOW YOU WIN ALL YOUR BATTLES, AM I RIGHT!?",
        "IT’S HONESTLY IMPRESSIVE THAT YOU’VE MADE IT THIS FAR.",
        "I HOPE YOU WEREN’T USING GODMODE, PATHETIC COWARD!",
        "YOU MUST BE SO NERVOUS THAT YOU’RE SO CLOSE!",
        "I HOPE YOU CHOKE, AND I HOPE YOU CHOKE ON THE ASH AND BLOOD OF YOUR SINS TOO!",
        "GLORIOUS LIGHT SHALL ELIMINATE YOU, YOU FOOLISH BUFFOON!",
        "AAAAAAAAAAAAA!",
        "THIS IS IT!",
        "NOW LET'S GET TO THE GOOD PART!!!!!"
    ];

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
        if (ai0 <= -1 || ai0 >= 200 || !Main.npc[ai0].active || Main.npc[ai0].type != MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type)
        {
            Projectile.Kill();
            return;
        }

        NPC npc = Main.npc[ai0];
        Projectile.Center = npc.Center;
        Projectile.timeLeft = 2;
        
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            if (ModLoader.GetMod("FargowiltasSouls") != null && npc.type == ModLoader.GetMod("FargowiltasSouls").Find<ModNPC>("MutantBoss").Type)
            {
                SoundStyle sound = new("Sounds/MutantHit", SoundType.Sound);
                sound.WithPitchOffset(0.25f);
                npc.HitSound = sound.WithVolumeScale(1.5f);
            }

            switch (MasomodeEXWorld.MutantSummons)
            {
                case < 1:
                    Talk("Spawn0");
                    break;

                case < 3:
                    Talk("Spawn1");
                    break;

                case < 5:
                    Talk("Spawn2");
                    break;

                case < 7:
                    Talk("Spawn3");
                    break;

                case < 10:
                    Talk("Spawn4");
                    break;

                case < 15:
                    Talk("Spawn5");
                    break;

                case < 20:
                    Talk("Spawn6");
                    break;

                case < 50:
                    Talk("Spawn7");
                    break;

                case < 75:
                    Talk("Spawn8");
                    break;

                case < 100:
                    Talk("Spawn9");
                    break;

                case < 250:
                    Talk("Spawn10");
                    break;

                case < 500:
                    Talk("Spawn11");
                    break;

                case < 1000:
                    Talk("Spawn12");
                    break;

                default:
                    Talk("SpawnCommon");
                    break;
            }

            MasomodeEXWorld.MutantSummons++;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
        }

        // so miserable.
        TalkCheck(0.9f, ref p1[0], "P1.0");
        TalkCheck(0.8f, ref p1[1], "P1.1");
        TalkCheck(0.7f, ref p1[2], "P1.2");
        TalkCheck(0.65f, ref p1[3], "P1.3");
        TalkCheck(0.6f, ref p1[4], "P1.4");
        TalkCheck(0.55f, ref p1[5], "P1.5");
        TalkCheck(0.5f, ref p1[6], "P1.6");
        
        if (p1[7])
        {
            TalkCheck(0.9f, ref p2[0], "P2.0");
            TalkCheck(0.8f, ref p2[1], "P2.1");
            TalkCheck(0.7f, ref p2[2], "P2.2");
            TalkCheck(0.6f, ref p2[3], "P2.3");
            TalkCheck(0.5f, ref p2[4], "P2.4");
            TalkCheck(0.4f, ref p2[5], "P2.5");
            TalkCheck(0.3f, ref p2[6], "P2.6");
            TalkCheck(0.25f, ref p2[7], "P2.7");
            TalkCheck(0.2f, ref p2[8], "P2.8");
            TalkCheck(0.15f, ref p2[9], "P2.9");
            TalkCheck(0.1f, ref p2[10], "P2.10");
            TalkCheck(0.05f, ref p2[11], "P2.11");
            TalkCheck(0.04f, ref p2[12], "P2.12");
            TalkCheck(0.03f, ref p2[13], "P2.13");
            TalkCheck(0.02f, ref p2[14], "P2.14");
            TalkCheck(0.01f, ref p2[15], "P2.15");
        }

        if (npc.ai[0] < -1f && (Projectile.localAI[1] += 1f) > 20f)
        {
            Projectile.localAI[1] = 0f;
            if (Projectile.ai[1] < spam.Length)
                EdgyBossText(npc, spam[(int)Projectile.ai[1]++]);
        }

        switch ((int)npc.ai[0])
        {
            case -7:
                if (npc.HasValidTarget)
                {
                    if (Main.rand.NextBool(30))
                    {
                        Vector2 speed = npc.DirectionTo(Main.player[npc.target].Center) * 25f;
                        if (Main.rand.NextBool(2))
                        {
                            for (int k = 0; k < 16; k++)
                            {
                                int p2 = Projectile.NewProjectile(null, npc.Center, speed.RotatedBy(Math.PI / 8.0 * k, default), MasomodeEX.Souls.Find<ModProjectile>("MutantSpearThrown").Type, npc.damage, 0f, Main.myPlayer, 0f, 0f, 0f);
                                if (p2 != 1000)
                                    Main.projectile[p2].timeLeft = 60;
                            }
                        }
                        else
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int p3 = Projectile.NewProjectile(null, npc.Center, speed.RotatedBy((double)(MathHelper.ToRadians(10f) * j), default), MasomodeEX.Souls.Find<ModProjectile>("MutantSpearThrown").Type, npc.damage, 0f, Main.myPlayer, 0f, 0f, 0f);
                                if (p3 != 1000)
                                    Main.projectile[p3].timeLeft = 60;
                            }
                        }
                    }
                }
                else
                    npc.TargetClosest(true);

                DeathTalk(npc);
                break;

            case -1:
                if (npc.ai[1] == 120f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(null, npc.Center, Vector2.Zero, Mod.Find<ModProjectile>("BossRush").Type, 0, 0f, Main.myPlayer, npc.whoAmI, 0f, 0f);
                }
                break;

            case 0:
                if (npc.ai[1] == 1f)
                {
                    Talk("Attack0");
                }
                break;

            case 1:
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Talk("Attack1");
                }
                break;

            case 2:
                Projectile.ai[1] = 0f;
                if (npc.ai[1] == 1f)
                {
                    Talk("Attack2");
                }
                break;

            case 4:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack3");
                }
                break;

            case 6:
                if (npc.ai[1] == 0f && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Talk("Attack4");
                }
                break;

            case 7:
                Projectile.ai[1] = 0f;
                if (npc.ai[2] == 0f)
                {
                    Talk("Attack5");
                }
                break;

            case 9:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack6");
                }
                break;

            case 10:
                if (npc.ai[1] == 1f)
                {
                    Talk("Attack7");
                }
                if (npc.ai[1] == 120f)
                {
                    Projectile.NewProjectile(null, npc.Center, Vector2.Zero, Mod.Find<ModProjectile>("BossRush").Type, 0, 0f, Main.myPlayer, npc.whoAmI, 0f, 0f);
                    Talk("Attack8");
                }
                break;

            case 11:
                p1[7] = true;
                if (npc.ai[2] == 1f)
                {
                    Talk("Attack9");
                }
                break;

            case 13:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack10");
                }
                break;

            case 15:
                if (npc.ai[1] == 0f && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Talk("Attack11");
                }
                break;

            case 16:
                Projectile.ai[1] = 0f;
                if (npc.ai[2] == 1f)
                {
                    Talk("Attack12");
                }
                break;

            case 18:
                Talk("Attack13");
                break;

            case 19:
                if (npc.ai[1] == 240f)
                {
                    Talk("Attack14");
                }
                break;

            case 20:
                if (npc.ai[1] == 0f && npc.ai[2] == 1f)
                {
                    Talk("Attack15");
                }
                break;

            case 21:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack10");
                }
                break;

            case 23:
                if (npc.ai[1] == 0f && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Talk("Attack17");
                }
                break;

            case 24:
                Projectile.ai[1] = 0f;
                if (npc.ai[1] == 1f)
                {
                    Talk("Attack18");
                }
                break;

            case 25:
                if (npc.ai[1] == 1f && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] = 1f;
                    Talk("Attack19");
                }
                break;

            case 26:
                Projectile.ai[1] = 0f;
                if (npc.ai[1] == 120f)
                {
                    Talk("Attack20");
                }
                break;

            case 28:
                if (npc.ai[3] == 30f)
                {
                    Talk("Attack21");
                }
                break;

            case 29:
                Talk("Attack22");
                break;

            case 31:
                if (npc.ai[1] == 1f)
                {
                    Talk("Attack2");
                }
                break;
            case 33:
                if (npc.ai[1] == 180f)
                {
                    Talk("Attack24");
                }
                break;
            case 35:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack25");
                }
                break;
            case 36:
                if (npc.ai[1] == 60f)
                {
                    Talk("Attack22");
                }
                break;
            case 38:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack25");
                }
                break;
            case 39:
                if (npc.ai[3] == 0f)
                {
                    Talk("Attack26");
                }
                break;
            case 40:
                if (npc.ai[1] == 179f)
                {
                    Talk("Attack19");
                }
                break;
            case 41:
                Talk("Attack23");
                break;
        }

        float lifeIntensity = 1f - npc.life / (float)npc.lifeMax;
        if (--delay < 0)
        {
            delay = Main.rand.Next(5 + (int)(115f * (1f - lifeIntensity)));
            int max = (int)(77f * lifeIntensity);
            for (int i = 0; i < max; i++)
                CombatText.NewText(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight), Color.LimeGreen, currentText, Main.rand.NextBool(2), Main.rand.NextBool(2));
        }

        void TalkCheck(float perc, ref bool check, string name, Color? color = null)
        {
            if (npc.life > npc.lifeMax * perc || check)
                return;

            check = true;

            color ??= Color.Cyan;
            string keyClear = "Mods.MasomodeEX.NPCYap.Mutant." + name;

            FargoSoulsUtil.PrintLocalization(keyClear, (Color)color);
        }

        void Talk(string name, Color? color = null)
        {
            color ??= Color.Cyan;
            string keyClear = "Mods.MasomodeEX.NPCYap.Mutant." + name;

            FargoSoulsUtil.PrintLocalization(keyClear, (Color)color);
        }
    }

    private void EdgyBossText(NPC npc, ref bool check, double threshold, string text)
    {
        if (!check && npc.life < npc.lifeMax * threshold)
        {
            check = true;
            EdgyBossText(npc, text);
        }
    }

    private void EdgyBossText(NPC npc, string text)
    {
        currentText = text;
        if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 5000f)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, (Color?)Color.LimeGreen);
            else if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen, -1);
        }
    }

    private void DeathTalk(NPC npc)
    {
        if (npc.alpha == 0)
        {
            switch (MasomodeEXWorld.MutantDefeats)
            {
                case < 3:
                    Talk("Kill0");
                    break;

                case < 5:
                    Talk("Kill1");
                    break;

                case < 10:
                    Talk("Kill2");
                    break;

                case < 15:
                    Talk("Kill3");
                    break;

                case < 50:
                    Talk("Kill4");
                    break;

                case < 100:
                    Talk("Kill5");
                    break;

                default:
                    Talk("KillCommon");
                    break;
            }

            MasomodeEXWorld.MutantDefeats++;

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
        }
    }

    private void Talk(ref bool check, string name, Color? color = null)
    {
        if (!check)
        {
            check = true;

            color ??= Color.Cyan;
            string keyClear = "Mods.MasomodeEX.NPCYap.Mutant." + name;

            FargoSoulsUtil.PrintLocalization(keyClear, (Color)color);
        }
    }

    private void Talk(string name, Color? color = null)
    {
        color ??= Color.Cyan;
        string keyClear = "Mods.MasomodeEX.NPCYap.Mutant." + name;

        FargoSoulsUtil.PrintLocalization(keyClear, (Color)color);
    }
}
