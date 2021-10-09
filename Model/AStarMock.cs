using Model.Api;

namespace Model
{
    class AStarMock: IAStar
    {
        public bool WayExists(FieldCell start, FieldCell end)
        {
            return true;
        }
    }
}