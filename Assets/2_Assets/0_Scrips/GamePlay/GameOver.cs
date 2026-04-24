using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    //public TextMeshProUGUI[] _txtTopBars;
    public GameObject[] _popupStatus;
    public GameObject[] _coinAds;
    // False Game
    public TextMeshProUGUI[] txtCoinAds;
    // Revive
    public TextMeshProUGUI txtSeconRevive;
    public GameObject[] _btnRevives;
    public List<Sprite> sprRevive;
    public GameObject imgRevive;
    int CoinBonus;
    Coroutine _coroutine;
    DataManager _dataManager;
    RewardInfo _pendingReward1;
    RewardInfo _pendingReward2;
    int _pendingCollectedCoin;
    bool _pendingCanClaimDataReward;
    bool _hasPendingVictoryReward;
    bool _isVictoryRewardClaimed;

    public CollectItemUICtrl collectItemUICtrl; // Reference to the CollectItemUICtrl script

    private void Awake()
    {
        _dataManager = DataManager.Instance;
    }
    void Start()
    {
        //SetTopBar();
    }
    public void BtnBackGameOver()
    {
        AudioBase.Instance.StopMusic();
        AudioBase.Instance.AudioGPl(1);
        StopCoroutine(_coroutine);
        _popupStatus[2].SetActive(false);
        GamePlayManager.Instance.CheckTurnPlayShowAds();
        _popupStatus[1].SetActive(true);
        GetRewardDefeat();
    }
    public void BtnBackHome()
    {
        AudioBase.Instance.SetAudioUI(0);
        GamePlayManager.Instance.BtnBackMain();
    }
    public void BtnBackLevels()
    {
        AudioBase.Instance.SetAudioUI(0);
        AudioBase.Instance.isOpenLevel = true;
        GamePlayManager.Instance.BtnBackMain();
    }

    public void BtnUpGrade()
    {
        AudioBase.Instance.SetAudioUI(0);
        GamePlayManager.Instance.BtnBackMain();
    }
    public void BtnAgain()
    {
        AudioBase.Instance.SetAudioUI(0);
        //if (PlayerPrefs.GetInt("Energy") >= 2)
        //{
        //    PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - 2);
        //    SceneManager.LoadSceneAsync("2_GamePlay");
        //}
        //else
        //{
        //    _popupStatus[3].SetActive(true);
        //}\
        SceneManager.LoadSceneAsync("2_GamePlay");
    }

    //public void BtnBuyEnergy()
    //{
    //    AudioBase.Instance.SetAudioUI(0);
    //    if (PlayerPrefs.GetInt("Diamont") >= 40)
    //    {
    //        AudioBase.Instance.SetAudioUI(2);
    //        PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 40);
    //        PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") + 20);
    //        GamePlayManager.Instance.SetMission(0, 40);
    //        _popupStatus[3].SetActive(false); 
    //    }
    //}
    //public void BtnClossBuyEnergy()
    //{
    //    AudioBase.Instance.SetAudioUI(0);
    //    _popupStatus[3].SetActive(false);
    //}

    public void BtnCoinAds()
    {
        AudioBase.Instance.SetAudioUI(0);
        MG_Interface.Current.Reward_Show((bool result) =>
        {
            if (result)
            {
                AudioBase.Instance.SetAudioUI(1);
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + (GamePlayManager.Instance.coin * 2));
                _coinAds[0].SetActive(false);
                _coinAds[1].SetActive(false);
                //SetTopBar();
                _dataManager.SaveFile();
            }
        });

    }
    public void SetTopBar()
    {
        //_txtTopBars[0].text = PlayerPrefs.GetInt("Energy").ToString();
        //_txtTopBars[1].text = PlayerPrefs.GetInt("Star").ToString();
        //_txtTopBars[2].text = PlayerPrefs.GetInt("Diamont").ToString();
        //_txtTopBars[3].text = PlayerPrefs.GetInt("Coin").ToString();
    }

    public void SetOpenPopup(int id)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < _popupStatus.Length; i++)
        {
            if (i == id) _popupStatus[i].SetActive(true);
            else _popupStatus[i].SetActive(false);
        }
        switch (id)
        {
            case 0:
                GamePlayManager.Instance.coin += (int)GamePlayManager.Instance._Player._attributesPet[6];
                for (int i = 0; i < _popupStatus[id].transform.GetChild(1).GetChild(0).childCount; i++)
                {
                    if (i == _dataManager.LevelMode)
                        _popupStatus[id].transform.GetChild(1).GetChild(0).GetChild(i).gameObject.SetActive(true);
                    else
                        _popupStatus[id].transform.GetChild(1).GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
                _popupStatus[id].transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = "Level " + (_dataManager.LevelSelect + 1);
                GetReward();
                _dataManager.SaveFile();
                break;
            case 1:
                break;
            case 2:
                //CheckBtnRevive();
                ShowPlayerRevive();
                _coroutine = StartCoroutine(SetTimeOffRevive());
                break;
        }
        //txtCoinAds[0].text = GamePlayManager.Instance.coin.ToString();
        //txtCoinAds[1].text = GamePlayManager.Instance.coin.ToString();
    }
    private void GetReward() //for voctory
    {
        Transform _reward1 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(0); //tem reward
        Transform _reward2 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(1); //coins reward
        Transform _reward3 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(2); //coins collect    

        _pendingReward1 = null;
        _pendingReward2 = null;
        _pendingCollectedCoin = 0;
        _pendingCanClaimDataReward = false;
        _hasPendingVictoryReward = false;
        _isVictoryRewardClaimed = false;

        int level = _dataManager.LevelSelect;
        int mode = Mathf.Clamp(_dataManager.LevelMode, 0, 2);
        int coinCollected = GamePlayManager.Instance.coin;

        if (level < 0 || level >= _dataManager.levelDatas.Count) return;

        StarReward currentStarReward = _dataManager.levelDatas[level].starRewards[mode];
        if (currentStarReward == null || currentStarReward.rewards == null || currentStarReward.rewards.Count == 0) return;

        RewardInfo rewardData1 = currentStarReward.rewards.Count > 1 ? currentStarReward.rewards[1] : null; // reward1
        RewardInfo rewardData2 = currentStarReward.rewards[0]; // reward2
        bool canClaimDataReward = GamePlayManager.Instance.canClaimStarDataRewardThisRun;


        if (!canClaimDataReward)
        {
            _reward1.gameObject.SetActive(false);
            _reward2.gameObject.SetActive(false);

            if (_reward3.childCount > 1)
                _reward3.GetChild(1).GetComponent<TextMeshProUGUI>().text = coinCollected.ToString();

            _pendingCollectedCoin = coinCollected;
            _pendingCanClaimDataReward = false;
            _hasPendingVictoryReward = true;
            return;
        }

        _reward1.gameObject.SetActive(true);
        _reward2.gameObject.SetActive(true);

        // set reward1 (sprite + amount from data)
        if (rewardData1 != null)
        {
            Sprite reward1Sprite = GetRewardSprite(rewardData1);
            if (_reward1.childCount > 0 && reward1Sprite != null)
                _reward1.GetChild(0).GetComponent<Image>().sprite = reward1Sprite;
            if (_reward1.childCount > 1)
                _reward1.GetChild(1).GetComponent<TextMeshProUGUI>().text = rewardData1.amount.ToString();
        }

        // set reward2 (sprite + amount from data)
        if (rewardData2 != null)
        {
            Sprite reward2Sprite = GetRewardSprite(rewardData2);
            if (_reward2.childCount > 0 && reward2Sprite != null)
                _reward2.GetChild(0).GetComponent<Image>().sprite = reward2Sprite;
            if (_reward2.childCount > 1)
                _reward2.GetChild(1).GetComponent<TextMeshProUGUI>().text = rewardData2.amount.ToString();
        }

        // reward :coin collect
        if (_reward3.childCount > 1)
            _reward3.GetChild(1).GetComponent<TextMeshProUGUI>().text = coinCollected.ToString();

        _pendingReward1 = rewardData1;
        _pendingReward2 = rewardData2;
        _pendingCollectedCoin = coinCollected;
        _pendingCanClaimDataReward = true;
        _hasPendingVictoryReward = true;
    }

    public void ButtonClaim()
    {
        if (!_hasPendingVictoryReward || _isVictoryRewardClaimed) return;

        if (_pendingCanClaimDataReward)
        {
            if (_pendingReward1 != null) ApplyRewardToInventory(_pendingReward1);
            if (_pendingReward2 != null) ApplyRewardToInventory(_pendingReward2);
        }

        _popupStatus[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);// show off

        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + _pendingCollectedCoin);
        _isVictoryRewardClaimed = true;
        _hasPendingVictoryReward = false;
        _dataManager.SaveFile();

        collectItemUICtrl.DoAddCoinEffect(Vector3.zero, PlayerPrefs.GetInt("Coin") - _pendingCollectedCoin, PlayerPrefs.GetInt("Coin"));
        //set UI 
        StartCoroutine(WaittingForShowButtonUI());
    }

    IEnumerator WaittingForShowButtonUI()
    {
        yield return new WaitForSeconds(2f);
        Transform _reward1 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(0); //tem reward
        _reward1.GetChild(2).gameObject.SetActive(true); //check mark for reward1
        Transform _reward2 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(1); //coins reward
        _reward2.GetChild(2).gameObject.SetActive(true); //check mark for reward2
        Transform _reward3 = _popupStatus[0].transform.GetChild(0).GetChild(0).GetChild(2); //coins collect   
        _reward3.GetChild(2).gameObject.SetActive(true); //check mark for reward3

        _popupStatus[0].transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        _popupStatus[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void ButtonClaimX2()
    {
        MG_Interface.Current.Reward_Show((bool result) =>
        {
            if (result)
            {
                if (!_hasPendingVictoryReward || _isVictoryRewardClaimed) return;

                if (_pendingCanClaimDataReward)
                {
                    if (_pendingReward1 != null) ApplyRewardToInventory(_pendingReward1);
                    if (_pendingReward2 != null) ApplyRewardToInventory(_pendingReward2);
                }
                _popupStatus[0].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);// show off

                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + (_pendingCollectedCoin*2));
                _isVictoryRewardClaimed = true;
                _hasPendingVictoryReward = false;
                _dataManager.SaveFile();

                collectItemUICtrl.DoAddCoinEffect(Vector3.zero, PlayerPrefs.GetInt("Coin") - _pendingCollectedCoin*2, PlayerPrefs.GetInt("Coin"));
                //set UI 
                StartCoroutine(WaittingForShowButtonUI());
            }
        });
    }

    private Sprite GetRewardSprite(RewardInfo rewardInfo)
    {
        if (rewardInfo == null || _dataManager == null || _dataManager.dataBase == null || _dataManager.dataBase.imgEquipItems == null)
            return null;

        ImgEquipItems db = _dataManager.dataBase.imgEquipItems;

        if (rewardInfo.itemId == 666) return GetSpriteSafe(db.sprItemCommon, 0); // coin
        if (rewardInfo.itemId == 661) return GetSpriteSafe(db.sprItemCommon, 1); // diamond
        if (rewardInfo.itemId == 662) return GetSpriteSafe(db.sprItemCommon, 2); // key

        switch (rewardInfo.itemType)
        {
            case 0:
                return GetSpriteSafe(db.sprPiecePlayerLevelUp, rewardInfo.itemId);
            case 1:
                return GetSpriteSafe(db.sprPiecePlayerEvolve, rewardInfo.itemId);
            case 2:
            case 3:
                return GetSpriteSafe(db.sprItem, rewardInfo.itemId);
            default:
                return null;
        }
    }

    private Sprite GetSpriteSafe(List<Sprite> sprites, int index)
    {
        if (sprites == null || index < 0 || index >= sprites.Count) return null;
        return sprites[index];
    }

    private void ApplyRewardToInventory(RewardInfo rewardInfo)
    {
        if (rewardInfo == null || rewardInfo.amount <= 0) return;

        int amount = rewardInfo.amount;

        if (rewardInfo.itemId == 666)
        {
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + amount);
            return;
        }
        if (rewardInfo.itemId == 661)
        {
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + amount);
            return;
        }
        if (rewardInfo.itemId == 662)
        {
            PlayerPrefs.SetInt("Key", PlayerPrefs.GetInt("Key") + amount);
            return;
        }

        switch (rewardInfo.itemType)
        {
            case 0:
                if (_dataManager.warehouse != null && _dataManager.warehouse.CountPiecePlayerLevelUp != null
                    && rewardInfo.itemId >= 0 && rewardInfo.itemId < _dataManager.warehouse.CountPiecePlayerLevelUp.Count)
                    _dataManager.warehouse.CountPiecePlayerLevelUp[rewardInfo.itemId] += amount;
                break;
            case 1:
                if (_dataManager.warehouse != null && _dataManager.warehouse.CountPiecePlayerEvolve != null
                    && rewardInfo.itemId >= 0 && rewardInfo.itemId < _dataManager.warehouse.CountPiecePlayerEvolve.Count)
                    _dataManager.warehouse.CountPiecePlayerEvolve[rewardInfo.itemId] += amount;
                break;
            case 2:
            case 3:
                if (_dataManager.warehouse != null && _dataManager.warehouse.CountItem != null
                    && rewardInfo.itemId >= 0 && rewardInfo.itemId < _dataManager.warehouse.CountItem.Count)
                {
                    _dataManager.warehouse.CountItem[rewardInfo.itemId] += amount;
                    if (_dataManager.warehouse.ListItems != null && !_dataManager.warehouse.ListItems.Contains(rewardInfo.itemId))
                        _dataManager.warehouse.ListItems.Add(rewardInfo.itemId);
                }
                break;
        }
    }

    //getcoin for defeat
    private void GetRewardDefeat()
    {
        int coinBonus = GamePlayManager.Instance.coin / 2; //get coin bonus = 50% coin collect
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + coinBonus);
        Transform _reward1 = _popupStatus[1].transform.GetChild(0).GetChild(0).GetChild(1); //coin reward
        _reward1.GetComponent<TextMeshProUGUI>().text = coinBonus.ToString();
    }

    //private void SetGetBonus()
    //{
    //    //_popupStatus[0].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetSprLoot();
    //    _popupStatus[0].transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = GetCoinBonus().ToString();
    //    PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + GetCoinBonus());
    //    //SetTopBar();
    //}

    //private Sprite GetSprLoot()
    //{
    //    int type = _dataManager.levelDatas[_dataManager.LevelSelect].Type;
    //    switch (type)
    //    {
    //        case 0:
    //            _dataManager.warehouse.CountPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
    //            return _dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
    //        case 1:
    //            _dataManager.warehouse.CountPiecePlayerEvolve[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
    //            return _dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
    //        case 2:
    //            _dataManager.warehouse.CountPieceEnemy[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
    //            return _dataManager.dataBase.imgEquipItems.sprPieceEnemy[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
    //        case 3:
    //            _dataManager.warehouse.CountItem[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
    //            return _dataManager.dataBase.imgEquipItems.sprItem[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
    //        default:
    //            return _dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
    //    }
    //}
    private int GetCoinBonus()
    {
        if (_dataManager.LevelSelect < 9)
            return 100;
        else if (_dataManager.LevelSelect < 19)
            return 150;
        else if (_dataManager.LevelSelect < 28)
            return 200;
        else if (_dataManager.LevelSelect < 37)
            return 250;
        else
            return 300;
    }
    IEnumerator SetTimeOffRevive()
    {
        int secon = 5;
        txtSeconRevive.text = secon.ToString();
        while (secon > 0)
        {
            yield return new WaitForSeconds(1);
            secon--;
            txtSeconRevive.text = secon.ToString();
        }
        _popupStatus[2].transform.GetChild(0).GetChild(3).gameObject.SetActive(true); // button no thanks
        //GamePlayManager.Instance.CheckTurnPlayShowAds();
        //_popupStatus[1].SetActive(true);
        //GetRewardDefeat();
    }
    public void BtnRevive(bool Revive)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (!Revive)
        {
            //if (PlayerPrefs.GetInt("Diamont") >= 40)
            //{
            //    AudioBase.Instance.AudioGPl(2);
            //    PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 40);
            //    GamePlayManager.Instance.SetMission(0, 40);
            //    GamePlayManager.Instance.SetMission(4, 1);
            //    PlayerRevive();
            //    //SetTopBar();
            //}
            AudioBase.Instance.AudioGPl(2);
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 5);
            PlayerRevive();
        }
        else
        {
            MG_Interface.Current.Reward_Show((bool result) =>
            {
                if (result)
                {
                    AudioBase.Instance.AudioGPl(2);
                    PlayerRevive();
                }
            });
        }
        _dataManager.SaveFile();
    }
    private void CheckBtnRevive()
    {
        bool checkBtn = PlayerPrefs.GetInt("Diamont") >= 5 ? true : false;
        _btnRevives[0].SetActive(checkBtn);
        _btnRevives[1].SetActive(!checkBtn);
    }
    private void PlayerRevive()
    {
        GamePlayManager.Instance.SetPlayerRevive();
        GamePlayManager.Instance.SetMission(4, 1);// mission revive
        gameObject.SetActive(false);
    }

    private void ShowPlayerRevive()
    {
        imgRevive.GetComponent<Image>().sprite = sprRevive[PlayerController.Instance.id];
    }
}
