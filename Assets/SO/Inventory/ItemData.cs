using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public List<Item> items = new List<Item>(); // Danh sách tất cả vật phẩm

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public Item GetItemByName(string name)
    {
        return items.Find(item => item.itemName == name);
    }
}
