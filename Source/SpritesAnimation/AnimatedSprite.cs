using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    class AnimatedSprite : SpriteArray
    {
        protected readonly float m_time;

        protected TimeSpan m_animationTime;
        protected int m_subImageIndex;

        public AnimatedSprite(SpriteSheet spriteSheet, int subImageWidth, int subImageHeight,
            (int, int)[] sprites, float time) :
            base (spriteSheet, subImageWidth, subImageHeight, sprites)
        {
            m_time = time;
        }

        public void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, double heading)
        {
            draw(spriteBatch, model, m_subImageIndex, heading, false, Color.White);
        }
        
        public virtual void update(GameTime gameTime)
        {
            m_animationTime += gameTime.ElapsedGameTime;
            if (m_animationTime.TotalMilliseconds >= m_time)
            {
                m_animationTime = TimeSpan.Zero;
                m_subImageIndex++;
                m_subImageIndex %= _sprites.Length;
            }
        }
    }
}
