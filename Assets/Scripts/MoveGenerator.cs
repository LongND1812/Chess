using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator
{
    private readonly int boardSize;

    public MoveGenerator(int size = 8)
    {
        boardSize = size;
    }

    /// <summary>
    /// Trả về danh sách nước đi hợp lệ (lọc chiếu/pin).
    /// </summary>
    public List<Vector2Int> GetValidMoves(Piece piece, Piece[,] board)
    {
        List<Vector2Int> possibleMoves = piece.GetAvailableMoves(board, boardSize);
        List<Vector2Int> validMoves = new();

        King myKing = (piece.type == PieceType.King)
            ? (King)piece
            : FindKing(piece.color, board);

        foreach (var move in possibleMoves)
        {
            if (IsMoveSafe(piece, move, board, myKing))
            {
                validMoves.Add(move);
            }
        }

        return validMoves;
    }

    /// <summary>
    /// Kiểm tra xem nước đi có an toàn (không bị chiếu) không.
    /// </summary>
    private bool IsMoveSafe(Piece piece, Vector2Int target, Piece[,] board, King myKing)
    {
        // Tạo snapshot "nhẹ" thay vì deep clone toàn bộ
        Piece captured = board[target.x, target.y];
        Vector2Int oldPos = piece.boardPosition;

        // Giả lập di chuyển
        board[oldPos.x, oldPos.y] = null;
        board[target.x, target.y] = piece;
        piece.boardPosition = target;

        bool safe = true;
        try
        {
            King kingToCheck = (piece.type == PieceType.King) ? (King)piece : myKing;
            if (kingToCheck != null &&
                ChessUtils.IsKingInCheck(kingToCheck, board, kingToCheck.boardPosition))
            {
                safe = false;
            }
        }
        finally
        {
            // 🔄 Khôi phục trạng thái
            piece.boardPosition = oldPos;
            board[oldPos.x, oldPos.y] = piece;
            board[target.x, target.y] = captured;
        }

        return safe;
    }

    private King FindKing(PieceColor color, Piece[,] board)
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (board[x, y] is King k && k.color == color)
                {
                    return k;
                }
            }
        }

        return null;
    }
}