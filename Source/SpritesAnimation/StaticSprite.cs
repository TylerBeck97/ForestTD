using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    class StaticSprite
    {
        private SpriteSheet m_spriteSheet;

        private int m_subImageWidth;
        private int m_subImageHeight;

        private int m_spriteX;
        private int m_spriteY;

        public StaticSprite(SpriteSheet spriteSheet, int subImageWidth, int subImageHeight, 
            int spriteX, int spriteY)
            : this(spriteSheet, subImageWidth, subImageHeight)
        {
            m_spriteX = spriteX;
            m_spriteY = spriteY;
        }

        public StaticSprite(SpriteSheet mSpriteSheet, int mSubImageWidth, int mSubImageHeight)
        {
            m_spriteSheet = mSpriteSheet;
            m_subImageWidth = mSubImageWidth;
            m_subImageHeight = mSubImageHeight;
        }

        protected void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, int spriteX, int spriteY, bool isTransparent, Color color)
        {
            spriteBatch.Draw(
                m_spriteSheet.Texture,
                new Rectangle((int)(model.Center.X), (int)(model.Center.Y), (int)model.Size.X, (int)model.Size.Y), // Destination rectangle
                new Rectangle((int) (spriteX + m_spriteSheet.Offset.X), 
                    (int) (spriteY + m_spriteSheet.Offset.Y), m_subImageWidth, m_subImageHeight), // Source sub-texture
                isTransparent ? color * 0.5f : color,
                0, // Angular rotation
                new Vector2(m_subImageWidth / 2.0f, m_subImageHeight / 2.0f), // Center point of rotation
                SpriteEffects.None, 0);
        }

        protected void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, int spriteX, int spriteY, double rotation, bool isTransparent, Color color)
        {
            spriteBatch.Draw(
                m_spriteSheet.Texture,
                new Rectangle((int)(model.Center.X), (int)(model.Center.Y), (int)model.Size.X, (int)model.Size.Y), // Destination rectangle
                new Rectangle((int)(spriteX + m_spriteSheet.Offset.X),
                    (int)(spriteY + m_spriteSheet.Offset.Y), m_subImageWidth, m_subImageHeight), // Source sub-texture
                isTransparent ? color * 0.5f : color,
                (float) rotation, // Angular rotation
                new Vector2(model.Size.X / 2.0f, model.Size.Y / 2.0f), // Center point of rotation
                SpriteEffects.None, 0);
        }

        public virtual void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, double rotation, Color color)
        {
            draw(spriteBatch, model, m_spriteX, m_spriteY, rotation , false, color);
        }

        public virtual void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, double rotation, bool isTransparent, Color color)
        {
            draw(spriteBatch, model, m_spriteX, m_spriteY, rotation, isTransparent, color);
        }
    }
}
