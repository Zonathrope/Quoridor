namespace Quoridor.View.Interfaces
{
    public interface IDrawer
    {
        public void ShowStartInfo();
        public void DrawBoard(string[][] array);
        public void WriteMoves(string[] possibleMoves);
        public void EndGame();
        public void ClearConsole();
    }
}