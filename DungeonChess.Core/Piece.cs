namespace DungeonChess.Core;
public enum PieceType { Pawn, Bishop, Knight, Rook, Queen, Archer, Chaplain, King }

public class Piece
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int InitialRow { get; set; }  // For movement logic.
    public PieceType Type { get; set; }
    public char Symbol { get; set; }
    public int MovementRange { get; set; }
    public int Attack { get; set; } = 10;  // Updated default attack.
    public int AttackRange { get; set; }   // New property for attack range.
    public bool IsRanged { get; set; }     // True for ranged pieces (e.g. Archer).
    public IMovementBehavior MovementBehavior { get; set; }
    public IAttackBehavior AttackBehavior { get; set; }
    private int hp;
    private Player player;

    public Piece(int row, int col, Player player, PieceType type)
    {
        Row = row;
        Col = col;
        InitialRow = row;  // Record starting row.
        this.player = player;
        Type = type;
        
        // Set defaults based on piece type:
        switch (type)
        {
            case PieceType.Pawn:
                Symbol = 'P';
                MovementRange = 1;
                AttackRange = 1;  
                hp = 10;
                MovementBehavior = new PawnMovementBehavior();
                IsRanged = false;
                AttackBehavior = new PawnAttackBehavior();
                break;
            case PieceType.Bishop:
                Symbol = 'B';
                MovementRange = 2;
                AttackRange = 4;
                hp = 10;
                MovementBehavior = new BishopMovementBehavior();
                IsRanged = false;
                AttackBehavior = new BishopAttackBehavior();
                break;
            case PieceType.Knight:
                Symbol = 'N';
                MovementRange = 3;
                AttackRange = MovementRange;
                hp = 10;
                MovementBehavior = new KnightMovementBehavior();
                IsRanged = false;
                AttackBehavior = new KnightAttackBehavior();
                break;
            case PieceType.Rook:
                Symbol = 'R';
                MovementRange = 10;
                AttackRange = 1;
                hp = 10;
                MovementBehavior = new RookMovementBehavior();
                IsRanged = false;
                AttackBehavior = new RookAttackBehavior();
                break;
            case PieceType.Queen:
                Symbol = 'Q';
                MovementRange = 10;
                AttackRange = MovementRange;
                hp = 10;
                MovementBehavior = new QueenMovementBehavior();
                IsRanged = false;
                AttackBehavior = new QueenAttackBehavior();
                break;
            case PieceType.Archer:
                Symbol = 'A';
                MovementRange = 1; 
                Attack = 5;
                AttackRange = 2; 
                hp = 10;
                MovementBehavior = new ArcherMovementBehavior();
                IsRanged = true;  // Ranged units do not move on kill.
                AttackBehavior = new ArcherAttackBehavior();
                break;
            case PieceType.Chaplain:
                Symbol = 'C';
                MovementRange = 1;
                AttackRange = MovementRange;
                hp = 10;
                MovementBehavior = null;
                IsRanged = false;
                break;
            case PieceType.King:
                Symbol = 'K';
                MovementRange = 1;
                AttackRange = MovementRange;
                hp = 20;
                MovementBehavior = new KingMovementBehavior();
                AttackBehavior = new KingAttackBehavior();
                IsRanged = false;
                break;
        }
    }

    // Overload constructor to accept a piece type as a string.
    public Piece(int row, int col, Player player, string pieceType)
        : this(row, col, player, (PieceType)Enum.Parse(typeof(PieceType), pieceType))
    {
    }

    public int GetHP() => hp;
    public void SetHP(int value) => hp = value;
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
    }
    public Player GetPlayer() => player;
    public void SetPlayer(Player p) => player = p;
}
