using System.Collections.Generic;
using Model.DataTypes;

namespace Model
{
    public class FieldCell
    {
        public CellPosition Position { get; }
        public List<FieldCell> ReachableNeighbours => _reachableNeighbours;
        private List<FieldCell> _reachableNeighbours = new List<FieldCell>();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
        }

        public void AddReachableNeighbour(FieldCell neighbour)
        {
            _reachableNeighbours.Add(neighbour);
        }

        public void RemoveReachableNeighbour(FieldCell neighbour)
        {
            _reachableNeighbours.Remove(neighbour);
        }
    }
}