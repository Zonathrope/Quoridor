namespace Model.Api
{
    interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}