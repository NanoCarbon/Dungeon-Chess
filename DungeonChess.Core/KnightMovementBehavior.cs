using System;

namespace DungeonChess.Core
{
    public class KnightMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int rowDiff = Math.Abs(targetRow - piece.Row);
            int colDiff = Math.Abs(targetCol - piece.Col);
            // A knight must move in an "L" shape: either 2 rows and 1 column, or 1 row and 2 columns.
            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }
    }
}
