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
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("fonts/Roboto36");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("fonts/Roboto46");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
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
            m_spriteBatch.Begin();

            var bottom = drawMenuItem(m_currentSelection == MenuState.SellTower ? m_fontMenuSelect : m_fontMenu, "Sell Tower: " + m_settings.Bindings.SellTower, 200, m_currentSelection == MenuState.SellTower ? Color.Yellow : Color.Red);
            bottom = drawMenuItem(m_currentSelection == MenuState.Upgrade ? m_fontMenuSelect : m_fontMenu, "Upgrade Tower: " + m_settings.Bindings.Upgrade, bottom, m_currentSelection == MenuState.Upgrade ? Color.Yellow : Color.Red);
            bottom = drawMenuItem(m_currentSelection == MenuState.StartLevel ? m_fontMenuSelect : m_fontMenu, "Start Level: " + m_settings.Bindings.StartLevel, bottom, m_currentSelection == MenuState.StartLevel ? Color.Yellow : Color.Red);

            if (m_bindingKey)
            {
                bottom = drawMenuItem(m_fontMenuSelect, String.Format("Press the key you wish to bind {0} to", m_setBinding), bottom,Color.Red);
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
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                1.0f);

            return y + stringSize.Y;
        }
    }
}
