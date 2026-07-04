using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewLevel", menuName = "TripleMatch/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public List<LevelTileData> tiles = new List<LevelTileData>();

    // Lấy width/height của từng layer
    public Vector2Int GetLayerSize(int layerIndex)
    {
        int maxX = 0, maxY = 0;
        foreach (LevelTileData t in tiles)
        {
            if (t.layerIndex != layerIndex) continue;
            if (t.x > maxX) maxX = t.x;
            if (t.y > maxY) maxY = t.y;
        }
        return new Vector2Int(maxX + 1, maxY + 1);
    }
}