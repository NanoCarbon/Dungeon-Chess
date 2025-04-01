using System;

namespace DungeonChess.Core
{
    public class PawnAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Calculate the differences.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);

            // The move must be diagonal, so dx should equal dy, and not zero.
            if (dx != dy || dx == 0)
                return false;

            // Ensure the diagonal distance is within the pawn's attack range.
            if (dx > piece.AttackRange)
                return false;

            // The target tile must contain an enemy piece.
            Piece occupant = board.GetPieceAt(targetRow, targetCol);
            if (occupant == null)
                return false;
            if (occupant.GetPlayer() == piece.GetPlayer())
                return false;

            return true;
        }
    }
}
