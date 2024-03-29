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
using SpaceMarines_TD.Source.Views;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SpaceMarines_TD.Source.Objects
{
    class UIOverlay
    {
        private const int ThumbNailSize = 100;

        private Texture2D m_airTowerTexture2D;
        private Rectangle m_airTowerRectangle;

        private Texture2D m_groundTowerTexture2D;
        private Rectangle m_groundTowerRectangle;

        private Texture2D m_bombTowerTexture2D;
        private Rectangle m_bombTowerRectangle;

        private Texture2D m_mixedTowerTexture2D;
        private Rectangle m_mixedTowerRectangle;

        private AnimatedSprite m_arrowRenderer;

        private SpriteFont m_font;
        private SpriteFont m_bigFont;

        private Button m_upgradeButton;
        private Button m_sellButton;

        private GameStateManager m_gameStateManager;
        private readonly TowerManager m_towerManager;

        private SettingsManager m_settingManager;

        private bool IsDetailPanelVisible => m_towerManager.SelectedTower != null;
        public Rectangle SideBarRectangle { get; }

        public UIOverlay(Rectangle rectangle, GameStateManager gameStateManager, TowerManager towerManager)
        {
            SideBarRectangle = rectangle;

            m_gameStateManager = gameStateManager;
            m_towerManager = towerManager;
        }

        public void Intialize(ContentManager contentManager, SettingsManager settingsManager, SpriteSheet spriteSheet)
        { 

            m_airTowerTexture2D = contentManager.Load<Texture2D>("images/AirThumb");
            m_airTowerRectangle = new Rectangle(1550, 50, ThumbNailSize, ThumbNailSize);

            m_groundTowerTexture2D = contentManager.Load<Texture2D>("images/GroundThumb");
            m_groundTowerRectangle = new Rectangle(1750, 50, ThumbNailSize, ThumbNailSize);

            m_bombTowerTexture2D = contentManager.Load<Texture2D>("images/BombThumb");
            m_bombTowerRectangle = new Rectangle(1550, 300, ThumbNailSize, ThumbNailSize);

            m_mixedTowerTexture2D = contentManager.Load<Texture2D>("images/MixedThumb");
            m_mixedTowerRectangle = new Rectangle(1750, 300, ThumbNailSize, ThumbNailSize);

            m_font = contentManager.Load<SpriteFont>("fonts/Roboto20");
            m_bigFont = contentManager.Load<SpriteFont>("fonts/Roboto36");

            m_upgradeButton = new Button(new Rectangle(SideBarRectangle.Center.X - 100, 880, 200, 50),
                contentManager.Load<Texture2D>("images/Button"),
                contentManager.Load<Texture2D>("images/ButtonDown"),
                contentManager.Load<SpriteFont>("fonts/Roboto20"),
                "Upgrade");
            m_sellButton = new Button(new Rectangle(SideBarRectangle.Center.X - 100, 950, 200, 50),
                contentManager.Load<Texture2D>("images/Button"),
                contentManager.Load<Texture2D>("images/ButtonDown"),
                contentManager.Load<SpriteFont>("fonts/Roboto20"),
                "Sell");

            m_arrowRenderer = new AnimatedSprite(spriteSheet, 150, 150,
                new []{
                    (1, 519),
                    (202, 519)
                }, 200.0f);

            m_upgradeButton.Clicked += OnUpgradeButtonClicked;
            m_sellButton.Clicked += OnSellButtonClicked;

            m_settingManager = settingsManager;
        }

        private void OnUpgradeButtonClicked(object sender, EventArgs e)
        {
            var tower = m_towerManager.SelectedTower;
            if (tower == null) return;

            m_towerManager.UpgradeTower(tower);
        }

        private void OnSellButtonClicked(object sender, EventArgs e)
        {
            var tower = m_towerManager.SelectedTower;
            if (tower == null) return;

            m_towerManager.SellTower(tower);
            m_towerManager.SelectedTower = null;
        }

        public bool HandleInput(MouseInput mouse)
        {
            var handled = false;

            if (IsDetailPanelVisible)
            {
                handled |= m_upgradeButton.Update(mouse.Position, mouse.Clicked, mouse.MouseDown);
                handled |= m_sellButton.Update(mouse.Position, mouse.Clicked, mouse.MouseDown);
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
                $"Wave: {m_gameStateManager.Wave}",
                $"Lives: {m_gameStateManager.Lives}", 
                $"Score: {m_gameStateManager.Score}",
                $"Money: {m_gameStateManager.Money}");

            // Draw buy menu.
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

                m_sellButton.Draw(spriteBatch);

                DrawText(spriteBatch, new Vector2(SideBarRectangle.Center.X, 600), 2, true,
                    $"{selectedTower.type} Tower",
                    $"Level {selectedTower.TowerLevel}",
                    " ",
                    $"Range: {selectedTower.Range}",
                    $"Fire Rate: {(1000.0 / selectedTower.FireRate):0.##}",
                    $"Damage: {selectedTower.Damage}",
                    selectedTower.CanUpgrade() ? $"Upgrade Cost: {selectedTower.UpgradeCost}" : "",
                    $"Sell Value: {selectedTower.TotalCost / 2}");
            }

            // Draw Start Level Text and Entrance Arrow(s)
            if (!m_gameStateManager.isLevelActive && !m_gameStateManager.isGameOver)
            {
                var text = $"Press {m_settingManager.Bindings.StartLevel} to start the next level";
                var stringSize = m_bigFont.MeasureString(text);
                drawOutlineText(spriteBatch, m_bigFont, text, Color.Black, Color.Red,
                    new Vector2(1500 / 2 - stringSize.X / 2, 200), 1.0f);

                var size = new Vector2(150, 150);
                if ((m_gameStateManager.Level + 1) % 2 == 0)
                {
                    m_arrowRenderer.draw(spriteBatch, new AnimatedSpriteModel(size, new Vector2(750, 100)), MathHelper.PiOver2);
                    m_arrowRenderer.draw(spriteBatch, new AnimatedSpriteModel(size, new Vector2(750, 980)), MathHelper.PiOver2 + MathHelper.Pi);  
                }
                else
                {
                    m_arrowRenderer.draw(spriteBatch, new AnimatedSpriteModel(size, new Vector2(300, 540)), 0);
                    m_arrowRenderer.draw(spriteBatch, new AnimatedSpriteModel(size, new Vector2(1200, 540)), MathHelper.Pi);
                }
            }
            if (m_gameStateManager.isGameOver)
            {
                var text = "Game over!";
                var stringSize = m_bigFont.MeasureString(text);
                drawOutlineText(spriteBatch, m_bigFont, text, Color.Black, Color.Red,
                    new Vector2(1500 / 2 - stringSize.X / 2, 200), 1.0f);

                text = "Press esc to return to main menu or F2 to restart";
                stringSize = m_bigFont.MeasureString(text);
                drawOutlineText(spriteBatch, m_bigFont, text, Color.Black, Color.Red,
                    new Vector2(1500 / 2 - stringSize.X / 2, 300), 1.0f);
            }
        }

        public void Update(GameTime gameTime)
        {
            m_arrowRenderer.update(gameTime);
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

        private void drawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color backColor, Color frontColor, Vector2 position, float scale)
        {
            const float PIXEL_OFFSET = 1.0f;
            //
            // Offset to the upper left and lower right - faster, but not as good
            //spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            //
            // Offset in each of left,right, up, down directions - slower, but good
            spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, 0), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, 0), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, PIXEL_OFFSET * scale), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, PIXEL_OFFSET * scale), backColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            //
            // This sits inside the text rendering done just above
            spriteBatch.DrawString(font, text, position, frontColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
