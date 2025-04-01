namespace DungeonChess.Core
{
    public class PawnMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            // Calculate the Chebyshev distance.
            int dx = System.Math.Abs(targetRow - piece.Row);
            int dy = System.Math.Abs(targetCol - piece.Col);
            int distance = System.Math.Max(dx, dy);

            // Ensure the target is within the piece's movement range.
            if (distance > piece.MovementRange)
                return false;

            // For a normal move, the target tile must be empty.
            if (board.GetPieceAt(targetRow, targetCol) != null)
                return false;

            // If all conditions are met, the move is valid.
            return true;
        }
    }
}
