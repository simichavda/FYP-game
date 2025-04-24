using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] Image previewImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Button buyButton;
    [SerializeField] GameObject pointsContainer;

    private SkinData _data;

    /// <summary>
    /// Initialize this UI with the data for a single skin.
    /// </summary>
    public async void Setup(SkinData data, UnityEngine.Events.UnityAction onBuyClicked)
    {
        _data = data;
        previewImage.sprite = data.PreviewSprite;
        nameText.text = data.DisplayName;
        priceText.text = data.Price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(onBuyClicked);



        // Ask Supabase if player already owns this skin and other stuff
        if (await SupabaseManager.DoesPlayerOwnSkin(data.DisplayName))
        {
            buyButton.GetComponentInChildren<TMP_Text>().text = "equip";
            pointsContainer.SetActive(false);
        }
        else 
        {
            buyButton.gameObject.SetActive(false);
            pointsContainer.SetActive(true);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
