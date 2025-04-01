namespace DungeonChess.Core
{
    public interface IMovementBehavior
    {
        /// <summary>
        /// Returns true if the move from the piece’s current position to the target position is valid.
        /// </summary>
        bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board);
    }
}
