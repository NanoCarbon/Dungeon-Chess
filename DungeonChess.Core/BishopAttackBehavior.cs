using System;

namespace DungeonChess.Core
{
    public class BishopAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if(distance > piece.AttackRange)
                return false;
            // Must move diagonally.
            if(dx != dy)
                return false;

            int rowStep = (targetRow - piece.Row) / dx;
            int colStep = (targetCol - piece.Col) / dy;
            int currentRow = piece.Row + rowStep;
            int currentCol = piece.Col + colStep;
            // Check the path (excluding the target cell).
            while(currentRow != targetRow || currentCol != targetCol)
            {
                if(board.GetPieceAt(currentRow, currentCol) != null)
                    return false;
                currentRow += rowStep;
                currentCol += colStep;
            }
            return true;
        }
    }
}
