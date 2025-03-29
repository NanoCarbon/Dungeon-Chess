using System.Collections.Generic;

namespace DungeonChess.Core;

public class Board
{
    public List<Piece> Pieces { get; private set; }

    public Board()
    {
        Pieces = new List<Piece>();

        // Add test pieces
        Pieces.Add(new Piece(0, 0));
        Pieces.Add(new Piece(1, 3));
        Pieces.Add(new Piece(4, 4));
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

    public void MovePiece(Piece piece, int newRow, int newCol)
    {
        piece.Row = newRow;
        piece.Col = newCol;
    }
}
