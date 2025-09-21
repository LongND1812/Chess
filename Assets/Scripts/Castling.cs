using System.Collections.Generic;
using UnityEngine;

public class Castling : MonoBehaviour
{
    [SerializeField] private ChessManager2D manager;

    /// <summary>
    /// Trả về danh sách ô castling hợp lệ cho vua này.
    /// </summary>
    public List<Vector2Int> GetCastlingMoves(King king, Piece[,] board)
    {
        List<Vector2Int> moves = new();

        if (king.hasMoved) return moves;

        // Kingside
        TryAddCastling(king, board, moves, new Vector2Int(7, king.boardPosition.y), Vector2Int.right);

        // Queenside
        TryAddCastling(king, board, moves, new Vector2Int(0, king.boardPosition.y), Vector2Int.left);

        return moves;
    }

    private void TryAddCastling(King king, Piece[,] board, List<Vector2Int> moves, Vector2Int rookPos, Vector2Int dir)
    {
        Piece rook = board[rookPos.x, rookPos.y];
        if (rook == null || rook.type != PieceType.Rook || rook.hasMoved) return;

        // Kiểm tra đường giữa vua và xe rỗng
        Vector2Int current = king.boardPosition + dir;
        while (current != rookPos)
        {
            if (board[current.x, current.y] != null) return;
            current += dir;
        }

        // Kiểm tra vua không đi qua ô bị chiếu
        for (int i = 0; i <= 2; i++)
        {
            Vector2Int checkPos = king.boardPosition + dir * i;
            if (ChessUtils.IsKingInCheck(king, board, checkPos))
                return;
        }

        // Hợp lệ → thêm ô castling (2 bước)
        moves.Add(king.boardPosition + dir * 2);
    }

    /// <summary>
    /// Thực hiện nhập thành nếu target là ô castling.
    /// </summary>
    public bool TryCastling(King king, Vector2Int originalPos, Vector2Int target, Piece[,] board, float offset)
    {
        int dx = target.x - originalPos.x;
        if (Mathf.Abs(dx) != 2) return false; // không phải castling

        // Kingside
        if (dx == 2)
            MoveRook(new Vector2Int(7, originalPos.y), new Vector2Int(target.x - 1, target.y), board, offset);
        // Queenside
        else if (dx == -2)
            MoveRook(new Vector2Int(0, originalPos.y), new Vector2Int(target.x + 1, target.y), board, offset);

        return true;
    }

    private void MoveRook(Vector2Int from, Vector2Int to, Piece[,] board, float offset)
    {
        Piece rook = board[from.x, from.y];
        if (rook == null || rook.type != PieceType.Rook) return;

        board[from.x, from.y] = null;
        board[to.x, to.y] = rook;
        rook.boardPosition = to;
        rook.transform.position = new Vector3(to.x - offset, to.y - offset, -1);
        rook.hasMoved = true;
    }
}
