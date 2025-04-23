using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Prefab & UI Parent")]
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] Transform contentParent;

    // You could load this from Supabase, or define in inspector:
    [SerializeField] List<SkinData> allSkins;

    async void Start()
    {
        allSkins = await SupabaseManager.GetSkinsAsync();

        PopulateShop();
    }

    private void PopulateShop()
    {
        foreach (var skin in allSkins)
        {
            var go = Instantiate(shopItemPrefab, contentParent);
            var ui = go.GetComponent<ShopItemUI>();

            ui.Setup(skin, () => OnBuyButton(skin));
        }
    }

    private void OnBuyButton(SkinData skin)
    {
        Debug.Log($"Attempting to buy {skin.DisplayName} for {skin.Price} points");
        // TODO: Check player points, call your purchase logic, update Supabase, etc.
    }
}
