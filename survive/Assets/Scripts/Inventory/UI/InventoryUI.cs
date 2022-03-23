using System;
using UnityEngine;

public class InventoryUI  : MonoBehaviour
{
    private Inventory inventory;

    void Start()
    {
        inventory = this.GetComponentInParent<Inventory>();
    }

    void OnGUI()
    {
        if (!enabled)
            return;

        var slots = GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (inventory.InventorySlots[i] != null)
            {
                slots[i].Index = i;
                slots[i].inventoryItem = inventory.InventorySlots[i];
            }
        }
    }
}

