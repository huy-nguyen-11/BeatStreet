using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ShopItemView : MonoBehaviour
{
    [Header("Text in Inspector")]
    [SerializeField] private TextMeshProUGUI _tmpItemName;
    [SerializeField] private TextMeshProUGUI _tmpItemPrice;
    [SerializeField] private TextMeshProUGUI _tmpQuantityReceived;

    public TextMeshProUGUI TmpItemName => _tmpItemName;
    public TextMeshProUGUI TmpItemPrice => _tmpItemPrice;
    public TextMeshProUGUI TmpQuantityReceived => _tmpQuantityReceived;

    public void SetItemName(string value)
    {
        if (_tmpItemName != null)
            _tmpItemName.text = value ?? string.Empty;
    }

    public void SetItemPrice(string value)
    {
        if (_tmpItemPrice != null)
            _tmpItemPrice.text = value ?? string.Empty;
    }

    public void SetQuantityReceived(string value)
    {
        if (_tmpQuantityReceived != null)
            _tmpQuantityReceived.text = $"+{value}" ?? string.Empty;
    }

    public void SetItemPrice(int price) => SetItemPrice(price.ToString());

    public void SetQuantityReceived(int amount) => SetQuantityReceived(amount.ToString());

    public void SetAll(string itemName, string priceText, string quantityText)
    {
        SetItemName(itemName);
        SetItemPrice(priceText);
        SetQuantityReceived(quantityText);
    }

    public void SetAll(string itemName, int price, int quantityReceived)
    {
        SetItemName(itemName);
        SetItemPrice(price);
        SetQuantityReceived(quantityReceived);
    }
}
