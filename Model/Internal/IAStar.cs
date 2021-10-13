namespace Model.Internal
{
    interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}