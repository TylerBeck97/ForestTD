using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceMarines_TD.Source.Objects;
using SpaceMarines_TD.Source.Particles;

namespace SpaceMarines_TD.Source.Manager
{
    class ParticleManager
    {
        private readonly Rectangle m_coinRectangle = new Rectangle(1, 405, 25, 25);
        private readonly Rectangle m_smokeRectangle = new Rectangle(1, 431, 25, 25);
        private readonly Rectangle m_explosionRectangle = new Rectangle(1, 460, 32, 32);
        private readonly Rectangle m_creepRectangle = new Rectangle(27, 405, 50, 50);
        private readonly Rectangle m_airScoreRectangle = new Rectangle(1, 493, 25, 25);
        private readonly Rectangle m_groundScoreRectangle = new Rectangle(27, 493, 25, 25);

        private List<Particle> m_particles;
        private List<Particle> m_animatedParticles;
        private Texture2D m_spriteSheet;

        public ParticleManager()
        {
            m_particles = new List<Particle>();
            m_animatedParticles = new List<Particle>();
        }

        public void loadContent(Texture2D spriteSheet)
        {
            m_spriteSheet = spriteSheet;
        }
        
        public void Update(GameTime gameTime)
        {
            var removeMe = new List<Particle>();
            foreach (var particle in m_particles)
            {
                particle.lifetime -= gameTime.ElapsedGameTime;
                if (particle.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(particle);
                }
                //
                // Update its position
                particle.position += (particle.direction * particle.speed);
                
            }

            //
            // Remove any expired particles
            foreach (var particle in removeMe)
            {
                m_particles.Remove(particle);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in m_particles)
            {
                var frameIndex = (int)((1 - particle.lifetime.TotalMilliseconds / particle.originalLifetime) *
                                       particle.frameCount);
                var color = particle.fadeOut
                    ? particle.color * (Math.Min(1, (float) particle.lifetime.TotalSeconds))
                    : particle.color;

                var r = new Rectangle((int)particle.position.X, (int)particle.position.Y,
                    (int)(particle.size * (float)particle.sourceRectangle.Width / particle.sourceRectangle.Height), particle.size);

                spriteBatch.Draw(
                    particle.texture,
                    r,
                    new Rectangle(particle.sourceRectangle.X + particle.offset * frameIndex, particle.sourceRectangle.Y,
                        particle.sourceRectangle.Width, particle.sourceRectangle.Height),
                     color,
                    particle.rotation,
                    new Vector2(particle.sourceRectangle.Width / 2.0f,
                        particle.sourceRectangle.Height / 2.0f),
                    SpriteEffects.None,
                    0);
            }
        }

        public void AddSellEffect(Vector2 pos)
        {
            const int particleCount = 3;
            const int particleSize = 25;
            const float particleSpeed = 2.0f;
            const float spawnRadius = 50;

            var random = new Random();

            for (var i = 0; i < particleCount; i++)
            {
                var angle = (float) random.NextDouble() * MathHelper.TwoPi;
                var radius = (float) random.NextDouble() * spawnRadius;
                var offset = new Vector2(radius * MathF.Cos(angle), radius * MathF.Sin(angle));

                var particle = new Particle(pos + offset, -Vector2.UnitY, particleSpeed,
                    TimeSpan.FromSeconds(1), m_spriteSheet, m_coinRectangle, particleSize, true);
                m_particles.Add(particle);
            }
        }

        public void AddSmokeEffect(Vector2 pos, Color color)
        {
            var particleSize = 50;

            var particle = new Particle(pos, Vector2.Zero, 0,
                TimeSpan.FromSeconds(1), m_spriteSheet, m_smokeRectangle, particleSize, true);

            particle.color = color;

            m_particles.Add(particle);
        }

        public void AddMissileExplosion(Vector2 pos)
        {
            var particleSize = 64;

            var particle = new Particle(pos, Vector2.Zero, 0,
                TimeSpan.FromSeconds(.5), m_spriteSheet, m_explosionRectangle, particleSize, 33, 6, false);

            m_particles.Add(particle);
        }

        public void AddBombExplosion(Vector2 pos, int diameter)
        {
            const int particleCount = 25;
            const int particleSize = 32;

            var random = new Random();

            for (var i = 0; i < particleCount; i++)
            {
                var angle = (float)random.NextDouble() * MathHelper.TwoPi;
                var radius = (float)random.NextDouble() * diameter / 2;
                var offset = new Vector2(radius * MathF.Cos(angle), radius * MathF.Sin(angle));

                var particle = new Particle(pos + offset, Vector2.Zero, 0,
                    TimeSpan.FromSeconds(random.NextDouble() * .25 + .5), m_spriteSheet, m_explosionRectangle, particleSize, 33, 6, false);
                m_particles.Add(particle);
            }
        }

        public void AddCreepDeath(Vector2 pos)
        {
            var particleSize = 75;

            var particle = new Particle(pos, Vector2.Zero, 0,
                TimeSpan.FromSeconds(.25), m_spriteSheet, m_creepRectangle, particleSize, 51, 4, false);

            m_particles.Add(particle);
        }

        public void AddCreepScore(Vector2 pos, CreepType type)
        {
            var particleSize = 50;
            var particleSpeed = 2.0f;

            var rect = type == CreepType.Ground ? m_groundScoreRectangle : m_airScoreRectangle;

            var particle = new Particle(pos, -Vector2.UnitY, particleSpeed,
                TimeSpan.FromSeconds(1), m_spriteSheet, rect, particleSize, true);
            m_particles.Add(particle);
        }
    }
}
