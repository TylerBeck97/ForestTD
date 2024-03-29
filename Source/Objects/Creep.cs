using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SpaceMarines_TD.Source.Graph;
using SpaceMarines_TD.Source.SpritesAnimation;

namespace SpaceMarines_TD.Source.Objects
{
    public class Creep : AnimatedSpriteModel
    {
        public readonly double m_moveRate;

        public int Health { get; set; }

        private Vector2 m_direction;
        private Vector2 m_wayPointDirection;
        private Vector2 m_wayPoint;
        private Stack<Vector2> m_path;

        private bool m_reachedGoal = false;

        public double Heading { get; set; }
        public int Money { get; set; }
        public int Score { get; set; }
        public CreepType Type { get; }

        public bool IsUpAndDown { get; }
        public bool IsMovingPositive { get; }
        public bool HasEnteredField { get; set; }

        public Creep(Vector2 size, Vector2 center, Vector2 direction, CreepType type, bool isUpAndDown, bool isMovingPositive, int level) : base(size, center)
        {
            m_direction = direction;
            m_moveRate = type == CreepType.Air ? Math.Min((90 + 5 * level), 300) / 1000.0 * (level / 4.0f) : (90 + 10 * level) / 1000.0;

            Health = 100 * level / 2;
            Money = GetMoneyReward(type);
            Score = GetScoreReward(type);
            Type = type;
            IsUpAndDown = isUpAndDown;
            IsMovingPositive = isMovingPositive;
            HasEnteredField = false;
        }

        public void Update(GameTime gameTime)
        {
            var moveScalar = (float) (m_moveRate * gameTime.ElapsedGameTime.TotalMilliseconds);
            if (Type == CreepType.Ground)
            {
                if (!m_reachedGoal)
                {
                    m_wayPointDirection =
                        Vector2.Normalize(new Vector2(m_wayPoint.X - Center.X, m_wayPoint.Y - Center.Y));
                    m_center += m_wayPointDirection * moveScalar;
                    Heading = Math.Atan2((m_wayPointDirection * moveScalar).Y, (m_wayPointDirection * moveScalar).X);

                    if (m_center == m_wayPoint)
                    {
                        m_wayPoint = m_path.Pop();
                    }
                    return;
                }
            }
            m_center += m_direction * moveScalar;
            Heading = Math.Atan2((m_direction * moveScalar).Y, (m_direction * moveScalar).X);
        }

        public void SetPath(Stack<Vector2> stack)
        {
            m_path = stack;
            if (m_path.Count == 0)
            {
                m_reachedGoal = true;
                return;
            }
            else
            {
                m_reachedGoal = false;
            }
            m_wayPoint = m_path.Pop();
        }

        private static int GetScoreReward(CreepType type)
        {
            switch (type)
            {
                case CreepType.Air: return 2;
                case CreepType.Ground: return 1;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static int GetMoneyReward(CreepType type)
        {
            switch (type)
            {
                case CreepType.Air: return 5;
                case CreepType.Ground: return 2;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

    }
    public enum CreepType
    {
        Air,
        Ground
    }
}
