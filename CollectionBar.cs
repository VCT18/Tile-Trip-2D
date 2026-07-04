using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectionBar : MonoBehaviour
{
    [Header("Settings")]
    public int maxSlots = 7;
    public SlotUI[] slots;
    public GameObject flyingTilePrefab;
    public Canvas canvas;

    private List<ItemData> collectedItems = new List<ItemData>();
    private int flyingCount = 0;

    public bool AddItem(ItemData item, Vector3 worldPosition)
    {
        if (collectedItems.Count >= maxSlots)
        {
            GameManager.Instance.GameOver();
            return false;
        }

        collectedItems.Add(item);
        SortCollectedItems();

        int targetSlotIndex = collectedItems.IndexOf(item);
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == item)
            {
                targetSlotIndex = i;
                break;
            }
        }

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.worldCamera,
            out Vector2 canvasPos
        );

        Vector2 slotPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, slots[targetSlotIndex].transform.position),
            canvas.worldCamera,
            out slotPos
        );

        RefreshSlotUI();
        slots[targetSlotIndex].icon.color = new Color(1, 1, 1, 0); // ẩn slot chờ animation

        GameObject flyObj = Instantiate(flyingTilePrefab, canvas.transform);
        FlyingTile flyingTile = flyObj.GetComponent<FlyingTile>();

        flyingCount++;

        flyingTile.Fly(item.sprite, item.color, canvasPos, slotPos, () =>
        {
            slots[targetSlotIndex].icon.sprite = item.sprite;
            slots[targetSlotIndex].icon.color = item.color; // hiện khi đáp xuống
            flyingCount--;

            if (flyingCount == 0)
            {
                CheckForMatch(item);
                if (collectedItems.Count >= maxSlots)
                    GameManager.Instance.GameOver();
            }
        });

        return true;
    }

    void SortCollectedItems()
    {
        collectedItems.Sort((a, b) =>
            string.Compare(a.itemName, b.itemName, System.StringComparison.Ordinal)
        );
    }

    bool CheckForMatch(ItemData item)
    {
        int count = collectedItems.FindAll(x => x == item).Count;
        if (count >= 3)
        {
            int removed = 0;
            for (int i = collectedItems.Count - 1; i >= 0; i--)
            {
                if (collectedItems[i] == item && removed < 3)
                {
                    collectedItems.RemoveAt(i);
                    removed++;
                }
            }
            RefreshSlotUI();
            Debug.Log("Match 3! Còn lại: " + collectedItems.Count);
            return true;
        }
        return false;
    }

    void RefreshSlotUI()
    {
        foreach (SlotUI slot in slots)
            slot.Clear();
        for (int i = 0; i < collectedItems.Count; i++)
            slots[i].SetItem(collectedItems[i]);
    }

    public void ClearBar()
    {
        collectedItems.Clear();
        RefreshSlotUI();
    }
}