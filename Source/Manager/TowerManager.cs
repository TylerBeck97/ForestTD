using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Graph;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.SpritesAnimation;
using SpaceMarines_TD.Source.Views;

namespace SpaceMarines_TD.Source.Manager
{
    class TowerManager
    {
        public const int AirTowerCost = 150;
        public const int BulletTowerCost = 100;
        public const int BombTowerCost = 200;
        public const int MixedTowerCost = 300;

        private AirTowerSprite m_airTowerRenderer;
        private AnimatedTowerSprite m_groundTowerRenderer;
        private StaticSprite m_bombTowerRenderer;

        private Texture2D m_towerBaseTexture2D;
        private Texture2D m_rangeCircleTexture2D;

        private Tower m_placeTower;

        private bool m_isPlaceable = false;

        private Rectangle m_towerZone = new Rectangle(225, 25, 1000, 1000);

        private GameStateManager m_gameStateManager;

        public Tower SelectedTower { get; private set; }

        public TowerManager(GameStateManager gameStateManager)
        {
            m_gameStateManager = gameStateManager;
        }

        public void loadContent(ContentManager contentManager, SpriteSheet spriteSheet)
        {
            m_airTowerRenderer = new AirTowerSprite(spriteSheet, 100, 100, new[]
            {
                (1, 103),
                (102, 103),
                (203, 103),
            });
            m_bombTowerRenderer = new StaticSprite(spriteSheet, 100, 100, 1, 203);

            m_groundTowerRenderer = new AnimatedTowerSprite(spriteSheet, 100, 100, 
                new []
                {
                    (1, 1),
                    (102, 1),
                    (203, 1),
                    (304, 1)
                });

            m_rangeCircleTexture2D = contentManager.Load<Texture2D>("rangeCircle");
            m_towerBaseTexture2D = contentManager.Load<Texture2D>("TowerBase");
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

                switch (tower.m_type)
                {
                    case TowerType.Air:
                        m_airTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;

                    case TowerType.Bullet:
                        m_groundTowerRenderer.draw(gameTime, spriteBatch, tower, tower.Heading + MathHelper.PiOver2, false, towerColor);
                        break;

                    case TowerType.Bomb:
                        m_bombTowerRenderer.draw(spriteBatch, tower, tower.Heading + MathHelper.PiOver2, towerColor);
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

            switch (tower.m_type)
            {
                case TowerType.Air:
                    m_airTowerRenderer.draw(gameTime, spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Bullet:
                    m_groundTowerRenderer.draw(gameTime, spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Bomb:
                    m_bombTowerRenderer.draw(spriteBatch, tower, MathHelper.PiOver2, true, towerColor);
                    break;

                case TowerType.Mixed:
                    // TODO
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
                    m_placeTower = null;
                }
                else
                {
                    // TODO: play error sound
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

        public void Update(GameTime gameTime, Rectangle backgroundRectangle, int creepSize)
        {
            var state = Mouse.GetState();

            if (m_placeTower != null)
            {
                // TODO Revisit how to guarantee that there is a path
                var pos = new Vector2(MathF.Round(state.X / 50.0f) * 50, MathF.Round(state.Y / 50.0f) * 50);

                m_placeTower.Center = pos;

                m_isPlaceable = CheckPlaceable(GamePlayView.TowerSize, backgroundRectangle, creepSize, pos,
                    m_placeTower.BaseCost);
            }

            foreach (var tower in m_gameStateManager.Towers)
            {
                var projectile = tower.Update(gameTime, m_gameStateManager.Creeps, GamePlayView.ProjectileSize);
                if (projectile != null)
                {
                    m_gameStateManager.Projectiles.Add(projectile);
                }
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
                if (upDownGraph.FindPath(500, 25, 500, 1025).Count == 0 ||
                    sideToSideGraph.FindPath(225, 500, 1225, 500).Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetPlaceTowerType(TowerType? towerType)
        {
            if (towerType != null)
            {
                var state = Mouse.GetState();

                var pos = new Vector2(MathF.Round(state.X / 50.0f) * 50, MathF.Round(state.Y / 50.0f) * 50);
                m_placeTower = new Tower(new Vector2(GamePlayView.TowerSize, GamePlayView.TowerSize), pos,
                    towerType.Value,
                    GetCost(towerType.Value));
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

        private static T CheckCollision<T>(BoundingBox check, IEnumerable<T> objects)
            where T : CollidableObject
        {
            return objects.FirstOrDefault(o => o.Bounds.Intersects(check));
        }
    }
}
