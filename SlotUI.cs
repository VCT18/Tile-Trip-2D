using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public ItemData itemData;
    private bool isEmpty = true;

    public bool IsEmpty => isEmpty;

    public void SetItem(ItemData data)
    {
        itemData = data;
        isEmpty = false;
        icon.sprite = data.sprite;
        icon.color = data.color;
    }

    public void Clear()
    {
        itemData = null;
        isEmpty = true;
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
    }
}