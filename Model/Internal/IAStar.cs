namespace Model.Internal
{
    internal interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}