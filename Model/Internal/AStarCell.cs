using System.Collections.Generic;
using Model.DataTypes;

namespace Model.Internal
{
    public class AStarCell
    {
        public CellPosition Position { get; set; }
        public int HCost { get; set; }
        public int GCost { get; set; }
        public AStarCell Parent { get; set; }
        public readonly List<AStarCell> ReachableNeighbours = new ();

        public AStarCell(CellPosition position)
        {
            Position = position;
        }

        public int FCost()
        {
            return HCost + GCost;
        }
    }
}