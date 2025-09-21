using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")] public int size = 8; // số ô 1 cạnh bàn cờ
    public GameObject tilePrefab; // prefab rỗng có SpriteRenderer
    public Sprite whiteSprite;
    public Sprite blackSprite;
    public Sprite whiteSprite2;
    public Sprite blackSprite2;

    private GameObject[,] tiles;
    private float offset;

    public int Size => size;
    public float Offset => offset;

    private void Awake()
    {
        offset = size / 2f - 0.5f;
    }

    void Start()
    {
        GenerateBoard();
        int savedTheme = PlayerPrefs.GetInt("BoardTheme", 1);
        SetBoardTheme(savedTheme);
    }

    private void GenerateBoard()
    {
        if (tilePrefab == null || whiteSprite == null || blackSprite == null)
        {
            Debug.LogError("Missing tilePrefab or sprites in Board!");
            return;
        }

        tiles = new GameObject[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(x - offset, y - offset, 0);
                tile.name = $"Tile_{x}_{y}";

                // Lấy sẵn SpriteRenderer thay vì gọi GetComponent nhiều lần
                if (tile.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    sr.sprite = ((x + y) % 2 == 0) ? blackSprite : whiteSprite;
                }

                tiles[x, y] = tile;
            }
        }
    }

    public void SetBoardTheme(int themeIndex)
    {
        if (tiles == null) return;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var tile = tiles[x, y];
                if (tile != null && tile.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    bool isBlack = (x + y) % 2 == 0;

                    if (themeIndex == 2)
                        sr.sprite = isBlack ? blackSprite2 : whiteSprite2;
                    else
                        sr.sprite = isBlack ? blackSprite : whiteSprite;
                }
            }
        }
    }

    public GameObject GetTileAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= size || pos.y >= size)
            return null;
        return tiles[pos.x, pos.y];
    }
}