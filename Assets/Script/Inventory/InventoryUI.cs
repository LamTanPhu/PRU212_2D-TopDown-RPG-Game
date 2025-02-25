using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public ItemData itemData;
    public Transform itemGrid; // Gán `Content` của ScrollView vào đây
    public GameObject itemSlotPrefab;
    public GameObject inventoryPanel;
    public Button closeButton;
    public ScrollRect scrollRect; // Thêm ScrollRect

    private void Start()
    {
        if (inventoryPanel == null)
            Debug.LogError("⚠ InventoryPanel chưa được gán trong Inspector!");
        inventoryPanel.SetActive(false);
        closeButton.onClick.AddListener(ToggleInventory);
        LoadInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            LoadInventory();
            ResetScrollPosition(); // Đưa Scroll lên đầu
        }
    }

    public void LoadInventory()
    {
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in itemData.items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemGrid);
            slot.GetComponent<ItemSlot>().SetItem(item);
        }
    }

    private void ResetScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f; // Đưa về đầu danh sách
    }
}
