using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public GameObject tilePrefab;
    public ItemData[] availableItems;
    public Transform[] layerParents;

    [Header("Layout Settings")]
    public Vector2 tileSize = new Vector2(0.6f, 0.6f);

    private List<ItemData> itemPool = new List<ItemData>();

    void Awake() => Instance = this;

    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        LevelData data = LevelLoader.Instance.LoadLevel(currentLevel);
        if (data != null) LoadLevel(data);
    }

    public void LoadLevel(LevelData level)
    {
        // Xóa board cũ
        foreach (Transform parent in layerParents)
            foreach (Transform child in parent)
                Destroy(child.gameObject);

        // Đếm tile random (itemName rỗng)
        int randomTileCount = 0;
        foreach (LevelTileData t in level.tiles)
        {
            if (t == null) continue;
            if (string.IsNullOrEmpty(t.itemName)) randomTileCount++;
        }

        GenerateItemPool(randomTileCount);

        List<Tile>[] layerTiles = new List<Tile>[layerParents.Length];
        for (int i = 0; i < layerParents.Length; i++)
            layerTiles[i] = new List<Tile>();

        // Tính size layer 0 làm base để căn giữa
        Vector2Int baseSize = GetLayerSize(level, 0);

        foreach (LevelTileData tileData in level.tiles)
        {
            if (tileData == null) continue;

            // Căn giữa từng layer so với layer 0
            Vector2Int layerSize = GetLayerSize(level, tileData.layerIndex);
            float offsetX = (baseSize.x - layerSize.x) * tileSize.x * 0.5f;
            float offsetY = (baseSize.y - layerSize.y) * tileSize.y * 0.5f;
            float offsetZ = -tileData.layerIndex * 0.1f;

            Vector3 pos = new Vector3(
                tileData.x * tileSize.x + offsetX,
                tileData.y * tileSize.y + offsetY,
                offsetZ
            );

            GameObject tileObj = Instantiate(
                tilePrefab, pos,
                Quaternion.identity,
                layerParents[tileData.layerIndex]
            );

            Tile tile = tileObj.GetComponent<Tile>();

            // Có tên → tìm ItemData, rỗng → lấy từ random pool
            ItemData item;
            if (!string.IsNullOrEmpty(tileData.itemName))
            {
                item = GetItemByName(tileData.itemName);
            }
            else
            {
                item = itemPool[0];
                itemPool.RemoveAt(0);
            }

            tile.Setup(item, tileData.layerIndex * 10);
            tile.layerIndex = tileData.layerIndex;

            layerTiles[tileData.layerIndex].Add(tile);
        }

        // Thiết lập blocking giữa các layer
        for (int i = 0; i < layerParents.Length - 1; i++)
            SetBlockingForLayer(layerTiles[i], layerTiles[i + 1]);

        // Unlock layer trên cùng
        int topLayer = 0;
        for (int i = 0; i < layerTiles.Length; i++)
            if (layerTiles[i].Count > 0) topLayer = i;

        foreach (Tile t in layerTiles[topLayer])
            t.UnlockTile();

        // Áp dụng settings từ level
        CollectionBar.Instance.maxSlots = level.maxSlots;
        GameManager.Instance.RegisterTotalTiles(GetRemainingTileCount());
    }

    // Thay thế level.GetLayerSize() — tính max x, y của từng layer
    Vector2Int GetLayerSize(LevelData level, int layerIndex)
    {
        int maxX = 0, maxY = 0;
        foreach (LevelTileData t in level.tiles)
        {
            if (t.layerIndex != layerIndex) continue;
            if (t.x > maxX) maxX = t.x;
            if (t.y > maxY) maxY = t.y;
        }
        return new Vector2Int(maxX + 1, maxY + 1);
    }

    ItemData GetItemByName(string itemName)
    {
        foreach (var item in availableItems)
            if (item.itemName == itemName) return item;

        Debug.LogWarning($"Không tìm thấy ItemData: '{itemName}', dùng item đầu tiên thay thế");
        return availableItems[0];
    }

    void GenerateItemPool(int totalTiles)
    {
        itemPool.Clear();
        int adjustedTotal = Mathf.CeilToInt(totalTiles / 3f) * 3;
        int setsPerType = Mathf.Max(1, adjustedTotal / (availableItems.Length * 3));

        foreach (ItemData item in availableItems)
            for (int i = 0; i < setsPerType * 3; i++)
                itemPool.Add(item);

        while (itemPool.Count < adjustedTotal)
        {
            ItemData randomItem = availableItems[Random.Range(0, availableItems.Length)];
            itemPool.Add(randomItem);
            itemPool.Add(randomItem);
            itemPool.Add(randomItem);
        }

        // Fisher-Yates shuffle
        for (int i = itemPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (itemPool[i], itemPool[j]) = (itemPool[j], itemPool[i]);
        }
    }

    void SetBlockingForLayer(List<Tile> lowerTiles, List<Tile> upperTiles)
    {
        foreach (Tile lower in lowerTiles)
        {
            List<Tile> blockers = new List<Tile>();
            foreach (Tile upper in upperTiles)
            {
                if (Mathf.Abs(lower.transform.position.x - upper.transform.position.x) < tileSize.x * 0.55f &&
                    Mathf.Abs(lower.transform.position.y - upper.transform.position.y) < tileSize.y * 0.55f)
                {
                    blockers.Add(upper);
                }
            }
            lower.SetBlockingTiles(blockers);
        }
    }

    public int GetRemainingTileCount()
    {
        int count = 0;
        foreach (Transform parent in layerParents)
            count += parent.childCount;
        return count;
    }
}
