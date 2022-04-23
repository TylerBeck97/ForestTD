using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    public class SpriteSheet
    {
        public SpriteSheet(Texture2D texture)
        {
            Texture = texture;
        }

        public Vector2 Offset { get; set; }

        public Texture2D Texture { get; set; }
    }
}
