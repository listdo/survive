using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] public short ItemId;

    [SerializeField] public string Name;

    [SerializeField] public GameObject Prefab;

    [SerializeField] public Sprite ItemImage;

    [SerializeField] public ItemType ItemType;

    [SerializeField] public float Health = -10.0F;
    [SerializeField] public float Saturation = 10F;
    [SerializeField] public float Hydration = 0F;

    [SerializeField] public float Armor = 0F;

    [SerializeField] public ToolType type;
    [SerializeField] public float gatherModifier;

    public virtual void Use(Player player, InventorySlot inventorySlot)
    {
        switch (ItemType)
        {
            case ItemType.Consumable:
                Consume(player, inventorySlot);
                break;
            case ItemType.Tool:
                Equipe(player, player.mainHand, inventorySlot);
                break;
            case ItemType.Helmet:
                Equipe(player, player.helmet, inventorySlot);
                break;
            case ItemType.Breast:
                Equipe(player, player.breast, inventorySlot);
                break;
            case ItemType.Legs:
                Equipe(player, player.legs, inventorySlot);
                break;
            case ItemType.Boots:
                Equipe(player, player.boots, inventorySlot);
                break;
            case ItemType.Gauntlets:
                Equipe(player, player.gauntlets, inventorySlot);
                break;
        }
    }

    private void Consume(Player player, InventorySlot inventorySlot)
    {
        player.currentHP = Mathf.Clamp(player.currentHP + Health, 0, Player.Instance.maxHP);
        player.currentHydration = Mathf.Clamp(player.currentHydration + Hydration, 0, Player.Instance.maxHydration);
        player.currentSaturation = Mathf.Clamp(player.currentSaturation + Saturation, 0, Player.Instance.maxSaturation);

        inventorySlot.RemoveFromSlot();
    }

    private void Equipe(Player player, EquipmentSlot slot, InventorySlot inventorySlot)
    {
        var currentItem = slot.inventoryItem;
        slot.inventoryItem = this;
        player.inventory.InventorySlots[inventorySlot.Index] = currentItem;
    }
}
