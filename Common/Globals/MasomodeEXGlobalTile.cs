using System;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Projectiles.Masomode;
using MasomodeEX.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
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
                    case TileID.Meteorite:
                        if (!NPC.downedBoss2)
                        {
                            int player = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            if (player != -1)
                                NPC.SpawnOnPlayer(player, NPCID.SolarCorite);
                            return false;
                        }

                        if (MasomodeEX.Instance.VeinMinerLoaded)
                        {
                            FargoSoulsUtil.PrintLocalization("Mods.MasomodeEX.NPCYap.MutantVeinMiner", Color.LimeGreen);

                            int player2 = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            if (player2 != -1)
                                NPC.SpawnOnPlayer(player2, ModContent.NPCType<MutantBoss>());
                        }
                        break;

                    case TileID.BlueDungeonBrick:
                    case TileID.GreenDungeonBrick:
                    case TileID.PinkDungeonBrick:
                    case TileID.LihzahrdBrick:
                        if (!NPC.downedMoonlord)
                            return false;
                        break;

                    case TileID.Hellstone:
                        if (!(NPC.downedBoss2 && NPC.downedBoss3))
                            return false;
                        break;

                    case TileID.Traps:
                    case TileID.PressurePlates:
                        if (Framing.GetTileSafely(i, j).WallType == WallID.LihzahrdBrickUnsafe)
                            return NPC.downedGolemBoss;
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!MasoModeUtils.checkIfMasoEX())
                return;

            switch (type)
            {
                case TileID.Trees:
                    NPC.NewNPC(null, i * 16, j * 16, Utils.NextBool(Main.rand, 2) ? NPCID.Bee : NPCID.BeeSmall);
                    break;

                case TileID.Pots:
                    for (int a = 0; a < 5; a++)
                    {
                        int p = Projectile.NewProjectile(null, new Vector2(i * 16, j * 16), Utils.NextVector2Unit(Main.rand, 0f, (float)Math.PI * 2f) * 6f, ModContent.ProjectileType<MothDust>(), 10, 0f, Main.myPlayer);
                        if (p != Main.projectile.Length - 1)
                            Main.projectile[p].timeLeft = 30;
                    }
                    break;

                case TileID.Explosives:
                    Projectile.NewProjectile(null, i * 16 + 8, j * 16 + 8, 0f, 0f, ProjectileID.Explosives, 500, 10, Main.myPlayer);
                    break;

                default:
                    break;
            }
        }

        public override void RightClick(int i, int j, int type)
        {
            if (MasoModeUtils.checkIfMasoEX() && (type == TileID.Containers || type == TileID.Containers2) && Utils.NextBool(Main.rand, 2))
                Projectile.NewProjectile(null, i * 16 + 8, j * 16 + 8, 0f, 0f, ProjectileID.Explosives, 500, 10f, Main.myPlayer);
        }
    }
}