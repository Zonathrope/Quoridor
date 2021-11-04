using System.Collections.Generic;
using Model.DataTypes;

namespace Model.Internal
{
    public class FieldCell
    {
        public CellPosition Position { get; }
        public List<FieldCell> ReachableNeighbours { get; set; } = new ();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
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