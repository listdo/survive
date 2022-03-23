using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public InventoryItem inventoryItem { get; set; }

    private Sprite defaultSprite;
    void Awake()
    {
        Texture2D texture = new Texture2D(128, 128);
        defaultSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    void OnGUI()
    {
        // TODO UNStupid this pls
        if (inventoryItem != null)
        {
            this.transform.GetComponent<Image>().sprite = inventoryItem.ItemImage;
            this.transform.GetComponent<Button>().interactable = true;
        }
        else
        {
            this.transform.GetComponent<Image>().sprite = defaultSprite;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }

    public void RemoveFromSlot()
    {
        Player.Instance.inventory.AddItem(inventoryItem);
        this.inventoryItem = null;
    }
}
