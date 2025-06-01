using System;

namespace DungeonChess.Core
{
    public class KnightMovementBehavior : IMovementBehavior
    {
        public bool IsMoveValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int rowDiff = Math.Abs(targetRow - piece.Row);
            int colDiff = Math.Abs(targetCol - piece.Col);
            bool validLShape = (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
            if(!validLShape)
                return false;
            // Knight's move does not have a blocking path, but the destination must be empty.
            if(board.GetPieceAt(targetRow, targetCol) != null)
                return false;
            return true;
        }
    }
}
