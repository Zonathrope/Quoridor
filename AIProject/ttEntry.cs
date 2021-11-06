using Model.DataTypes;
using Model.Internal;

namespace AIProject
{
    public class ttEntry
    {
        public Field Position;
        public Move Value;
        public int Depth;
        public EntryFlag Flag;

        public ttEntry(Field position)
        {
            this.Position = position;
        }
    }
    
}