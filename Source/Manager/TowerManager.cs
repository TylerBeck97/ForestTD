using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Graph;
using SpaceMarines_TD.Source.Input;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;
using SpaceMarines_TD.Source.Views;

namespace SpaceMarines_TD.Source.Manager
{
    class TowerManager
    {
        public const int AirTowerCost = 30;
        public const int BulletTowerCost = 15;
        public const int BombTowerCost = 45;
        public const int MixedTowerCost = 30;

        private AirTowerSprite m_airTowerRenderer;
        private AnimatedTowerSprite m_groundTowerRenderer;
        private AnimatedTowerSprite m_bombTowerRenderer;
        private AnimatedTowerSprite m_mixedTowerRenderer;

        private MouseInput m_mouseInput;

        private Texture2D m_towerBaseTexture2D;
        private Texture2D m_rangeCircleTexture2D;

        private Tower m_placeTower;

        private bool m_isPlaceable = false;

        private Rectangle m_towerZone = new Rectangle(225, 25, 1050, 1050);

        private GameStateManager m_gameStateManager;
        private readonly ParticleManager m_particleManager;
        private readonly SoundManager m_soundManager;

        public Tower SelectedTower { get; set; }

        public TowerManager(GameStateManager gameStateManager, ParticleManager particleManager, SoundManager soundManager, MouseInput mouseInput)
        {
            m_gameStateManager = gameStateManager;
            m_particleManager = particleManager;
            m_mouseInput = mouseInput;
            m_soundManager = soundManager;
        }

        public void loadContent(ContentManager contentManager, SpriteSheet spriteSheet)
        {
            m_airTowerRenderer = new AirTowerSprite(spriteSheet, 100, 100,
                new[]
                {
                    (1, 103),
                    (102, 103),
                    (203, 103),
                });

            m_bombTowerRenderer = new AnimatedTowerSprite(spriteSheet, 100, 100, 
                new[]
                {
                    (1, 203),
                    (102, 203),
                    (203, 203),
                    (304, 203)
                });

            m_groundTowerRenderer = new AnimatedTowerSprite(spriteSheet, 100, 100, 
                new []
                {
                    (1, 1),
                    (102, 1),
                    (203, 1),
                    (304, 1)
                });

            m_mixedTowerRenderer = new AnimatedTowerSprite(spriteSheet, 100, 100,
                new[]
                {
                    (1, 304),
                    (102, 304),
                    (203, 304),
                    (304, 304)
                });

            m_rangeCircleTexture2D = contentManager.Load<Texture2D>("images/rangeCircle");
            m_towerBaseTexture2D = contentManager.Load<Texture2D>("images/TowerBase");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var tower in m_gameStateManager.Towers)
            {
                var towerColor = Color.White;

                spriteBatch.Draw(m_towerBaseTexture2D, 
                    new Rectangle((int)(tower.Center.X - tower.Size.X / 2), (int)(tower.Center.Y - tower.Size.Y / 2),
                        (int)tower.Size.X, (int)tower.Size.Y), towerColor);
                if (tower == SelectedTower)
                {
                    spriteBatch.Draw(m_rangeCircleTexture2D, new Rectangle((int)tower.Center.X - tower.Range / 2,
                        (int)tower.Center.Y - tower.Range / 2, tower.Range, tower.Range), Color.White);
                }

                switch (tower.type)
                {
                    case TowerType.Air:
                        m_airTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;

                    case TowerType.Bullet:
                        m_groundTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;

                    case TowerType.Bomb:
                        m_bombTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;
                    case TowerType.Mixed:
                        m_mixedTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;
                }
            }

            if (m_placeTower != null)
            {
                DrawPlaceTower(gameTime, spriteBatch, m_placeTower);
            }
        }

