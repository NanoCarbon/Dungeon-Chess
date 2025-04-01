using System.Collections.Generic;
using System.Drawing;

namespace DungeonChess.Core
{
    public class Player
    {
        public int Energy { get; set; }
        public int HP { get; set; }
        public Dictionary<string, int> PieceInventory { get; set; }
        public Dictionary<string, int> ItemInventory { get; set; }
        public int StartingEnergy { get; } = 1;
        public Color PieceColor { get; set; }

        public Player()
        {
            Energy = StartingEnergy;
            HP = 3;
            PieceInventory = new Dictionary<string, int>();
            ItemInventory = new Dictionary<string, int>();
            PieceColor = Color.White; // default; will be updated in Board.cs.
        }
    }
}
