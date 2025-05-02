using Assets.Scripts;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public WaypointManager waypointManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waypointManager == null)
        {
            waypointManager = FindFirstObjectByType<WaypointManager>();
        }

        if (waypointManager == null)
        {
            Debug.LogError("PlayerCharacter could not find WaypointManager in the scene!");
            this.enabled = false;
        }

        RefreshSkin();

        // Subscribe to the event when the skin is changed
        InventoryManager.Instance.OnSkinSelected += RefreshSkin;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSkinSelected -= RefreshSkin;
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        Waypoint waypoint = other.GetComponent<Waypoint>();
        if (waypoint != null)
        {
            // Call the Collect function to handle the waypoint collection
            Collect(other.gameObject);
        }
    }

    void Collect(GameObject waypointObject) // Add 'Waypoint waypointScript' if using component check
    {
        Debug.Log("Collected Waypoint!");

        // Notify the WaypointManager that this waypoint was collected
        if (waypointManager != null)
        {
            waypointManager.WaypointCollected(waypointObject);
        }

        // Destroy the waypoint GameObject
        Destroy(waypointObject);
    }

    public void RefreshSkin()
    {
        // Get the current skin from Supabase
        Texture2D texture = InventoryManager.Instance.GetSelectedSkin().MeshTexture;


        if (texture == null)
        {
            Debug.LogError("No active skin found!");
            return;
        }

        // Apply the texture to the player character's material
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.material != null)
            {
                renderer.material.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Material not found on the renderer!");
            }
        }

    }
}
