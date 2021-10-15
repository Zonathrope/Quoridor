namespace Model.Api
{
    interface IAStar
    {
        bool WayExists(CellPosition start, CellPosition end, Field field);
    }
}