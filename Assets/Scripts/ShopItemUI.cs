using Assets.Scripts;
using TMPro;
using Unity.Android.Gradle.Manifest;
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
    public void Setup(SkinData data, UnityEngine.Events.UnityAction onBuyClicked)
    {
        _data = data;
        previewImage.sprite = _data.PreviewSprite;
        nameText.text = _data.DisplayName;
        priceText.text = _data.Price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(onBuyClicked);

        UpdateUI();

    }

    // Can afford, show button with "buy"
    // Can't afford, show cost and hide button
    // Owns, show button with "equip"
    // Eqipped, show button with "equipped"

    private void UpdateUI()
    {
        switch (_data.Status)
        {
            case SkinStatus.Purchased:
                buyButton.GetComponentInChildren<TMP_Text>().text = "Equip";
                pointsContainer.SetActive(false);
                break;
            case SkinStatus.Equipped:
                buyButton.gameObject.SetActive(false);
                buyButton.GetComponentInChildren<TMP_Text>().text = "Equipped";
                pointsContainer.SetActive(false);
                break;
            case SkinStatus.Locked:
                if(PointsManager.Instance.GetCurrentPoints() >= _data.Price)
                {
                    buyButton.GetComponentInChildren<TMP_Text>().text = "Buy";
                    pointsContainer.SetActive(false);
                }
                else
                {
                    buyButton.gameObject.SetActive(false);
                    pointsContainer.SetActive(true);
                }
                break;
        }
    }

    public void UpdateStatus(SkinStatus status)
    {
        _data.Status = status;
        UpdateUI();
    }
}
