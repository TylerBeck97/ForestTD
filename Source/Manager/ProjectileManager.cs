using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Manager
{
    class ProjectileManager
    {
        private StaticSprite m_missileRenderer;
        private StaticSprite m_bulletRenderer;
        private StaticSprite m_bombRenderer;

        private GameStateManager m_gameStateManager;

        public ProjectileManager(GameStateManager mGameStateManager)
        {
            m_gameStateManager = mGameStateManager;
        }

        public void loadContent(ContentManager contentManager, SpriteSheet spriteSheet)
        {
            m_missileRenderer = new StaticSprite(spriteSheet, 50, 50, 1, 822);
            m_bulletRenderer = new StaticSprite(spriteSheet, 50, 50, 1, 771);
            m_bombRenderer = new StaticSprite(spriteSheet, 50, 50, 1, 720);
        }

        public void Update(GameTime gameTime, Rectangle backgroundRectangle)
        {
            var deadProjectiles = new List<Projectile>();

            foreach (var projectile in m_gameStateManager.Projectiles)
            {
                if (backgroundRectangle.Contains(projectile.Center))
                {
                    projectile.Update(gameTime);

                    var projectileCollision = CheckCollision(projectile.Bounds, m_gameStateManager.Creeps);

                    if (projectileCollision != null)
                    {
                        // TODO Ground bullets hit air creep
                        if (projectile.Type == ProjectileType.Missile &&
                            projectileCollision.Type == CreepType.Air ||
                            projectile.Type == ProjectileType.Bullet)
                        {
                            projectileCollision.Health -= projectile.Damage;
                            deadProjectiles.Add(projectile);
                        }
                        else if (projectile.Type == ProjectileType.Bomb && projectileCollision.Type == CreepType.Ground)
                        {
                            var damageRectangle = new Rectangle((int) projectile.Center.X - projectile.Diameter / 2, (int) projectile.Center.Y - projectile.Diameter / 2,
                                projectile.Diameter, projectile.Diameter);

                            var hitCreeps = m_gameStateManager.Creeps
                                .Where(creep => damageRectangle.Contains(creep.Center) && creep.Type == CreepType.Ground).ToList();
                            foreach (var hitCreep in hitCreeps)
                            {
                                hitCreep.Health -= projectile.Damage;
                            }
                            deadProjectiles.Add(projectile);
                        }
                    }
                }
                else
                {
                    deadProjectiles.Add(projectile);
                }
            }

            foreach (var projectile in deadProjectiles)
            {
                m_gameStateManager.Projectiles.Remove(projectile);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var projectile in m_gameStateManager.Projectiles)
            {
                if (projectile.Type == ProjectileType.Missile)
                {
                    m_missileRenderer.draw(spriteBatch, projectile, projectile.Heading + MathHelper.PiOver2, Color.White);
                }

                else if (projectile.Type == ProjectileType.Bullet)
                {
                    m_bulletRenderer.draw(spriteBatch, projectile, projectile.Heading + MathHelper.PiOver2, Color.White);
                }
                else
                {
                    m_bombRenderer.draw(spriteBatch, projectile, projectile.Heading, Color.White);
                }
            }
        }

        private T CheckCollision<T>(BoundingBox check, IEnumerable<T> objects)
            where T : CollidableObject
        {
            return objects.FirstOrDefault(o => o.Bounds.Intersects(check));
        }
    }
}
