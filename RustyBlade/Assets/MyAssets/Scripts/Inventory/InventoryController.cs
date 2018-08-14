using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    public static InventoryController Instance { get; set; }
    public WeaponController playerWeaponController;
    public List<Item> playerItems = new List<Item>();

    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerWeaponController = FindObjectOfType<WeaponController>();
        GiveItem("sword");
    }

    public void GiveItem(string itemSlug)
    {
        Item item = ItemDatabase.Instance.GetItem(itemSlug);
        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    public void GiveItem(Item item)
    {
        playerItems.Add(item);
        UIEventHandler.ItemAddedToInventory(item);
    }

    public void GiveItem(List<Item> items)
    {
        playerItems.AddRange(items);
        UIEventHandler.ItemAddedToInventory(items);
    }

    /* public void SetItemDetails(Item item, Button selectedButton)
    {
        inventoryDetailsPanel.SetItem(item, selectedButton);
    } */

    public void EquipItem(Item itemToEquip)
    {
        playerWeaponController.EquipItem(itemToEquip);
    }

}
