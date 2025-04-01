using System;

namespace DungeonChess.Core
{
    public class ArcherMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Calculate Chebyshev distance.
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);

            // Ensure the target is within the movement range.
            if (distance > piece.MovementRange)
                return false;
            
            // Get occupant at the target square.
            Piece occupant = board.GetPieceAt(targetRow, targetCol);
            
            if (occupant == null)
            {
                // Normal move: Allow omnidirectional movement if exactly 1 tile away.
                return (distance == 1);
            }
            else
            {
                // Attack move: Allow attack if exactly 2 cells away.
                return (distance == 2);
            }
        }
    }
}
