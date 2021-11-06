using Model;
using Model.DataTypes;

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