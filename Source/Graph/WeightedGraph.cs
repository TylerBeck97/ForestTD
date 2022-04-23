using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Serialization;
using SpaceMarines_TD.Source.Objects;
using TestGame.Services;

namespace SpaceMarines_TD.Source.Graph
{
    class WeightedGraph
    {
        public Node[,] GraphNodes { get; }
        
        private int m_gridSize;
        //private readonly DebugService _debugService;
        private int m_startX;
        private int m_startY;

        public WeightedGraph(List<Tower> towers, int startX, int startY, int rowCount, int colCount, int gridSize)
        {
            GraphNodes = new Node[rowCount, colCount];

            m_gridSize = gridSize;
            //_debugService = debugService;
            m_startX = startX;
            m_startY = startY;

            InitializeGrid(towers, rowCount, colCount);

            LinkGrid(rowCount, colCount);
        }

        // Garbage
        private static int _temp = 0;

        public Stack<Vector2> FindPath(float startX, float startY, float endX, float endY)
        {
            // DEBUG
            ResetEdges();

            var (sX, sY) = ConvertToGridCoord(startX, startY);
            var (eX, eY) = ConvertToGridCoord(endX, endY);

            if (sY < 0 || sY >= GraphNodes.GetLength(0) ||
                eY < 0 || eY >= GraphNodes.GetLength(0))
            {
                return new Stack<Vector2>();
            }

            var startNode = GraphNodes[sY, sX];
            var endNode = GraphNodes[eY, eX];

            // DEBUG
            if (startNode == null) return new Stack<Vector2>();

            var sortedSet = new SortedSet<Node>(new ByDistanceToGoal());

            startNode.DistanceToGoal = TaxiCabDistance(startNode.Coordinates, endNode.Coordinates);
            startNode.DistanceFromStart = 0;

            sortedSet.Add(startNode);
            _temp = (_temp + 1) % 3;

            while (sortedSet.Count > 0)
            {
                var currNode = sortedSet.First();

                Color c;
                switch (_temp)
                {
                    case 0: c = Color.Blue; break;
                    case 1: c = Color.Red; break;
                    case 2: c = Color.Green; break;
                    default: c = Color.Yellow; break;
                }

                //_debugService.MarkPoint(currNode.Coordinates, c);

                if (currNode.Equals(endNode))
                {
                    var stack = new Stack<Vector2>();
                    while (!currNode.Equals(startNode))
                    {
                        stack.Push(currNode.Coordinates);
                        currNode = currNode.ParentNode;
                    }

                    return stack;
                }

                // Debug.WriteLine(currNode);
                sortedSet.Remove(currNode);
                foreach (var neighboringNode in currNode.NeighboringNodes)
                {
                    var tempDistance = currNode.DistanceFromStart + TaxiCabDistance(neighboringNode.Coordinates, currNode.Coordinates);
                    if (tempDistance < neighboringNode.DistanceFromStart)
                    {
                        sortedSet.Remove(neighboringNode);

                        neighboringNode.ParentNode = currNode;
                        neighboringNode.DistanceFromStart = tempDistance;
                        neighboringNode.DistanceToGoal = tempDistance + TaxiCabDistance(neighboringNode.Coordinates, endNode.Coordinates);
                        
                        sortedSet.Add(neighboringNode);
                    }
                }
            }
            
            return new Stack<Vector2>();
        }

        public void ResetEdges()
        {
            foreach (var node in GraphNodes)
            {
                if (node != null)
                {
                    node.DistanceFromStart = float.PositiveInfinity;
                    node.DistanceToGoal = float.PositiveInfinity;
                }
            }
        }

        private float TaxiCabDistance(Vector2 v1, Vector2 v2)
        {
            return Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);
        }

        private void GetNodeRight(int i, int j, Node currNode)
        {
            if (GraphNodes[i, j + 1] != null) currNode.NeighboringNodes.Add(GraphNodes[i, j + 1]); // Right
        }

        private void GetNodeLeft(int i, int j, Node currNode)
        {
            if (GraphNodes[i, j - 1] != null) currNode.NeighboringNodes.Add(GraphNodes[i, j - 1]); // Left
        }

        private void GetNodeBelow(int i, int j, Node currNode)
        {
            if (GraphNodes[i + 1, j] != null) currNode.NeighboringNodes.Add(GraphNodes[i + 1, j]); // Up
        }

        private void GetNodeAbove(int i, int j, Node currNode)
        {
            if (GraphNodes[i - 1, j] != null) currNode.NeighboringNodes.Add(GraphNodes[i - 1, j]); // Down
        }

        private void InitializeGrid(List<Tower> towers, int rowCount, int colCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (ValidPoint(towers, new Vector3(m_startX + m_gridSize * j, m_startY + m_gridSize * i, 0)))
                    {
                        GraphNodes[i, j] = new Node(new Vector2(m_startX + m_gridSize * j, m_startY + m_gridSize * i));
                    }
                    else
                    {
                        GraphNodes[i, j] = null;
                    }
                }
            }
        }

        private bool ValidPoint(List<Tower> towers, Vector3 point)
        {
            foreach (var tower in towers)
            {
                if (tower.Bounds.Contains(point) == ContainmentType.Contains)
                {
                    return false;
                }
            }

            return true;
        }

        private void LinkGrid(int rowCount, int colCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    var currNode = GraphNodes[i, j];
                    if (currNode == null) continue;

                    if (j != 0) GetNodeLeft(i, j, currNode);
                    if (j != colCount - 1) GetNodeRight(i, j, currNode);

                    if (i != 0) GetNodeAbove(i, j, currNode);
                    if (i != rowCount - 1) GetNodeBelow(i, j, currNode);
                }
            }
        }

        private (int, int) ConvertToGridCoord(float x, float y)
        {
            var convertedX = MathF.Round(x / (m_gridSize / 2)) * (m_gridSize / 2);
            var convertedY = MathF.Round(y / (m_gridSize / 2)) * (m_gridSize / 2);

            return ((int)((convertedX - m_startX) / m_gridSize), (int)((convertedY - m_startY) / m_gridSize));
        }

    }

    class ByDistanceToGoal : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            // Debug.WriteLine($"Compare {x} {y}.");

            if (x.Equals(y)) return 0;

            if (x.DistanceToGoal < y.DistanceToGoal)
            {
                return -1;
            }
            else if (x.DistanceToGoal == y.DistanceToGoal)
            {
                if (x.Coordinates.X != y.Coordinates.X)
                    return x.Coordinates.X.CompareTo(y.Coordinates.X);
                else
                    return x.Coordinates.Y.CompareTo(y.Coordinates.Y);
            }
            
            return 1;
        }
    }
}
