using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite chestClose;

    public ItemData chestItems; // Danh sách vật phẩm trong rương
    public ItemData inventoryData; // Inventory của Player
   
    private bool isPlayerNear = false;
    private bool isOpened = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (isPlayerNear && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    private void OpenChest()
    {
        isOpened = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = chestClose;

        foreach (Item item in chestItems.items)
        {
            inventoryData.AddItem(item);
        }

        Debug.Log("📦 Rương đã mở! Vật phẩm đã được thêm vào Inventory.");

        GetComponent<Collider2D>().enabled = false; // Vô hiệu hóa Collider
        this.enabled = false; // Tắt Script sau khi mở rương
    }
}
