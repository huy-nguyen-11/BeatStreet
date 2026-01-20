using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance { get; private set; }
    DataManager dataManager;
    [SerializeField] TextMeshProUGUI _txtLevelPlayer;
    [SerializeField] GameObject[] _BtnGamePlays;
    [SerializeField] GameObject _tutorialGame;
    public PlayerController _Player;
    public EnemyController _Enemy;
    public bool isCheckUlti = false;
    private List<EnemyController> listEnemyBeforeUlti = new List<EnemyController>();
    public CameraFollow2D _CameraFollow;
    public GameObject _IconNextTurn;
    public GameObject _Pause;
    public GameObject _DarkScene;
    [SerializeField] Transform[] _BtnPause;
    [SerializeField] Sprite[] _sprBtnSettingTrue;
    [SerializeField] Sprite[] _sprBtnSettingFalse;
    [SerializeField] Sprite[] _sprRotateScreen;
    // GameOver
    public GameOver gameOver;
    // Item
    List<int> idSkill = new List<int>() { 0, 3, 6 };
    List<int> idDefence = new List<int>() { 1, 4, 7 };
    List<int> idAttack = new List<int>() { 2, 5, 8 };
    List<int> idHealMana = new List<int>() { 9, 10, 11 };
    List<int> idHealHp = new List<int>() { 12, 13, 14, 15, 16, 17 };
    List<int> idEquip = new List<int>() { 18, 19, 20, 21, 22, 23, 24, 25, 26 };
    Coroutine coroutineItem1;
    Coroutine coroutineItem2;
    public bool isActiveSkill;
    public bool isActiveDefence;
    // Game
    [SerializeField] Transform[] _Items;
    public List<float> attributePlayer = new List<float>();
    // Level
    public int levelMap;
    public GameObject[] _prfLevelMap;
    public LevelMap _levelMap;
    // Combo
    public TextMeshProUGUI[] txtCombos;
    // Coin
    public TextMeshProUGUI txtCoin;
    public int coin = 0;

    //enemy AI manager
    public Dictionary<int, bool> isEnemyOnLeft = new Dictionary<int, bool>();
    public Dictionary<int, bool> isEnemyOnRight = new Dictionary<int, bool>();
    public float minPosX , maxPosX , maxPosY , minPosY;

    //ulti
    [SerializeField] public GameObject backUlti;

    [SerializeField] private GameObject _showFightBoss;

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
        AudioBase.Instance.SetMusicGPL(0);
    }
    public void Start()
    {
        dataManager = DataManager.Instance;
        _Player = FindObjectOfType<PlayerController>();
        levelMap = dataManager.LevelSelect;
        _txtLevelPlayer.text = "Lv." + dataManager.playerData[_Player.id].Level.ToString();
        if (levelMap == 0)
        {
            Time.timeScale = 0;
            _Pause.SetActive(true);
            if (!_tutorialGame.activeSelf)
            {
                _Pause.SetActive(false);
                _tutorialGame.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                _tutorialGame.SetActive(false);
            }
        }
        SpawnMap();
        CheckAudio();
        SetItem();

        backUlti.SetActive(false);
        _showFightBoss.SetActive(false);
    }

    private void Update()
    {
        minPosX = _CameraFollow.transform.position.x - 2.1f;
        maxPosX = _CameraFollow.transform.position.x + 2.1f;
        //Debug.Log("range from: " + (_CameraFollow.transform.position.x - 2f) + "to" + (_CameraFollow.transform.position.x + 2f));
    }

    private void SpawnMap()
    {
        GameObject map = Instantiate(_prfLevelMap[levelMap], Vector3.zero, Quaternion.identity);
        _levelMap = map.GetComponent<LevelMap>();
    }
    public void SetBtnUlti(bool isActive)
    {
        _BtnGamePlays[0].SetActive(isActive);
    }
    public void BtnUlti()
    {
        AudioBase.Instance.SetAudioUI(0);
        if (_Player.state == PlayerCharacter.State.Dead) return;
        if (_Player.state == PlayerCharacter.State.Idle)
        {
            var (enemy, distance) = _Player.GetNearestEnemy();
            if (enemy != null && enemy.enemyController.state != EnemyCharacter.State.Fall
                && enemy.enemyController.state != EnemyCharacter.State.Hit)
            {
                Vector2 PlayerPos = _Player.transform.position;
                Vector2 EnemyPos = enemy.transform.position;
                if (Mathf.Abs(EnemyPos.y - PlayerPos.y) <= 0.5f)
                {
                    if (Mathf.Abs(EnemyPos.x - PlayerPos.x) <= 2f)
                    {
                        backUlti.SetActive(true);
                        isCheckUlti = true;
                        _BtnGamePlays[0].SetActive(false);
                        SetMission(8, 1);
                        GetEnemyBeforeUlti();// get enemy who is alive before ulti
                        _Player.SetMana(-100);
                        _Enemy = enemy.transform.GetChild(0).GetComponent<EnemyController>();
                        bool isEnemyOnRight = enemy.transform.position.x > _Player.transform.position.x;
                        SetCharsToCharSortingLayer();
                        _Player.Char.transform.position = new Vector3(_CameraFollow.transform.position.x - 0.5f ,_CameraFollow.transform.position.y - 1 , 0);
                        _Player.SetFacingDirection(true);
                        _Enemy.Char.transform.position = new Vector3(_CameraFollow.transform.position.x + 0.3f, _CameraFollow.transform.position.y -1 , 0);
                        _Enemy.transform.rotation = Quaternion.Euler(0, 180, 0);
                        _Enemy.SetUltiPlayer();
                        _Player.SetUltiPlayer();
                    }
                }
            }
        }
    }

    private void GetEnemyBeforeUlti()
    {
        for (int i = 0; i < _levelMap.listTurnEnemy.GetChild(_levelMap.TurnEnemy).childCount; i++)
        {
            if (_levelMap.listTurnEnemy.GetChild(_levelMap.TurnEnemy).GetChild(i).gameObject.activeSelf)
            {
                EnemyChar enemyChar = _levelMap.listTurnEnemy.GetChild(_levelMap.TurnEnemy).GetChild(i).GetComponent<EnemyChar>();
                if (enemyChar.enemyController.state != EnemyController.State.Dead)
                {
                    listEnemyBeforeUlti.Add(enemyChar.enemyController);
                }
            }
        }
    }

    public void SetCharsToCharSortingLayer()
    {
        SetSortingForTransform(_Player.transform, "Canvas", 2);
        SetSortingForTransform(_Player.transform.GetChild(1), "Canvas", 2);
        SetSortingForTransform(_Player.transform.GetChild(2), "Canvas", 2);
        SetSortingForTransform(_Enemy.transform, "Canvas", 2);
    }

    public void SetPlayerToDefaultSortingLayer()
    {
        SetSortingForTransform(_Player.transform, "Default", 5);
        SetSortingForTransform(_Player.transform.GetChild(1), "Default", 5);
        SetSortingForTransform(_Player.transform.GetChild(2), "Default", 7);
        SetSortingForTransform(_Enemy.transform, "Default", 6);
    }

    private void SetSortingForTransform(Transform root, string layerName, int order)
    {
        if (root == null) return;

        root.GetComponent<Renderer>().sortingLayerName = layerName;
        root.GetComponent<Renderer>().sortingOrder = order;
    }


    public void SetAnimCombo(int count)
    {
        foreach (var txt in txtCombos)
        {
            txt.DOKill();
            txt.DOFade(1, 0);
        }
        txtCombos[0].text = count.ToString();
        if (count > 9)
            SetMission(5, 1);
        SetOffTxtCombo();
    }
    private void SetOffTxtCombo()
    {
        foreach (var txt in txtCombos)
        {
            txt.DOFade(0, 1).SetDelay(1);
        }
    }
    public IEnumerator OpenPopupGameOver(int id)
    {
        yield return new WaitForSeconds(1f);
        if (id == 0)
            AudioBase.Instance.AudioGPl(0);
        yield return new WaitForSeconds(1.5f);
        if (id == 0)
        {
            SetDataWinGame();
            AudioBase.Instance.SetMusicGPL(2);
        }
        else if (id == 1)
        {
            AudioBase.Instance.StopMusic();
            AudioBase.Instance.AudioGPl(1);
        }
        gameOver.SetOpenPopup(id);
    }
    private void SetDataWinGame()
    {
        if (dataManager.LevelMode == dataManager.levelDatas[levelMap].Star)
        {
            if (dataManager.levelDatas[levelMap].Star < 3)
            {
                dataManager.levelDatas[levelMap].Star++;
            }
            if (levelMap + 1 == dataManager.LevelCurren)
            {
                dataManager.LevelCurren++;
            }
        }
        dataManager.SaveFile();
    }
    private void SetItem()
    {
        _Items[0].GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99);
        _Items[0].GetChild(1).gameObject.SetActive(dataManager.idItem1 != 99);
        _Items[2].GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99);
        _Items[1].GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99);
        _Items[1].GetChild(1).gameObject.SetActive(dataManager.idItem2 != 99);
        _Items[3].GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99);
        if (dataManager.idItem1 != 99)
        {
            _Items[0].GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem1];
            _Items[0].GetChild(1).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.titleAttributeItems[dataManager.idItem1];
            _Items[2].GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem1];
            _Items[2].GetComponent<Button>().onClick.AddListener(delegate
            {
                BtnItem(false);
            });
        }
        if (dataManager.idItem2 != 99)
        {
            _Items[1].GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem2];
            _Items[1].GetChild(1).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.titleAttributeItems[dataManager.idItem2];
            _Items[3].GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem2];
            _Items[3].GetComponent<Button>().onClick.AddListener(delegate
            {
                BtnItem(true);
            });
        }
    }
    public void BtnItem(bool check)
    {
        AudioBase.Instance.SetAudioUI(4);
        SetMission(10, 1);
        UseOfItems(!check ? dataManager.idItem1 : dataManager.idItem2);
        if (!check)
            dataManager.idItem1 = 99;
        else
            dataManager.idItem2 = 99;
        _Items[2].GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99);
        _Items[3].GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99);
    }
    private void UseOfItems(int id)
    {
        if (idSkill.Contains(id))
        {
            attributePlayer[0] = dataManager.dataBase.imgEquipItems.AttributeItems[id];
            StartCoroutine(SetItemSkill());
        }
        else if (idDefence.Contains(id))
        {
            attributePlayer[1] = dataManager.dataBase.imgEquipItems.AttributeItems[id];
            StartCoroutine(SetItemDefence());
        }
        else if (idAttack.Contains(id))
        {
            attributePlayer[2] = dataManager.dataBase.imgEquipItems.AttributeItems[id];
            StartCoroutine(SetItemDame());
        }
        else if (idHealMana.Contains(id))
        {
            attributePlayer[3] = dataManager.dataBase.imgEquipItems.AttributeItems[id];
            _Player.SetMana(_Player.fillBar.maxMana * (attributePlayer[3] / 100));
        }
        else if (idHealHp.Contains(id))
        {
            attributePlayer[4] = dataManager.dataBase.imgEquipItems.AttributeItems[id];
            _Player.SetHp(_Player.fillBar.maxHp * (attributePlayer[4] / 100f));
        }
        else if (idEquip.Contains(id))
        {

        }
    }
    IEnumerator SetItemSkill()
    {
        isActiveSkill = true;
        yield return new WaitForSeconds(_Player._attributesPet[2]);
        isActiveSkill = false;
    }
    IEnumerator SetItemDefence()
    {
        isActiveDefence = true;
        yield return new WaitForSeconds(_Player._attributesPet[2]);
        isActiveDefence = false;
    }
    IEnumerator SetItemDame()
    {
        float dameItem = _Player.Dame * (attributePlayer[2] / 100f);
        _Player.Dame += dameItem;
        yield return new WaitForSeconds(_Player._attributesPet[2]);
        _Player.Dame -= dameItem;
    }
    // for level game
    public void CheckEnemyDead()
    {
        // wake up enemy in turn
        _levelMap.WakeUpEnemyInTurn();
        
        int count = 0;
        for (int i = 0; i < _levelMap.listTurnEnemy.GetChild(_levelMap.TurnEnemy).childCount; i++)
            if (_levelMap.listTurnEnemy.GetChild(_levelMap.TurnEnemy)
                .GetChild(i).GetComponent<EnemyChar>().enemyController.state != EnemyController.State.Dead)
                count++;
        if (count <= 0)
        {
            if (_levelMap.TurnEnemy >= _levelMap.listTurnEnemy.childCount - 1)
            {
                _Player.SwitchToRunState(_Player.playerWingame);
                AudioBase.Instance.AudioPlayer(11);
                StartCoroutine(OpenPopupGameOver(0));
            }
            else
            {
                // Next Turn
                _IconNextTurn.SetActive(true);
                _levelMap.TurnEnemy++;

            }
        }
    }

    //for reset affter ulti
    // only reset enemy who is alive before ulti
    public void ResetAfterUlti()
    {
        foreach (var enemy in listEnemyBeforeUlti)
        {
            if (enemy.state != EnemyController.State.Dead)
            {
                enemy.SetRun();
            }
        }
    }

    public void SetNextTurn()
    {
        _DarkScene.SetActive(false);
        _DarkScene.SetActive(true);
        _IconNextTurn.SetActive(false);
    }
    public void SetOffDarkScene()
    {
        _DarkScene.SetActive(false);
        if (_levelMap.TurnEnemy == 3 && _levelMap._pointMaxXs.Length == 4)
        {
            ShowFightBoss();
        }
    }
    public void SetStopFollowCamera()
    {
        _CameraFollow.isFollow = false;
    }
    public void SetFollowCamera()
    {
        _CameraFollow.isFollow = true;
    }
    public void AddCoin()
    {
        coin++;
        txtCoin.text = coin.ToString();
        SetVfxCoin();
    }
    public void SetVfxCoin()
    {
        txtCoin.DOKill();
        txtCoin.transform.parent.GetComponent<Image>().DOKill();
        txtCoin.DOFade(1, 0);
        txtCoin.transform.parent.GetComponent<Image>().DOFade(1, 0);
        txtCoin.DOFade(0, 0.75f).SetDelay(0.5f);
        txtCoin.transform.parent.GetComponent<Image>().DOFade(0, 0.75f).SetDelay(0.5f);
    }
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
    // Revive
    public void SetPlayerRevive()
    {
        _Player.SetRevive();
        ResetAllEnemiesToIdle();
    }

    // Reset all alive enemies to idle state when player revives
    private void ResetAllEnemiesToIdle()
    {
        // Find all enemy controllers in the scene
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemy in allEnemies)
        {
            // Skip dead enemies
            if (enemy.state == EnemyController.State.Dead)
                continue;

            // Force switch to idle state (now that isGrabbed is false, this will work)
            if(enemy.isActiveRun)
                enemy.SwitchToRunState(enemy.enemyRun);
            if(enemy.state == EnemyController.State.Grabed)
                enemy.SwitchToRunState(enemy.enemyIdle);
            enemy.isGrabbed = false;
            enemy.SwitchToRunState(enemy.enemyIdle);
            enemy.StopAllCoroutines();
        }
    }
    // Pause
    public void BtnPause()
    {
        AudioBase.Instance.SetAudioUI(0);
        Time.timeScale = 0;
        _Pause.SetActive(true);
    }
    public void BtnContinue()
    {
        Time.timeScale = 1;
        AudioBase.Instance.SetAudioUI(0);
        _Pause.SetActive(false);
    }
    private void CheckAudio()
    {
        if (PlayerPrefs.GetFloat("Sound") > 0)
            _BtnPause[0].GetChild(0).GetComponent<Image>().sprite = _sprBtnSettingTrue[0];
        else
            _BtnPause[0].GetChild(0).GetComponent<Image>().sprite = _sprBtnSettingFalse[0];
        if (PlayerPrefs.GetFloat("Music") > 0)
        {
            _BtnPause[1].GetChild(0).GetComponent<Image>().sprite = _sprBtnSettingTrue[1];
        }
        else
        {
            _BtnPause[1].GetChild(0).GetComponent<Image>().sprite = _sprBtnSettingFalse[1];
        }
    }
    public void BtnSound()
    {
        AudioBase.Instance.SetAudioUI(0);
        int sound = PlayerPrefs.GetFloat("Sound") > 0 ? 0 : 1;
        AudioBase.Instance.SetVolumeSound(sound);
        CheckAudio();
    }
    public void BtnMusic()
    {
        AudioBase.Instance.SetAudioUI(0);
        int music = PlayerPrefs.GetFloat("Music") > 0 ? 0 : 1;
        AudioBase.Instance.SetVolumeMusic(music);
        CheckAudio();
    }
    public void BtnTutorialGame()
    {
        AudioBase.Instance.SetAudioUI(0);
        if (!_tutorialGame.activeSelf)
        {
            _Pause.SetActive(false);
            _tutorialGame.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            _tutorialGame.SetActive(false);
        }
    }
    public void BtnBackMain()
    {
        Time.timeScale = 1;
        AudioBase.Instance.SetAudioUI(0);
        AudioBase.Instance.isCheckPlayed = true;
        _DarkScene.SetActive(true);
        Invoke(nameof(BackMain), 0.5f);
    }
    private void BackMain()
    {
        /* if (DataManager.Instance.LevelCurren == 1)
             SceneManager.LoadSceneAsync("2_GamePlay");
         else*/
        SceneManager.LoadSceneAsync("1_Main");
    }

    public void ButtonRestar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Add near other serialized fields
    [SerializeField] private float showBossDuration = 1.2f;
    [SerializeField] private float cameraMoveDuration = 0.6f;
    public bool isShowingBoss = false;

    // Replace or modify existing ShowFightBoss() to start coroutine:
    public void ShowFightBoss()
    {
        if (_levelMap == null || _levelMap.pointBoss == null)
        {
            _showFightBoss.SetActive(true);
            StartCoroutine(WaitingForOff());
            return;
        }
        StartCoroutine(ShowFightBossCoroutine());
    }

    // Add this coroutine
    private IEnumerator ShowFightBossCoroutine()
    {
        isShowingBoss = true;

        // Block player input via flag on PlayerController (see PlayerController changes below)
        if (_Player != null) _Player.isInputBlocked = true;

        // Stop camera follow and tween camera to boss point
        bool prevFollow = _CameraFollow != null ? _CameraFollow.isFollow : true;
        if (_CameraFollow != null) _CameraFollow.isFollow = false;

        Vector3 bossCamPos = new Vector3(_levelMap.pointBoss.position.x, _CameraFollow.transform.position.y, _CameraFollow.transform.position.z);
        yield return _CameraFollow.transform.DOMove(bossCamPos, cameraMoveDuration).SetEase(Ease.OutCubic).WaitForCompletion();

        // show fight boss UI
        _showFightBoss.SetActive(true);
        yield return new WaitForSeconds(showBossDuration);
        _showFightBoss.SetActive(false);

        // return camera to player (center on player Char position)
        Vector3 playerCamPos = _Player != null && _Player.Char != null
            ? new Vector3(_Player.Char.position.x, _CameraFollow.transform.position.y, _CameraFollow.transform.position.z)
            : bossCamPos;
        yield return _CameraFollow.transform.DOMove(playerCamPos, cameraMoveDuration).SetEase(Ease.OutCubic).WaitForCompletion();

        // restore camera follow, UI and input
        if (_CameraFollow != null) _CameraFollow.isFollow = prevFollow;
        if (_Player != null) _Player.isInputBlocked = false;
        isShowingBoss = false;
    }

    IEnumerator WaitingForOff()
    {
        yield return new WaitForSeconds(1.2f);
        _showFightBoss.SetActive(false);
    }
}