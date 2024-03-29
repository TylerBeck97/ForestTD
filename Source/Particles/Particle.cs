using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.Particles
{
    public class Particle
    {
        public int name;
        public Vector2 position;
        public float rotation; // on the chopping block
        public Vector2 direction;
        public float speed;
        public TimeSpan lifetime;
        public double originalLifetime;
        public Rectangle sourceRectangle;
        public Texture2D texture;
        public int size;
        public int offset;
        public int frameCount;
        public bool fadeOut;
        public Color color;

        public Particle( Vector2 position, Vector2 direction, float speed, TimeSpan lifetime, Texture2D texture, 
            Rectangle sourceRectangle, int size, bool fade)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.lifetime = lifetime;
            originalLifetime = lifetime.TotalMilliseconds;
            this.texture = texture;
            this.size = size;
            this.sourceRectangle = sourceRectangle;
            fadeOut = fade;
            color = Color.White;

            this.frameCount = 1;
        }

        public Particle(Vector2 position, Vector2 direction, float speed, TimeSpan lifetime, Texture2D texture,
            Rectangle sourceRectangle, int size, int offset, int frameCount, bool fade)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.lifetime = lifetime;
            originalLifetime = lifetime.TotalMilliseconds;
            this.texture = texture;
            this.size = size;
            this.sourceRectangle = sourceRectangle;
            this.offset = offset;
            this.frameCount = frameCount;
            fadeOut = fade;
            color = Color.White;
        }
    }
}
