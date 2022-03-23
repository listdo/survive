using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] 
    private short InventoryDimension = 25;

    [SerializeField] private Canvas inventoryCanvas;

    public InventoryItem[] InventorySlots { get; set; }

    public bool IsOpen = false;

    // Start is called before the first frame update
    void Awake()
    {
        this.InventorySlots = new InventoryItem[InventoryDimension];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGUI()
    {
        inventoryCanvas.enabled = IsOpen;
    }

    public void AddItem(InventoryItem item)
    {
        if (item == null)
            return;

        for (var i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i] == null)
            {
                InventorySlots[i] = (from dbEntry in ItemDatabase.Instance.Items
                                        where dbEntry.ItemId == item?.ItemId
                                        select dbEntry).First();

                Debug.Log($"=== Added Item: {item.Name}");
                return;
            }
        }
    }

    public void ToggleInventory()
    {
        IsOpen = !IsOpen;

        Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsOpen;
    }

    public void RemoveItem(int index)
    {
        this.InventorySlots[index] = null;
    }

    public void DropItem(int index)
    {
        var item = Instantiate(this.InventorySlots[index].Prefab, Player.Instance.transform);
        item.transform.SetParent(null);
    }

    public int ItemCount()
    {
        return (from item in this.InventorySlots
                    where item != null
                    select item).Count();
    }
}
