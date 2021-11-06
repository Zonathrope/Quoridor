using Model.DataTypes;

namespace Model
{
    internal interface IAStar
    {
        bool WayExists(CellPosition start, CellPosition end, Field field);
    }
}