using System.Collections.Generic;
using Model.DataTypes;

namespace Model.Internal
{
    internal interface IAStar
    {
        bool WayExists(CellPosition start, CellPosition end, Field field);
        public List<FieldCell> FindPath(FieldCell startPos, FieldCell targetPos);
    }
}