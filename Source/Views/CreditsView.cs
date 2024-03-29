using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceMarines_TD.Source.Views
{
    class CreditsView : GameStateView
    {
        private SpriteFont m_font;

        private Texture2D m_background;
        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("fonts/Roboto36");
            m_background = contentManager.Load<Texture2D>("images/MainMenu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.Credits;
        }

        public override void update(GameTime gameTime){}

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin(transformMatrix: m_scalingMatrix);

            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, 1920, 1080), Color.White);

            var bottom = drawMenuItem(m_font, "Made by Tyler Beck", 300, Color.Red);
            bottom = drawMenuItem(m_font, "Tree and Ground Tile Set made by Surt from opengameart.org", bottom, Color.Red);
            bottom = drawMenuItem(m_font, "Explosion Sprite made by Sogomn from opengameart.org", bottom, Color.Red);
            bottom = drawMenuItem(m_font, "Menu Background made by Cethiel from opengameart.org", bottom, Color.Red);
            bottom = drawMenuItem(m_font, "Sounds Effects and Songs made by Juhani Junkala from opengameart.org", bottom, Color.Red);
            drawMenuItem(m_font, "All other art assets made by me", bottom, Color.Red);

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
