using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "TripleMatch/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public Color color = Color.white;
}