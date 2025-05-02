using Assets.Scripts;
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

        LoadPoints(); 
    }

    public int GetCurrentPoints()
    {
        return _currentPoints;
    }

    public void AddPoints(int amount)
    {
        _currentPoints += amount;
        OnPointsUpdated?.Invoke(_currentPoints);
        SavePoints();
    }

    private async void SavePoints()
    {
        // Will implement with Supabase later
        //PlayerPrefs.SetInt("PlayerPoints", _currentPoints);

        // Save points to Supabase
        await SupabaseManager.setPoints(_currentPoints);
    }

    private async void LoadPoints()
    {
        //_currentPoints = PlayerPrefs.GetInt("PlayerPoints", 0);
        //OnPointsUpdated?.Invoke(_currentPoints);

        // Load points from Supabase
        _currentPoints = await SupabaseManager.getPoints();
        OnPointsUpdated?.Invoke(_currentPoints);
    }
}