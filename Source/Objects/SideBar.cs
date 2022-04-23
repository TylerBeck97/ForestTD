using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Input;
using SpaceMarines_TD.Source.Manager;
using SpaceMarines_TD.Source.SpritesAnimation;
using SpaceMarines_TD.Source.UI;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SpaceMarines_TD.Source.Objects
{
    class SideBar
    {
        private const int ThumbNailSize = 100;

        private Texture2D m_sideBarTexture2D;

        private Texture2D m_airTowerTexture2D;
        private Rectangle m_airTowerRectangle;

        private Texture2D m_groundTowerTexture2D;
        private Rectangle m_groundTowerRectangle;

        private Texture2D m_bombTowerTexture2D;
        private Rectangle m_bombTowerRectangle;

        private Texture2D m_mixedTowerTexture2D;
        private Rectangle m_mixedTowerRectangle;

        private SpriteFont m_font;

        private Button m_upgradeButton;

        private GameStateManager m_gameStateManager;
        private readonly TowerManager m_towerManager;

        private bool IsDetailPanelVisible => m_towerManager.SelectedTower != null;
        public Rectangle SideBarRectangle { get; }

        public SideBar(Rectangle rectangle, GameStateManager gameStateManager, TowerManager towerManager)
        {
            SideBarRectangle = rectangle;

            m_gameStateManager = gameStateManager;
            m_towerManager = towerManager;
        }

        public void Intialize(ContentManager contentManager)
        {
            m_sideBarTexture2D = contentManager.Load<Texture2D>("SideBar");

            m_airTowerTexture2D = contentManager.Load<Texture2D>("AirThumb");
            m_airTowerRectangle = new Rectangle(1550, 50, ThumbNailSize, ThumbNailSize);

            m_groundTowerTexture2D = contentManager.Load<Texture2D>("GroundThumb");
            m_groundTowerRectangle = new Rectangle(1750, 50, ThumbNailSize, ThumbNailSize);

            m_bombTowerTexture2D = contentManager.Load<Texture2D>("BombThumb");
            m_bombTowerRectangle = new Rectangle(1550, 300, ThumbNailSize, ThumbNailSize);

            m_mixedTowerTexture2D = contentManager.Load<Texture2D>("MixedThumb");
            m_mixedTowerRectangle = new Rectangle(1750, 300, ThumbNailSize, ThumbNailSize);

            m_font = contentManager.Load<SpriteFont>("fonts/Roboto20");

            m_upgradeButton = new Button(new Rectangle(SideBarRectangle.Center.X - 100, 900, 200, 50),
                contentManager.Load<Texture2D>("Button"),
                contentManager.Load<Texture2D>("ButtonDown"),
                contentManager.Load<SpriteFont>("fonts/Roboto20"),
                "Upgrade");
            m_upgradeButton.Clicked += OnUpgradeButtonClicked;
        }

        private void OnUpgradeButtonClicked(object? sender, EventArgs e)
        {
            var tower = m_towerManager.SelectedTower;
            if (tower == null) return;

            if (tower.CanUpgrade() &&
                m_gameStateManager.Money >= tower.UpgradeCost)
            {
                m_gameStateManager.Money -= tower.UpgradeCost;
                tower.Upgrade();
            }
        }

        public bool HandleInput(MouseInput mouse)
        {
            var handled = false;

            if (IsDetailPanelVisible)
            {
                handled |= m_upgradeButton.Update(mouse.Position, mouse.Clicked, mouse.MouseDown);
            }

            if (!handled && mouse.Clicked && SideBarRectangle.Contains(mouse.Position))
            {
                var towerType = GetPlacedTower(mouse.Position);
                m_towerManager.SetPlaceTowerType(towerType);

                if (towerType != null)
                {
                    handled = true;
                }
            }

            return handled;
        }

        private TowerType? GetPlacedTower(Vector2 pos)
        {
            if (m_airTowerRectangle.Contains(pos)) return TowerType.Air;
            if (m_groundTowerRectangle.Contains(pos)) return TowerType.Bullet;
            if (m_bombTowerRectangle.Contains(pos)) return TowerType.Bomb;
            if (m_mixedTowerRectangle.Contains(pos)) return TowerType.Mixed;

            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw top left info.
            DrawText(spriteBatch, new Vector2(20, 20), 2, false,
                $"Level: {m_gameStateManager.Level}", 
                $"Lives: {m_gameStateManager.Lives}", 
                $"Score: {m_gameStateManager.Score}",
                $"Money: {m_gameStateManager.Money}");

            // Draw buy menu.
            spriteBatch.Draw(m_sideBarTexture2D, SideBarRectangle, Color.White);
            
            spriteBatch.Draw(m_airTowerTexture2D, m_airTowerRectangle, Color.White);

            spriteBatch.Draw(m_groundTowerTexture2D, m_groundTowerRectangle, Color.White);

            spriteBatch.Draw(m_bombTowerTexture2D, m_bombTowerRectangle, Color.White);

            spriteBatch.Draw(m_mixedTowerTexture2D, m_mixedTowerRectangle, Color.White);
            
            DrawText(spriteBatch, new Vector2(1600, 160), 2, true,
                "Anti-Air Tower", $"Cost: {TowerManager.AirTowerCost}");
            DrawText(spriteBatch, new Vector2(1800, 160), 2, true,
                "Bullet Tower", $"Cost: {TowerManager.BulletTowerCost}");
            DrawText(spriteBatch, new Vector2(1600, 410), 2, true,
                "Bomb Tower", $"Cost: {TowerManager.BombTowerCost}");
            DrawText(spriteBatch, new Vector2(1800, 410), 2, true,
                "Mixed Tower", $"Cost: {TowerManager.MixedTowerCost}");

            // Draw tower info for selected tower.
            if (IsDetailPanelVisible)
            {
                var selectedTower = m_towerManager.SelectedTower;

                if (selectedTower.CanUpgrade())
                {
                    m_upgradeButton.Draw(spriteBatch);
                }

                DrawText(spriteBatch, new Vector2(SideBarRectangle.Center.X, 625), 2, true,
                    $"{selectedTower.m_type} Tower",
                    $"Level {selectedTower.TowerLevel}",
                    " ",
                    $"Range: {selectedTower.Range}",
                    $"Fire Rate: {(1000.0 / selectedTower.FireRate):0.##}",
                    $"Damage: {selectedTower.Damage}");
            }
        }

        private void DrawText(SpriteBatch spriteBatch, Vector2 pos, float lineSpacing, bool centered, params string[] strings)
        {
            foreach (var str in strings)
            {
                var stringSize = m_font.MeasureString(str);
                var offset = centered ? -Vector2.UnitX * stringSize.X / 2 : Vector2.Zero;

                var offsetPos = pos + offset;
                spriteBatch.DrawString(m_font, str, new Vector2((int) offsetPos.X, (int) offsetPos.Y), Color.White);
                pos += Vector2.UnitY * (stringSize.Y + lineSpacing);
            }
        }
    }
}
