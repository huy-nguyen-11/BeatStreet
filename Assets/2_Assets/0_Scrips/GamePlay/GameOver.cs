using System.Collections;
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
    int CoinBonus;
    Coroutine _coroutine;
    DataManager _dataManager;
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
        _popupStatus[1].SetActive(true);
    }
    public void BtnBackHome()
    {
        AudioBase.Instance.SetAudioUI(0);
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
        if (PlayerPrefs.GetInt("Energy") >= 2)
        {
            PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - 2);
            SceneManager.LoadSceneAsync("2_GamePlay");
        }
        else
        {
            _popupStatus[3].SetActive(true);
        }
    }
    public void BtnBuyEnergy()
    {
        AudioBase.Instance.SetAudioUI(0);
        if (PlayerPrefs.GetInt("Diamont") >= 40)
        {
            AudioBase.Instance.SetAudioUI(2);
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 40);
            PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") + 20);
            GamePlayManager.Instance.SetMission(0, 40);
            _popupStatus[3].SetActive(false); 
        }
    }
    public void BtnClossBuyEnergy()
    {
        AudioBase.Instance.SetAudioUI(0);
        _popupStatus[3].SetActive(false);
    }
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
                for (int i = 0; i < _popupStatus[id].transform.GetChild(0).childCount; i++)
                {
                    if (i == _dataManager.LevelMode)
                        _popupStatus[id].transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    else
                        _popupStatus[id].transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
                SetGetBonus();
                _dataManager.SaveFile();
                break;
            case 1:
                break;
            case 2:
                CheckBtnRevive();
                _coroutine = StartCoroutine(SetTimeOffRevive());
                break;
        }
        txtCoinAds[0].text = GamePlayManager.Instance.coin.ToString();
        txtCoinAds[1].text = GamePlayManager.Instance.coin.ToString();
    }
    private void SetGetBonus()
    {
        //_popupStatus[0].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetSprLoot();
        _popupStatus[0].transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = GetCoinBonus().ToString();
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + GetCoinBonus());
        //SetTopBar();
    }
    private Sprite GetSprLoot()
    {
        int type = _dataManager.levelDatas[_dataManager.LevelSelect].Type;
        switch (type)
        {
            case 0:
                _dataManager.warehouse.CountPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
                return _dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
            case 1:
                _dataManager.warehouse.CountPiecePlayerEvolve[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
                return _dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
            case 2:
                _dataManager.warehouse.CountPieceEnemy[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
                return _dataManager.dataBase.imgEquipItems.sprPieceEnemy[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
            case 3:
                _dataManager.warehouse.CountItem[_dataManager.levelDatas[_dataManager.LevelSelect].idItem]++;
                return _dataManager.dataBase.imgEquipItems.sprItem[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
            default:
                return _dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[_dataManager.levelDatas[_dataManager.LevelSelect].idItem];
        }
    }
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
        int secon = 10;
        txtSeconRevive.text = secon.ToString();
        while (secon > 0)
        {
            yield return new WaitForSeconds(1);
            secon--;
            txtSeconRevive.text = secon.ToString();
        }
        _popupStatus[2].SetActive(false);
        _popupStatus[1].SetActive(true);
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
            GamePlayManager.Instance.SetMission(0, 40);
            GamePlayManager.Instance.SetMission(4, 1);
            PlayerRevive();
        }
        else
        {
            MG_Interface.Current.Reward_Show((bool result) =>
            {
                if (result)
                {
                    AudioBase.Instance.AudioGPl(2);
                    GamePlayManager.Instance.SetMission(4, 1);
                    PlayerRevive();
                }
            });
        }
        _dataManager.SaveFile();
    }
    private void CheckBtnRevive()
    {
        bool checkBtn = PlayerPrefs.GetInt("Diamont") >= 20 ? true : false;
        _btnRevives[0].SetActive(checkBtn);
        _btnRevives[1].SetActive(!checkBtn);
    }
    private void PlayerRevive()
    {
        GamePlayManager.Instance.SetPlayerRevive();
        gameObject.SetActive(false);
    }
}
