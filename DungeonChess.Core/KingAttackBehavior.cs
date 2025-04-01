using System;

namespace DungeonChess.Core
{
    public class KingAttackBehavior : IAttackBehavior
    {
        public bool IsAttackValid(Piece piece, int targetRow, int targetCol, Board board)
        {
            int dx = Math.Abs(targetRow - piece.Row);
            int dy = Math.Abs(targetCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if(distance > piece.AttackRange)
                return false;
            // King can only attack one square away.
            return (distance == 1);
        }
    }
}
