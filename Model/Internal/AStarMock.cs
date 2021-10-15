using Model.DataTypes;

namespace Model.Internal
{
    internal class AStarMock: IAStar
    {
        public bool WayExists(CellPosition start, CellPosition end, Field field)
        {
            return true;
        }
    }
}