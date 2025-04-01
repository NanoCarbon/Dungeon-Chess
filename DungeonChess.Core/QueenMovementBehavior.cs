using System;

namespace DungeonChess.Core
{
    public class QueenMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Check range.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if (distance > piece.MovementRange)
                return false;

            // Queen moves horizontally, vertically, or diagonally.
            bool isHorizontal = (piece.Row == targetRow) && (piece.Col != targetCol);
            bool isVertical = (piece.Col == targetCol) && (piece.Row != targetRow);
            bool isDiagonal = (dx == dy);
            if (!(isHorizontal || isVertical || isDiagonal))
                return false;

            int rowStep = 0, colStep = 0;
            if (isHorizontal)
            {
                colStep = targetCol > piece.Col ? 1 : -1;
            }
            else if (isVertical)
            {
                rowStep = targetRow > piece.Row ? 1 : -1;
            }
            else if (isDiagonal)
            {
                rowStep = targetRow > piece.Row ? 1 : -1;
                colStep = targetCol > piece.Col ? 1 : -1;
            }

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
