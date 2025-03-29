namespace DungeonChess.Core;

public class Piece
{
    public int Row { get; set; }
    public int Col { get; set; }
    public char Symbol { get; set; } = 'P';

    private int hp;

    public Piece(int row, int col)
    {
        Row = row;
        Col = col;
        hp = 10;
    }

    public int GetHP() => hp;

    public void SetHP(int value) => hp = value;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
    }
}
