using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceMarines_TD.Source.Graph
{
    class Node
    {
        public Node ParentNode { get; set; }
        public float DistanceFromStart { get; set; }
        public float DistanceToGoal { get; set; }
        public bool BeenVisited { get; set; }
        public Vector2 Coordinates { get; }
        public List<Node> NeighboringNodes { get; set; }

        public Node(Vector2 coordinates)
        {
            Coordinates = coordinates;
            NeighboringNodes = new List<Node>();
            DistanceFromStart = float.PositiveInfinity;
            DistanceToGoal = float.PositiveInfinity;
            BeenVisited = false;
        }

        protected bool Equals(Node other)
        {
            return Coordinates.Equals(other.Coordinates);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return Coordinates.GetHashCode();
        }

        public override string ToString() => $"{Coordinates} ({DistanceFromStart}) ({DistanceToGoal})";
    }
}
