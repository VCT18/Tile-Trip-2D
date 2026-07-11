using System;
using System.Collections.Generic;

[Serializable]
public class LevelTileData
{
    public string itemName;  // rỗng = random
    public int x;
    public int y;
    public int layerIndex;
}

[Serializable]
public class LevelData
{
    public int levelId;
    public float timeLimit;
    public int maxSlots;
    public List<LevelTileData> tiles;
}
