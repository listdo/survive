using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingRecipe : MonoBehaviour
{
    [SerializeField] private InventoryItem[] craftingMaterials;
    [SerializeField] public InventoryItem craftedItem;

    public void CraftItem()
    {
        if (!CanCraft())
            return;

        RemoveItemsFromInventor();

        Player.Instance.inventory.AddItem(ItemDatabase.Instance.Items.Find(e => e.ItemId == craftedItem.ItemId));
    }

    // TODO: THIS IS A MAJOR SHIT FUNCTION
    public bool CanCraft()
    {
        if (Player.Instance.inventory.ItemCount() < craftingMaterials.Length)
            return false;

        var inventoryItems = Player.Instance.inventory.InventorySlots
            .Select(n => n)
            .Where(n => n != null)
            .GroupBy(n => n)
            .ToDictionary(g => g.Key, g => g.Count());

        Dictionary<short, int> craftingItems = craftingMaterials?
            .Select(n => n)
            .Where(n => n != null)
            .GroupBy(n => n.ItemId)
            .ToDictionary(g => g.Key, g => g.Count());

        int i = 0;

        // TODO: This is total shitcode
        foreach (var craftingItem in craftingItems)
        {
            foreach (var inventoryItem in inventoryItems)
            {
                if (inventoryItem.Key.ItemId == craftingItem.Key)
                {
                    if (inventoryItem.Value >= craftingItem.Value)
                    {
                        i++;
                    }
                }
            }
        }

        return craftingItems.Keys.Count == i;
    }

    private void RemoveItemsFromInventor()
    {
        for (var i = 0; i < craftingMaterials.Length; i++)
        {
            for (var j = 0; j < Player.Instance.inventory.InventorySlots.Length; j++)
            {
                if (i < craftingMaterials.Length && Player.Instance.inventory.InventorySlots[j]?.ItemId == craftingMaterials[i].ItemId)
                {
                    Player.Instance.inventory.RemoveItem(j);
                    i++;
                }
            }
        }
    }
}
