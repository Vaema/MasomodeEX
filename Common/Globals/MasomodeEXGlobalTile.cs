using System;
using MasomodeEX.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MasomodeEX.Common.Globals
{
    public class MasomodeEXGlobalTile : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (MasoModeUtils.checkIfMasoEX())
            {
                switch (type)
                {
                    case 37:
                        if (!NPC.downedBoss2)
                        {
                            int player = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            if (player != -1)
                                NPC.SpawnOnPlayer(player, 418);
                            return false;
                        }

                        if (MasomodeEX.Instance.VeinMinerLoaded)
                        {
                            int player2 = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            MasoModeUtils.Talk(Language.GetTextValue("Mods.MasomodeEX.Messages.MeteoriteVeinMiner"), Color.LimeGreen);
                            if (player2 != -1)
                                NPC.SpawnOnPlayer(player2, MasomodeEX.Souls.Find<ModNPC>("MutantBoss").Type);
                        }
                        break;

                    case 41:
                    case 43:
                    case 44:
                    case 226:
                        if (!NPC.downedMoonlord)
                        {
                            MasoModeUtils.Talk(Language.GetTextValue("Mods.MasomodeEX.Messages.MLDungeon"), Color.Cyan);
                            return false;
                        }
                        break;

                    case 58:
                        if (!NPC.downedBoss2)
                        {
                            if (WorldGen.crimson)
                                MasoModeUtils.Talk(Language.GetTextValue("Mods.MasomodeEX.Messages.BrainHellstone"), Color.Crimson);
                            else
                                MasoModeUtils.Talk(Language.GetTextValue("Mods.MasomodeEX.Messages.WormHellstone"), Color.Purple);
                        }

                        if (!NPC.downedBoss3)
                            MasoModeUtils.Talk(Language.GetTextValue("Mods.MasomodeEX.Messages.SkullHellstone"), Color.PeachPuff);
                        
                        if (!NPC.downedBoss2 && !NPC.downedBoss3)
                            return false;
                        break;

                    case 135:
                    case 137:
                        {
                            Tile tileSafely = Framing.GetTileSafely(i, j);
                            if (tileSafely.WallType == 87)
                                return NPC.downedGolemBoss;
                            break;
                        }
                }
            }
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!MasoModeUtils.checkIfMasoEX())
                return;

            switch (type)
            {
                case 5:
                    NPC.NewNPC(null, i * 16, j * 16, Utils.NextBool(Main.rand, 2) ? 210 : 211, 0, 0f, 0f, 0f, 0f, 255);
                    break;

                case 28:
                    {
                        for (int a = 0; a < 5; a++)
                        {
                            int p = Projectile.NewProjectile(null, new Vector2(i * 16, j * 16), Utils.NextVector2Unit(Main.rand, 0f, (float)Math.PI * 2f) * 6f, MasomodeEX.Souls.Find<ModProjectile>("MothDust").Type, 10, 0f, Main.myPlayer, 0f, 0f, 0f);
                            if (p != Main.projectile.Length - 1)
                                Main.projectile[p].timeLeft = 30;
                        }
                        break;
                    }

                case 141:
                    Projectile.NewProjectile(null, i * 16 + 8, j * 16 + 8, 0f, 0f, 108, 500, 10f, Main.myPlayer, 0f, 0f, 0f);
                    break;
            }
        }

        public override void RightClick(int i, int j, int type)
        {
            if (MasoModeUtils.checkIfMasoEX() && (type == 21 || type == 467) && Utils.NextBool(Main.rand, 2))
                Projectile.NewProjectile(null, i * 16 + 8, j * 16 + 8, 0f, 0f, 108, 500, 10f, Main.myPlayer, 0f, 0f, 0f);
        }
    }
}