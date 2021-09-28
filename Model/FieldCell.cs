using System.Collections.Generic;

namespace Quoridor.Model
{
    class FieldCell
    {
        public CellPosition Position { get; }
        public List<FieldCell> NeighbourCells => _neighbourCells;
        private List<FieldCell> _neighbourCells = new List<FieldCell>();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
        }
        public void AddNeighbour(FieldCell neighbour)
        {
            _neighbourCells.Add(neighbour);
        }

        public void RemoveNeighbour(FieldCell neighbour)
        {
            _neighbourCells.Remove(neighbour);
        }
    }
}