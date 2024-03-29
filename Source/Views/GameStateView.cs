using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.Views
{
    public abstract class GameStateView : IGameState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected Matrix m_scalingMatrix;
        public void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
            m_scalingMatrix = Matrix.CreateScale(m_graphics.GraphicsDevice.Viewport.Width / 1920.0f,
                m_graphics.GraphicsDevice.Viewport.Height / 1080.0f, 1);
        }

        protected static void drawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color backColor, Color frontColor, Vector2 position, float scale)
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

        public abstract void loadContent(ContentManager contentManager);

        public abstract GameStateEnum processInput(GameTime gameTime);

        public abstract void update(GameTime gameTime);

        public abstract void render(GameTime gameTime);
    }
}
