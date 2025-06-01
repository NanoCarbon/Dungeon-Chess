using System;

namespace DungeonChess.Core
{
    public class KnightAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int rowDiff = Math.Abs(targetRow - piece.Row);
            int colDiff = Math.Abs(targetCol - piece.Col);
            bool validLShape = (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
            if (!validLShape)
                return false;
            // Check the target cell.
            Piece occupant = board.GetPieceAt(targetRow, targetCol);
            if (occupant == null)
                return false;
            if (occupant.player == piece.player)
                return false;
            return true;
        }
    }

}
