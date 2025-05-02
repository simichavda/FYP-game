using UnityEngine;
using Supabase;
using Assets.Scripts;


// CLASS NO LONGER USED //
public class PlayerSkinApplier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer targetRenderer; // Assign your 3D model's renderer

    private async void Start()
    {
        LoadAndApplySkin();
    }

    private async void LoadAndApplySkin()
    {
        // Get active skin from Supabase
        SkinData activeSkin = InventoryManager.Instance.GetSelectedSkin();

        if (activeSkin.MeshTexture == null)
        {
            Debug.LogError("No active skin found!");
            return;
        }

        
        targetRenderer.material.mainTexture = activeSkin.MeshTexture;
    }
}