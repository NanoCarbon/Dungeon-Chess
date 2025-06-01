using System;

namespace DungeonChess.Core
{
    public class ArcherMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);

            // Ensure the target is within the movement range.
            if (distance > piece.MovementRange)
                return false;
                
            // For movement, the target cell must be empty.
            if (board.GetPieceAt(targetRow, targetCol) != null)
                return false;
                
            // Allow movement only if the distance is exactly 1.
            return distance == 1;
        }
    }
}
