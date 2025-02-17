using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace MasomodeEX.Content.NPCs;

public class PhantomPortal : ModNPC
{
    public virtual int spawn => 454;

    public override void SetDefaults()
    {
        NPC.width = 140;
        NPC.height = 190;
        NPC.lifeMax = 25000;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.lavaImmune = true;
        NPC.aiStyle = -1;
        NPC.alpha = 0;
        for (int i = 0; i < NPC.buffImmune.Length; i++)
            NPC.buffImmune[i] = true;
    }

    public override void AI()
    {
        if (NPC.ai[0] < 0f || NPC.ai[0] >= 200f)
        {
            NPC.StrikeInstantKill();
            NPC.active = false;
            return;
        }

        NPC hand = Main.npc[(int)NPC.ai[0]];
        if (!hand.active || hand.type != NPCID.MoonLordHand)
        {
            NPC.StrikeInstantKill();
            NPC.active = false;
            return;
        }

        NPC.Center = hand.Center;
        NPC.position.Y -= 250f;
        if ((NPC.ai[1] += 1f) > 120f)
        {
            NPC.ai[1] = 0f;
            if (!NPC.AnyNPCs(spawn) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.NewText(Main.rand.Next(5) switch
                {
                    0 => "Guys, Will you assist me for a moment?",
                    1 => "I'm going to bring more of my servants to assist me!",
                    2 => "Go servants, what are you all waiting for?",
                    3 => "Servants! Get this little peasant out of my sight at once!",
                    _ => "Mwahahahahahahahahaha! Trap this tiny struggle at once!",
                }, Color.LimeGreen);

                int i = NPC.NewNPC(null, (int)NPC.Center.X, (int)NPC.Center.Y, spawn, 0, 0f, 0f, 0f, 0f, 255);
                if (i != 200 && Main.netMode == 2)
                    NetMessage.SendData(MessageID.SyncNPC);
            }
        }

        if (NPC.ai[3] > 0f && (NPC.ai[2] += 1f) > 1200f)
        {
            NPC.ai[3] = 0f;
            NPC.ai[2] = 0f;
            NPC.netUpdate = true;
        }
        
        NPC.dontTakeDamage = NPC.ai[3] > 0f;
    }

    public override bool CheckDead()
    {
        NPC.ai[3] = 1f;
        NPC.active = true;
        NPC.life = 1;
        NPC.netUpdate = true;
        return false;
    }

    public override bool CheckActive() => false;

    public override bool PreKill() => false;
}