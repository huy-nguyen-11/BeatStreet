using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }
    DataManager dataManager;
    [SerializeField] GameObject[] _panels;
    [SerializeField] GameObject _BarBottom, buttonSetting , buttonBack , buttonNoAds , buttonStarterPack ;
    [SerializeField] Transform _BarTop;
    [SerializeField] Image[] _imgBtnBottoms;
    [SerializeField] Sprite[] _sprBtnBottomsTrue;
    [SerializeField] Sprite[] _sprBtnBottomsFalse;
    [SerializeField] Transform[] _btnUpgrade;
    [SerializeField] Sprite[] _sprAvtPlayer;
    [SerializeField] Sprite[] _sprAvtEnemy;
    [SerializeField] private TextMeshProUGUI _textCoins, _textGems;

    // Sound, Music
    [SerializeField] Image[] _imgBtnSetting;
    [SerializeField] Sprite[] _sprBtnSettingTrue;
    [SerializeField] Sprite[] _sprBtnSettingFalse;
    // VIP
    private const string LAST_REWARD_TIME = "LastVIPTime";
    private TimeSpan rewardInterval = new TimeSpan(24, 0, 0);
    private DateTime lastRewardTime;
    private DateTime currentTime;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        dataManager = DataManager.Instance;
        ClossAllPanel();
        SetBottomBar(3);
        _panels[3].SetActive(true);
        SetImgBtnBottom(3 - 1);
        //if (3 == 3)
        //    SetAvtBtn();
    }
    private void OnEnable()
    {
        CheckVIP();
        AudioBase.Instance.SetVolumeMusic(PlayerPrefs.GetFloat("Music"));
        AudioBase.Instance.SetVolumeSound(PlayerPrefs.GetFloat("Sound"));
        AudioBase.Instance.SetMusicUI();
    }
    void Start()
    {
        CheckBtnSetting();
        SetTopBar();

        //
        buttonSetting.SetActive(true);
        buttonBack.SetActive(false);
        SetShowButtonPack();
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("Coin") != 0)
        {
            _textCoins.text = PlayerPrefs.GetInt("Coin").ToString("##,#");
        }
        else
        {
            _textCoins.text = "0";
        }

        if (PlayerPrefs.GetInt("Diamont") != 0)
        {
            _textGems.text = PlayerPrefs.GetInt("Diamont").ToString("##,#");
        }
        else
        {
            _textGems.text = "0";
        }
    }

    public void OpenPanel(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        ClossAllPanel();
        SetBottomBar(id);
        _panels[id].SetActive(true);
        SetImgBtnBottom(id - 1);
        if (id == 3)
        {
            buttonSetting.SetActive(true);
            buttonBack.SetActive(false);
        }
        else
        {
            buttonSetting.SetActive(false);
            buttonBack.SetActive(true);
        }
    }
    public void SetTopBar()
    {
        //_BarTop.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Energy").ToString() + "/20";
        //_BarTop.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _panels[0].GetComponent<LevelControllerMain>().GetCountStar().ToString();
        //_BarTop.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Diamont").ToString();
        //_BarTop.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("Coin").ToString();
    }
    private void SetBottomBar(int id)
    {
        List<int> id1 = new List<int>() { 2, 3, 4, 5 };
        _BarBottom.SetActive(id1.Contains(id));
    }
    private void ClossAllPanel()
    {
        for (int i = 1; i < _panels.Length; i++)
        {
            _panels[i].SetActive(false);
        }
    }
    private void SetImgBtnBottom(int id)
    {
        //for (int i = 0; i < _imgBtnBottoms.Length; i++)
        //{
        //    if (i == id)
        //        _imgBtnBottoms[i].sprite = _sprBtnBottomsTrue[i];
        //    else
        //        _imgBtnBottoms[i].sprite = _sprBtnBottomsFalse[i];
        //}
    }
    // Setting
    private void CheckBtnSetting()
    {
        if (PlayerPrefs.GetFloat("Sound") > 0)
        {
            _imgBtnSetting[0].sprite = _sprBtnSettingTrue[0];
        }
        else
            _imgBtnSetting[0].sprite = _sprBtnSettingFalse[0];
        if (PlayerPrefs.GetFloat("Music") > 0)
        {
            _imgBtnSetting[1].sprite = _sprBtnSettingTrue[1];
        }
        else
            _imgBtnSetting[1].sprite = _sprBtnSettingFalse[1];
    }
    public void BtnSound()
    {
        AudioBase.Instance.SetAudioUI(0);
        int sound = PlayerPrefs.GetFloat("Sound") > 0 ? 0 : 1;
        AudioBase.Instance.SetVolumeSound(sound);
        CheckBtnSetting();
    }
    public void BtnMusic()
    {
        AudioBase.Instance.SetAudioUI(0);
        int music = PlayerPrefs.GetFloat("Music") > 0 ? 0 : 1;
        AudioBase.Instance.SetVolumeMusic(music);
        CheckBtnSetting();
    }
    // Vip
    public void CheckRewardStatus()
    {
        if (DataManager.Instance.isCheckVip)
        {
            if (PlayerPrefs.HasKey(LAST_REWARD_TIME))
            {
                string lastRewardString = PlayerPrefs.GetString(LAST_REWARD_TIME);
                lastRewardTime = DateTime.Parse(lastRewardString);
            }
            else
            {
                lastRewardTime = DateTime.MinValue;
            }
            currentTime = DateTime.Now;
            if (currentTime - lastRewardTime >= rewardInterval)
            {
                BonusVip();
            }
        }
    }
    public void BtnBuyVip()
    {
        AudioBase.Instance.SetAudioUI(0);
        MG_Interface.Current.Purchase_Item(MG_ProductData.VIP_Pack.productId, (bool result, bool onIAP, string productId) =>
        {
            if (result)
            {
                AudioBase.Instance.SetAudioUI(1);
                DataManager.Instance.isCheckVip = true;
                CheckVIP();
                OpenPanel(3);
                DataManager.Instance.SaveFile();
            }
            else
            {
            }
        });
    }
    private void CheckVIP()
    {
        if (DataManager.Instance.isCheckVip)
        {
            //ScrollSnapPagination scrollSnap = _panels[3].GetComponent<ScrollSnapPagination>();
            //scrollSnap.totalPages = 1;
            //scrollSnap.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
            //scrollSnap.autoScroll = false;
            //scrollSnap.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            CheckRewardStatus();
        }
    }
    private void BonusVip()
    {
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 1000);
        PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 200);
        lastRewardTime = DateTime.Now;
        PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString());
        SetTopBar();
    }

    //starter Pack
    public void Onclick_StarterPack()
    {
        MG_Interface.Current.Purchase_Item(MG_ProductData.Starter_Pack.productId, (bool result, bool onIAP, string productId) =>
        {
            if (result)
            {
                //AudioBase.Instance.SetAudioUI(1);
                PlayerPrefs.SetInt("StarterPack", 1);
                SetShowButtonPack();
                //OpenPanel(3);
                //DataManager.Instance.SaveFile();
            }
            else
            {
            }
        });
    }

    // No ads pack
    public void Onclick_NoAdsPack()
    {
        MG_Interface.Current.Purchase_Item(MG_ProductData.NoAds_Pack.productId, (bool result, bool onIAP, string productId) =>
        {
            if (result)
            {
                AudioBase.Instance.SetAudioUI(1);
                PlayerPrefs.SetInt("NoAds", 1);
                SetShowButtonPack();
                //OpenPanel(3);
                //DataManager.Instance.SaveFile();
            }
            else
            {
            }
        });
    }

    void SetShowButtonPack()
    {
        buttonNoAds.SetActive(PlayerPrefs.GetInt("NoAds") == 0);
        buttonStarterPack.SetActive(PlayerPrefs.GetInt("StarterPack") == 0);
    }

    // Home
    private void SetAvtBtn()
    {
        _btnUpgrade[0].GetChild(0).GetComponent<Image>().sprite = _sprAvtPlayer[dataManager.idPlayer];
        _btnUpgrade[1].gameObject.SetActive(dataManager.LevelCurren >= 6);
        _btnUpgrade[2].gameObject.SetActive(dataManager.LevelCurren >= 6);
        if (dataManager.LevelCurren >= 6)
        {
            if (dataManager.idPet1 != 99)
            {
                _btnUpgrade[1].GetChild(0).GetComponent<Image>().sprite = _sprAvtEnemy[dataManager.idPet1];
                _btnUpgrade[1].GetChild(0).gameObject.SetActive(true);
            }
            else
                _btnUpgrade[1].GetChild(0).gameObject.SetActive(false);
            if (dataManager.idPet2 != 99)
            {
                _btnUpgrade[2].GetChild(0).GetComponent<Image>().sprite = _sprAvtEnemy[dataManager.idPet2];
                _btnUpgrade[2].GetChild(0).gameObject.SetActive(true);
            }
            else
                _btnUpgrade[2].GetChild(0).gameObject.SetActive(false);
        }
    }
    public void BtnOpenPlayerUpgrade()
    {
        AudioBase.Instance.SetAudioUI(0);
        OpenPanel(6);
    }
    public void BtnOpenEnemyUpgrade(bool isCheck)
    {
        AudioBase.Instance.SetAudioUI(0);
        dataManager.isActivePet1 = !isCheck;
        dataManager.isActivePet2 = isCheck;
        OpenPanel(9);
    }
    public void BtnMap()
    {
        AudioBase.Instance.SetAudioUI(0);
        _panels[0].GetComponent<LevelControllerMain>().OnInit();
        _BarBottom.SetActive(false);
        for (int i = 1; i < _panels.Length; i++)
        {
            _panels[i].gameObject.SetActive(false);
        }
    }
    //
    public void SetMission(int id, int count)
    {
        if (id == dataManager.questsCurrent.IdQuest1 || id == dataManager.questsCurrent.IdQuest2)
        {
            int idMission = id == dataManager.questsCurrent.IdQuest1 ?
                dataManager.questsCurrent.IdQuest1 : dataManager.questsCurrent.IdQuest2;
            int progress = id == dataManager.questsCurrent.IdQuest1 ? dataManager.questsCurrent.ProgressQuest1
                : dataManager.questsCurrent.ProgressQuest2;
            int milestone = dataManager.dataBase.ListQuests[id].Milestone;
            if (progress < milestone)
            {
                bool check = id == dataManager.questsCurrent.IdQuest1 ? false : true;
                if (!check) dataManager.questsCurrent.ProgressQuest1 += count;
                else dataManager.questsCurrent.ProgressQuest2 += count;
            }
        }
    }
}
