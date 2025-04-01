namespace DungeonChess.Core
{
    public interface IAttackBehavior
    {
        /// <summary>
        /// Returns true if the attack move from the piece's current position to the target position is valid.
        /// </summary>
        bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board);
    }
}
