using System.Drawing;

namespace DungeonChess.Core
{
    public class Tile
    {
        // All tiles are traversable by default.
        public bool IsTraversable { get; set; } = true;
        // All tiles have a white background by default.
        public Color BackgroundColor { get; set; } = Color.White;
    }
}