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
    public GameObject popUpGetRewardNoAds, popUpGteRewardStarterpack;
    [SerializeField] Transform _BarTop;
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

    //intdex popup shop
    public int indexPopupShop = 0;

    public CharacterMenu characterMenu;

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
        //SetAvtBtn();
    }
    private void OnEnable()
    {
        CheckVIP();
        // MainMenu music: if enabled, force volume = 0.6
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        AudioBase.Instance.ToggleMusic(musicOn);
        AudioBase.Instance.SetVolumeMusic(musicOn ? 0.6f : 0f);
        bool sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        AudioBase.Instance.ToggleSFX(sfxOn);
        AudioBase.Instance.SetVolumeSound(sfxOn ? PlayerPrefs.GetFloat("Sound", 1f) : 0f);
        AudioBase.Instance.SetMusicUI();
        CheckBtnSetting();
    }
    void Start()
    {
        CheckBtnSetting();
        SetTopBar();

        //
        buttonSetting.SetActive(true);
        buttonBack.SetActive(false);
        SetShowButtonPack();

        //opendaily reward
        if (System.DateTime.Now.Day != PlayerPrefs.GetInt("Yesterday", 0)&& PlayerPrefs.GetInt("is claimed yesterday", 0) == 0 && AudioBase.Instance.isCheckPlayed)
        {
            OpenPanel(11); // open dailyreward;
        }

        SetAvtBtn();

        popUpGetRewardNoAds.SetActive(false);
        popUpGteRewardStarterpack.SetActive(false);
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
            SetAvtBtn();
        }
        else
        {
            buttonSetting.SetActive(false);
            buttonBack.SetActive(true);
        }

        if(AudioBase.Instance.isOpenLevel)
        {
            AudioBase.Instance.isOpenLevel = false;
            ClossAllPanel();
            SetBottomBar(id);
            _panels[0].SetActive(true);
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
        for (int i = 0; i < _panels.Length; i++)
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
        if (_imgBtnSetting == null || _imgBtnSetting.Length < 2) return;

        bool sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        _imgBtnSetting[0].sprite = sfxOn ? _sprBtnSettingTrue[0] : _sprBtnSettingFalse[0];
        _imgBtnSetting[1].sprite = musicOn ? _sprBtnSettingTrue[1] : _sprBtnSettingFalse[1];
    }
    public void BtnSound()
    {
        AudioBase.Instance.SetAudioUI(0);
        bool newSfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 0;
        AudioBase.Instance.ToggleSFX(newSfxOn);
        AudioBase.Instance.SetVolumeSound(newSfxOn ? 1f : 0f);
        CheckBtnSetting();
    }
    public void BtnMusic()
    {
        AudioBase.Instance.SetAudioUI(0);
        bool newMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 0;
        AudioBase.Instance.ToggleMusic(newMusicOn);
        AudioBase.Instance.SetVolumeMusic(newMusicOn ? 0.6f : 0f);
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
    //public void BtnBuyVip()
    //{
    //    AudioBase.Instance.SetAudioUI(0);
    //    MG_Interface.Current.Purchase_Item(MG_ProductData.VIP_Pack.productId, (bool result, bool onIAP, string productId) =>
    //    {
    //        if (result)
    //        {
    //            AudioBase.Instance.SetAudioUI(1);
    //            DataManager.Instance.isCheckVip = true;
    //            CheckVIP();
    //            OpenPanel(3);
    //            DataManager.Instance.SaveFile();
    //        }
    //        else
    //        {
    //        }
    //    });
    //}
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
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + MG_ProductData.StarterPack.amount);
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + MG_ProductData.StarterPack.bonusCoin); // bounus coin
                DebugRollStarterPackRandomRewards();
                SetShowButtonPack();
                popUpGteRewardStarterpack.SetActive(true);
                OpenPanel(3);
                DataManager.Instance.SaveFile();
            }
            else
            {
            }
        });
    }


    public void DebugRollStarterPackRandomRewards()
    {
        var dm = DataManager.Instance;
        if (dm == null || dm.dataBase == null || dm.dataBase.imgEquipItems == null)
        {
            Debug.LogWarning("StarterPack roll: DataManager/DataBase/imgEquipItems is null.");
            return;
        }
        if (dm.warehouse == null)
        {
            Debug.LogWarning("StarterPack roll: warehouse is null.");
            return;
        }

        var db = dm.dataBase.imgEquipItems;
        EnsureWarehouseSizedForDb(dm, db);

        List<int> itemIds = PickUniqueRandomIds(db.sprItem != null ? db.sprItem.Count : 0, 3);
        List<int> pieceLevelUpIds = PickUniqueRandomIds(db.sprPiecePlayerLevelUp != null ? db.sprPiecePlayerLevelUp.Count : 0, 2);
        List<int> pieceEvolveIds = PickUniqueRandomIds(db.sprPiecePlayerEvolve != null ? db.sprPiecePlayerEvolve.Count : 0, 1);

        // Container has 6 reward slots:
        // 0-2: random items, 3-4: PlayerLevelUp pieces, 5: PlayerEvolve piece.
        Transform popContent = popUpGteRewardStarterpack.transform.GetChild(0).GetChild(2).transform;
        if (popContent.childCount < 6)
        {
            Debug.LogWarning($"StarterPack roll: popContent needs at least 6 children, current={popContent.childCount}.");
            return;
        }

        // 1) Random 3 item types, each quantity 1-3.
        for (int i = 0; i < itemIds.Count && i < 3; i++)
        {
            int id = itemIds[i];
            int quantity = UnityEngine.Random.Range(1, 4);

            dm.warehouse.CountItem[id] += quantity;
            if (dm.warehouse.ListItems != null && !dm.warehouse.ListItems.Contains(id))
                dm.warehouse.ListItems.Add(id);

            Sprite itemSprite = (db.sprItem != null && id >= 0 && id < db.sprItem.Count) ? db.sprItem[id] : null;
            SetRewardSlot(popContent.GetChild(i), itemSprite, quantity);
        }

        // 2) Random 2 PlayerLevelUp piece types, each quantity 3-5.
        for (int i = 0; i < pieceLevelUpIds.Count && i < 2; i++)
        {
            int id = pieceLevelUpIds[i];
            int quantity = UnityEngine.Random.Range(3, 6);

            dm.warehouse.CountPiecePlayerLevelUp[id] += quantity;

            Sprite pieceSprite = (db.sprPiecePlayerLevelUp != null && id >= 0 && id < db.sprPiecePlayerLevelUp.Count)
                ? db.sprPiecePlayerLevelUp[id]
                : null;
            SetRewardSlot(popContent.GetChild(3 + i), pieceSprite, quantity);
        }

        // 3) Random 1 PlayerEvolve piece type, quantity 3-5.
        if (pieceEvolveIds.Count > 0)
        {
            int id = pieceEvolveIds[0];
            int quantity = UnityEngine.Random.Range(3, 6);

            dm.warehouse.CountPiecePlayerEvolve[id] += quantity;

            Sprite pieceEvolveSprite = (db.sprPiecePlayerEvolve != null && id >= 0 && id < db.sprPiecePlayerEvolve.Count)
                ? db.sprPiecePlayerEvolve[id]
                : null;
            SetRewardSlot(popContent.GetChild(5), pieceEvolveSprite, quantity);
        }
    }

    private static void SetRewardSlot(Transform rewardSlot, Sprite sprite, int quantity)
    {
        if (rewardSlot == null || rewardSlot.childCount < 2) return;

        Image image = rewardSlot.GetChild(0).GetComponent<Image>();
        if (image != null) image.sprite = sprite;

        TextMeshProUGUI textAmount = rewardSlot.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (textAmount != null) textAmount.text = quantity.ToString();
    }

    private static List<int> PickUniqueRandomIds(int maxExclusive, int count)
    {
        List<int> result = new List<int>(count);
        if (maxExclusive <= 0 || count <= 0) return result;

        int target = Mathf.Min(count, maxExclusive);
        HashSet<int> chosen = new HashSet<int>();

        // hard cap attempts to avoid infinite loops
        int attempts = 0;
        int attemptCap = 1000;
        while (chosen.Count < target && attempts < attemptCap)
        {
            attempts++;
            int id = UnityEngine.Random.Range(0, maxExclusive);
            if (chosen.Add(id))
                result.Add(id);
        }
        return result;
    }

    private static void EnsureWarehouseSizedForDb(DataManager dm, ImgEquipItems db)
    {
        if (dm == null || dm.warehouse == null || db == null) return;

        if (dm.warehouse.CountItem == null) dm.warehouse.CountItem = new List<int>();
        if (dm.warehouse.CountPiecePlayerLevelUp == null) dm.warehouse.CountPiecePlayerLevelUp = new List<int>();
        if (dm.warehouse.CountPiecePlayerEvolve == null) dm.warehouse.CountPiecePlayerEvolve = new List<int>();

        EnsureListSize(dm.warehouse.CountItem, db.sprItem != null ? db.sprItem.Count : 0);
        EnsureListSize(dm.warehouse.CountPiecePlayerLevelUp, db.sprPiecePlayerLevelUp != null ? db.sprPiecePlayerLevelUp.Count : 0);
        EnsureListSize(dm.warehouse.CountPiecePlayerEvolve, db.sprPiecePlayerEvolve != null ? db.sprPiecePlayerEvolve.Count : 0);
        if (dm.warehouse.ListItems == null) dm.warehouse.ListItems = new List<int>();
    }

    private static void EnsureListSize(List<int> list, int size)
    {
        if (size <= 0) return;
        if (list == null) return;
        while (list.Count < size) list.Add(0);
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
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + MG_ProductData.NoAdsReward.amount);
                SetShowButtonPack();
                popUpGetRewardNoAds.SetActive(true);
                OpenPanel(3);
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
        int _levelEnableEquipEnemy = 0;
        ////_btnUpgrade[0].GetComponent<Image>().sprite = _sprAvtPlayer[dataManager.idPlayer];
        //_btnUpgrade[1].gameObject.SetActive(dataManager.LevelCurren >= _levelEnableEquipEnemy);
        //_btnUpgrade[2].gameObject.SetActive(dataManager.LevelCurren >= _levelEnableEquipEnemy);
        //if (dataManager.LevelCurren >= _levelEnableEquipEnemy)
        //{
        //    if (dataManager.idPet1 != 99)
        //    {
        //        _btnUpgrade[1].GetChild(0).GetComponent<Image>().sprite = _sprAvtEnemy[dataManager.idPet1];
        //        _btnUpgrade[1].GetChild(0).gameObject.SetActive(true);
        //    }
        //    else
        //        _btnUpgrade[1].GetChild(0).gameObject.SetActive(false);
        //    if (dataManager.idPet2 != 99)
        //    {
        //        _btnUpgrade[2].GetChild(0).GetComponent<Image>().sprite = _sprAvtEnemy[dataManager.idPet2];
        //        _btnUpgrade[2].GetChild(0).gameObject.SetActive(true);
        //    }
        //    else
        //        _btnUpgrade[2].GetChild(0).gameObject.SetActive(false);
        //}
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
        //_panels[0].GetComponent<LevelControllerMain>().OnInit();
        //_BarBottom.SetActive(false);
        //for (int i = 1; i < _panels.Length; i++)
        //{
        //    _panels[i].gameObject.SetActive(false);
        //}
        OpenPanel(0);
    }
    //
    public void SetMission(int id, int count)
    {
        if (id == dataManager.questsCurrent.IdQuest1)
        {
            int milestone = dataManager.dataBase.ListQuests[id].Milestone;
            if (dataManager.questsCurrent.ProgressQuest1 < milestone)
                dataManager.questsCurrent.ProgressQuest1 += count;
        }
        else if (id == dataManager.questsCurrent.IdQuest2)
        {
            int milestone = dataManager.dataBase.ListQuests[id].Milestone;
            if (dataManager.questsCurrent.ProgressQuest2 < milestone)
                dataManager.questsCurrent.ProgressQuest2 += count;
        }
        else if (id == dataManager.questsCurrent.IdQuest3)
        {
            int milestone = dataManager.dataBase.ListQuests[id].Milestone;
            if (dataManager.questsCurrent.ProgressQuest3 < milestone)
                dataManager.questsCurrent.ProgressQuest3 += count;
        }
        else if (id == dataManager.questsCurrent.IdQuest4)
        {
            int milestone = dataManager.dataBase.ListQuests[id].Milestone;
            if (dataManager.questsCurrent.ProgressQuest4 < milestone)
                dataManager.questsCurrent.ProgressQuest4 += count;
        }
    }

    // button shop from topbar
    public void BtnShop(int index)
    {
        indexPopupShop = index;
        OpenPanel(5);
    }
}
