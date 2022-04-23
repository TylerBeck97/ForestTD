using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Objects
{
    public class Projectile : AnimatedSpriteModel
    {
        public ProjectileType Type { get; }

        public int Damage { get; }

        public int Diameter { get; }

        public double Heading { get; set; }

        private Creep m_target;

        private double m_moveRate;

        private Vector2 m_lastDirectionMoved;

        public Projectile(Vector2 size, Vector2 center, Creep target, ProjectileType projectileType, double heading, int damage) : base(size, center)
        {
            m_target = target;
            Type = projectileType;
            Heading = heading;
            Damage = damage;

            switch (Type)
            {
                case ProjectileType.Missile:
                    m_moveRate = 300 / 1000.0;
                    break;
                case ProjectileType.Bullet:
                    m_moveRate = 1200 / 1000.0;
                    break;
                case ProjectileType.Bomb:
                    m_moveRate = 200 / 1000.0;
                    Diameter = 400;
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (m_target != null)
            {
                m_lastDirectionMoved = Vector2.Normalize(new Vector2(m_target.Center.X - Center.X, m_target.Center.Y - Center.Y))
                                * (float) (m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);

                if (m_target.Bounds.Intersects(Bounds))
                {
                    m_target = null;
                }
            }
            m_center += m_lastDirectionMoved;

            if (Type == ProjectileType.Bullet || Type == ProjectileType.Missile)
            {
                Heading = Math.Atan2(m_lastDirectionMoved.Y, m_lastDirectionMoved.X);
            }
            else
            {
                Heading += MathHelper.PiOver4;
            }
        }
    }

    public enum ProjectileType
    {
        Missile,
        Bullet,
        Bomb
    }
}
