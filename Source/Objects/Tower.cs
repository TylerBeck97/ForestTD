using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Objects
{
    public class Tower : AnimatedSpriteModel
    {
        private const double HeadingTolerance = Math.PI / 20;

        public readonly TowerType m_type;
        public readonly ProjectileType m_projectileType;

        private readonly double m_turnAcceleration;
        private readonly double m_maxTurnSpeed;

        private double m_targetHeading;
        private double m_turnSpeed;

        public int BaseCost { get; }

        /// <summary>
        /// Cooldown between shots in burst.
        /// </summary>
        public double BurstFireRate { get; private set; }

        /// <summary>
        /// Current shot in burst.
        /// </summary>
        public int BurstIndex { get; set; }

        /// <summary>
        /// Number of shots per burst.
        /// </summary>
        public int BurstLength { get; set; }

        public int Damage { get; }

        /// <summary>
        /// Cooldown between last shots in burst.
        /// </summary>
        public double FireRate { get; private set; }

        public double Heading { get; private set; }

        /// <summary>
        /// Game time of last burst start.
        /// </summary>
        public TimeSpan LastShot { get; private set; }

        public int Range { get; private set; }

        public int TowerLevel { get; private set; }

        public int UpgradeCost { get; private set; }

        public Tower(Vector2 size, Vector2 center, TowerType type, int baseCost) : base(size, center)
        {
            m_type = type;
            BaseCost = baseCost;
            BurstLength = 1;
            TowerLevel = 1;

            switch (m_type)
            {
                case TowerType.Air:
                    Range = 750;
                    Damage = 50;
                    FireRate = 500;
                    BurstLength = 2;
                    BurstFireRate = 350;
                    m_turnAcceleration = 100;
                    m_maxTurnSpeed = 2 * Math.PI;
                    m_projectileType = ProjectileType.Missile;
                    break;
                case TowerType.Bullet:
                    Range = 400;
                    Damage = 15;
                    FireRate = 150;
                    m_turnAcceleration = 100;
                    m_maxTurnSpeed = 2 * Math.PI; ;
                    m_projectileType = ProjectileType.Bullet;
                    break;
                case TowerType.Bomb:
                    Range = 600;
                    Damage = 40;
                    FireRate = 500;
                    m_turnAcceleration = 100;
                    m_maxTurnSpeed = 2 * Math.PI; ;
                    m_projectileType = ProjectileType.Bomb;
                    break;
                case TowerType.Mixed:
                    Range = 500;
                    Damage = 20;
                    FireRate = 200;
                    m_turnAcceleration = 100;
                    m_maxTurnSpeed = 2 * Math.PI; ;
                    m_projectileType = ProjectileType.Bullet;
                    break;
            }
        }

        public Projectile Update(GameTime gameTime, List<Creep> creeps, int projectileSize)
        {
            var filteredCreeps = creeps;
            if (m_type == TowerType.Air)
            {
                filteredCreeps = creeps.Where(creep => creep.Type == CreepType.Air).ToList();
            }

            if (m_type == TowerType.Bullet || m_type == TowerType.Bomb)
            {
                filteredCreeps = creeps.Where(creep => creep.Type == CreepType.Ground).ToList();
            }

            if (filteredCreeps.Count > 0)
            {
                var closestCreep = filteredCreeps[0];
                var smallestDistance = Vector2.Distance(m_center, closestCreep.Center);

                for (var i = 1; i < filteredCreeps.Count; i++)
                {
                    var distance = Math.Abs(Vector2.Distance(m_center, filteredCreeps[i].Center));
                    if ( distance < smallestDistance)
                    {
                        closestCreep = filteredCreeps[i];
                        smallestDistance = distance;
                    }
                }

                if (smallestDistance <= Range / 2.0)
                {
                    m_targetHeading =
                        Math.Atan2(closestCreep.Center.X - m_center.X, closestCreep.Center.Y - m_center.Y)
                        - Math.PI / 2;
                    m_targetHeading = -m_targetHeading;
                    while (m_targetHeading > Math.PI * 2)
                    {
                        m_targetHeading -= Math.PI * 2;
                    }

                    while (m_targetHeading < 0)
                    {
                        m_targetHeading += Math.PI * 2;
                    }

                    // Reset the burst if the cooldown has passed, regardless of whether we are aiming at a target.
                    if ((gameTime.TotalGameTime - LastShot).TotalMilliseconds > FireRate &&
                        BurstIndex == BurstLength)
                    {
                        LastShot = gameTime.TotalGameTime;
                        BurstIndex = 0;
                    }

                    if (Math.Abs(Heading - m_targetHeading) > HeadingTolerance) // Need to turn towards the target.
                    {
                        var headingDiff = m_targetHeading - Heading;
                        if (Math.Abs(Math.PI * 2 - headingDiff) < Math.Abs(headingDiff))
                        {
                            headingDiff -= Math.PI;
                        }

                        m_turnSpeed = Math.Min(m_maxTurnSpeed,
                            m_turnSpeed + m_turnAcceleration * gameTime.ElapsedGameTime.TotalSeconds);
                        
                        var turnAmount = m_turnSpeed * gameTime.ElapsedGameTime.TotalSeconds;
                        if (Math.Abs(Heading - m_targetHeading) < turnAmount)
                        {
                            Heading = m_targetHeading;
                        }
                        else
                        {
                            Heading += headingDiff < 0 ? -turnAmount : turnAmount;
                            while (Heading > Math.PI * 2)
                            {
                                Heading -= Math.PI * 2;
                            }

                            while (Heading < 0)
                            {
                                Heading += Math.PI * 2;
                            }
                        }
                    }
                    else // Aiming at target - we can fire.
                    {
                        m_turnSpeed = Math.Max(0,
                            m_turnSpeed - m_turnAcceleration * gameTime.TotalGameTime.TotalSeconds);

                        if (BurstIndex == 0 || (BurstIndex < BurstLength) &&
                            (gameTime.TotalGameTime - LastShot).TotalMilliseconds > BurstFireRate)
                        {
                            if (BurstIndex < BurstLength)
                            {
                                BurstIndex++;
                            }

                            LastShot = gameTime.TotalGameTime;

                            if (BurstIndex <= BurstLength)
                            {
                                return CreateProjectile(projectileSize, closestCreep);
                            }
                        }
                    }
                }
                else
                {
                    m_targetHeading = Heading;
                }

            }
            return null;
        }

        private Projectile CreateProjectile(int projectileSize, Creep closestCreep)
        {
            var offset = Vector2.Zero;
            if (m_type == TowerType.Air)
            {
                offset = new Vector2(0, 29) * (BurstIndex * 2 - 3);
            }

            // Apply heading.
            var rotMatrix = Matrix.CreateRotationZ((float) -Heading - MathHelper.PiOver2);
            offset = Vector2.TransformNormal(offset, rotMatrix);

            return new Projectile(new Vector2(projectileSize, projectileSize), m_center + offset,
                closestCreep,
                m_projectileType,
                Heading, Damage);
        }

        public bool CanUpgrade()
        {
            return TowerLevel < 4;
        }

        public void Upgrade()
        {
            if (!CanUpgrade()) return;

            switch (m_type)
            {
                case TowerType.Air:
                    Range += 50;
                    BurstFireRate -= 25;
                    FireRate -= 50;
                    UpgradeCost += BaseCost / 2;
                    break;
            }

            TowerLevel++;
        }
    }
    public enum TowerType
    {
        Air,
        Bullet,
        Bomb,
        Mixed
    }
}
