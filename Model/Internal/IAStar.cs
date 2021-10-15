using Model.DataTypes;

namespace Model.Internal
{
    internal interface IAStar
    {
        bool WayExists(CellPosition start, CellPosition end, Field field);
    }
}