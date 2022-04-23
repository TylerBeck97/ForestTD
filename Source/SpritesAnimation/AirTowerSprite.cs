using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Objects;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    class AirTowerSprite : SpriteArray
    {
        public AirTowerSprite(SpriteSheet spriteSheet,
            int subImageWidth, int subImageHeight, (int, int)[] sprites) :
            base(spriteSheet, subImageWidth, subImageHeight, sprites)
        {
        }

        public void draw(GameTime time, SpriteBatch spriteBatch, Tower tower, double heading, bool isTransparent, Color color)
        {
            var spriteIndex = tower.BurstIndex;
            if (tower.BurstIndex == tower.BurstLength &&
                (time.TotalGameTime - tower.LastShot).TotalMilliseconds > (tower.FireRate - tower.FireRate / 2))
            {
                spriteIndex = 0;
            }

            draw(spriteBatch, tower, spriteIndex, heading, isTransparent, color);
        }
    }
}