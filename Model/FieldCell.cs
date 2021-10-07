using System.Collections.Generic;
using Model.Api;

namespace Model
{
    public class FieldCell
    {
        public CellPosition Position { get; }
        public int FCost { get; set; }
        public int HCost { get; set; }
        public int GCost { get; set; }
        public FieldCell Parent { get; set; }
        public List<FieldCell> NeighbourCells => _neighbourCells;
        private List<FieldCell> _neighbourCells = new List<FieldCell>();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
            GCost = FCost + HCost;
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