using System.Collections.Generic;
using Model.DataTypes;

namespace Model.Internal
{
    public class FieldCell
    {
        public CellPosition Position { get; }
        public int FCost { get; set; }
        public int HCost { get; set; }
        public int GCost { get; set; }
        public FieldCell Parent { get; set; }
        public List<FieldCell> ReachableNeighbours { get; } = new ();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
            GCost = FCost + HCost;
        }

        public void AddReachableNeighbour(FieldCell neighbour)
        {
            ReachableNeighbours.Add(neighbour);
        }

        public void RemoveReachableNeighbour(FieldCell neighbour)
        {
            ReachableNeighbours.Remove(neighbour);
        }
    }
}