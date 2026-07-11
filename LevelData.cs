using System;
using System.Collections.Generic;

[Serializable]
public class TileEntry
{
    public string itemName;
    public float x;
    public float y;
    public int layer;
}

[Serializable]
public class LevelData
{
    public int levelId;
    public float timeLimit;
    public int maxSlots;
    public List<TileEntry> tiles;
}
