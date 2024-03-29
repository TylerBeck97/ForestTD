using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceMarines_TD.Source.Particles
{
    public class ParticleEmitter
    {

        private Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        private Texture2D m_tex;
        private MyRandom m_random = new MyRandom();

        private TimeSpan m_rate;
        private int m_centerX;
        private int m_minY;
        private int m_maxY;
        private int m_particleSize;
        private int m_speed;
        private TimeSpan m_lifetime;

        public Vector2 Gravity { get; set; }
        public bool emit { get; set; }

        public ParticleEmitter(ContentManager content, TimeSpan rate, int size, int speed, TimeSpan lifetime, string texName)
        {
            m_rate = rate;
            m_particleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;

            m_tex = content.Load<Texture2D>(texName);

            this.Gravity = new Vector2(0, 0);
        }

        private TimeSpan m_accumulated = TimeSpan.Zero;

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void update(GameTime gameTime)
        {
            //
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            var removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                p.rotation += 2 * p.speed / 50.0f;
                //
                // Apply some gravity
                p.direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, (int) (m_particleSize * (float) m_tex.Width / m_tex.Height), m_particleSize);
            foreach (Particle p in m_particles.Values)
            {
                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    m_tex,
                    r,
                    null,
                    Color.White,
                    p.rotation,
                    new Vector2(m_tex.Width / 2, m_tex.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }

        public void ChangeSource(int centerX, int minY, int maxY)
        {
            m_centerX = centerX;
            m_minY = minY;
            m_maxY = maxY;
        }
    }
}
