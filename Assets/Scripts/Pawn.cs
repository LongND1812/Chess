using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board, int boardSize = 8)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = (color == PieceColor.White)? 1 : -1; 
        
        // di 1 buoc
        Vector2Int forward = new  Vector2Int(boardPosition.x, boardPosition.y + direction);
        if(IsInsideBoard(forward, boardSize) && board[forward.x, forward.y] == null)
        {
            moves.Add(forward);
        }
        
        // buoc dau di 2 buoc
        if ((color == PieceColor.White && boardPosition.y == 1) || (color == PieceColor.Black && boardPosition.y == 6))
        {
            Vector2Int doubleForward = new Vector2Int(boardPosition.x, boardPosition.y + 2 * direction);
            if (board[forward.x, forward.y] == null && board[doubleForward.x, doubleForward.y] == null)
                moves.Add(doubleForward);
        }
        
        // an cheo
        Vector2Int[] diagonals = {
            new Vector2Int(boardPosition.x - 1, boardPosition.y + direction),
            new Vector2Int(boardPosition.x + 1, boardPosition.y + direction)
        };

        foreach (var d in diagonals)
        {
            if (IsInsideBoard(d, boardSize) && board[d.x, d.y] != null && board[d.x, d.y].color != this.color)
                moves.Add(d);
        }

        return moves;
    }
}
