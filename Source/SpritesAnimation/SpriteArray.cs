using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    class SpriteArray : StaticSprite
    {
        protected readonly (int, int)[] _sprites;

        public SpriteArray(SpriteSheet spriteSheet, int subImageWidth, int subImageHeight,
            (int, int)[] sprites)
            : base(spriteSheet, subImageWidth, subImageHeight)
        {
            _sprites = sprites;
        }

        public void draw(SpriteBatch spriteBatch, AnimatedSpriteModel model, int spriteIndex, double heading, bool isTransparent,
            Color color)
        {
            var (spriteX, spriteY) = _sprites[spriteIndex];
            draw(spriteBatch, model, spriteX, spriteY, heading, isTransparent, color);
        }
    }
}
