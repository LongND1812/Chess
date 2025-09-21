using System.Collections.Generic;
using UnityEngine;

public static class ChessUtils
{
    /// <summary>
    /// Kiểm tra vua có đang bị chiếu tại vị trí hiện tại không.
    /// </summary>
    public static bool IsKingInCheck(Piece king, Piece[,] board, Vector2Int kingPos)
    {
        int size = board.GetLength(0);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Piece p = board[x, y];
                if (p == null || p.color == king.color)
                    continue;

                if (IsThreateningSquare(p, kingPos, board, size))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Kiểm tra quân p có đang đe doạ ô target không.
    /// </summary>
    private static bool IsThreateningSquare(Piece p, Vector2Int target, Piece[,] board, int size)
    {
        if (p is King)
        {
            // Vua chỉ có thể chiếu các ô kề 1 ô
            Vector2Int[] dirs =
            {
                new(1, 0), new(-1, 0),
                new(0, 1), new(0, -1),
                new(1, 1), new(1, -1),
                new(-1, 1), new(-1, -1)
            };

            foreach (var d in dirs)
            {
                if (p.boardPosition + d == target)
                    return true;
            }

            return false;
        }

        // Quân khác → dùng nước đi cơ bản
        List<Vector2Int> moves = p.GetAvailableMoves(board, size);
        return moves.Contains(target);
    }

    /// <summary>
    /// Kiểm tra chiếu hết (checkmate).
    /// </summary>
    public static bool IsCheckmate(PieceColor color, Piece[,] board, MoveGenerator moveGen)
    {
        King king = FindKing(color, board);
        if (king == null) return false;

        if (!IsKingInCheck(king, board, king.boardPosition))
            return false;

        return !HasAnyValidMove(color, board, moveGen);
    }

    /// <summary>
    /// Kiểm tra hòa (stalemate).
    /// </summary>
    public static bool IsStalemate(PieceColor color, Piece[,] board, MoveGenerator moveGen)
    {
        King king = FindKing(color, board);
        if (king == null) return false;

        return !IsKingInCheck(king, board, king.boardPosition) &&
               !HasAnyValidMove(color, board, moveGen);
    }

    /// <summary>
    /// Kiểm tra xem màu quân có còn nước đi hợp lệ nào không.
    /// </summary>
    private static bool HasAnyValidMove(PieceColor color, Piece[,] board, MoveGenerator moveGen)
    {
        int size = board.GetLength(0);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Piece p = board[x, y];
                if (p != null && p.color == color)
                {
                    var moves = moveGen.GetValidMoves(p, board);
                    if (moves.Count > 0)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Tìm vua theo màu.
    /// </summary>
    public static King FindKing(PieceColor color, Piece[,] board)
    {
        int size = board.GetLength(0);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (board[x, y] is King k && k.color == color)
                    return k;
            }
        }
        return null;
    }
}