using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Ressource : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private LODGroup lod;

    [Header("Collider")]
    [SerializeField] private Collider collider;

    [Header("Requirement")] 
    [SerializeField] private ToolType toolType;

    [Header("Ressource Properties")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isRespawnable = false;
    [SerializeField] private float respawnTime;
    [SerializeField] private float harvestRate = 0.2F;
    [SerializeField] private Animal animal;

    [Header("Items")]
    [SerializeField] private List<Item> harvestableRessources;

    [Header("Ressource Status")] 
    [SerializeField] private bool isHarvestable = true;
    [SerializeField] private bool isDepleted = false;

    private Stopwatch respawnTimer = new Stopwatch();

    void Update()
    {
        RespawnRessource();
    }

    protected virtual void RespawnRessource()
    {
        if (respawnTimer.Elapsed.Seconds >= respawnTime && isRespawnable)
        {
            ToggleRenderer(true);
            isDepleted = false;
        }
    }

    public virtual InventoryItem TakeDamage(float damage, InventoryItem tool)
    {
        if (tool == null)
            return null;

        if (tool.type != toolType)
            return null;

        //tool.currentId++;
        //Debug.Log(tool.currentId);

        Debug.Log("DEAL DAMAGE TO " + this.tag);

        if (animal == null || (animal != null && animal.currentHP <= 0))
        {
            this.currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        }
        else
        {
            if (animal != null && animal.currentHP > 0)
                return null;
        }

        if (isDepleted)
        {
            if(animal != null)
                Destroy(gameObject);
        }

        if (currentHealth <= 0)
        {
            if (isRespawnable)
            {
                respawnTimer.Restart();
                isDepleted = true;
                ToggleRenderer(false);

                return null;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        var rate = Mathf.Abs(Random.value);

        return rate < harvestRate
            ? ItemDatabase.Instance.Items.Find((i) => i?.ItemId == harvestableRessources[Random.Range(0, harvestableRessources.Count)].Id)
            : null;
    }

    protected virtual void ToggleRenderer(bool state)
    {
        if (collider != null)
            collider.enabled = state;

        if (lod != null)
        {
            foreach (var loD in lod.GetLODs())
            {
                foreach (var loDRenderer in loD.renderers)
                {
                    loDRenderer.enabled = state;
                }
            }
        }

        if (gameObject.GetComponent<MeshRenderer>() != null)
            gameObject.GetComponent<MeshRenderer>().enabled = state;

        if (gameObject.GetComponent<Collider>() != null)
            gameObject.GetComponent<Collider>().enabled = state;
    }
}
