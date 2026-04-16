using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DealController : MonoBehaviour
{
    DataManager dataManager;
    public GameObject[] _objDeals;
    public Sprite _iconKey;
    public Sprite[] _iconBtn;

    // List Check
    //public List<int> PiecePlayer;
    private bool TypeKey;
    [SerializeField] private Sprite _iconNotUp , _iconUp;

    private void OnEnable()
    {
        dataManager = DataManager.Instance;
        CheckAndSaveDeals();
    }
    void CheckAndSaveDeals()
    {
        string savedDate = PlayerPrefs.GetString("SavedDateDeals", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrEmpty(savedDate))
        {
            PlayerPrefs.SetString("SavedDateDeals", today);
            PlayerPrefs.Save();
            RefreshDeals();
            SetDeals();
        }
        else
        {
            System.DateTime savedDateTime = System.DateTime.Parse(savedDate);
            System.DateTime todayDateTime = System.DateTime.Parse(today);
            if (todayDateTime > savedDateTime)
            {
                PlayerPrefs.SetString("SavedDateDeals", today);
                PlayerPrefs.Save();
                RefreshDeals();
            }
            SetDeals();
        }
    }
    public void BtnRestock()
    {
        AudioBase.Instance.SetAudioUI(0);
        if (PlayerPrefs.GetInt("Diamont") >= 5)
        {
            AudioBase.Instance.SetAudioUI(1);
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 5);
            MainManager.Instance.SetMission(0, 5);
            MainManager.Instance.SetTopBar();
            RefreshDeals();
            SetDeals();
        }
    }
    private void RefreshDeals()
    {
        dataManager.dealsCurrent.Clear();
        dataManager.questsCurrent.checkQuest.Clear();
        TypeKey = false;
        for (int i = 0; i < _objDeals.Length; i++)
        {
            int idType = RandomType(i);
            DealsCurrent deals = new DealsCurrent
            {
                IdType = idType,
                IdPiece = RandomIdPiece(idType),
                Quantily = 0,
                PriceDeal = SetPrice(idType),
                TypePrice = SetTypePrice(idType)
            };
            dataManager.dealsCurrent.Add(deals);
        }
        DataManager.Instance.SaveFile();
    }
    private int RandomType(int id)
    {
        int type = Random.Range(0, 4); // 0: Player Piece, 1: Evolve Piece, 2: Key, 3: Item
        
        // Ensure at least one Key (Type 2)
        if (id == _objDeals.Length - 1 && !TypeKey)
        {
            TypeKey = true;
            return 2;
        }

        if (type == 2)
        {
            if (TypeKey) // If already have a Key, pick another type
                return (Random.Range(0, 3) + (Random.Range(0, 2) == 0 ? 0 : 1)) % 4; // pick 0, 1, or 3
            else
                TypeKey = true;
        }
        
        return type;
    }
    private int RandomIdPiece(int idType)
    {
        var db = dataManager.dataBase.imgEquipItems;
        switch (idType)
        {
            case 0: // Player Piece
                return Random.Range(0, Mathf.Min(3, db.sprPiecePlayerLevelUp.Count));
            case 1: // Evolve Piece
                return Random.Range(0, db.sprPiecePlayerEvolve.Count);
            case 3: // Item
                return Random.Range(0, db.sprItem.Count);
            default: // Key
                return 0;
        }
    }
    private int SetPrice(int idType)
    {
        switch (idType)
        {
            case 0: return 125; // Player Piece
            case 1: return 175; // Evolve Piece
            case 2: return 20;  // Key (Gem)
            case 3: return 150; // Item
            default: return 0;
        }
    }
    private bool SetTypePrice(int idType)
    {
        return idType == 2; // Only Key uses gems (true)
    }
    private void SetDeals()
    {
        for (int i = 0; i < _objDeals.Length; i++)
        {
            DealsCurrent deals = dataManager.dealsCurrent[i];
            _objDeals[i].transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = SetImgIcon(deals);//icon deal
            _objDeals[i].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = SetName(deals); //name dael
            
            int maxQty = deals.IdType == 2 ? 1 : 3;
            _objDeals[i].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount((float)deals.Quantily / (float)maxQty, 0); //fill amount
            _objDeals[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = deals.Quantily + "/" + maxQty; //text quantily
            _objDeals[i].transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = _iconNotUp; //icon not up
            
            if (deals.Quantily < maxQty)
            {
                _objDeals[i].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true; // button buy active
                SetBtnBuy(deals, _objDeals[i].transform.GetChild(3).gameObject);
                int id = i;
                bool check = deals.TypePrice;
                int price = CalculateCurrentPrice(deals);
                _objDeals[i].transform.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
                _objDeals[i].transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnBuy(id, check, price);
                });
            }
            else
            {
                _objDeals[i].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false; // button buy deactive
                _objDeals[i].transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = _iconUp; //icon up
            }
        }
    }
    private int CalculateCurrentPrice(DealsCurrent deal)
    {
        switch (deal.IdType)
        {
            case 0: return 125 + (35 * deal.Quantily); // Player Piece: 125 base, +35 per buy
            case 1: return 175 + (55 * deal.Quantily); // Evolve Piece: 175 base, +55 per buy
            case 2: return 20;  // Key: 20 gem, max 1 (no increase)
            case 3: return 150 + (45 * deal.Quantily); // Item: 150 base, +45 per buy
            default: return 0;
        }
    }
    private Sprite SetImgIcon(DealsCurrent deal)
    {
        var db = dataManager.dataBase.imgEquipItems;
        switch (deal.IdType)
        {
            case 0: return db.sprPiecePlayerLevelUp[deal.IdPiece];
            case 1: return db.sprPiecePlayerEvolve[deal.IdPiece];
            case 3: return db.sprItem[deal.IdPiece];
            default: return _iconKey; // Type 2
        }
    }
    private string SetName(DealsCurrent deal)
    {
        int maxQty = deal.IdType == 2 ? 1 : 3;
        if (deal.Quantily >= maxQty)
        {
            return "<color=red>Sold out!</color>";
        }
        else
        {
            var db = dataManager.dataBase.imgEquipItems;
            switch (deal.IdType)
            {
                case 0: return db.namePlayer[deal.IdPiece];
                case 1: return "Evolve Piece"; // You might want to add nameEvolve to db later
                case 3: return db.nameItems[deal.IdPiece];
                default: return "Key"; // Type 2
            }
        }
    }
    private void SetBtnBuy(DealsCurrent deal, GameObject btn)
    {
        int price = CalculateCurrentPrice(deal);
        if (!deal.TypePrice)
        {
            btn.transform.GetChild(0).GetComponent<Image>().sprite = _iconBtn[0]; // Coin icon
            btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString();
        }
        else
        {
            btn.transform.GetChild(0).GetComponent<Image>().sprite = _iconBtn[1]; // Gem icon
            btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString();
        }
    }
    public void BtnBuy(int id, bool check, int price)
    {
        AudioBase.Instance.SetAudioUI(0);
        int money = !check ? PlayerPrefs.GetInt("Coin") : PlayerPrefs.GetInt("Diamont");
        string keyPrice = !check ? "Coin" : "Diamont";
        if (money >= price)
        {
            AudioBase.Instance.SetAudioUI(1);
            if (keyPrice == "Diamont") MainManager.Instance.SetMission(0, price);
            else MainManager.Instance.SetMission(1, price);
            PlayerPrefs.SetInt(keyPrice, PlayerPrefs.GetInt(keyPrice) - price);
            dataManager.dealsCurrent[id].Quantily++;
            SetDataEquip(id);
            dataManager.SaveFile();
            MainManager.Instance.SetTopBar();
            SetDeals();
        }
    }
    private void SetDataEquip(int id)
    {
        DealsCurrent deal = dataManager.dealsCurrent[id];
        int type = deal.IdType;
        int idPiece = deal.IdPiece;

        switch (type)
        {
            case 0: // Player Piece
                dataManager.warehouse.CountPiecePlayerLevelUp[idPiece]++;
                break;
            case 1: // Evolve Piece
                dataManager.warehouse.CountPiecePlayerEvolve[idPiece]++;
                break;
            case 2: // Key
                PlayerPrefs.SetInt("Key", PlayerPrefs.GetInt("Key") + 1);
                PlayerPrefs.Save();
                break;
            case 3: // Item
                dataManager.warehouse.CountItem[idPiece]++;
                if (!dataManager.warehouse.ListItems.Contains(idPiece))
                    dataManager.warehouse.ListItems.Add(idPiece);
                break;
        }
    }
}
