using Supabase.Gotrue.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class ErrorPopup: MonoBehaviour
{
    public TextMeshProUGUI errorText;
    public Button dismissButton;

    private void ShowError(string message)
    {
        errorText.text = message;
        dismissButton.onClick.AddListener(Dismiss);
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }

    public static void Show(string message)
    {
        GameObject errorPopupPrefab = Resources.Load<GameObject>("prefabs/ErrorPopup");
        GameObject errorPopup = Instantiate(errorPopupPrefab);
        errorPopup.transform.SetParent(GameObject.Find("Canvas").transform, false);
        ErrorPopup errorPopupScript = errorPopup.GetComponent<ErrorPopup>();
        errorPopupScript.ShowError(message);
    }

    public static void Show(GotrueException ex)
    {
        string friendlyMessage = string.Empty;

        try
        {
            var jsonDoc = JsonDocument.Parse(ex.Message);
            if (jsonDoc.RootElement.TryGetProperty("msg", out var msgElement))
            {
                friendlyMessage = msgElement.GetString();
            }
        }
        catch (JsonException jsonEx)
        {
            Debug.LogError($"JSON parsing error: {jsonEx.Message}");
            friendlyMessage = "An unexpected error occurred.";
        }

        if (string.IsNullOrEmpty(friendlyMessage))
        {
            friendlyMessage = "An unexpected error occurred.";
        }

        Show(friendlyMessage);
    }
}