using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using System;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("References")]
    [SerializeField] private GameObject _checkpointPrefab;
    [SerializeField] private AbstractMap _map;
    [SerializeField] private Transform _playerTarget;

    [Header("Settings")]
    [SerializeField] private float _spawnRadius = 50f;
    [SerializeField] private int _maxCheckpoints = 10;
    [SerializeField] private float _respawnTime = 300f;
    [SerializeField] private float _checkpointHeightOffset = 1f;

    private List<GameObject> _activeCheckpoints = new List<GameObject>();
    private Vector2d _currentPlayerLocation;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (_map == null) _map = FindObjectOfType<AbstractMap>();
        if (_playerTarget == null) _playerTarget = GameObject.Find("PlayerTarget").transform;
    }

    void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        var locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;

        int maxWait = 20; // 20 seconds timeout
        while (locationProvider.CurrentLocation.IsLocationUpdated || !IsLocationFresh(locationProvider.CurrentLocation))
        {
            yield return new WaitForSeconds(1);
            maxWait--;

            if (maxWait < 0)
            {
                Debug.LogError("Location service initialization timed out");
                yield break;
            }
        }

        InvokeRepeating(nameof(UpdateCheckpoints), 5f, 30f);
    }

    private bool IsLocationFresh(Location location, int maxSecondsOld = 10)
    {
        // Check for default/uninitialized location
        if (location.LatitudeLongitude.x == 0 && location.LatitudeLongitude.y == 0)
            return false;

        DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds((long)location.Timestamp);
        TimeSpan difference = DateTime.UtcNow - timestamp.UtcDateTime;

        return difference.TotalSeconds < maxSecondsOld;
    }

    private void UpdateCheckpoints()
    {
        _currentPlayerLocation = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation.LatitudeLongitude;
        RemoveDistantCheckpoints();
        SpawnNewCheckpoints();
    }

    private void SpawnNewCheckpoints()
    {
        while (_activeCheckpoints.Count < _maxCheckpoints)
        {
            Vector2d randomPosition = GenerateRandomPosition();
            SpawnCheckpoint(randomPosition);
        }
    }

    private Vector2d GenerateRandomPosition()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * _spawnRadius;
        return new Vector2d(
            _currentPlayerLocation.x + randomOffset.y / 111319.5f,
            _currentPlayerLocation.y + randomOffset.x / (111319.5f * Mathf.Cos((float)_currentPlayerLocation.x))
        );
    }

    private void SpawnCheckpoint(Vector2d position)
    {
        Vector3 worldPosition = _map.GeoToWorldPosition(position, true);
        worldPosition.y = _playerTarget.position.y + _checkpointHeightOffset;

        GameObject checkpoint = Instantiate(_checkpointPrefab, worldPosition, Quaternion.identity);
        checkpoint.GetComponent<Checkpoint>().Initialize(position);
        _activeCheckpoints.Add(checkpoint);
    }

    private void RemoveDistantCheckpoints()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var checkpoint in _activeCheckpoints)
        {
            Vector2d checkpointPosition = checkpoint.GetComponent<Checkpoint>().Position;
            double distance = Vector2d.Distance(_currentPlayerLocation, checkpointPosition) * 1000;

            if (distance > _spawnRadius * 2)
            {
                toRemove.Add(checkpoint);
            }
        }

        foreach (var checkpoint in toRemove)
        {
            _activeCheckpoints.Remove(checkpoint);
            Destroy(checkpoint);
        }
    }

    public void CollectCheckpoint(GameObject checkpoint)
    {
        _activeCheckpoints.Remove(checkpoint);
        Destroy(checkpoint);
        PointsManager.Instance.AddPoints(10);
        StartCoroutine(RespawnCheckpointAfterDelay());
    }

    private IEnumerator RespawnCheckpointAfterDelay()
    {
        yield return new WaitForSeconds(_respawnTime);
        SpawnNewCheckpoints();
    }

    public Vector3 GetPlayerWorldPosition()
    {
        return _playerTarget.position;
    }
}