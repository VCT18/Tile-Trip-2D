using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelGenerator : EditorWindow
{
    [MenuItem("TripleMatch/Generate Levels")]
    public static void GenerateLevels()
    {
        // Load 5 ItemData
        ItemData[] items = new ItemData[5];
        for (int i = 1; i <= 5; i++)
        {
            items[i - 1] = AssetDatabase.LoadAssetAtPath<ItemData>(
    $"Assets/ScriptableObjects/F{i}.asset"
);
            if (items[i - 1] == null)
                Debug.LogWarning($"Không tìm thấy F{i}.asset — tile sẽ dùng random");
        }

        CreateLevel1(items);
        CreateLevel2(items);
        CreateLevel3(items);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Đã tạo 3 level thành công!");
    }

    // ─── Level 1: Hình Kim Tự Tháp ───
    // Layer 0: 4x4 = 16 tiles
    // Layer 1: 3x3 =  9 tiles (căn giữa)
    // Layer 2: 2x2 =  4 tiles (căn giữa)
    // Layer 3: 1x1 =  1 tile  (đỉnh)
    // Tổng: 30 tiles → bội số 3 ✅
    static void CreateLevel1(ItemData[] items)
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        level.levelName = "Level 1 - Kim Tu Thap";
        level.tiles = new List<LevelTileData>();

        AddGrid(level, layerIndex: 0, cols: 4, rows: 4, items);
        AddGrid(level, layerIndex: 1, cols: 3, rows: 3, items);
        AddGrid(level, layerIndex: 2, cols: 2, rows: 2, items);
        AddGrid(level, layerIndex: 3, cols: 1, rows: 1, items);

        SaveLevel(level, "Level_01_Pyramid");
    }

    // ─── Level 2: Hình Chữ Thập ───
    // Layer 0: chữ thập 5x5 bỏ 4 góc = 16 tiles
    // Layer 1: 3x3 = 9 tiles
    // Layer 2: 1x1 = 1 tile
    // Tổng: 26 → làm tròn lên 27 ✅
    static void CreateLevel2(ItemData[] items)
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        level.levelName = "Level 2 - Chu Thap";
        level.tiles = new List<LevelTileData>();

        // Layer 0: chữ thập (bỏ 4 góc của 5x5)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                bool isCorner = (x == 0 || x == 4) && (y == 0 || y == 4);
                if (isCorner) continue;
                level.tiles.Add(new LevelTileData { layerIndex = 0, x = x, y = y });
            }
        }

        // Layer 1: 3x3
        AddGrid(level, layerIndex: 1, cols: 3, rows: 3, items);

        // Layer 2: 1x1
        AddGrid(level, layerIndex: 2, cols: 1, rows: 1, items);

        // Thêm 1 tile random để đủ bội số 3 (21+9+1=31 → cần 33)
        level.tiles.Add(new LevelTileData { layerIndex = 0, x = 5, y = 2 });
        level.tiles.Add(new LevelTileData { layerIndex = 0, x = 5, y = 3 });

        SaveLevel(level, "Level_02_Cross");
    }

    // ─── Level 3: Hình Thoi ───
    // Layer 0: hình thoi từ 5x5
    // Layer 1: 3x3 = 9 tiles
    // Layer 2: 2x2 = 4 tiles
    // Layer 3: 1x1 = 1 tile
    // Tổng: 13+9+4+1 = 27 ✅
    static void CreateLevel3(ItemData[] items)
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        level.levelName = "Level 3 - Hinh Thoi";
        level.tiles = new List<LevelTileData>();

        // Layer 0: hình thoi (Manhattan distance <= 2 từ tâm 2,2)
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (Mathf.Abs(x - 2) + Mathf.Abs(y - 2) <= 2)
                    level.tiles.Add(new LevelTileData { layerIndex = 0, x = x, y = y });
            }
        }

        AddGrid(level, layerIndex: 1, cols: 3, rows: 3, items);
        AddGrid(level, layerIndex: 2, cols: 2, rows: 2, items);
        AddGrid(level, layerIndex: 3, cols: 1, rows: 1, items);

        SaveLevel(level, "Level_03_Diamond");
    }

    // ─── Helper: thêm grid đều vào level ───
    static void AddGrid(LevelData level, int layerIndex, int cols, int rows, ItemData[] items)
    {
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                level.tiles.Add(new LevelTileData { layerIndex = layerIndex, x = x, y = y });
    }

    // ─── Helper: lưu file asset ───
    static void SaveLevel(LevelData level, string fileName)
    {
        string path = $"Assets/Data/{fileName}.asset";

        // Tạo folder nếu chưa có
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");

        AssetDatabase.CreateAsset(level, path);
        Debug.Log($"Đã tạo: {path} ({level.tiles.Count} tiles)");
    }
}