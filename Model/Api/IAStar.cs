using Quoridor.Model;

namespace Model.Api
{
    interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}