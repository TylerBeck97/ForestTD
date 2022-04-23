using System;
using Microsoft.Xna.Framework;

namespace SpaceMarines_TD.Source.Particles
{
    public class Particle
    {
        public Particle(int name, Vector2 position, Vector2 direction, float speed, TimeSpan lifetime)
        {
            this.name = name;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.lifetime = lifetime;

            this.rotation = 0;
        }

        public int name;
        public Vector2 position;
        public float rotation;
        public Vector2 direction;
        public float speed;
        public TimeSpan lifetime;
    }
}
