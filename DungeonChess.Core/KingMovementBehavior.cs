using System;

namespace DungeonChess.Core
{
    public class KingMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            // King moves one square in any direction.
            if(distance != 1 || distance > piece.MovementRange)
                return false;
            // The target cell must be empty.
            if(board.GetPieceAt(targetRow, targetCol) != null)
                return false;
            return true;
        }
    }

}
