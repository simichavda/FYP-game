using TMPro;
using UnityEngine;

public class PointsDisplay : MonoBehaviour
{
    private int _currentPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentPoints = PointsManager.Instance.GetCurrentPoints();
        UpdatePointsDisplay(_currentPoints);
        PointsManager.Instance.OnPointsUpdated += UpdatePointsDisplay;
    }

    // UpdatePointsDisplay is called whenever the points are updated
    private void UpdatePointsDisplay(int newPoints)
    {
        _currentPoints = newPoints;
        
        // Update the UI text or any other display element with the new points
        Debug.Log("Updated Points: " + _currentPoints);

        TextMeshProUGUI pointsText = GetComponent<TextMeshProUGUI>();
        if(pointsText == null)
        {
            Debug.LogError("TextMeshPro component not found on the GameObject.");
            return;
        }

        pointsText.text = "" + _currentPoints;
    }

    // OnDestroy is called when the MonoBehaviour will be destroyed
    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.OnPointsUpdated -= UpdatePointsDisplay;
        }
    }
}
