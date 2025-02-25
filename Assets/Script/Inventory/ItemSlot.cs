using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour
{
    public Image itemIcon; // Ảnh hiển thị vật phẩm
    public TextMeshProUGUI itemName; // Tên vật phẩm
    private Item item; // Dữ liệu vật phẩm


    public void SetItem(Item newItem)
    {
        item = newItem;
        itemIcon.sprite = item.icon; // Gán hình ảnh
        itemIcon.enabled = true;
        itemName.text = item.itemName; // Gán tên vật phẩm
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemName.text = "";
    }
}
