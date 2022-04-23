using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Graph;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;
using TestGame.Services;

namespace SpaceMarines_TD.Source.Manager
{
    class CreepManager
    {
        private AnimatedSprite m_airCreepRenderer;
        private AnimatedSprite m_groundCreepRenderer;

        private Texture2D m_healthBar;

        private const double CreepSpawnCooldown = 500;
        private TimeSpan m_lastCreepSpawn;

        private WeightedGraph m_graph;
        private DebugService _debugService;

        private GameStateManager m_gameStateManager;

        public CreepManager(GameStateManager gameStateManager)
        {
            m_gameStateManager = gameStateManager;
        }


        public void loadContent(ContentManager contentManager, SpriteSheet spriteSheet, DebugService debugService)
        {
            m_healthBar = contentManager.Load<Texture2D>("HealthBar");
            _debugService = debugService;

            m_airCreepRenderer = new AnimatedSprite(spriteSheet, 50, 50, 
                new []
                {
                    (1, 898),
                    (52, 898),
                    (103, 898),
                    (154, 898)
                }, 100.0f);
            m_groundCreepRenderer = new AnimatedSprite(spriteSheet, 50, 50, 
                new []
                {
                    (1, 949),
                    (52, 949),
                    (103, 949),
                    (154, 949)
                }, 100.0f);
        }

        public void Update(GameTime gameTime, Rectangle backgroundRectangle, int creepSize)
        {
            m_graph = new WeightedGraph(m_gameStateManager.Towers, backgroundRectangle.X + 50 + creepSize / 2,
                backgroundRectangle.Y + creepSize / 2,
                backgroundRectangle.Size.Y / creepSize,
                (backgroundRectangle.Size.X - 100) / creepSize, creepSize);

            if ((gameTime.TotalGameTime - m_lastCreepSpawn).TotalMilliseconds > CreepSpawnCooldown)
            {
                var rng = new Random();

                var center = new Vector2(MathF.Round(rng.Next(275, 1175) / 50.0f) * 50 + 25, -25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(creepSize * 1.5f, creepSize * 1.5f), center, Vector2.UnitY, CreepType.Air));

                center = new Vector2(MathF.Round(rng.Next(275, 1175) / 50.0f) * 50 + 25, -25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(creepSize, creepSize), center, Vector2.UnitY, CreepType.Ground));

                m_lastCreepSpawn = gameTime.TotalGameTime;
            }

            var deadCreeps = new List<Creep>();

            //foreach (var node in m_graph.GraphNodes)
            //{
            //    if (node == null) continue;

            //    _debugService.MarkPoint(node.Coordinates, Color.Blue);
            //}

            foreach (var creep in m_gameStateManager.Creeps)
            {
                if ((creep.Center.Y - creep.Size.Y / 2) < backgroundRectangle.Bottom)
                {
                    if (creep.Type == CreepType.Ground)
                    {
                        var path = m_graph.FindPath(creep.Center.X, creep.Center.Y, creep.Center.X, 1025.0f);

                        // Draw debug lines for path.
                        //var last = Vector2.Zero;
                        //foreach (var node in path)
                        //{
                        //    if (last != Vector2.Zero)
                        //    {
                        //        _debugService.MarkLine(last, node, Color.Red);
                        //    }

                        //    last = node;
                        //}

                        creep.SetPath(path);
                    }
                    creep.Update(gameTime);

                    if (creep.Health <= 0)
                    {
                        // Update score and money values.
                        m_gameStateManager.Score += creep.Score;
                        m_gameStateManager.Money += creep.Money;

                        deadCreeps.Add(creep);
                    }
                }
                else
                {
                    // Creep made it to the end of the screen - subtract lives.
                    m_gameStateManager.SubtractLives(1);

                    deadCreeps.Add(creep);
                }
            }

            foreach (var creep in deadCreeps)
            {
                m_gameStateManager.Creeps.Remove(creep);
            }

            m_airCreepRenderer.update(gameTime);
            m_groundCreepRenderer.update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var creep in m_gameStateManager.Creeps)
            {
                var healthRatio = creep.Health / 100.0f;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (healthRatio != 1)
                {
                    spriteBatch.Draw(m_healthBar,
                        new Rectangle((int)(creep.Center.X - 25), (int)(creep.Center.Y - 45)
                            , 50, 10), Color.Red);
                    spriteBatch.Draw(m_healthBar,
                        new Rectangle((int)(creep.Center.X - 25), (int)(creep.Center.Y - 45)
                            , (int)(50 * healthRatio), 10), Color.Green);
                }

                if (creep.Type == CreepType.Ground)
                {
                    m_groundCreepRenderer.draw(spriteBatch, creep, creep.Heading + MathHelper.PiOver2);
                }
                else
                {
                    m_airCreepRenderer.draw(spriteBatch, creep, creep.Heading + MathHelper.PiOver2);
                }
            }
        }
    }
}
