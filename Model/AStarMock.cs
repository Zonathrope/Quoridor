using Model.Api;

namespace Model
{
    public class AStarMock: IAStar
    {
        public bool WayExists(FieldCell start, FieldCell end)
        {
            return true;
        }
    }
}