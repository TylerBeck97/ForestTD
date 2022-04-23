using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceMarines_TD.Source.Views
{
    class CreditsView : GameStateView
    {
        private SpriteFont m_font;
        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("fonts/OpenSans36");
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
            m_spriteBatch.Begin();

            var bottom = drawMenuItem(m_font, "Made by Tyler Beck", 200, Color.Red);
            bottom = drawMenuItem(m_font,
                "Sprites from spriters-resource.com", bottom, Color.Red);
            drawMenuItem(m_font, "Sounds from opengameart.org, Authors: Prinsu-Kun and Muncheybobo", bottom, Color.Red);

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
