using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board, int boardSize = 8)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        // 8 hướng xung quanh vua
        Vector2Int[] directions =
        {
            new Vector2Int(1, 0), // phải
            new Vector2Int(-1, 0), // trái
            new Vector2Int(0, 1), // lên
            new Vector2Int(0, -1), // xuống
            new Vector2Int(1, 1), // chéo phải lên
            new Vector2Int(1, -1), // chéo phải xuống   
            new Vector2Int(-1, 1), // chéo trái lên
            new Vector2Int(-1, -1) // chéo trái xuống
        };
        foreach (var dir in directions)
        {
            Vector2Int target = boardPosition + dir;
            if (!IsInsideBoard(target, boardSize))
                continue;

            int check = CheckTile(target, board, boardSize);
            if (check == 0 || check == 1)
            {
                // Dùng snapshot để kiểm tra chiếu mà KHÔNG động vào board thật
                Piece[,] snapshot = (Piece[,])board.Clone();
                snapshot[boardPosition.x, boardPosition.y] = null;
                snapshot[target.x, target.y] = this;

                if (!ChessUtils.IsKingInCheck(this, snapshot, target))
                    moves.Add(target);
            }
        }

        return moves;
    }
}