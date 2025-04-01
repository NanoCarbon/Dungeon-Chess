using System;

namespace DungeonChess.Core
{
    public class RookMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Check range.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if (distance > piece.MovementRange)
                return false;

            // Rook must move horizontally or vertically.
            if (piece.Row != targetRow && piece.Col != targetCol)
                return false;

            int rowStep = targetRow == piece.Row ? 0 : (targetRow > piece.Row ? 1 : -1);
            int colStep = targetCol == piece.Col ? 0 : (targetCol > piece.Col ? 1 : -1);
            int currentRow = piece.Row + rowStep;
            int currentCol = piece.Col + colStep;
            while (currentRow != targetRow || currentCol != targetCol)
            {
                if (board.GetPieceAt(currentRow, currentCol) != null)
                    return false;
                currentRow += rowStep;
                currentCol += colStep;
            }
            return true;
        }
    }
}
