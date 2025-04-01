using System;
using System.Collections.Generic;
using System.Drawing;

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
                }
            }

            // --- Player 1 (top side) ---
            // Row 0: Back row with standard chess order:
            // Rook, Knight, Bishop, Queen, King, Bishop, Knight, Rook.
            for (int col = 0; col < boardSize; col++)
            {
                if (col == 0 || col == boardSize - 1)
                    Pieces.Add(new Piece(0, col, player1, PieceType.Rook));
                else if (col == 1 || col == 6)
                    Pieces.Add(new Piece(0, col, player1, PieceType.Knight));
                else if (col == 3)
                    Pieces.Add(new Piece(0, col, player1, PieceType.Queen));
                else if (col == 4)
                    Pieces.Add(new Piece(0, col, player1, PieceType.King));
                else // col 2 and col 5.
                    Pieces.Add(new Piece(0, col, player1, PieceType.Bishop));
            }
            // Row 1: Instead of pawns, use archers for Player 1.
            for (int col = 0; col < boardSize; col++)
            {
                Pieces.Add(new Piece(1, col, player1, PieceType.Archer));
            }

            // --- Player 2 (bottom side) ---
            // Row 6: Pawns for Player 2.
            for (int col = 0; col < boardSize; col++)
            {
                Pieces.Add(new Piece(6, col, player2, PieceType.Pawn));
            }
            // Row 7: Back row for Player 2:
            // Rook, Knight, Bishop, Queen, King, Bishop, Knight, Rook.
            for (int col = 0; col < boardSize; col++)
            {
                if (col == 0 || col == boardSize - 1)
                    Pieces.Add(new Piece(7, col, player2, PieceType.Rook));
                else if (col == 1 || col == 6)
                    Pieces.Add(new Piece(7, col, player2, PieceType.Knight));
                else if (col == 3)
                    Pieces.Add(new Piece(7, col, player2, PieceType.Queen));
                else if (col == 4)
                    Pieces.Add(new Piece(7, col, player2, PieceType.King));
                else // col 2 and col 5.
                    Pieces.Add(new Piece(7, col, player2, PieceType.Bishop));
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
            // Check if the new position is the same as the current position.
            if (piece.Row == newRow && piece.Col == newCol)
                return false;

            // Check that the piece belongs to the current player.
            if (piece.GetPlayer() != currentPlayer)
                return false;

            // Check if the destination tile is occupied by a different piece.
            Piece occupant = GetPieceAt(newRow, newCol);
            if (occupant != null && occupant != piece)
                return false;

            // Check if the destination tile is traversable.
            if (!Tiles[newRow, newCol].IsTraversable)
                return false;

            // Calculate Chebyshev distance.
            int dx = Math.Abs(newRow - piece.Row);
            int dy = Math.Abs(newCol - piece.Col);
            int distance = Math.Max(dx, dy);

            if (distance > piece.MovementRange)
                return false;

            // Check movement behavior if defined.
            if (piece.MovementBehavior != null)
            {
                if (!piece.MovementBehavior.IsMoveValid(piece, newRow, newCol, this))
                    return false;
            }

            // Check that the current player has enough energy.
            if (currentPlayer.Energy <= 0)
                return false;

            // Valid move: deduct energy and perform the move.
            currentPlayer.Energy -= 1;
            piece.Row = newRow;
            piece.Col = newCol;
            return true;
        }

        // EndTurn: switches the turn and resets the new current player's energy.
        public void EndTurn()
        {
            currentPlayer = (currentPlayer == player1) ? player2 : player1;
            currentPlayer.Energy = currentPlayer.StartingEnergy;
        }
    }
}
