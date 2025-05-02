using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{

    [Header("References")]
    public GameObject waypointPrefab; // Assign your Waypoint prefab in the Inspector
    public Transform actor;         // Assign the player/actor Transform in the Inspector

    [Header("Spawning Settings")]
    [Tooltip("Maximum distance from the actor where waypoints can spawn.")]
    public float spawnRadius = 50f;
    [Tooltip("Maximum number of waypoints allowed to exist at once.")]
    public int maxWaypoints = 5;
    [Tooltip("How often (in seconds) the manager tries to spawn a new waypoint if below max.")]
    public float spawnInterval = 2.0f;

    // Internal list to keep track of active waypoints
    private List<GameObject> activeWaypoints = new List<GameObject>();
    private Coroutine spawnCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waypointPrefab == null)
        {
            Debug.LogError("Waypoint Prefab not assigned in WaypointManager!");
            this.enabled = false; // Disable script if prefab is missing
            return;
        }
        if (actor == null)
        {
            Debug.LogError("Actor Transform not assigned in WaypointManager!");
            this.enabled = false; // Disable script if actor is missing
            return;
        }

        // Start the spawning process
        spawnCoroutine = StartCoroutine(SpawnWaypointRoutine());
    }

    private IEnumerator SpawnWaypointRoutine()
    {
        while (true) // Loop indefinitely
        {
            // Check if we need to spawn more waypoints
            if (activeWaypoints.Count < maxWaypoints)
            {
                SpawnWaypoint();
            }
            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnWaypoint()
    {
        if (actor == null || waypointPrefab == null) return;

        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        Vector3 spawnPosition = actor.position + randomDirection;
        spawnPosition.y = 0; // Ensure the waypoint spawns at ground level

        // Instantiate the waypoint prefab
        GameObject newWaypoint = Instantiate(waypointPrefab, spawnPosition, Quaternion.identity);

        // Add the new waypoint to our tracking list
        activeWaypoints.Add(newWaypoint);

        // Optional: Pass reference to this manager to the waypoint script
        Waypoint wpScript = newWaypoint.GetComponent<Waypoint>();
        if (wpScript != null)
        {
            wpScript.manager = this;
        }
    }

    // Called by the player model
    public void WaypointCollected(GameObject collectedWaypoint)
    {
        if (activeWaypoints.Contains(collectedWaypoint))
        {
            activeWaypoints.Remove(collectedWaypoint);
            Debug.Log($"Waypoint collected. Remaining: {activeWaypoints.Count}");
        }
        else
        {
            Debug.LogWarning("WaypointCollected called for a waypoint not in the active list.", collectedWaypoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        // Stop the coroutine if this manager object is destroyed
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }
}
