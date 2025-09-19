using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgrade : MonoBehaviour
{
    [SerializeField] Sprite[] _sprPlayerShadow;
    [SerializeField] Sprite[] _sprPlayerLock;
    [SerializeField] Sprite[] _sprPlayerUnLock;
    [SerializeField] Sprite[] _sprAvatarLock;
    [SerializeField] Sprite[] _sprAvatarUnLock;
    //[SerializeField] Sprite[] _sprFrame;
    [SerializeField] Transform[] _btnPlayer;
    [SerializeField] Sprite[] _sprLock;
    [SerializeField] Sprite[] _sprStatusUpgrade;
    [SerializeField] Sprite[] _sprPrice;
    [SerializeField] ScrollSnapPagination scrollSnap;
    [SerializeField] GameObject _buttonUpgrade , _buttonSwitch , _buttonIncrease , _buttonDeacrease , _progressTicket , playerChar;
    DataManager _dataManager;
    int idPlayer;
    int countUpGrade = 1;
    bool isCheckUnLock;
    public bool isStatusUpgrade;
    int priceCoin;
    int priceDiamont;
    int pieceLevelUp;
    int pieceLevelEvolve;
    int pieceUpgradeLevelUp;
    int pieceUpgradeLevelEvolve;
    int pieceUnlockCharacter;
    int HP, DAME, MANA = 0;
    private void OnEnable()
    {
        _dataManager = DataManager.Instance;
        idPlayer = _dataManager.idPlayer;
        isStatusUpgrade = false;
        SetOnClickBtn();
        OnInit();
        if (scrollSnap != null)
            Invoke(nameof(PagePlayerSellect), 0.01f);
    }
    private void OnInit()
    {
        SetBtnSwitchStatus();
        SetBtnPlayer();
        BtnInformation(idPlayer);
    }
    private void PagePlayerSellect()
    {
        if (idPlayer < 3)
            scrollSnap.PagesSellect(0);
        else if (idPlayer > 2 && idPlayer < 6)
            scrollSnap.PagesSellect(1);
        else if (idPlayer > 5 && idPlayer < 9)
            scrollSnap.PagesSellect(2);
        else
            scrollSnap.PagesSellect(3);
    }
    public void BtnActive() // butotn select
    {
        AudioBase.Instance.SetAudioUI(0);
        if (_dataManager.playerData[idPlayer].isUnlock)
        {
            if (idPlayer == 0 || idPlayer == 1)
                _dataManager.idPlayer = 0;
        }
        //MainManager.Instance.OpenPanel(3);
    }
    public void BtnSwitchStatus()
    {
        AudioBase.Instance.SetAudioUI(0);
        isStatusUpgrade = !isStatusUpgrade;
        SetBtnSwitchStatus();
        OnInit();
    }
    private void SetBtnSwitchStatus()
    {
        countUpGrade = 1;
        int idStatus = isStatusUpgrade ? 1 : 0;
        string txt = isStatusUpgrade ? "Evolve" : "Level Up";
       _buttonUpgrade.transform.GetChild(0).GetComponent<Image>().sprite = _sprPrice[idStatus]; // icon coe or diamont in button upgrade
        _buttonSwitch.transform.GetChild(0).GetComponent<Image>().sprite = _sprStatusUpgrade[idStatus]; // icon evo or update in button swwitch status
        _buttonSwitch.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = txt; // text in button switch status
    }
    private void SetOnClickBtn()
    {
        for (int i = 0; i < _btnPlayer.Length; i++)
        {
            int id = i;
            _btnPlayer[i].GetComponent<Button>().onClick.RemoveAllListeners();
            _btnPlayer[i].GetComponent<Button>().onClick.AddListener(delegate
            {
                BtnInformation(id);
            });
        }
    }
    private void SetBtnPlayer()
    {
        for (int i = 0; i < _btnPlayer.Length; i++)
        {
            if (_dataManager.playerData[i].isUnlock)
            {
                _btnPlayer[i].GetChild(0).GetComponent<Image>().sprite = _sprAvatarUnLock[i]; // icon char  
                _btnPlayer[i].GetChild(1).gameObject.SetActive(true); // text level
                _btnPlayer[i].GetChild(3).gameObject.SetActive(false); // lock
            }
            else
            {
                _btnPlayer[i].GetChild(0).GetComponent<Image>().sprite = _sprAvatarLock[i]; //icon char 
                _btnPlayer[i].GetChild(1).gameObject.SetActive(true); // text level
                _btnPlayer[i].GetChild(3).gameObject.SetActive(true); // lock
                if (_dataManager.warehouse.CountPiecePlayerLevelUp[i] >= _dataManager.playerData[i].pieceUnlock)
                {
                    _btnPlayer[i].GetChild(3).GetChild(1).GetComponent<Image>().sprite = _sprLock[1]; // icon lock
                }
                else
                {
                    //if (_dataManager.playerData[i].CheckVip)
                    //    _btnPlayer[i].GetChild(2).GetComponent<Image>().sprite = _sprLock[2];
                    //else
                    //    _btnPlayer[i].GetChild(2).GetComponent<Image>().sprite = _sprLock[0];

                    _btnPlayer[i].GetChild(3).GetChild(1).GetComponent<Image>().sprite = _sprLock[0]; // icon lock
                }
            }
        }
    }
    public void BtnInformation(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        idPlayer = id;
        countUpGrade = 1;
        isCheckUnLock = _dataManager.playerData[id].isUnlock;
        pieceUpgradeLevelUp = PieceLevelUp(idPlayer);
        pieceUpgradeLevelEvolve = PieceLevelEvolve(idPlayer);
        SetBtnPlayerInformation(id);
        SetParameter(id);
        SetBtnUpGrade(id);
        SetBarPieceAndPrice(id);
        SetPriceUpgrade(id);
        SetBtnPlayer();
        SetOnOffBtn(idPlayer);
    }
    private void SetBtnPlayerInformation(int id)
    {
        for (int i = 0; i < _btnPlayer.Length; i++)
        {
            if (i == id)
                _btnPlayer[i].transform.GetChild(2).gameObject.SetActive(true);
            else
                _btnPlayer[i].transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    private void SetParameter(int id)
    {
        transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dataManager.dataBase.imgEquipItems.namePlayer[id]; // name player
        string indexHp = "";
        string indexDame = "";
        string indexMana = "";
        SetTxtLevel(id);
        if (_dataManager.playerData[id].Level < 40)
        {
            GetAttribute(id);
            indexHp = isCheckUnLock ? "<color=yellow>+" + HP + "</color>" : "";
            indexDame = isCheckUnLock ? "<color=yellow>+" + DAME + "</color>" : "";
            indexMana = isCheckUnLock ? "<color=yellow>+" + MANA + "</color>" : "";
        }
        //content sate infor player
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dataManager.playerData[id].Hp + (!isStatusUpgrade ? indexHp : ""); // text hp
        transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dataManager.playerData[id].Dame + (!isStatusUpgrade ? indexDame : ""); // text damage
        transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dataManager.playerData[id].Mana + (!isStatusUpgrade ? indexMana : ""); // text mana
    }
    private void SetTxtLevel(int id)
    {
        if (!isStatusUpgrade)
            transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                "Level " + "<color=yellow>" + _dataManager.playerData[id].Level + "</color>" + "/" + _dataManager.playerData[id].LevelEvolve; // text level player
        else
            transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text =
                "Level " + _dataManager.playerData[id].Level + "/" + "<color=yellow>" + _dataManager.playerData[id].LevelEvolve + "</color>"; // text level player
    }
    private void SetBtnUpGrade(int id)
    {
        _buttonSwitch.GetComponent<Button>().interactable = isCheckUnLock; //button switch status
        _buttonUpgrade.GetComponent<Button>().onClick.RemoveAllListeners(); // button upgrade
        _buttonUpgrade.transform.GetChild(0).gameObject.SetActive(isCheckUnLock); // icon
        _buttonUpgrade.transform.GetChild(1).gameObject.SetActive(isCheckUnLock); // text
        _buttonUpgrade.transform.GetChild(2).gameObject.SetActive(isCheckUnLock); // text coin
        _buttonUpgrade.transform.GetChild(3).gameObject.SetActive(!isCheckUnLock); // text mission
        if (!isCheckUnLock)
        {
            if (_dataManager.LevelCurren < _dataManager.playerData[id].LevelUnlockCharacter)
            {
                _buttonUpgrade.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
                    "Clear\r\nmission " + _dataManager.playerData[id].LevelUnlockCharacter;
                int idPlayer = id;
                int idStatus = 2;
                _buttonUpgrade.GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnUpgrade(idPlayer, idStatus);
                });
            }
            else
            {
                int idPlayer = id;
                int idStatus = 1;
                _buttonUpgrade.GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnUpgrade(idPlayer, idStatus);
                });
                _buttonUpgrade.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Unlock";
            }
        }
        else
        {
            if (isStatusUpgrade && _dataManager.playerData[id].LevelEvolve < 40
                || !isStatusUpgrade && _dataManager.playerData[id].Level < _dataManager.playerData[id].LevelEvolve)
            {
                _buttonUpgrade.transform.GetChild(0).gameObject.SetActive(true); // icon
                _buttonUpgrade.transform.GetChild(1).gameObject.SetActive(true); // text
                _buttonUpgrade.transform.GetChild(2).gameObject.SetActive(true); // text coin
                _buttonUpgrade.transform.GetChild(3).gameObject.SetActive(false); // text mission
                int idPlayer = id;
                int idStatus = 0;
                _buttonUpgrade.GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnUpgrade(idPlayer, idStatus);
                });
                if (countUpGrade > 1)
                    _buttonUpgrade.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                            "level up\r\n" + countUpGrade + "x";
                else
                    _buttonUpgrade.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                        "level up";
            }
            else
            {
                _buttonUpgrade.transform.GetChild(0).gameObject.SetActive(false); // icon
                _buttonUpgrade.transform.GetChild(1).gameObject.SetActive(false); // text
                _buttonUpgrade.transform.GetChild(2).gameObject.SetActive(false); // text coin
                _buttonUpgrade.transform.GetChild(3).gameObject.SetActive(true);
                _buttonUpgrade.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                 "Max Level";
                int idPlayer = id;
                int idStatus = 2;
                _buttonUpgrade.GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnUpgrade(idPlayer, idStatus);
                });
            }
        }
    }
    private void SetOnOffBtn(int id)
    {
        int level = _dataManager.playerData[id].Level;
        bool check = false;
        if (isCheckUnLock)
        {
            if (!isStatusUpgrade && _dataManager.playerData[id].Level + countUpGrade < _dataManager.playerData[id].LevelEvolve)
            {
                int piece = PieceLevelUp(id);
                if (_dataManager.warehouse.CountPiecePlayerLevelUp[id] >= piece)
                    check = true;
            }
            if (isStatusUpgrade && _dataManager.playerData[id].LevelEvolve < 40)
            {
                int piece = PieceLevelEvolve(id);
                if (_dataManager.warehouse.CountPiecePlayerEvolve[id] >= piece)
                    check = true;
            }
        }
       _buttonIncrease.GetComponent<Button>().interactable = check;
        if (isCheckUnLock)
           _buttonDeacrease.GetComponent<Button>().interactable = countUpGrade > 1;
    }
    private void SetBarPieceAndPrice(int id)
    {
        if (!isStatusUpgrade)
        {
            pieceLevelUp = _dataManager.warehouse.CountPiecePlayerLevelUp[id];
           _progressTicket.transform.GetChild(3).GetComponent<Image>().sprite = _dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[id]; // icon piece
            if (!_dataManager.playerData[id].isUnlock)
            {
                pieceUnlockCharacter = _dataManager.playerData[id].pieceUnlock;
                float fill = (float)pieceLevelUp / pieceUnlockCharacter;
                if (fill < 0) fill = 0;
                if (fill > 1) fill = 1;
                _progressTicket.transform.GetChild(0).GetComponent<Image>().DOFillAmount(fill, 0); // img fill
                _progressTicket.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    pieceLevelUp + "/" + pieceUnlockCharacter; // text fill
                _progressTicket.transform.GetChild(2).gameObject.SetActive(pieceLevelUp >= pieceUnlockCharacter); // icon upp
            }
            else
            {
                float fill = (float)pieceLevelUp / pieceUpgradeLevelUp;
                if (fill < 0) fill = 0;
                if (fill > 1) fill = 1;
                _progressTicket.transform.GetChild(0).GetComponent<Image>().
                    DOFillAmount(fill, 0);
                _progressTicket.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    pieceLevelUp + "/" + pieceUpgradeLevelUp;
                _progressTicket.transform.GetChild(2).gameObject.SetActive(pieceLevelUp >= pieceUpgradeLevelUp);
            }
        }
        else
        {
            pieceLevelEvolve = _dataManager.warehouse.CountPiecePlayerEvolve[id];
            _progressTicket.transform.GetChild(3).GetComponent<Image>().sprite = _dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[id];
            float fill = (float)pieceLevelEvolve / pieceUpgradeLevelEvolve;
            if (fill < 0) fill = 0;
            if (fill > 1) fill = 1;
            _progressTicket.transform.GetChild(0).GetComponent<Image>().DOFillAmount(fill, 0);
            _progressTicket.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                pieceLevelEvolve + "/" + pieceUpgradeLevelEvolve;
            _progressTicket.transform.GetChild(2).gameObject.SetActive(pieceLevelEvolve >= pieceUpgradeLevelEvolve);
        }
    }
    private int PieceLevelUp(int id)
    {
        int level = _dataManager.playerData[id].Level + 1;
        if (level == 2)
            return 2;
        else if (level > 2 && level < 21)
            return 3;
        else if (level > 20 && level < 31)
            return 5;
        else
            return 7;
    }
    private int PieceLevelEvolve(int id)
    {
        int level = _dataManager.playerData[id].Level + 1;
        if (level < 30)
            return 2;
        else if (level > 29 && level < 40)
            return 3;
        else
            return 4;
    }
    private void SetPriceUpgrade(int id)
    {
        if (!isStatusUpgrade)
            priceCoin = SetPrice(id);
        else
            priceDiamont = SetPrice(id);
        int price = isStatusUpgrade ? priceDiamont : priceCoin;
       _buttonUpgrade.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = price.ToString();
    }
    private int SetPrice(int id)
    {
        if (!isStatusUpgrade)
        {
            int level = _dataManager.playerData[id].Level;
            if (countUpGrade == 1)
            {
                return 50 + (75 * level);
            }
            else
            {
                int priceCurren = 50 + (75 * level);
                int priceSellect = priceCurren;
                for (int i = 2; i <= countUpGrade; i++)
                {
                    priceSellect += 50 + (75 * i);
                }
                return priceSellect;
            }
        }
        else
        {
            int level = _dataManager.playerData[id].Level;
            int price = 0;
            if (level + 1 < 29)
                price = 20;
            else if (level + 1 > 29 && level + 1 < 39)
                price = 30;
            else
                price = 40;
            if (countUpGrade > 1)
                for (int i = level + 1; i < countUpGrade + level; i++)
                {
                    if (i < 29)
                        price += 20;
                    else if (i > 28 && i < 39)
                        price += 30;
                    else
                        price += 40;
                }
            return price;
        }
    }
    public void BtnCountUpGrade(bool check)
    {
        AudioBase.Instance.SetAudioUI(0);
        int level = _dataManager.playerData[idPlayer].Level;
        if (!check)
        {
            if (!isStatusUpgrade && level + countUpGrade <= _dataManager.playerData[idPlayer].LevelEvolve
                && pieceLevelUp >= pieceUpgradeLevelUp + CheckCardQuantityLevelUp(idPlayer)
                || isStatusUpgrade && _dataManager.playerData[idPlayer].LevelEvolve < 40
                && pieceLevelEvolve >= pieceUpgradeLevelEvolve + CheckCardQuantityLevelEvolve(idPlayer))
            {
                countUpGrade++;
                if (!isStatusUpgrade)
                    pieceUpgradeLevelUp += CheckCardQuantityLevelUp(idPlayer);
                else
                    pieceUpgradeLevelEvolve += CheckCardQuantityLevelEvolve(idPlayer);
                if (!isStatusUpgrade)
                {
                    if (level + countUpGrade >= _dataManager.playerData[idPlayer].LevelEvolve
                        || pieceLevelUp < pieceUpgradeLevelUp + CheckCardQuantityLevelUp(idPlayer))
                       _buttonIncrease.GetComponent<Button>().interactable = false;
                }
                else
                {
                    if (_dataManager.playerData[idPlayer].LevelEvolve >= 40
                        || _dataManager.playerData[idPlayer].LevelEvolve + countUpGrade >= 40
                        || pieceLevelEvolve < pieceUpgradeLevelEvolve + CheckCardQuantityLevelEvolve(idPlayer))
                        _buttonIncrease.GetComponent<Button>().interactable = false;
                }
               _buttonDeacrease.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            if (countUpGrade > 1)
                if (!isStatusUpgrade)
                    pieceUpgradeLevelUp -= CheckCardQuantityLevelUp(idPlayer);
                else
                    pieceUpgradeLevelEvolve -= CheckCardQuantityLevelEvolve(idPlayer);
            countUpGrade--;
            if (countUpGrade <= 1)
            {
                countUpGrade = 1;
               _buttonDeacrease.GetComponent<Button>().interactable = false;
            }
            else
            {
                _buttonIncrease.GetComponent<Button>().interactable = true;
            }
        }
        RefreshInformartion(idPlayer);
    }
    private void RefreshInformartion(int id)
    {
        idPlayer = id;
        isCheckUnLock = _dataManager.playerData[id].isUnlock;

        SetBtnPlayerInformation(id);
        SetParameter(id);
        SetBtnUpGrade(id);
        SetBarPieceAndPrice(id);
        SetPriceUpgrade(id);
        SetBtnPlayer();
    }
    private int CheckCardQuantityLevelUp(int id)
    {
        int level = _dataManager.playerData[id].Level + 1;
        if (level + countUpGrade == 2)
            return 2;
        else if (level + countUpGrade > 2 && level + countUpGrade < 21)
            return 3;
        else if (level + countUpGrade > 20 && level + countUpGrade < 31)
            return 5;
        else
            return 7;
    }
    private int CheckCardQuantityLevelEvolve(int id)
    {
        int level = _dataManager.playerData[id].Level + 1;
        if (level + countUpGrade < 30)
            return 2;
        else if (level + countUpGrade > 29 && level + countUpGrade < 40)
            return 3;
        else
            return 4;
    }
    public void BtnUpgrade(int id, int status)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (status == 0)
        {
            string keyPrice = !isStatusUpgrade ? "Coin" : "Diamont";
            int price = !isStatusUpgrade ? priceCoin : priceDiamont;
            if (!isStatusUpgrade && _dataManager.warehouse.CountPiecePlayerLevelUp[id] < pieceUpgradeLevelUp) return;
            if (isStatusUpgrade && _dataManager.warehouse.CountPiecePlayerEvolve[id] < pieceUpgradeLevelEvolve) return;
            if (PlayerPrefs.GetInt(keyPrice) >= price)
            {
                AudioBase.Instance.SetAudioUI(4);
                if (keyPrice == "Diamont") MainManager.Instance.SetMission(0, price);
                else MainManager.Instance.SetMission(1, price);
                MainManager.Instance.SetMission(2, 1);
                PlayerPrefs.SetInt(keyPrice, PlayerPrefs.GetInt(keyPrice) - price);
                SetAttribute(id);
                BtnInformation(id);
                _dataManager.SaveFile();
                MainManager.Instance.SetTopBar();
            }
        }
        else if (status == 1)
        {
            if (_dataManager.warehouse.CountPiecePlayerLevelUp[id] >= _dataManager.playerData[id].pieceUnlock)
            {
                AudioBase.Instance.SetAudioUI(3);
                _dataManager.warehouse.CountPiecePlayerLevelUp[id] -= _dataManager.playerData[id].pieceUnlock;
                _dataManager.playerData[id].isUnlock = true;
                BtnInformation(id);
                _dataManager.SaveFile();
            }
        }
    }
    private void SetAttribute(int id)
    {
        if (!isStatusUpgrade)
        {
            int level = _dataManager.playerData[id].Level;
            _dataManager.warehouse.CountPiecePlayerLevelUp[id] -= pieceUpgradeLevelUp;
            _dataManager.playerData[id].Level += countUpGrade;
            for (int i = level; i < level + countUpGrade; i++)
            {
                _dataManager.playerData[id].Hp += _dataManager.dataBase.HpUpgrade[i];
                _dataManager.playerData[id].Dame += _dataManager.dataBase.dameUpgrade[i];
                _dataManager.playerData[id].Mana++;
            }
        }
        else
        {
            _dataManager.warehouse.CountPiecePlayerEvolve[id] -= pieceUpgradeLevelEvolve;
            _dataManager.playerData[id].LevelEvolve += countUpGrade;
        }
    }
    private void GetAttribute(int id)
    {
        int level = _dataManager.playerData[id].Level;
        HP = 0; DAME = 0; MANA = 0;
        for (int i = level; i < level + countUpGrade; i++)
        {
            HP += _dataManager.dataBase.HpUpgrade[i];
            DAME += _dataManager.dataBase.dameUpgrade[i];
            MANA++;
        }
    }
}
