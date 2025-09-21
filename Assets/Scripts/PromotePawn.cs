using System;
using UnityEngine;
using UnityEngine.UI;

public class PromotePawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChessManager2D manager;

    private Piece pawnToPromote;

    [Header("UI Options")]
    [SerializeField] private Image[] optionImages;
    [SerializeField] private Sprite[] whiteSprites;
    [SerializeField] private Sprite[] blackSprites;

    private void Awake()
    {
        if (manager == null)
            Debug.LogWarning("⚠️ ChessManager2D chưa được gán trong PromotePawn!");

        gameObject.SetActive(false);
    }
    
    public void Show(Piece pawn)
    {
        pawnToPromote = pawn;
        Sprite[] sprites = (pawn.color == PieceColor.White) ? whiteSprites : blackSprites;

        for (int i = 0; i < optionImages.Length; i++)
        {
            if (i < sprites.Length)
                optionImages[i].sprite = sprites[i];
        }

        gameObject.SetActive(true);
    }
    
    public void OnChoosePromotion(int typeIndex)
    {
        if (pawnToPromote == null) return;

        PieceType chosenType = (PieceType)typeIndex; // enum index: 0=Pawn,1=Rook...
        // if (chosenType == PieceType.Pawn) chosenType = PieceType.Queen; // fallback

        manager.ApplyPromotion(pawnToPromote, chosenType);
        gameObject.SetActive(false);
    }
}