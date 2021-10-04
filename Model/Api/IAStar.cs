namespace Model.Api
{
    public interface IAStar
    {
        bool WayExists(FieldCell start, FieldCell end);
    }
}