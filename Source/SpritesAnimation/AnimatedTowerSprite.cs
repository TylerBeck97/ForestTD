using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Objects;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    class AnimatedTowerSprite : SpriteArray
    {
        public AnimatedTowerSprite(SpriteSheet spriteSheet,
            int subImageWidth, int subImageHeight, (int, int)[] sprites) :
            base(spriteSheet, subImageWidth, subImageHeight, sprites)
        {
        }

        public void draw(GameTime time, SpriteBatch spriteBatch, Tower tower, double heading, bool isTransparent, Color color)
        {
            var subImageIndex = (int) ((time.TotalGameTime - tower.LastShot).TotalMilliseconds / (tower.FireRate / _sprites.Length));
            if (subImageIndex >= _sprites.Length)
            {
                subImageIndex = 0;
            }

            draw(spriteBatch, tower, subImageIndex, heading, isTransparent, color);
        }
    }
}
