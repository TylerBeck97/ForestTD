using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceMarines_TD.Source.Input;

namespace SpaceMarines_TD.Source.Views
{
    class HighScoreView : GameStateView
    {
        private readonly HighScoreManager m_leaderBoard;
        private SpriteFont m_font;

        private Texture2D m_background;
        public HighScoreView(HighScoreManager leaderBoard)
        {
            m_leaderBoard = leaderBoard;
        }
        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("fonts/Roboto36");
            m_background = contentManager.Load<Texture2D>("images/MainMenu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            return state.IsKeyDown(Keys.Escape) ? GameStateEnum.MainMenu : GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(transformMatrix: m_scalingMatrix);

            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, 1920, 1080), Color.White);

            float bottom = drawMenuItem(m_font, "Top 5 HighScores", 300, Color.Red);
            foreach (var score in m_leaderBoard.LeaderBoard.Scores)
            {
                if (score > 0)
                {
                    bottom = drawMenuItem(m_font, score.ToString(), bottom, Color.Red);
                }
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

        public override void update(GameTime gameTime) { }
    }
}
