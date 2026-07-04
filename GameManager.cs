using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Board board;
    public CollectionBar collectionBar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectTile(Tile tile)
    {
        Vector3 tileWorldPos = tile.transform.position; // lưu trước khi destroy

        bool success = collectionBar.AddItem(tile.itemData, tileWorldPos);
        if (success)
        {
            tile.isDestroyed = true;
            Destroy(tile.gameObject);
            UnlockTilesIfNeeded();
            if (board.GetRemainingTileCount() == 0)
                Debug.Log("You Win!");
        }
    }

    public void UnlockTilesIfNeeded()
    {
        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile t in allTiles)
        {
            if (t == null || !t.isLocked) continue;

            bool allBlockersGone = true;
            foreach (Tile blocker in t.blockingTiles)
            {
                if (blocker != null && !blocker.isDestroyed)
                {
                    allBlockersGone = false;
                    break;
                }
            }

            if (allBlockersGone)
                t.UnlockTile();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
}