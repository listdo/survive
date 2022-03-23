using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int Index;
    public InventoryItem inventoryItem { get; set; }

    public Inventory inventory;

    private Sprite defaultSprite;

    void Awake()
    {
        Texture2D texture = new Texture2D(128, 128);
        defaultSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    void OnGUI()
    {
        inventoryItem = inventory.InventorySlots[Index];

        // TODO UNStupid this pls
        if (inventoryItem != null)
        {
            this.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = inventoryItem.ItemImage;
            this.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
        else
        {
            this.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = defaultSprite;
            this.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
    }

    public void Use()
    {
        inventoryItem?.Use(Player.Instance, this);
    }

    public void RemoveFromSlot()
    {
        this.inventoryItem = null;
        this.inventory.RemoveItem(Index);
    }

    public void Drop()
    {
        this.inventory.DropItem(Index);
        RemoveFromSlot();
    }
}
