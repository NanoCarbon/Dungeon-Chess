using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace DungeonChess.Core
{
    public class Board
    {
        public List<Piece> Pieces { get; private set; }
        
        public Player player1;
        public Player player2;
        public Player currentPlayer;

        // For an 8x8 board.
        private const int boardSize = 8;

        // The grid of tiles.
        public Tile[,] Tiles { get; private set; }

        public Board()
        {
            // Initialize players and assign colors.
            player1 = new Player();
            player2 = new Player();
            player1.PieceColor = Color.LimeGreen;
            player2.PieceColor = Color.Purple;
            currentPlayer = player1;

            Pieces = new List<Piece>();

            // Initialize the tile grid.
            Tiles = new Tile[boardSize, boardSize];
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Tiles[row, col] = new Tile();
                    // Mark the middle 4 squares (rows 3 and 4, cols 3 and 4) as barriers.
                    if ((row == 3 || row == 4) && (col == 3 || col == 4))
                    {
                        Tiles[row, col].IsTraversable = false;
                        Tiles[row, col].BackgroundColor = Color.DarkGray;
                    }
                    else
                    {
                        Tiles[row, col].IsTraversable = true;
                        Tiles[row, col].BackgroundColor = Color.White;
                    }
                }
            }

            // Read JSON from the external file.
            string saveFilePath = Path.Combine("saves", "save_0001.json");
            Console.WriteLine("Looking for save file at: " + Path.GetFullPath(saveFilePath));
            if (!File.Exists(saveFilePath))
            {
                throw new FileNotFoundException($"Save file not found: {saveFilePath}");
            }
            string json = File.ReadAllText(saveFilePath);
            
            // Deserialize JSON.
            BoardState state = JsonSerializer.Deserialize<BoardState>(json);
            if (state != null)
            {
                // Optionally, update board size using state.BoardSize if needed.
                // Initialize Tiles based on JSON.
                foreach (TileData td in state.Tiles)
                {
                    // For each tile in the JSON, update the corresponding tile in our Tiles array.
                    if (td.Row < boardSize && td.Col < boardSize)
                    {
                        Tiles[td.Row, td.Col].IsTraversable = td.IsTraversable;
                        // Convert color string to a Color. For simplicity, assume "White" or "DarkGray".
                        Tiles[td.Row, td.Col].BackgroundColor = td.BackgroundColor == "DarkGray" ? Color.DarkGray : Color.White;
                    }
                }
                // Deserialize pieces.
                if (state.Pieces != null)
                {
                    foreach (PieceData pd in state.Pieces)
                    {
                        PieceType type = (PieceType)Enum.Parse(typeof(PieceType), pd.Type);
                        Player owner = (pd.Player == 1) ? player1 : player2;
                        Piece piece = new Piece(pd.Row, pd.Col, owner, type);
                        Pieces.Add(piece);
                    }
                }
            }
        }

        public Piece GetPieceAt(int row, int col)
        {
            foreach (var piece in Pieces)
            {
                if (piece.Row == row && piece.Col == col)
                    return piece;
            }
            return null;
        }

        public bool MovePiece(Piece piece, int newRow, int newCol)
        {
            if (piece.Row == newRow && piece.Col == newCol)
                return false;
            if (piece.GetPlayer() != currentPlayer)
                return false;
            Piece occupant = GetPieceAt(newRow, newCol);
            if (occupant != null && occupant != piece)
                return false;
            if (!Tiles[newRow, newCol].IsTraversable)
                return false;
            int dx = Math.Abs(newRow - piece.Row);
            int dy = Math.Abs(newCol - piece.Col);
            int distance = Math.Max(dx, dy);
            if (distance > piece.MovementRange)
                return false;
            if (piece.MovementBehavior != null)
            {
                if (!piece.MovementBehavior.IsMoveValid(piece, newRow, newCol, this))
                    return false;
            }
            if (currentPlayer.Energy <= 0)
                return false;
            currentPlayer.Energy -= 1;
            piece.Row = newRow;
            piece.Col = newCol;
            return true;
        }

        public void EndTurn()
        {
            currentPlayer = (currentPlayer == player1) ? player2 : player1;
            currentPlayer.Energy = currentPlayer.StartingEnergy;
        }
    }

    public class BoardState
    {
        public int BoardSize { get; set; }
        public List<TileData> Tiles { get; set; }
        public List<PieceData> Pieces { get; set; }
    }

    public class TileData
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsTraversable { get; set; }
        public string BackgroundColor { get; set; }
    }

    public class PieceData
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Player { get; set; }
        public string Type { get; set; }
    }
}
