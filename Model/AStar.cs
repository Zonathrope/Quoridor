﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model.DataTypes;

namespace Model
{
    public class AStar : IAStar
    {
        private List<CellPosition> _path;

        public List<CellPosition> FindPath(CellPosition startPos, CellPosition targetPos, Field field)
        {
            //_currentField = ConvertField(field.FieldMatrix);
            FieldCell startNode = field.FieldMatrix[startPos.Y, startPos.X];
            FieldCell targetNode = field.FieldMatrix[targetPos.Y, targetPos.X];

            List<FieldCell> openSet = new List<FieldCell>();
            HashSet<FieldCell> closedSet = new HashSet<FieldCell>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                FieldCell node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if ((openSet[i].FCost < node.FCost || openSet[i].FCost == node.FCost) && openSet[i].HCost < node.HCost)
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

        private AStarCell[,] ConvertField(FieldCell[,] fieldMatrix)
        {
            AStarCell[,] newMatrix = new AStarCell[9,9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    newMatrix[j, i] = new AStarCell(i,j);
                    
                }
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    foreach (FieldCell neighbour in fieldMatrix[j,i].ReachableNeighbours)
                    {
                        newMatrix[j, i].ReachableNeighbours.Add(newMatrix[neighbour.Position.Y,neighbour.Position.X]);
                    }
                }
            }
            
            return newMatrix;
        }

        void RetracePath(FieldCell startNode, FieldCell endNode)
        {
            List<CellPosition> path = new List<CellPosition>();
            FieldCell currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            _path = path;
        }

        int GetDistance(FieldCell nodeA, FieldCell nodeB)
        {
            return Math.Abs(nodeA.Position.X - nodeB.Position.X) + Math.Abs(nodeA.Position.Y - nodeB.Position.Y);
        }

        public bool WayExists(CellPosition start, CellPosition end, Field field)
        {
            //_currentField = ConvertField(field.FieldMatrix);
            FieldCell startCell = field.FieldMatrix[start.Y, start.X];
            FieldCell endCell = field.FieldMatrix[end.Y, end.X];
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