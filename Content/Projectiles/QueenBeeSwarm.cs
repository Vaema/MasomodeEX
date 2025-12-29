using System;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasomodeEX.Content.Projectiles
{
    public class QueenBeeSwarm : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            base.SetDefaults(projectile);
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (!WorldSavingSystem.EternityMode || !Condition.ForTheWorthyWorld.IsMet())
                return true;

            if (projectile.type == ModContent.ProjectileType<Bee>())
            {
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                SoundEngine.PlaySound(SoundID.Item10);

                if (Math.Abs(projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    projectile.velocity.X = 0f - oldVelocity.X;
                if (Math.Abs(projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    projectile.velocity.Y = 0f - oldVelocity.Y;

                return false;
            }

            return !projectile.hostile;
        }
    }
}