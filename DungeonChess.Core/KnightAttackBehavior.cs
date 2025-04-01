using System;

namespace DungeonChess.Core
{
    public class KnightAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int rowDiff = Math.Abs(targetRow - piece.Row);
            int colDiff = Math.Abs(targetCol - piece.Col);
            // For a knight, attack must be in an L-shape (2 and 1).
            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }
    }
}
