namespace Model.Internal
{
    class AStarMock: IAStar
    {
        public bool WayExists(FieldCell start, FieldCell end)
        {
            return true;
        }
    }
}