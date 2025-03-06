using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite chestClose;

    public ItemData chestItems; // Danh sách vật phẩm trong rương
    public ItemData inventoryData; // Inventory của Player
   
    private bool isPlayerNear = false;
    private bool isOpened = false;

    AudioManager audioManager;
    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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
        audioManager.PlaySFX(audioManager.chestOpen);
        isOpened = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = chestClose;

        foreach (Item item in chestItems.items)
        {
            inventoryData.AddItem(item);
        }

        GetComponent<Collider2D>().enabled = false; // Vô hiệu hóa Collider
        this.enabled = false; // Tắt Script sau khi mở rương
    }
}
