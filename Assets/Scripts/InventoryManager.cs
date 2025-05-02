using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public System.Action OnSkinSelected;

    private SkinData selectedSkin;

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

        // Load the selected skin from Supabase
        LoadSelectedSkin();
    }

    public SkinData GetSelectedSkin()
    {
        return selectedSkin;
    }

    public void SetSelectedSkin(SkinData skin)
    {
        selectedSkin = skin;
        Debug.Log("Selected skin set to: " + selectedSkin.DisplayName);
        OnSkinSelected?.Invoke();
    }

    private async void LoadSelectedSkin()
    {
        // Load the selected skin from Supabase
        selectedSkin = await SupabaseManager.GetActiveSkin();
        Debug.Log("Loaded selected skin: " + selectedSkin.DisplayName);
    }
}