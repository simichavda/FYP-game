using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    private int _currentPoints;

    public System.Action<int> OnPointsUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int amount)
    {
        _currentPoints += amount;
        OnPointsUpdated?.Invoke(_currentPoints);
        SavePoints();
    }

    private void SavePoints()
    {
        // Will implement with Supabase later
        PlayerPrefs.SetInt("PlayerPoints", _currentPoints);
    }

    public void LoadPoints()
    {
        _currentPoints = PlayerPrefs.GetInt("PlayerPoints", 0);
        OnPointsUpdated?.Invoke(_currentPoints);
    }
}