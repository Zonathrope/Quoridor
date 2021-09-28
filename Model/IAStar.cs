namespace Quoridor.Model
{
    interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}