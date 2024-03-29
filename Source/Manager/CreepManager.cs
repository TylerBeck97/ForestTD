using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Graph;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Manager
{
    class CreepManager
    {
        private AnimatedSprite m_airCreepRenderer;
        private AnimatedSprite m_groundCreepRenderer;

        private Texture2D m_healthBar;

        private const double CreepSpawnCooldown = 4000;
        private TimeSpan m_lastCreepSpawn;

        private WeightedGraph m_upDownGraph;
        private WeightedGraph m_sideTOSideGraph;

        private GameStateManager m_gameStateManager;
        private readonly ParticleManager m_particleManager;
        private readonly SoundManager m_soundManager;

        private int m_towerCount;
        private const int NumOfCreepPerWave = 5;
        private int m_numOfWavesSpawned;
        public const int CreepSize = 50;

        private Random m_random;

        public CreepManager(GameStateManager gameStateManager, ParticleManager particleManager, SoundManager soundManager)
        {
            m_gameStateManager = gameStateManager;
            m_particleManager = particleManager;
            m_towerCount = -1;
            m_random = new Random();
            m_soundManager = soundManager;
        }


        public void loadContent(ContentManager contentManager, SpriteSheet spriteSheet)
        {
            m_healthBar = contentManager.Load<Texture2D>("images/HealthBar");

            m_airCreepRenderer = new AnimatedSprite(spriteSheet, 75, 75, 
                new []
                {
                    (1, 873),
                    (77, 873),
                    (153, 873),
                    (229, 873)
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

        public void Update(GameTime gameTime, Rectangle backgroundRectangle)
        {
            // Update Graphs if a tower has been placed
            if (m_gameStateManager.Towers.Count != m_towerCount)
            {
                m_upDownGraph = new WeightedGraph(m_gameStateManager.Towers, backgroundRectangle.X + 50 + CreepSize / 2,
                    backgroundRectangle.Y - CreepSize / 2,
                    backgroundRectangle.Size.Y + 100 / CreepSize,
                    (backgroundRectangle.Size.X - 100) / CreepSize, CreepSize);

                m_sideTOSideGraph = new WeightedGraph(m_gameStateManager.Towers, backgroundRectangle.X - CreepSize / 2,
                   backgroundRectangle.Y + 50 + CreepSize / 2,
                   (backgroundRectangle.Size.Y - 100) / CreepSize, backgroundRectangle.Size.X + 100 / CreepSize, CreepSize);

                m_towerCount = m_gameStateManager.Towers.Count;
            }
            // Spawn Waves for level
            if (m_gameStateManager.isLevelActive && GameStateManager.NumOfWaves > m_numOfWavesSpawned)
            {
                if ((gameTime.TotalGameTime - m_lastCreepSpawn).TotalMilliseconds > CreepSpawnCooldown)
                {
                    if(m_numOfWavesSpawned != 0) m_soundManager.PlayWaveSound();
                    for (int i=0; i< Math.Min(NumOfCreepPerWave * m_gameStateManager.Level, 30); i++)
                    {
                        if (i % 5 == 0 && m_gameStateManager.Level > 2)
                        {
                            if (m_gameStateManager.Level % 2 == 0)
                            {
                                SpawnUpDownAirCreep();
                            }
                            else
                            {
                                SpawnSidewaysAirCreep();
                            }
                        }
                        else
                        {
                            if (m_gameStateManager.Level % 2 == 0)
                            {
                                SpawnUpDownGroundCreep();
                            }
                            else
                            {
                                SpawnSidewaysGroundCreep();
                            }
                        }
                    }

                    m_numOfWavesSpawned++;
                    m_gameStateManager.Wave++;
                    m_gameStateManager.Score += 10;

                    m_lastCreepSpawn = gameTime.TotalGameTime;
                }
            }
            else
            {
                m_gameStateManager.isLevelActive = false;
                m_numOfWavesSpawned = 0;
            }

            var deadCreeps = new List<Creep>();

            foreach (var creep in m_gameStateManager.Creeps)
            {
               
                if (creep.Type == CreepType.Ground)
                {
                    Stack<Vector2> path;
                    if (creep.IsUpAndDown)
                    {
                        if (creep.IsMovingPositive)
                        {
                            path = m_upDownGraph.FindPath(creep.Center.X, creep.Center.Y, creep.Center.X, 1075.0f);
                        }
                        else
                        {
                            path = m_upDownGraph.FindPath(creep.Center.X, creep.Center.Y, creep.Center.X, -25.0f);
                        }

                    }
                    else
                    {
                        if (creep.IsMovingPositive)
                        {
                            path = m_sideTOSideGraph.FindPath(creep.Center.X, creep.Center.Y, 1325.0f, creep.Center.Y);
                        }
                        else
                        {
                            path = m_sideTOSideGraph.FindPath(creep.Center.X, creep.Center.Y, 175.0f, creep.Center.Y);
                        }
                    }

                    //Draw debug lines for path.
                    //var last = Vector2.Zero;
                    //foreach (var node in path)
                    //{
                    //    if (last != Vector2.Zero)
                    //    {
                    //        _debugService.MarkLine(last, node, Color.Red);
                    //    }
                    //
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

                    m_particleManager.AddCreepScore(creep.Center - Vector2.UnitY * CreepSize / 2, creep.Type);
                    m_soundManager.PlayDeathSound(gameTime);
                    m_particleManager.AddCreepDeath(creep.Center);
                    deadCreeps.Add(creep);
                }

                if (!creep.HasEnteredField && backgroundRectangle.Contains(creep.Center)) creep.HasEnteredField = true;
                if (creep.HasEnteredField && !backgroundRectangle.Contains(creep.Center))
                {
                    m_gameStateManager.SubtractLives(1);
                    m_soundManager.PlayLostLifeSound(gameTime);
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
                var healthRatio = creep.Health / (100.0f * m_gameStateManager.Level / 2);

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
        private void SpawnSidewaysAirCreep()
        {
            
            if(m_random.Next() % 2 == 0)
            {
                var center = new Vector2(225, MathF.Round(m_random.Next(75, 1005) / 51.0f) * 51 + 25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize * 1.5f, CreepSize * 1.5f), center, Vector2.UnitX, CreepType.Air, false, true, m_gameStateManager.Level));
            }
            else
            {
                var center = new Vector2(1275, MathF.Round(m_random.Next(75, 1005) / 51.0f) * 51 + 25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize * 1.5f, CreepSize * 1.5f), center, -Vector2.UnitX, CreepType.Air, false, false, m_gameStateManager.Level));
            }
            
        }

        private void SpawnUpDownAirCreep()
        {
            if (m_random.Next() % 2 == 0)
            {
                var center = new Vector2(MathF.Round(m_random.Next(276, 1176) / 51.0f) * 51 + 25, 25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize * 1.5f, CreepSize * 1.5f), center, Vector2.UnitY, CreepType.Air, true, true, m_gameStateManager.Level));
            }
            else
            {
                var center = new Vector2(MathF.Round(m_random.Next(276, 1176) / 51.0f) * 51 + 25, 1050);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize * 1.5f, CreepSize * 1.5f), center, -Vector2.UnitY, CreepType.Air, true, false, m_gameStateManager.Level));
            }
        }

        private void SpawnSidewaysGroundCreep()
        {
            if (m_random.Next() % 2 == 0)
            {
                var center = new Vector2(220, MathF.Round((m_random.Next(75, 1005) / 50.0f)) * 50 + 25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize, CreepSize), center, Vector2.UnitX, CreepType.Ground, false, true, m_gameStateManager.Level));
            }
            else
            {
                var center = new Vector2(1300, MathF.Round((m_random.Next(75, 1005) / 50.0f)) * 50 + 25);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize, CreepSize), center, -Vector2.UnitX, CreepType.Ground, false, false, m_gameStateManager.Level));
            }
        }

        private void SpawnUpDownGroundCreep()
        {
            if (m_random.Next() % 2 == 0)
            {
                var center = new Vector2(MathF.Round(m_random.Next(275, 1175) / 50.0f) * 50 + 25, 0);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize, CreepSize), center, Vector2.UnitY, CreepType.Ground, true, true, m_gameStateManager.Level));
            }
            else
            {
                var center = new Vector2(MathF.Round(m_random.Next(275, 1175) / 50.0f) * 50 + 25, 1100);
                m_gameStateManager.Creeps.Add(new Creep(new Vector2(CreepSize, CreepSize), center, -Vector2.UnitY, CreepType.Ground, true, false, m_gameStateManager.Level));
            }
        }
    }
}
