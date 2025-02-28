using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer; // Gán SpriteRenderer của vũ khí
    public Sprite[] weaponSprites = new Sprite[6]; // Mảng chứa 6 hình vũ khí

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
        if (index >= 0 && index < weaponSprites.Length && weaponSprites[index] != null)
        {
            weaponSpriteRenderer.sprite = weaponSprites[index];
            Debug.Log("Đã đổi vũ khí sang: " + index);
        }
        else
        {
            Debug.LogWarning("Vũ khí không hợp lệ hoặc chưa có hình ảnh.");
        }
    }
}
