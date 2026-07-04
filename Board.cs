using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public GameObject tilePrefab;
    public ItemData[] availableItems;
    public Transform[] layerParents;

    [Header("Layout Settings")]
    public Vector2 tileSize = new Vector2(0.6f, 0.6f);
    public LevelData[] levels; // kéo cả 3 level vào đây, thay cho currentLevel

    private List<ItemData> itemPool = new List<ItemData>();

    void Start()
    {
        int randomIndex = Random.Range(0, levels.Length);
        LoadLevel(levels[randomIndex]);
    }
    public void LoadLevel(LevelData level)
    {
        foreach (Transform parent in layerParents)
            foreach (Transform child in parent)
                Destroy(child.gameObject);

        int randomTileCount = 0;
        foreach (LevelTileData t in level.tiles)
        {
            if (t == null) continue;
            if (t.itemData == null) randomTileCount++;
        }

        GenerateItemPool(randomTileCount);

        List<Tile>[] layerTiles = new List<Tile>[layerParents.Length];
        for (int i = 0; i < layerParents.Length; i++)
            layerTiles[i] = new List<Tile>();

        // Tính size của layer 0 làm base
        Vector2Int baseSize = level.GetLayerSize(0);

        foreach (LevelTileData tileData in level.tiles)
        {
            // Tính size của layer hiện tại
            Vector2Int layerSize = level.GetLayerSize(tileData.layerIndex);

            // Tính offset để căn giữa so với layer 0
            float offsetX = (baseSize.x - layerSize.x) * tileSize.x * 0.5f;
            float offsetY = (baseSize.y - layerSize.y) * tileSize.y * 0.5f;
            float offsetZ = -tileData.layerIndex * 0.1f; // Z nhỏ thôi, chỉ để sort

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

            ItemData item = tileData.itemData != null
                ? tileData.itemData
                : itemPool[0];

            if (tileData.itemData == null)
                itemPool.RemoveAt(0);

            tile.Setup(item, tileData.layerIndex * 10);
            tile.layerIndex = tileData.layerIndex;

            layerTiles[tileData.layerIndex].Add(tile);
        }

        for (int i = 0; i < layerParents.Length - 1; i++)
            SetBlockingForLayer(layerTiles[i], layerTiles[i + 1]);

        int topLayer = 0;
        for (int i = 0; i < layerTiles.Length; i++)
        {
            if (layerTiles[i].Count > 0)
                topLayer = i;
        }
        foreach (Tile t in layerTiles[topLayer])
            t.UnlockTile();
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