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
    public List<int> PiecePlayer;
    public List<int> PieceEnemy;
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
        int type = Random.Range(0, 2);
        if (id == 0)
        {
            if (type == 2)
                TypeKey = true;
            return type;
        }
        else
        {
            if (TypeKey)
            {
                if (type == 2)
                    return Random.Range(0, 2);
                else
                    return type;
            }
            else
            {
                if (type == 2)
                    TypeKey = true;
                return type;
            }
        }
    }
    private int RandomIdPiece(int idType)
    {
        if (idType == 0)
        {
            int id = Random.Range(0, 3);
            return id;
        }
        //else if (idType == 1)
        //{
        //    int id = Random.Range(0, 18);
        //    return id;
        //}
        else
            return 0;
    }
    private int SetPrice(int id)
    {
        int price = id == 2 ? 16 : 125;
        return price;
    }
    private bool SetTypePrice(int id)
    {
        bool typePrice = id == 2 ? true : false;
        return typePrice;
    }
    private void SetDeals()
    {
        for (int i = 0; i < _objDeals.Length; i++)
        {
            DealsCurrent deals = dataManager.dealsCurrent[i];
            _objDeals[i].transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = SetImgIcon(deals);//icon deal
            _objDeals[i].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = SetName(deals); //name dael
            _objDeals[i].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount((float)deals.Quantily / 5f, 0); //fill amount
            _objDeals[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = deals.Quantily + "/5"; //text quantily
            _objDeals[i].transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = _iconNotUp; //icon not up
            if (deals.Quantily < 5)
            {
                _objDeals[i].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true; // button buy active
                SetBtnBuy(deals, _objDeals[i].transform.GetChild(3).gameObject);
                int id = i;
                bool check = deals.TypePrice;
                int price = !check ? 125 + (75 * deals.Quantily) : 16 + (16 * deals.Quantily);
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
    private Sprite SetImgIcon(DealsCurrent deal)
    {
        if (deal.IdType == 0)
            return dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[deal.IdPiece];
        //else if (deal.IdType == 1)
        //    return dataManager.dataBase.imgEquipItems.sprPieceEnemy[deal.IdPiece];
        else
            return _iconKey;
    }
    private string SetName(DealsCurrent deal)
    {
        if (deal.Quantily >= 5)
        {
            return "<color=red>Sold out!</color>";
        }
        else
        {
            if (deal.IdType == 0)
                return dataManager.dataBase.imgEquipItems.namePlayer[deal.IdPiece];
            //else if (deal.IdType == 1)
            //    return dataManager.dataBase.imgEquipItems.nameEnemy[deal.IdPiece];
            else
                return "Key";
        }
    }
    private void SetBtnBuy(DealsCurrent deal, GameObject btn)
    {
        if (!deal.TypePrice)
        {
            int price = 125 + (75 * deal.Quantily);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = _iconBtn[0];
            btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString();
        }
        else
        {
            int price = 16 + (16 * deal.Quantily);
            btn.transform.GetChild(0).GetComponent<Image>().sprite = _iconBtn[1];
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
        int type = dataManager.dealsCurrent[id].IdType;
        if (type == 0)
        {
            dataManager.warehouse.CountPiecePlayerLevelUp[dataManager.dealsCurrent[id].IdPiece]++;
        }
        else if (type == 1)
        {
            dataManager.warehouse.CountPieceEnemy[dataManager.dealsCurrent[id].IdPiece]++;
        }
        else
        {
            PlayerPrefs.SetInt("Key", PlayerPrefs.GetInt("Key") + 1);
        }
    }
}
