using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessManager2D : MonoBehaviour
{
    [Header("Board References")] [SerializeField]
    private Board board;

    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private GameObject highlightPrefab;

    [Header("Sprites")] [SerializeField]
    private Sprite[] whiteSprites; // [0]=pawn,1=rook,2=knight,3=bishop,4=queen,5=king

    [SerializeField] private Sprite[] blackSprites;

    [Header("UI")] [SerializeField] private PromotePawn promotePawn;
    [SerializeField] private CheckNotifier checkNotifier;
    [SerializeField] private Castling castling;

    [Header(("Audio"))] [SerializeField] private AudioSource moveSound;


    private Piece[,] boards;
    private MoveGenerator moveGenerator;
    private bool isGameOver = false;

    private Piece selectedPiece;
    private List<Vector2Int> validMoves = new List<Vector2Int>();
    private List<GameObject> highlights = new List<GameObject>();

    public PieceColor CurrentTurn { get; private set; } = PieceColor.White;

    private void Start()
    {
        boards = new Piece[board.Size, board.Size];
        moveGenerator = new MoveGenerator(board.Size);
        SetupBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButton(1))
        {
            DeselectPiece();
        }
    }

    #region Input Handling

    private void HandleLeftClick()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int boardPos = WorldToBoard(worldPos);

        if (!IsInsideBoard(boardPos)) return;

        Piece clickedPiece = boards[boardPos.x, boardPos.y];

        if (selectedPiece == null)
        {
            TrySelectPiece(clickedPiece);
        }
        else
        {
            if (clickedPiece != null && clickedPiece.color == selectedPiece.color)
            {
                SelectPiece(clickedPiece);
            }
            else if (validMoves.Contains(boardPos))
            {
                MovePiece(selectedPiece, boardPos);
                EndTurn();
            }
            else
            {
                DeselectPiece();
            }
        }
    }

    #endregion

    #region Turn Management

    private void EndTurn()
    {
        CurrentTurn = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

        if (ChessUtils.IsCheckmate(CurrentTurn, boards, moveGenerator))
        {
            CurrentTurn = CurrentTurn == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;
            checkNotifier.ShowEndGame($"{CurrentTurn} win!!!");
            isGameOver = true;
        }
        else if (ChessUtils.IsStalemate(CurrentTurn, boards, moveGenerator))
        {
            checkNotifier.ShowEndGame("HÒA!\nSTALEMATE!");
            isGameOver = true;
        }
        else
        {
            King kingToCheck = ChessUtils.FindKing(CurrentTurn, boards);
            if (kingToCheck != null && ChessUtils.IsKingInCheck(kingToCheck, boards, kingToCheck.boardPosition))
                checkNotifier.ShowCheck();
            else
                checkNotifier.HideAll();
        }

        DeselectPiece();
    }

    private King FindKing(PieceColor color)
    {
        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                if (boards[x, y] is King k && k.color == color)
                    return k;
            }
        }

        return null;
    }

    #endregion

    #region Piece Selection

    private void TrySelectPiece(Piece piece)
    {
        if (piece == null || piece.color != CurrentTurn) return;
        SelectPiece(piece);
    }

    private void SelectPiece(Piece piece)
    {
        if (isGameOver) return;
        selectedPiece = piece;
        validMoves = moveGenerator.GetValidMoves(piece, boards);
        if (piece is King king)
        {
            var castleMoves = castling.GetCastlingMoves(king, boards);
            validMoves.AddRange(castleMoves);
        }

        ShowHighlights(validMoves);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        validMoves.Clear();
        ClearHighlights();
    }

    #endregion

    #region Highlights

    private void ShowHighlights(List<Vector2Int> moves)
    {
        ClearHighlights();
        foreach (var pos in moves)
        {
            Vector3 worldPos = new Vector3(pos.x - board.Offset, pos.y - board.Offset, -0.5f);
            GameObject h = Instantiate(highlightPrefab, worldPos, Quaternion.identity, transform);
            highlights.Add(h);
        }
    }

    private void ClearHighlights()
    {
        foreach (var h in highlights)
            Destroy(h);
        highlights.Clear();
    }

    #endregion

    #region Moves & Promotion

    public void MovePiece(Piece piece, Vector2Int target)
    {
        if (isGameOver) return;
        Vector2Int originalPos = piece.boardPosition;
        //delete old tile
        boards[piece.boardPosition.x, piece.boardPosition.y] = null;

        if (boards[target.x, target.y] != null)
            Destroy(boards[target.x, target.y].gameObject);

        //update position king
        piece.boardPosition = target;
        piece.transform.position = new Vector3(target.x - board.Offset, target.y - board.Offset, -1);
        boards[target.x, target.y] = piece;

        //Promote Pawn
        if (piece.type == PieceType.Pawn &&
            ((piece.color == PieceColor.White && target.y == 7) ||
             (piece.color == PieceColor.Black && target.y == 0)))
        {
            promotePawn.Show(piece);
        }

        //Castling
        if (piece is King king)
        {
            castling.TryCastling(king, originalPos, target, boards, board.Offset);
        }

        if (moveSound != null && !AudioListener.pause)
            moveSound.Play();

        piece.hasMoved = true;
    }

    public void ApplyPromotion(Piece pawn, PieceType newType)
    {
        Vector2Int pos = pawn.boardPosition;
        PieceColor color = pawn.color;

        Destroy(pawn.gameObject); // ❗ Hủy pawn cũ

        // Spawn quân mới bằng prefab
        Spawn(newType, color, pos);
        PieceColor enemyColor = (pawn.color == PieceColor.White) ? PieceColor.Black : PieceColor.White;
        King enemyKing = ChessUtils.FindKing(enemyColor, boards);

        if (enemyKing != null && ChessUtils.IsKingInCheck(enemyKing, boards, enemyKing.boardPosition))
            checkNotifier.ShowCheck();
    }

    #endregion

    #region Helpers

    private Vector2Int WorldToBoard(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x + board.Offset);
        int y = Mathf.RoundToInt(pos.y + board.Offset);
        return new Vector2Int(x, y);
    }

    private bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < board.Size && pos.y >= 0 && pos.y < board.Size;
    }

    #endregion

    #region Setup

    public void SelectTheme(int index)
    {
        board.SetBoardTheme(index);
        PlayerPrefs.SetInt("BoardTheme", index);
        PlayerPrefs.Save();
    }

    private void SetupBoard()
    {
        // Pawns
        for (int x = 0; x < 8; x++)
        {
            Spawn(PieceType.Pawn, PieceColor.White, new Vector2Int(x, 1));
            Spawn(PieceType.Pawn, PieceColor.Black, new Vector2Int(x, 6));
        }

        // Rooks
        Spawn(PieceType.Rook, PieceColor.White, new Vector2Int(0, 0));
        Spawn(PieceType.Rook, PieceColor.White, new Vector2Int(7, 0));
        Spawn(PieceType.Rook, PieceColor.Black, new Vector2Int(0, 7));
        Spawn(PieceType.Rook, PieceColor.Black, new Vector2Int(7, 7));

        // Knights
        Spawn(PieceType.Knight, PieceColor.White, new Vector2Int(1, 0));
        Spawn(PieceType.Knight, PieceColor.White, new Vector2Int(6, 0));
        Spawn(PieceType.Knight, PieceColor.Black, new Vector2Int(1, 7));
        Spawn(PieceType.Knight, PieceColor.Black, new Vector2Int(6, 7));

        // Bishops
        Spawn(PieceType.Bishop, PieceColor.White, new Vector2Int(2, 0));
        Spawn(PieceType.Bishop, PieceColor.White, new Vector2Int(5, 0));
        Spawn(PieceType.Bishop, PieceColor.Black, new Vector2Int(2, 7));
        Spawn(PieceType.Bishop, PieceColor.Black, new Vector2Int(5, 7));

        // Queens
        Spawn(PieceType.Queen, PieceColor.White, new Vector2Int(3, 0));
        Spawn(PieceType.Queen, PieceColor.Black, new Vector2Int(3, 7));

        // Kings
        Spawn(PieceType.King, PieceColor.White, new Vector2Int(4, 0));
        Spawn(PieceType.King, PieceColor.Black, new Vector2Int(4, 7));
    }

    private void Spawn(PieceType type, PieceColor color, Vector2Int pos)
    {
        GameObject obj = Instantiate(piecePrefab, transform);
        Piece p = obj.AddComponent(GetPieceClass(type)) as Piece;

        p.type = type;
        p.color = color;
        p.PlaceAt(pos, board.Offset);

        p.boardPosition = pos;
        boards[pos.x, pos.y] = p;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = (color == PieceColor.White) ? whiteSprites[(int)type] : blackSprites[(int)type];
        sr.sortingLayerName = "Pieces";
        sr.sortingOrder = 10;
    }

    private System.Type GetPieceClass(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => typeof(Pawn),
            PieceType.Rook => typeof(Rook),
            PieceType.Knight => typeof(Knight),
            PieceType.Bishop => typeof(Bishop),
            PieceType.Queen => typeof(Queen),
            PieceType.King => typeof(King),
            _ => typeof(Pawn)
        };
    }

    #endregion
}