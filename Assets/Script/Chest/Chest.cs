using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject pressEImage; // Hình ảnh "Nhấn E"
    public ItemData chestItems; // Danh sách vật phẩm trong rương
    public ItemData inventorydata;
    private bool isPlayerNear = false;
    private bool isOpened = false;

    private void Start()
    {
        pressEImage.SetActive(false); // Ẩn nút E ban đầu
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
            pressEImage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            pressEImage.SetActive(false);
        }
    }

    private void OpenChest()
    {
        isOpened = true;
        pressEImage.SetActive(false); // Ẩn nút E sau khi mở

        // Thêm vật phẩm vào Inventory
        foreach (Item item in chestItems.items)
        {
            inventorydata.AddItem(item);
        }

        Debug.Log("Rương đã mở! Tất cả vật phẩm đã được thêm vào Inventory.");

        // Vô hiệu hóa rương sau khi mở
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
