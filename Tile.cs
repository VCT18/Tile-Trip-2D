using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public ItemData itemData;
    public List<Tile> blockingTiles = new List<Tile>();
    public bool isLocked = true;
    public bool isDestroyed = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    public int layerIndex;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Setup(ItemData data, int sortingOrder)
    {
        itemData = data;
        spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = data.color;
        spriteRenderer.sortingOrder = sortingOrder;
        UpdateVisual();
    }

    public void SetBlockingTiles(List<Tile> blockers)
    {
        blockingTiles = blockers;
    }

    void UpdateVisual()
    {
        if (isLocked)
        {
            Color darkenedColor = itemData.color * 0.6f; 
            darkenedColor.a = 1f;
            spriteRenderer.color = darkenedColor;
            boxCollider.enabled = false;
        }
        else
        {
            spriteRenderer.color = itemData.color;
            boxCollider.enabled = true;
        }
    }

    public void UnlockTile()
    {
        isLocked = false;
        UpdateVisual();
    }

    void OnMouseDown()
    {
        Debug.Log($"Clicked on {gameObject.name}, isLocked={isLocked}");
        if (isLocked) return;
        GameManager.Instance.SelectTile(this);
    }
}


