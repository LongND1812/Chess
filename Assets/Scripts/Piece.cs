using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    White,
    Black
}

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public abstract class Piece : MonoBehaviour
{
    [Header("Piece")] public PieceColor color;
    public PieceType type;
    public bool hasMoved;

    [Header("Piece Position")] public Vector2Int boardPosition;
    public Board board;

    public abstract List<Vector2Int> GetAvailableMoves(Piece[,] board, int boardSize = 8);

    public virtual void PlaceAt(Vector2Int pos, float offset)
    {
        boardPosition = pos;
        transform.position = new Vector3(pos.x - offset, pos.y - offset, -2);
    }

    public bool IsInsideBoard(Vector2Int pos, int size)
    {
        return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
    }

    public int CheckTile(Vector2Int pos, Piece[,] board, int size)
    {
        if (!IsInsideBoard(pos, size))
            return -1; // ngoài bàn thì coi như chặn luôn

        Piece target = board[pos.x, pos.y];
        if (target is null)
            return 0; // trống
        if (target.color != this.color)
            return 1; // có quân đối phương
        if (target.color == this.color)
            return -2; // có quân cùng màu
        return -1;
    }
}