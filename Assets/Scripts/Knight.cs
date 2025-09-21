using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override void PlaceAt(Vector2Int pos, float offset)
    {
        boardPosition = pos;
        transform.position = new Vector3(pos.x - offset, pos.y - offset, 0);
    }
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board, int boardSize = 8)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        // di chuyển   
        Vector2Int[] directions =
        {
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(-1, -2)
        };
        foreach (var dir in directions)
        {
            Vector2Int target = boardPosition + dir;

            if (!IsInsideBoard(target, boardSize))
                continue;

            int check = CheckTile(target, board, boardSize);
            if (check == 0 || check == 1) // ô trống hoặc có quân địch
                moves.Add(target);
        }
        return moves;
    }
}
