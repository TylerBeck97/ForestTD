using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Input;
using SpaceMarines_TD.Source.Manager;
using System.Diagnostics;

namespace SpaceMarines_TD.Source.Views
{
    class MainMenuView : GameStateView
    {
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private SpriteFont m_fontTitle;

        private Texture2D m_menuBackground;

        private Rectangle m_startRectangle;
        private Rectangle m_highscoreRectangle;
        private Rectangle m_controlRectangle;
        private Rectangle m_creditRectangle;
        private Rectangle m_quitRectangle;

        private MouseInput m_inputMouse;

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
            m_fontTitle = contentManager.Load<SpriteFont>("fonts/Roboto64");

            m_menuBackground = contentManager.Load<Texture2D>("images/MainMenu");
            m_inputMouse = new MouseInput();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputMouse.Update(gameTime, m_scalingMatrix);

            var mousePos = new Vector2(m_inputMouse.Position.X, m_inputMouse.Position.Y);
            if (m_startRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    return GameStateEnum.GamePlay;
                }
                m_currentSelection = MenuState.StartGame;
            }
            if (m_highscoreRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    return GameStateEnum.HighScores;
                }
                m_currentSelection = MenuState.HighScores;
            }
            if (m_controlRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    return GameStateEnum.Controls;
                }
                m_currentSelection = MenuState.Controls;
            }
            if (m_creditRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    return GameStateEnum.Credits;
                }
                m_currentSelection = MenuState.Credits;
            }
            if (m_quitRectangle.Contains(mousePos))
            {
                if (m_inputMouse.Clicked)
                {
                    return GameStateEnum.Exit;
                }
                m_currentSelection = MenuState.Quit;
            }
            Debug.WriteLine(mousePos);

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
            else if (state.IsKeyUp(Keys.Down) && state.IsKeyUp(Keys.Up) && state.IsKeyUp(Keys.F1))
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.MainMenu;
        }

        public override void update(GameTime gameTime){}

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(transformMatrix: m_scalingMatrix);

            m_spriteBatch.Draw(m_menuBackground, new Rectangle(0, 0, 1920, 1080), Color.White);

            var stringSize = m_fontTitle.MeasureString("Forest Tower Defence");
            drawOutlineText(m_spriteBatch, m_fontTitle, "Forest Tower Defence", Color.White, Color.DarkGreen, new Vector2(1920 / 2 - stringSize.X / 2, 100), 1.0f);

            var text = "Start Game";
            var font = m_currentSelection == MenuState.StartGame ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_startRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), 300, (int)stringSize.X, (int)stringSize.Y);
            float bottom = drawMenuItem(font,text,300,m_currentSelection == MenuState.StartGame ? Color.Yellow : Color.Red);
            

            text = "HighScores";
            font = m_currentSelection == MenuState.HighScores ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_highscoreRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(font, text, bottom, m_currentSelection == MenuState.HighScores ? Color.Yellow : Color.Red);
            

            text = "Controls";
            font = m_currentSelection == MenuState.Controls ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_controlRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(font, text, bottom, m_currentSelection == MenuState.Controls ? Color.Yellow : Color.Red);
            
            text = "Credits";
            font = m_currentSelection == MenuState.Credits ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_creditRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(font, text, bottom, m_currentSelection == MenuState.Credits ? Color.Yellow : Color.Red);

            text = "Quit";
            font = m_currentSelection == MenuState.Quit ? m_fontMenuSelect : m_fontMenu;
            stringSize = font.MeasureString(text);
            m_quitRectangle = new Rectangle((int)(1920 / 2 - stringSize.X / 2), (int)bottom, (int)stringSize.X, (int)stringSize.Y);
            bottom = drawMenuItem(font, text, bottom, m_currentSelection == MenuState.Quit ? Color.Yellow : Color.Red);

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
