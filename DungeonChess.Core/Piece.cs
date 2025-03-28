namespace DungeonChess.Core;

public class Piece
{
    private int hp;

    public Piece()
    {
        hp = 10;
    }

    public int GetHP()
    {
        return hp;
    }

    public void SetHP(int value)
    {
        hp = value;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
    }
}
