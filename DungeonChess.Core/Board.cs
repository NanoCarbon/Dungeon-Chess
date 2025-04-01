using System;
using System.Diagnostics;
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

            // Read JSON from the external file using the application's base directory.
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string saveFilePath = Path.Combine(baseDir, "saves", "save_current.json");
            Debug.WriteLine("Looking for save file at: " + saveFilePath);
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

        // New method to capture the game state (including player info) for saving.
        public BoardState GetBoardState()
        {
            var state = new BoardState();
            state.BoardSize = boardSize;
            state.Tiles = new List<TileData>();
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    state.Tiles.Add(new TileData
                    {
                        Row = row,
                        Col = col,
                        IsTraversable = Tiles[row, col].IsTraversable,
                        BackgroundColor = Tiles[row, col].BackgroundColor == Color.DarkGray ? "DarkGray" : "White"
                    });
                }
            }
            
            state.Pieces = new List<PieceData>();
            foreach (var piece in Pieces)
            {
                state.Pieces.Add(new PieceData
                {
                    Row = piece.Row,
                    Col = piece.Col,
                    // Use the PieceType property (assumed to be of type PieceType) so that the saved value matches your enum.
                    Player = (piece.GetPlayer() == player1) ? 1 : 2,
                    Type = piece.Type.ToString()  // Updated to use the enum value.
                });
            }
                        // Save player data.
            state.Player1 = new PlayerData
            {
                Energy = player1.Energy,
                HP = player1.HP,
                PieceColor = player1.PieceColor.Name
            };
            state.Player2 = new PlayerData
            {
                Energy = player2.Energy,
                HP = player2.HP,
                PieceColor = player2.PieceColor.Name
            };
            
            state.CurrentPlayer = (currentPlayer == player1) ? 1 : 2;
            
            return state;
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

    // Updated BoardState class with player information.
    public class BoardState
    {
        public int BoardSize { get; set; }
        public List<TileData> Tiles { get; set; }
        public List<PieceData> Pieces { get; set; }
        public PlayerData Player1 { get; set; }
        public PlayerData Player2 { get; set; }
        public int CurrentPlayer { get; set; }
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

    // New class to capture player details.
    public class PlayerData
    {
        public int Energy { get; set; }
        public int HP { get; set; }
        public string PieceColor { get; set; }
    }
}
