namespace Model.Internal
{
    interface IAStar
    {
        //TODO maybe replace FieldCell with CellPosition
        bool WayExists(FieldCell start, FieldCell end);
    }
}