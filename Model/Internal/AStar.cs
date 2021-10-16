using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;

namespace Model.Internal
{
    internal class AStar
    {
        private List<FieldCell> _path;

        public List<FieldCell> FindPath(FieldCell startPos, FieldCell targetPos)
        {
            FieldCell startNode = startPos;
            FieldCell targetNode = targetPos;

            List<FieldCell> openSet = new List<FieldCell>();
            HashSet<FieldCell> closedSet = new HashSet<FieldCell>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                FieldCell node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < node.FCost || openSet[i].FCost == node.FCost && openSet[i].HCost < node.HCost) 
                    {
                        node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return _path;
                }

                foreach (FieldCell neighbour in node.ReachableNeighbours)
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.GCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return _path;
        }

        void RetracePath(FieldCell startNode, FieldCell endNode)
        {
            List<FieldCell> path = new List<FieldCell>();
            FieldCell currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            _path = path;
        }

        int GetDistance(FieldCell nodeA, FieldCell nodeB)
        {
            int dstX, dstY;
            dstX = Math.Abs(nodeA.Position.X - nodeB.Position.X);
            dstY = Math.Abs(nodeA.Position.Y - nodeB.Position.Y);
            return dstX + dstY;
        }

        public bool WayExists(CellPosition start, CellPosition end, Field field)
        {
            FieldCell startCell = field.CellByPosition(start);
            FieldCell endCell = field.CellByPosition(end);
            HashSet<FieldCell> openSet = new HashSet<FieldCell>();
            HashSet<FieldCell> closedSet = new HashSet<FieldCell>();
            if (start == end)
            {
                throw new Exception("They are the same nodes");
            }

            openSet.Add(startCell);
            return NeighbourLinkSearch(openSet, closedSet, endCell);
        }

        private bool NeighbourLinkSearch(HashSet<FieldCell> openSet, HashSet<FieldCell> closedSet, FieldCell end)
        {
            while (openSet.Count > 0)
            {
                foreach (FieldCell node in openSet.ToList())
                {
                    if (node == end)
                    {
                        return true;
                    }

                    openSet.Remove(node);
                    closedSet.Add(node);
                    foreach (FieldCell neighbour in node.ReachableNeighbours)
                    {
                        if (neighbour == end)
                        {
                            return true;
                        }

                        if (!closedSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
                return NeighbourLinkSearch(openSet, closedSet, end);
            }

            return false;
        }
    }
}