﻿namespace Model.Internal
{
    internal class AStarMock: IAStar
    {
        public bool WayExists(FieldCell start, FieldCell end)
        {
            return true;
        }
    }
}