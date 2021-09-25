using System.Collections.Generic;
using System.Numerics;

namespace Quoridor.Model
{
public class Game: IGame
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

    public void StartNewGame()
    {
        throw new System.NotImplementedException();
    }

    public void MovePlayer(PlayerNumber playerNumber, int x, int y)
    {
        throw new System.NotImplementedException();
    }

    public void PlaceWall(PlayerNumber playerPlacing, WallPosition position)
    {
        throw new System.NotImplementedException();
    }
}
class Field
{
    class FieldCell
    {
        public CellPosition Position { get; }
        private List<FieldCell> _neighbourCells = new List<FieldCell>();

        public FieldCell(int x, int y)
        {
            Position = new CellPosition(x, y);
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
    private static readonly CellPosition Player1DefaultPosition = new CellPosition(4, 0);
    private static readonly CellPosition Player2DefaultPosition = new CellPosition(4, 8);
    
    private FieldCell[,] _fieldMatrix = new FieldCell[FieldSize,FieldSize];
    public CellPosition Player1Position { set; get; }
    public CellPosition Player2Position { set; get; }
    public Field()
    {
        InitMatrix();
        Player1Position = Player1DefaultPosition;
        Player2Position = Player2DefaultPosition;
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
                FieldCell cell = CellByPosition(new CellPosition(x, y));
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
        CellPosition cellPosition = cell.Position;
        if (cellPosition.Y != 0)
        {
            //TODO think if it is okay what cell positon can accept incorrect values
            neighbours.Add(CellByPosition(cellPosition + new CellPosition(0, -1)));
        }
        if (cellPosition.X != (FieldSize - 1))
        {
            neighbours.Add(CellByPosition(cellPosition + new CellPosition(1, 0)));
        }
        if (cellPosition.Y != (FieldSize - 1))
        {
            neighbours.Add(CellByPosition(cellPosition + new CellPosition(0, 1)));
        }
        if (cellPosition.X != 0)
        {
            neighbours.Add(CellByPosition(cellPosition + new CellPosition(-1, 0)));
        }
        return neighbours;
    }

    private FieldCell CellByPosition(CellPosition cellPosition)
    {
        return _fieldMatrix[cellPosition.Y, cellPosition.X];
    }
}

public class CellPosition
{
    public int X { set; get; }
    public int Y { set; get; }

    public CellPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static CellPosition operator +(CellPosition position1, CellPosition position2)
    {
        return new CellPosition(position1.X + position2.X, position2.Y + position2.Y);
    }
}
}