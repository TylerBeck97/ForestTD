using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Input;

namespace SpaceMarines_TD.Source.Views
{
    class ControlsView : GameStateView
    {
        private SettingsManager m_settings;

        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;

        private Texture2D m_background;

        private Rectangle m_sellRectangle;
        private Rectangle m_upgradeRectangle;
        private Rectangle m_startRectangle;

        private MouseInput m_inputMouse;

        private enum MenuState
        {
            SellTower,
            Upgrade,
            StartLevel
        }

        private string m_setBinding;

        private MenuState m_currentSelection = MenuState.SellTower;
        private bool m_waitForKeyRelease = true;
        private bool m_bindingKey = false;

        public ControlsView(SettingsManager settings)
        {
            m_settings = settings;
            m_inputMouse = new MouseInput();
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("fonts/Roboto36");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("fonts/Roboto46");

            m_background = contentManager.Load<Texture2D>("images/MainMenu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputMouse.Update(gameTime, m_scalingMatrix);

            var mousePos = new Vector2(m_inputMouse.Position.X, m_inputMouse.Position.Y);

            if (m_sellRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    m_setBinding = "Sell Tower";
                    m_bindingKey = true;
                }
                m_currentSelection = MenuState.SellTower;
            }
            if (m_upgradeRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    m_setBinding = "Upgrade Tower";
                    m_bindingKey = true;
                }
                m_currentSelection = MenuState.Upgrade;
            }
            if (m_startRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    m_setBinding = "Start Level";
                    m_bindingKey = true;
                }
                m_currentSelection = MenuState.StartLevel;
            }
            

            var state = Keyboard.GetState();
            if (!m_waitForKeyRelease)
            {
                if (m_bindingKey)
                {
                    if (state.IsKeyDown(Keys.Escape))
                    {
                        m_bindingKey = false;
                        m_waitForKeyRelease = true;
                        return GameStateEnum.Controls;
                    }

                    if (state.GetPressedKeys().Length > 0)
                    {
                        switch (m_setBinding)
                        {
                            case "Sell Tower":
                                m_settings.Bindings.SellTower = state.GetPressedKeys()[0].ToString();
                                break;
                            case "Upgrade Tower":
                                m_settings.Bindings.Upgrade = state.GetPressedKeys()[0].ToString();
                                break;
                            case "Start Level":
                                m_settings.Bindings.StartLevel = state.GetPressedKeys()[0].ToString();
                                break;
                        }
                        m_settings.Store();
                        m_bindingKey = false;
                        m_waitForKeyRelease = true;
                    }

                }
                else
                {
                    if (state.IsKeyDown(Keys.Escape))
                    {
                        m_waitForKeyRelease = true;
                        return GameStateEnum.MainMenu;
                    }

                    if (state.IsKeyDown(Keys.Down))
                    {
                        m_currentSelection = m_currentSelection + 1 <= MenuState.StartLevel ? m_currentSelection + 1 : MenuState.StartLevel;
                        m_waitForKeyRelease = true;
                    }

                    if (state.IsKeyDown(Keys.Up))
                    {
                        m_currentSelection = m_currentSelection - 1 >= MenuState.SellTower ? m_currentSelection - 1 : MenuState.SellTower;
                        m_waitForKeyRelease = true;
                    }

                    if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.SellTower)
                    {
                        m_setBinding = "Sell Tower";
                        m_bindingKey = true;
                        m_waitForKeyRelease = true;
                    }

                    if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Upgrade)
                    {
                        m_setBinding = "Upgrade Tower";
                        m_bindingKey = true;
                        m_waitForKeyRelease = true;
                    }

                    if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.StartLevel)
                    {
                        m_setBinding = "Start Level";
                        m_bindingKey = true;
                        m_waitForKeyRelease = true;
                    }
                }


            }
            else if (state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.Enter) && state.IsKeyUp(Keys.Escape))
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.Controls;
        }

        public override void update(GameTime gameTime) { }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(transformMatrix: m_scalingMatrix);

            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, 1920, 1080), Color.White);
            var text = $"Sell Tower: {m_settings.Bindings.SellTower}";
            var font = m_currentSelection == MenuState.SellTower ? m_fontMenuSelect : m_fontMenu;
            var stringSize = font.MeasureString(text);
            m_sellRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), 300, (int)stringSize.X, (int)stringSize.Y);
            var bottom = drawMenuItem(font, text, 300, m_currentSelection == MenuState.SellTower ? Color.Yellow : Color.Red);

            text = $"Upgrade Tower: {m_settings.Bindings.Upgrade}";
            font = m_currentSelection == MenuState.Upgrade ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_upgradeRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(font, text, bottom, m_currentSelection == MenuState.Upgrade ? Color.Yellow : Color.Red);
            

            text = $"Start Level: {m_settings.Bindings.StartLevel}";
            font = m_currentSelection == MenuState.StartLevel ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_startRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(m_currentSelection == MenuState.StartLevel ? m_fontMenuSelect : m_fontMenu, "Start Level: " + m_settings.Bindings.StartLevel, bottom, m_currentSelection == MenuState.StartLevel ? Color.Yellow : Color.Red);


            if (m_bindingKey)
            {
                bottom = drawMenuItem(m_fontMenuSelect, $"Press the key you wish to bind {m_setBinding} to", bottom,Color.Red);
                drawMenuItem(m_fontMenuSelect, "Press esc to cancel", bottom, Color.Red);
            }
            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            drawOutlineText(
                m_spriteBatch,
                font,
                text,
                Color.White,
                color,
                new Vector2(1920 / 2 - stringSize.X / 2, y),
                1.0f);

            return y + stringSize.Y;
        }
    }
}
