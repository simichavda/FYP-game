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
            Debug.Log($"Adding skin: {skin.DisplayName} with price:  {skin.Price}");
            ui.Setup(skin, () => OnBuyButton(skin, ui));
        }
    }

    private async void OnBuyButton(SkinData skin, ShopItemUI ui)
    {
        // Debug.Log($"Attempting to buy {skin.DisplayName} for {skin.Price} points");
        // TODO: Check player points, call your purchase logic, update Supabase, etc.


        Debug.Log($"Clicked on skin: {skin.DisplayName}");
        switch(skin.Status)
        {
            case SkinStatus.Purchased:
                // Equip the skin
                Debug.Log($"Equipping skin: {skin.DisplayName}");
                if(!await SupabaseManager.EquipSkin(skin.DisplayName))
                {
                    Debug.Log($"Failed to equip skin: {skin.DisplayName}");
                    break;
                }
                InventoryManager.Instance.SetSelectedSkin(skin);
                skin.Status = SkinStatus.Equipped;
                ui.UpdateStatus(SkinStatus.Equipped);
                break;
            case SkinStatus.Equipped:
                // Already equipped; the button should not be visible anyway but just in case
                Debug.Log($"Skin {skin.DisplayName} is already equipped.");
                break;
            case SkinStatus.Locked:
                // Check if player has enough points
                int playerPoints = PointsManager.Instance.GetCurrentPoints();
                if (playerPoints >= skin.Price)
                {
                    // Have user confirm the purchase
                    ConfirmPopup.Show($"Are you sure you want to buy {skin.DisplayName} for {skin.Price} points?", async () =>
                    {
                        // Deduct points and mark skin as purchased
                        PointsManager.Instance.AddPoints(-skin.Price);
                        skin.Status = SkinStatus.Purchased;
                        await SupabaseManager.AddSkin(skin);
                        Debug.Log($"Purchased skin: {skin.DisplayName}");
                        ui.UpdateStatus(SkinStatus.Purchased);
                    });
                }
                else
                {
                    Debug.Log($"Not enough points to buy {skin.DisplayName}. Required: {skin.Price}, Available: {playerPoints}");
                    ErrorPopup.Show($"Not enough points to buy {skin.DisplayName}. Required: {skin.Price}, Available: {playerPoints}");
                }
                break;
        }
    }
}
