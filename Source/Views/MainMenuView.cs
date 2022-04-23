using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceMarines_TD.Source.Views
{
    class MainMenuView : GameStateView
    {
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;

        private enum MenuState
        {
            StartGame,
            HighScores,
            Controls,
            Credits,
            Quit
        }

        private MenuState m_currentSelection = MenuState.StartGame;
        private bool m_waitForKeyRelease = false;

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
                // Arrow keys to navigate the menu
                if (state.IsKeyDown(Keys.Down))
                {
                    m_currentSelection = m_currentSelection + 1 <= MenuState.Quit ? m_currentSelection + 1 : MenuState.Quit;
                    m_waitForKeyRelease = true;
                }
                if (state.IsKeyDown(Keys.Up))
                {
                    m_currentSelection = m_currentSelection - 1 >= MenuState.StartGame ? m_currentSelection - 1 : MenuState.StartGame;
                    m_waitForKeyRelease = true;
                }

                // If enter is pressed, return the appropriate new state
                if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.StartGame)
                {
                    return GameStateEnum.GamePlay;
                }
                if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.HighScores)
                {
                    return GameStateEnum.HighScores;
                }
                if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Controls)
                {
                    return GameStateEnum.Controls;
                }
                if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Credits)
                {
                    return GameStateEnum.Credits;
                }
                if (state.IsKeyDown(Keys.Enter) && m_currentSelection == MenuState.Quit)
                {
                    return GameStateEnum.Exit;
                }
            }
            else if (state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.MainMenu;
        }

        public override void update(GameTime gameTime){}

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // I split the first one's parameters on separate lines to help you see them better
            float bottom = drawMenuItem(
                m_currentSelection == MenuState.StartGame ? m_fontMenuSelect : m_fontMenu,
                "Start Game",
                200,
                m_currentSelection == MenuState.StartGame ? Color.Yellow : Color.Red);
            bottom = drawMenuItem(m_currentSelection == MenuState.HighScores ? m_fontMenuSelect : m_fontMenu, "HighScores", bottom, m_currentSelection == MenuState.HighScores ? Color.Yellow : Color.Red);
            bottom = drawMenuItem(m_currentSelection == MenuState.Controls ? m_fontMenuSelect : m_fontMenu, "Controls", bottom, m_currentSelection == MenuState.Controls ? Color.Yellow : Color.Red);
            bottom = drawMenuItem(m_currentSelection == MenuState.Credits ? m_fontMenuSelect : m_fontMenu, "Credits", bottom, m_currentSelection == MenuState.Credits ? Color.Yellow : Color.Red);
            drawMenuItem(m_currentSelection == MenuState.Quit ? m_fontMenuSelect : m_fontMenu, "Quit", bottom, m_currentSelection == MenuState.Quit ? Color.Yellow : Color.Red);

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
