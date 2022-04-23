using Microsoft.Xna.Framework;

namespace SpaceMarines_TD.Source.SpritesAnimation
{
    public class CollidableObject
    {
        protected readonly Vector2 m_size;
        protected Vector2 m_center;

        public CollidableObject(Vector2 size, Vector2 center, BoundingBox baseBounds)
        {
            BaseBounds = baseBounds;

            m_size = size;
            m_center = center;
        }

        public BoundingBox BaseBounds { get; set; }

        public BoundingBox Bounds
        {
            get
            {
                var shift = new Vector3(Center, 0);
                return new BoundingBox(BaseBounds.Min + shift, BaseBounds.Max + shift);
            }
        }

        public Vector2 Size
        {
            get { return m_size; }
        }

        public Vector2 Center
        {
            get { return m_center; }
            set { m_center = value; }
        }
    }

    public class AnimatedSpriteModel : CollidableObject
    {
        public AnimatedSpriteModel(Vector2 size, Vector2 center)
            : base(size, center, new BoundingBox(
                new Vector3(-size / 2, -1),
                new Vector3(size / 2, 1)))
        {
        }
    }
}
