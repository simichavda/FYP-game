using UnityEngine;
using Supabase;
using Assets.Scripts;

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
        var activeSkin = await SupabaseManager.GetActiveSkin();

        if (!string.IsNullOrEmpty(activeSkin))
        {
            // Load texture from Resources
            var skinTexture = Resources.Load<Texture>($"Textures/{activeSkin}");

            if (skinTexture != null)
            {
                targetRenderer.material.mainTexture = skinTexture;
            }
            else
            {
                Debug.LogError($"Failed to load texture: {activeSkin}");
            }
        }
    }
}