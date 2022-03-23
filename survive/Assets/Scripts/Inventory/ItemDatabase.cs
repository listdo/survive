using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    // Singelton
    private static ItemDatabase _instance;
    public static ItemDatabase Instance { get { return _instance; } }

    [SerializeField] public List<InventoryItem> Items;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
