using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot: MonoBehaviour
{
    [SerializeField] public CraftingRecipe recipe;

    private Sprite defaultSprite;

    void Awake()
    {
        Texture2D texture = new Texture2D(128, 128);
        defaultSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    void OnGUI()
    {
        // TODO UNStupid this pls
        if (recipe != null)
        {
            this.transform.GetChild(0).GetComponent<Image>().sprite = recipe?.craftedItem.ItemImage;
            this.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
        else
        {
            this.transform.GetChild(0).GetComponent<Image>().sprite = defaultSprite;
            this.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }

    public void CraftItem()
    {
        recipe.CraftItem();
    }
}