        public void DrawPlaceTower(GameTime gameTime, SpriteBatch spriteBatch, Tower tower)
        {
            var towerColor = m_isPlaceable ? Color.White : Color.Red;

            spriteBatch.Draw(m_towerBaseTexture2D,
                new Rectangle((int)(tower.Center.X - tower.Size.X / 2), (int)(tower.Center.Y - tower.Size.Y / 2),
                    (int)tower.Size.X, (int)tower.Size.Y), towerColor);

            switch (tower.type)
            {
                case TowerType.Air:
                    m_airTowerRenderer.draw(gameTime, spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Bullet:
                    m_groundTowerRenderer.draw(gameTime, spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Bomb:
                    m_bombTowerRenderer.draw(gameTime, spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Mixed:
                    m_mixedTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, true, towerColor);
                    break;
            }

            if (m_isPlaceable) spriteBatch.Draw(m_rangeCircleTexture2D, new Rectangle((int)tower.Center.X - tower.Range / 2,
                (int)tower.Center.Y - tower.Range / 2, tower.Range, tower.Range), Color.White);
        }

        public void OnClick(Vector2 mousePos)
        {
            if (m_placeTower != null) // Handle tower placement.
            {
                if (m_isPlaceable && m_gameStateManager.Money >= m_placeTower.BaseCost)
                {
                    m_gameStateManager.Towers.Add(m_placeTower);
                    m_gameStateManager.Money -= m_placeTower.BaseCost;
                    m_gameStateManager.Score += m_placeTower.BaseCost;
                    m_placeTower = null;
                    m_soundManager.PlayPlaceSound();
                }
                else
                {
                    m_soundManager.PlayErrorSound();
                }
            }
            else // Handle tower selection.
            {
                var selectedTower = m_gameStateManager.Towers.FirstOrDefault(t => 
                    t.Bounds.Contains(new Vector3(mousePos.X, mousePos.Y, 0)) == ContainmentType.Contains);

                if (SelectedTower != selectedTower)
                {
                    SelectedTower = selectedTower;
                }
                else // De-select when clicking on the active tower.
                {
                    SelectedTower = null;
                }
            }
        }

        public void Update(GameTime gameTime, Rectangle backgroundRectangle)
        {
            if (!m_gameStateManager.isGameOver)
            {
                if (m_placeTower != null)
                {
                    var pos = new Vector2(MathF.Round(m_mouseInput.Position.X / 50.0f) * 50, MathF.Round(m_mouseInput.Position.Y / 50.0f) * 50);

                    m_placeTower.Center = pos;

                    m_isPlaceable = CheckPlaceable(GamePlayView.TowerSize, backgroundRectangle, CreepManager.CreepSize, pos,
                        m_placeTower.BaseCost);
                }

                foreach (var tower in m_gameStateManager.Towers)
                {
                    var projectile = tower.Update(gameTime, m_gameStateManager.Creeps, GamePlayView.ProjectileSize);
                    if (projectile != null)
                    {
                        switch (tower.type)
                        {
                            case TowerType.Air:
                                m_soundManager.PlayMissileSound(gameTime);
                                break;
                            case TowerType.Bullet:
                                m_soundManager.PlayBulletSound(gameTime);
                                break;
                            case TowerType.Bomb:
                                m_soundManager.PlaySlingSound(gameTime);
                                break;
                            case TowerType.Mixed:
                                m_soundManager.PlayCannonSound(gameTime);
                                break;
                        }
                        m_gameStateManager.Projectiles.Add(projectile);
                    }
                }
            }
            else
            {
                foreach(var tower in m_gameStateManager.Towers)
                {
                    m_particleManager.AddMissileExplosion(tower.Center);
                }
                m_gameStateManager.Towers.Clear();
                m_placeTower = null;
                SelectedTower = null;
            }
        }

        private bool CheckPlaceable(int towerSize, Rectangle backgroundRectangle, int creepSize, Vector2 pos, int cost)
        {
            var isInTowerZone = m_towerZone.Contains(new Rectangle((int) (pos.X - towerSize / 2.0), (int) (pos.Y - towerSize / 2.0), towerSize, towerSize));

            if (!isInTowerZone) return false; // Check place bounds.
            else if (CheckCollision(m_placeTower.Bounds, m_gameStateManager.Towers) != null) return false; // Check for tower overlaps.
            else if (CheckCollision(m_placeTower.Bounds, m_gameStateManager.Creeps) != null) return false; // Check for creep overlaps.
            else if (m_gameStateManager.Money < cost) return false; // Check to make sure we have enough money.
            else // Check pathing.
            {
                m_gameStateManager.Towers.Add(m_placeTower);

                var upDownGraph = new WeightedGraph(m_gameStateManager.Towers, backgroundRectangle.X + 50 + creepSize / 2,
                    backgroundRectangle.Y + creepSize / 2,
                    backgroundRectangle.Size.Y / creepSize, (backgroundRectangle.Size.X - 100) / creepSize, creepSize);

                var sideToSideGraph = new WeightedGraph(m_gameStateManager.Towers, backgroundRectangle.X + creepSize / 2,
                    backgroundRectangle.Y + 50 + creepSize / 2,
                    (backgroundRectangle.Size.Y - 100) / creepSize, backgroundRectangle.Size.X / creepSize, creepSize);

                m_gameStateManager.Towers.Remove(m_placeTower);
                if (upDownGraph.FindPath(500, 25, 500, 1075).Count == 0 ||
                    sideToSideGraph.FindPath(225, 500, 1275, 500).Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public TowerType? GetPlaceTowerType()
        {
            if (m_placeTower != null) return m_placeTower.type;

            return null;
        }

        public void SetPlaceTowerType(TowerType? towerType)
        {
           
            if (towerType != null)
            {
                if (m_placeTower != null && towerType == m_placeTower.type)
                {
                    m_placeTower = null;
                    return;
                }

                if (m_gameStateManager.Money >= GetCost(towerType.Value))
                {
                    var pos = new Vector2(MathF.Round(m_mouseInput.Position.X / 50.0f) * 50, MathF.Round(m_mouseInput.Position.Y / 50.0f) * 50);
                    m_placeTower = new Tower(new Vector2(GamePlayView.TowerSize, GamePlayView.TowerSize), pos,
                        towerType.Value,
                        GetCost(towerType.Value));
                }
                else
                {
                    m_soundManager.PlayErrorSound();
                } 
            }
            else
            {
                m_placeTower = null;
            }
        }

        public static int GetCost(TowerType type)
        {
            switch (type)
            {
                case TowerType.Air: return AirTowerCost;
                case TowerType.Bullet: return BulletTowerCost;
                case TowerType.Bomb: return BombTowerCost;
                case TowerType.Mixed: return MixedTowerCost;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SellTower(Tower tower)
        {
            m_gameStateManager.Towers.Remove(tower);
            m_gameStateManager.Money += tower.TotalCost / 2;
            m_gameStateManager.Score -= tower.TotalCost;

            m_particleManager.AddSellEffect(tower.Center);
            m_soundManager.PlaySellSound();
        }

        public void UpgradeTower(Tower tower)
        {
            if (tower.CanUpgrade() &&
                m_gameStateManager.Money >= tower.UpgradeCost)
            {
                m_gameStateManager.Money -= tower.UpgradeCost;
                m_gameStateManager.Score += tower.UpgradeCost;
                tower.Upgrade();
                m_soundManager.PlayUpgradeSound();
            }
        }

        private static T CheckCollision<T>(BoundingBox check, IEnumerable<T> objects)
            where T : CollidableObject
        {
            return objects.FirstOrDefault(o => o.Bounds.Intersects(check));
        }
    }
}
