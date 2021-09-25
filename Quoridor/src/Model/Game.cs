using System.Collections.Generic;
using System.Numerics;

namespace Quoridor.Model
{
public class Game
{
    private IPlayer _player1;
    private IPlayer _player2;
    private Field _field;
    public Game(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;
        _field = new Field();
    }
}
class Field
{
    class FieldCell
    {
        public int X { set; get; }
        public int Y { set; get; }
        private List<FieldCell> _neighbourCells = new List<FieldCell>();

        public FieldCell(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void AddNeighbour(FieldCell neighbour)
        {
            _neighbourCells.Add(neighbour);
        }

        public void RemoveNeighbour(FieldCell neighbour)
        {
            _neighbourCells.Remove(neighbour);
        }
    }

    private const int FieldSize = 9;
    private static readonly Vector2 Player1DefaultPosition = new Vector2(4, 0);
    private static readonly Vector2 Player2DefaultPosition = new Vector2(4, 8);
    
    private FieldCell[,] _fieldMatrix = new FieldCell[FieldSize,FieldSize];
    private Vector2 _player1Position;
    private Vector2 _player2Position;
    public Field()
    {
        InitMatrix();
        _player1Position = Player1DefaultPosition;
        _player2Position = Player2DefaultPosition;
    }

    private void InitMatrix()
    {
        for (int y = 0; y < FieldSize; y++)
        {
            for (int x = 0; x < FieldSize; x++)
            {
                _fieldMatrix[y, x] = new FieldCell(x, y);
            }
        }
        for (int y = 0; y < FieldSize; y++)
        {
            for (int x = 0; x < FieldSize; x++)
            {
                FieldCell cell = CellByPosition(x, y);
                foreach (FieldCell neighbour in GetCellNeighbours(cell))
                {
                    cell.AddNeighbour(neighbour);
                }
            }
        }
    }
    // TODO think how to rename method to not conflict with fact what neighbours
    // can be removed during game flow
    private List<FieldCell> GetCellNeighbours(FieldCell cell)
    {
        List<FieldCell> neighbours = new List<FieldCell>();
        if (cell.Y != 0)
        {
            neighbours.Add(CellByPosition(cell.X, cell.Y - 1));
        }
        if (cell.X != (FieldSize - 1))
        {
            neighbours.Add(CellByPosition(cell.X + 1, cell.Y));

        }
        if (cell.Y != (FieldSize - 1))
        {
            neighbours.Add(CellByPosition(cell.X, cell.Y + 1));
        }
        if (cell.X != 0)
        {
            neighbours.Add(CellByPosition(cell.X - 1, cell.Y));
        }
        return neighbours;
    }

    private FieldCell CellByPosition(int x, int y)
    {
        return _fieldMatrix[y, x];
    }
}
}