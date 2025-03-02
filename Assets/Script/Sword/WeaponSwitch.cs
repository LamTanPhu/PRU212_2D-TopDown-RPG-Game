using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer; // Gán SpriteRenderer của vũ khí
    public Sprite[] weaponSprites = new Sprite[6]; // Mảng chứa 6 hình vũ khí
    public GameObject bow;
    public GameObject weaponHolder;

    private void Start()
    {
        if (weaponSpriteRenderer == null)
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Mặc định chọn vũ khí đầu tiên
        ChangeWeapon(0);
    }

    public void ChangeWeapon(int index)
    {
        if (index >= 0 && index < weaponSprites.Length)
        {
            if (weaponSprites[index] != null)
            {
                weaponSpriteRenderer.sprite = weaponSprites[index];
                Debug.Log("Đã đổi vũ khí sang: " + index);
            }

            // Nếu chọn ô số 5 (index = 4) → Bật cung, tắt cận chiến
            if (index == 4)
            {
                bow.SetActive(true);
                weaponHolder.SetActive(false);
                Debug.Log("🔫 Cung đã được chọn! Cận chiến bị vô hiệu hóa.");
            }
            else // Nếu chọn ô khác → Bật cận chiến, tắt cung
            {
                bow.SetActive(false);
                weaponHolder.SetActive(true);
                Debug.Log("⚔ Cận chiến đã được chọn! Cung bị vô hiệu hóa.");
            }
        }
        else
        {
            Debug.LogWarning("Vũ khí không hợp lệ hoặc chưa có hình ảnh.");
        }
    }
}
