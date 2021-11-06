using System.Collections.Generic;
using Model.DataTypes;

namespace Model.Internal
{
    public class AStarCell
    {
        public CellPosition Position { get; }
        public int HCost { get; set; }
        public int GCost { get; set; }
        
        public int FCost => HCost + GCost;
        public AStarCell Parent { get; set; }
        public readonly List<AStarCell> ReachableNeighbours = new ();

        public AStarCell(int x, int y)
        {
            Position = new CellPosition(x, y);
        }
    }
}