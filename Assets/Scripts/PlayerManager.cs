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

    // Update is called once per frame
    void Update()
    {
        
    }
}
