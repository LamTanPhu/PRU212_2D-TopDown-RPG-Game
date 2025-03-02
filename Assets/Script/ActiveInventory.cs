using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    private int activeSlotIndexNum = 0;

    private PlayerControls playerControls;
    public WeaponSwitch weaponSwitcher;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Inventory.Keyboard.performed += ctx =>
        {
            int selectedIndex = (int)ctx.ReadValue<float>() - 1;
            ToggleActiveSlot(selectedIndex);
            weaponSwitcher.ChangeWeapon(selectedIndex);
        };
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void ToggleActiveSlot(int indexNum)
    {
        activeSlotIndexNum = indexNum;

        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);
    }
}
