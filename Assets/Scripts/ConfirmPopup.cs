using Supabase.Gotrue.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

class ConfirmPopup: MonoBehaviour
{
    public TextMeshProUGUI msgText;
    public Button dismissButton;
    public Button confirmButton;

    private void ShowMessage(string message, UnityAction onConfirm)
    {
        msgText.text = message;
        dismissButton.onClick.AddListener(Dismiss);
        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Dismiss();
        });
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }


    public static void Show(string message, UnityAction onConfirm)
    {
        GameObject confirmPopupPrefab = Resources.Load<GameObject>("prefabs/ConfirmPopup");
        GameObject confirmPopup = Instantiate(confirmPopupPrefab);
        confirmPopup.transform.SetParent(GameObject.Find("Canvas").transform, false);
        ConfirmPopup confirmPopupScript = confirmPopup.GetComponent<ConfirmPopup>();
        confirmPopupScript.ShowMessage(message, onConfirm);
    }
}