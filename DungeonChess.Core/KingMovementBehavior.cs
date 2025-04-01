using System;

namespace DungeonChess.Core
{
    public class KingMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // King can only move one square in any direction.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            return (distance <= piece.MovementRange && distance == 1);
        }
    }
}
