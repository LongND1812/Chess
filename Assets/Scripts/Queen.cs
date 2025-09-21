using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), // cheo len phai
            new Vector2Int(-1, -1), // cheo xuong trai
            new Vector2Int(1, -1), // cheo xuong phai
            new Vector2Int(-1, 1) // cheo len trai
        };

        foreach (var dir in directions)
        {
            Vector2Int current = boardPosition + dir;
            while (IsInsideBoard(current, boardSize))
            {
                int check = CheckTile(current, board, boardSize);
                // neu trong thi di chuyen
                if (check == 0)
                {
                    moves.Add(current);
                }
                else if (check == 1)
                {
                    //neu co quan dich thi an
                    moves.Add(current);
                    break;
                }
                else if (check == -2)
                {
                    break;
                }

                current += dir;
            }
        }

        return moves;
    }
}