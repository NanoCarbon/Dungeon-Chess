using System;

namespace DungeonChess.Core
{
    public class ArcherAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Allow attack if Chebyshev distance is exactly 2.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            return (distance == 2);
        }
    }
}
