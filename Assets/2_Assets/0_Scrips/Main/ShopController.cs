using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] GameObject[] _panels;
    [SerializeField] GameObject prfItem;
    [SerializeField] GameObject _content;
    [SerializeField] List<GameObject> _listPopUp ;
    [SerializeField] List<Image> _listImageButtons;
    [SerializeField] Sprite spSlect, spNormal;
    [SerializeField] Button _btnVip;
    private List<string> nameChests = new List<string>() { "Classic", "Specail", "Mythic" };
    public List<int> listPlayerLevelUp = new List<int>();
    public List<int> listEnemy = new List<int>();
    public List<int> listItem = new List<int>();
    public List<int> listPlayerEvolve = new List<int>();
    public List<Transform> _listPosGems , _listPosCoins;
    public TimeReward timeReward1 , timeReward2;
    public CollectItemUICtrl getReward;
    public GameObject _contentShop;

    private bool isOnClick = true;

    private void OnEnable()
    {
        OpenPopup(MainManager.Instance.indexPopupShop);
        isOnClick = true;
    }

    public void OpenPopup(int index)
    {

        for (int i = 0; i < _listPopUp.Count; i++)
        {
            if (i == index)
            {
                _listPopUp[i].SetActive(true);
                _listImageButtons[i].sprite = spSlect;
            }
            else
            {
                _listPopUp[i].SetActive(false);
                _listImageButtons[i].sprite = spNormal;
            }
        }

        if (index == 2)
        {
            SetContentSize();
        }
    }

    public void BtnClaim()
    {
        AudioBase.Instance.SetAudioUI(0);
        _panels[0].SetActive(false);
        _panels[1].SetActive(false);
        for (int i = 0; i < _content.transform.childCount; i++)
        {
            Destroy(_content.transform.GetChild(i).gameObject);
        }
    }
    public void BtnOpenPopupChests(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        SetImgPopChest(id, _panels[0].transform.GetChild(0).GetChild(0));
        _panels[0].transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = nameChests[id];
        SetBonusCoin(id, _panels[0].transform.GetChild(0).GetChild(2));
        SetBtnBuy(id, _panels[0].transform.GetChild(0).GetChild(7));
        _panels[0].gameObject.SetActive(true);
    }
    private void SetImgPopChest(int id, Transform listIcon)
    {
        for (int i = 0; i < listIcon.childCount; i++)
        {
            if (i == id)
                listIcon.GetChild(i).gameObject.SetActive(true);
            else
                listIcon.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void SetBonusCoin(int id, Transform coin)
    {
        coin.gameObject.SetActive(true);
        if (id == 1)
        {
            coin.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+5000";
            SetTxtCountItem(35, 6, 20);
        }
        else if (id == 2)
        {
            coin.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+12000";
            SetTxtCountItem(100, 6, 60);
        }
        else
        {
            coin.gameObject.SetActive(false);
            SetTxtCountItem(4, 1, 1);
        }
    }
    private void SetTxtCountItem(int item1, int item2, int item3)
    {
        _panels[0].transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + item1.ToString();
        _panels[0].transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + item2.ToString();
        _panels[0].transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + item3.ToString();
    }
    private void SetBtnBuy(int id, Transform btn)
    {
        bool check = id == 0 ? true : false;
        btn.GetChild(0).gameObject.SetActive(!check);
        btn.GetChild(1).gameObject.SetActive(!check);
        btn.GetChild(2).gameObject.SetActive(check);
        btn.GetChild(3).gameObject.SetActive(check);
        if (id == 0)
            btn.GetChild(3).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Key") + "/3";
        else if (id == 1)
            btn.GetChild(1).GetComponent<TextMeshProUGUI>().text = "900";
        else
            btn.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2500";
        btn.GetComponent<Button>().onClick.RemoveAllListeners();
        btn.GetComponent<Button>().onClick.AddListener(delegate
        {
            BtnBuy(id);
        });
    }
    public void BtnClossePopupChests()
    {
        AudioBase.Instance.SetAudioUI(0);
        _panels[0].gameObject.SetActive(false);
    }
    public void BtnBuy(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (id == 0)
        {
            //if (PlayerPrefs.GetInt("Key") >= 3)
            //{
            //    AudioBase.Instance.SetAudioUI(1);
            //    PlayerPrefs.SetInt("Key", PlayerPrefs.GetInt("Key") - 3);
            //    RandomChest(3, 1, 1, 1);
            //}
            WatchingAdsForOpenTreasure();
        }
        else if (id == 1)
        {
            if (PlayerPrefs.GetInt("Diamont") >= 900)
            {
                AudioBase.Instance.SetAudioUI(1);
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 900);
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 500);
                RandomChest(30, 5, 6, 20);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("Diamont") >= 2500)
            {
                AudioBase.Instance.SetAudioUI(1);
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 2500);
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 12000);
                RandomChest(90, 10, 6, 60);
            }
        }
        MainManager.Instance.SetTopBar();
    }
    private void RandomChest(int PlLevelUp, int EnLevelUp, int item, int PlEvolve)
    {
        listPlayerLevelUp.Clear();
        listEnemy.Clear();
        listItem.Clear();
        listPlayerEvolve.Clear();
        _content.transform.position = new Vector2(_content.transform.position.x, 0);
        for (int i = 0; i < PlLevelUp; i++)
        {
            int id = Random.Range(0, DataManager.Instance.dataBase.imgEquipItems.sprPiecePlayerLevelUp.Count);
            listPlayerLevelUp.Add(id);
        }
        for (int i = 0; i < EnLevelUp; i++)
        {
            int id = Random.Range(0, DataManager.Instance.dataBase.imgEquipItems.sprPieceEnemy.Count);
            listEnemy.Add(id);
        }
        for (int i = 0; i < item; i++)
        {
            int id = Random.Range(0, DataManager.Instance.dataBase.imgEquipItems.sprItem.Count);
            listItem.Add(id);
            if (!DataManager.Instance.warehouse.ListItems.Contains(id))
                DataManager.Instance.warehouse.ListItems.Add(id);
        }
        for (int i = 0; i < PlEvolve; i++)
        {
            int id = Random.Range(0, DataManager.Instance.dataBase.imgEquipItems.sprPiecePlayerEvolve.Count);
            listPlayerEvolve.Add(id);
        }

        CheckPlayerLevelUp();
        CheckEnemy();
        CheckListPlayerEvolve();
        CheckListItem();

        _panels[0].SetActive(false);
        _panels[1].SetActive(true);
        DataManager.Instance.SaveFile();
    }

    //for button watch ads open reward treasure
    private void WatchingAdsForOpenTreasure()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection available.");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            timeReward2.OnFreeButtonClick();
            MG_Interface.Current.Reward_Show(HandleOpenTreasure); 
        }
    }

    void HandleOpenTreasure(bool resut)
    {
        RandomChest(3, 1, 1, 1);
    }

    private void CheckPlayerLevelUp()
    {
        while (listPlayerLevelUp.Count > 0)
        {
            int checkNumber = listPlayerLevelUp[0];
            int count = listPlayerLevelUp.FindAll(x => x == checkNumber).Count;
            GameObject item = Instantiate(prfItem, _content.transform.position, Quaternion.identity, _content.transform);
            item.transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.dataBase.imgEquipItems.sprPiecePlayerLevelUp[checkNumber];
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
            DataManager.Instance.warehouse.CountPiecePlayerLevelUp[checkNumber] += count;
            listPlayerLevelUp.RemoveAll(x => x == checkNumber);
        }
    }
    private void CheckEnemy()
    {
        while (listEnemy.Count > 0)
        {
            int checkNumber = listEnemy[0];
            int count = listEnemy.FindAll(x => x == checkNumber).Count;
            GameObject item = Instantiate(prfItem, _content.transform.position, Quaternion.identity, _content.transform);
            item.transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.dataBase.imgEquipItems.sprPieceEnemy[checkNumber];
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
            DataManager.Instance.warehouse.CountPieceEnemy[checkNumber] += count;
            listEnemy.RemoveAll(x => x == checkNumber);
        }
    }
    private void CheckListItem()
    {
        while (listItem.Count > 0)
        {
            int checkNumber = listItem[0];
            int count = listItem.FindAll(x => x == checkNumber).Count;
            GameObject item = Instantiate(prfItem, _content.transform.position, Quaternion.identity, _content.transform);
            item.transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.dataBase.imgEquipItems.sprItem[checkNumber];
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
            DataManager.Instance.warehouse.CountItem[checkNumber] += count;
            listItem.RemoveAll(x => x == checkNumber);
        }
    }
    private void CheckListPlayerEvolve()
    {
        while (listPlayerEvolve.Count > 0)
        {
            int checkNumber = listPlayerEvolve[0];
            int count = listPlayerEvolve.FindAll(x => x == checkNumber).Count;
            GameObject item = Instantiate(prfItem, _content.transform.position, Quaternion.identity, _content.transform);
            item.transform.GetChild(0).GetComponent<Image>().sprite = DataManager.Instance.dataBase.imgEquipItems.sprPiecePlayerEvolve[checkNumber];
            item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = count.ToString();
            DataManager.Instance.warehouse.CountPiecePlayerEvolve[checkNumber] += count;
            listPlayerEvolve.RemoveAll(x => x == checkNumber);
        }
    }
    //public void BtnBuyCoin(int id)
    //{
    //    AudioBase.Instance.SetAudioUI(0);
    //    if (id == 0)
    //    {
    //        if (PlayerPrefs.GetInt("Diamont") >= 30)
    //        {
    //            AudioBase.Instance.SetAudioUI(1);
    //            MainManager.Instance.SetMission(0, 30);
    //            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 30);
    //            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 360);
    //        }
    //    }
    //    else if (id == 1)
    //    {
    //        if (PlayerPrefs.GetInt("Diamont") >= 500)
    //        {
    //            AudioBase.Instance.SetAudioUI(1);
    //            MainManager.Instance.SetMission(0, 500);
    //            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 500);
    //            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 6600);
    //        }
    //    }
    //    else
    //    {
    //        if (PlayerPrefs.GetInt("Diamont") >= 1000)
    //        {
    //            AudioBase.Instance.SetAudioUI(1);
    //            MainManager.Instance.SetMission(0, 1000);
    //            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 1000);
    //            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 13200);
    //        }
    //    }
    //    MainManager.Instance.SetTopBar();
    //    DataManager.Instance.SaveFile();
    //}

    public void BtnBuyCoin(int id)
    {
        if (!isOnClick) return;
        AudioBase.Instance.SetAudioUI(0);
        // Mảng các gói: {diamond, coin}
        int[,] exchangePackages = new int[,]
        {
        {30, 360},
        {500, 6600},
        {1000, 13200},
        {2000, 26400},
        {4000, 52800}
        };

        if (id < 0 || id >= exchangePackages.GetLength(0)) return;

        int diamondCost = exchangePackages[id, 0];
        int coinReward = exchangePackages[id, 1];

        TryExchangeDiamondForCoins(diamondCost, coinReward , id);
        MainManager.Instance.SetTopBar();
        DataManager.Instance.SaveFile();
    }

    public void ButtonWatchAdsToGteCoins()
    {
        Debug.Log("watch ads to get coins!");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection available.");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            timeReward2.OnFreeButtonClick();
            MG_Interface.Current.Reward_Show(HandleRewardCoins); // get gems free
        }
    }

    void HandleRewardCoins(bool resut)
    {
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 50);
        getReward.DoAddCoinEffect(_listPosCoins[0].position, PlayerPrefs.GetInt("Coin") - 50, PlayerPrefs.GetInt("Coin"));
    }

    private void TryExchangeDiamondForCoins(int diamondCost, int coinReward , int _id)
    {
        int currentDiamond = PlayerPrefs.GetInt("Diamont");

        if (currentDiamond >= diamondCost)
        {
            AudioBase.Instance.SetAudioUI(1);
            MainManager.Instance.SetMission(0, diamondCost);

            PlayerPrefs.SetInt("Diamont", currentDiamond - diamondCost);
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + coinReward);
            isOnClick = false;
            getReward.DoAddCoinEffect(_listPosCoins[_id+1].position, PlayerPrefs.GetInt("Coin") - coinReward, PlayerPrefs.GetInt("Coin"));
            StartCoroutine(WaitForCoolDownOnClick());
        }
    }
    public void BtnBuyDiamont(int id)
    {
        if(!isOnClick) return;
        AudioBase.Instance.SetAudioUI(0);
        if(id == 0)
        {
            Debug.Log("watch ads to get gems!");
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
               Debug.Log("No internet connection available.");
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                timeReward1.OnFreeButtonClick();
                MG_Interface.Current.Reward_Show(HandleRewardGems); // get gems free
            }

        }
        else
        {
            MG_Interface.Current.Purchase_Item(MG_ProductData.DiamontPacks[id].productId, (bool result, bool onIAP, string productId) =>
            {
                if (result)
                {
                    AudioBase.Instance.SetAudioUI(1);
                    PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + SetDiamont(id));
                    isOnClick = false;
                    getReward.DoAddGemsEffect(_listPosGems[id].position, PlayerPrefs.GetInt("Diamont") - SetDiamont(id), PlayerPrefs.GetInt("Diamont"));
                    MainManager.Instance.SetTopBar();
                    DataManager.Instance.SaveFile();
                    StartCoroutine(WaitForCoolDownOnClick());
                }
                else
                {
                }
            });
        }

    }

    void HandleRewardGems(bool resutl)
    {
        if (resutl)
        {
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 50);
            isOnClick = false;
            getReward.DoAddGemsEffect(_listPosGems[0].position, PlayerPrefs.GetInt("Diamont") - 50, PlayerPrefs.GetInt("Diamont"));
            StartCoroutine(WaitForCoolDownOnClick());
        }
    }

    IEnumerator WaitForCoolDownOnClick()
    {
        yield return new WaitForSeconds(2f);
        isOnClick = true;
    }

    private int SetDiamont(int id)
    {
        switch (id)
        {
            case 1:
                return 440;
            case 2:
                return 960;
            case 3:
                return 2600;
            case 4:
                return 5600;
            case 5:
                return 12000;
            default:
                return 0;
        }
    }
    public void BtnOpenBuyVip()
    {
        if (!DataManager.Instance.isCheckVip)
            MainManager.Instance.OpenPanel(8);
    }

    //ads , starterpack
    public void OnClick_ButtonNoAds()
    {
        MainManager.Instance.Onclick_NoAdsPack();
        SetContentSize();
    }

    public void OnClick_ButtonStarterPack()
    {
        MainManager.Instance.Onclick_StarterPack();
        SetContentSize();
    }

    void SetContentSize()
    {
        if(PlayerPrefs.GetInt("NoAds") == 1 && PlayerPrefs.GetInt("StarterPack") == 1)
        {
            _contentShop.transform.GetChild(0).gameObject.SetActive(false);
            _contentShop.GetComponent<VerticalLayoutGroup>().padding.top = 100;
        }
        else if(PlayerPrefs.GetInt("NoAds") == 1)
        {
            _contentShop.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
        else if(PlayerPrefs.GetInt("StarterPack") == 1)
        {
            _contentShop.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentShop.GetComponent<RectTransform>());
    }
}
