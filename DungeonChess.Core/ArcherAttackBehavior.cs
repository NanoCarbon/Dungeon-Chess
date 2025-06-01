using System;

namespace DungeonChess.Core
{
    public class ArcherAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if (distance != 2)
                return false;
            // Check target cell.
            Piece occupant = board.GetPieceAt(targetRow, targetCol);
            if (occupant == null)
                return false;
            if (occupant.player == piece.player)
                return false;
            return true;
        }
    }
}
