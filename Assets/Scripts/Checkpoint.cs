using UnityEngine;
using Mapbox.Utils;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float _collectionRadius = 5f;
    [SerializeField] private GameObject _collectionEffect;

    public Vector2d Position { get; private set; }
    private bool _collected = false;

    public void Initialize(Vector2d position)
    {
        Position = position;
        _collected = false;
    }

    void Update()
    {
        if (!_collected && IsPlayerInRange())
        {
            Collect();
        }
    }

    private bool IsPlayerInRange()
    {
        if (CheckpointManager.Instance == null || CheckpointManager.Instance.GetPlayerWorldPosition() == null)
            return false;

        float distance = Vector3.Distance(
            transform.position,
            CheckpointManager.Instance.GetPlayerWorldPosition()
        );

        return distance <= _collectionRadius;
    }

    private void Collect()
    {
        _collected = true;
        if (_collectionEffect != null)
        {
            Instantiate(_collectionEffect, transform.position, Quaternion.identity);
        }
        CheckpointManager.Instance.CollectCheckpoint(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _collectionRadius);
    }
}