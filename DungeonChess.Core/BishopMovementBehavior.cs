using System;

namespace DungeonChess.Core
{
    public class BishopMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if (distance > piece.MovementRange)
                return false;
            // Bishop must move diagonally.
            if (dx != dy)
                return false;

            int rowStep = (targetRow - piece.Row) / dx;
            int colStep = (targetCol - piece.Col) / dy;
            int currentRow = piece.Row + rowStep;
            int currentCol = piece.Col + colStep;
            while (currentRow != targetRow || currentCol != targetCol)
            {
                if (board.GetPieceAt(currentRow, currentCol) != null)
                    return false;
                currentRow += rowStep;
                currentCol += colStep;
            }
            // Check that the target cell is empty.
            if (board.GetPieceAt(targetRow, targetCol) != null)
                return false;
            return true;
        }
    }

}
