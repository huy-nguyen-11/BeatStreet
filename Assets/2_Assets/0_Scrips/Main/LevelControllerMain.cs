using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelControllerMain : MonoBehaviour
{
    DataManager dataManager;
    [SerializeField] Transform[] _btnLevels;
    [SerializeField] Transform[] _UnlockLevel;
    [SerializeField] Transform[] _popups;
    [SerializeField] Sprite[] _sprBtn;
    [SerializeField] Sprite[] _sprStarLevel;
    //[SerializeField] Sprite[] _sprFrame;

    // Inventory
    [SerializeField] GameObject _prfListItem ;
    [SerializeField] Transform _content;

    //mode 
    [SerializeField] private Transform _modeSelect;
    [SerializeField] private Sprite starOn, starOff;
    [SerializeField] private TextMeshProUGUI textModeLevel;

    ScrollSnapPagination _scrollSnapPagination;
    int level;
    int mode;
    int countStar;

    [SerializeField] GameObject _popUpChangeItem1 , _popUpChangeItem2;
    private int _indexItemSelected = 0;


    void Start()
    {
        dataManager = DataManager.Instance;
        mode = dataManager.LevelMode;

        _scrollSnapPagination = transform.GetComponent<ScrollSnapPagination>();
        //OnInit();
    }

    private void OnEnable()
    {
        dataManager = DataManager.Instance;
        OnInit();

        foreach(Transform pop in _popups)
        {
            pop.gameObject.SetActive(false);
        }

        _popUpChangeItem1.SetActive(false);
        _popUpChangeItem2.SetActive(false);
    }

    public void OnInit()
    {
        //SetObjUnlockLevel();
        SetListBtn();
        UpdateModeButtons();
    }
    //private void SetObjUnlockLevel()
    //{
    //    for (int i = 0; i < _UnlockLevel.Length; i++)
    //    {
    //        if (GetCountStar() >= dataManager.dataBase.CountUnlockListLevel[i])
    //            _UnlockLevel[i].gameObject.SetActive(false);
    //        else
    //        {
    //            _UnlockLevel[i].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.CountUnlockListLevel[i].ToString();
    //            _UnlockLevel[i].gameObject.SetActive(true);
    //        }
    //    }
    //}
    //public int GetCountStar()
    //{
    //    int count = 0;
    //    for (int i = 0; i < dataManager.LevelCurren; i++)
    //    {
    //        count += dataManager.levelDatas[i].Star;
    //    }
    //    return count;
    //}
    public void BtnLevel(int id) //select level
    {
        //AudioBase.Instance.SetAudioUI(0);
        //if (id >= dataManager.LevelCurren) return;
        //level = id;
        //_popups[0].gameObject.SetActive(true); //pop level
        //_popups[0].GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "LEVEL " + (id + 1).ToString(); // level text

        //// Set reward
        //_popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id); // reward icon
        //_popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id); // reward icon

        //// select mode
        //GameObject _modeGo = _popups[0].GetChild(0).GetChild(1).GetChild(1).gameObject; // mode game object
        //TextMeshProUGUI _modeText = _popups[0].GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(); // text mode
        //for (int i = 0; i < 3; i++)
        //{
        //    _modeGo.transform.GetChild(i).GetComponent<Image>().sprite = (i < dataManager.levelDatas[i].Star) ? _sprStarLevel[1] : _sprStarLevel[0]; // set star mode
        //}

        //// Take Item
        //SetTakeItem();
        //_popups[0].GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(dataManager.AutoSelect); // text is check

        ////set mode
        //mode = dataManager.LevelMode;
        //SetStarImageModeLevel();
        //UpdateModeButtons();
        AudioBase.Instance.SetAudioUI(0);
        if (id >= dataManager.LevelCurren) return;

        level = id;
        _popups[0].gameObject.SetActive(true); // pop level
        _popups[0].GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "LEVEL " + (id + 1).ToString(); // level text

        // Set reward
        //_popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id,0);
        //_popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id,1);
        UpdateImageReward(id);

        // read star data for this level
        int starCount = dataManager.levelDatas[id].Star; // 0..3
        // clamp global saved mode to available stars for this level
        if (dataManager.LevelMode > starCount)
            dataManager.LevelMode = starCount;
        mode = dataManager.LevelMode; 
        // === Popup: stars (luôn cập nhật, show on/off) ===
        // primary star container (popup content) — đảm bảo đường dẫn đúng với hierarchy hiện tại
        Transform popupStarContainer = _popups[0].GetChild(0).GetChild(1).GetChild(1);
        for (int i = 0; i < 3; i++)
        {
            Image img = popupStarContainer.GetChild(i).GetComponent<Image>();
            img.sprite = (i < starCount) ? starOn : starOff; // on nếu i < starCount, else off
            img.gameObject.SetActive(true);

        }

        // Take Item
        SetTakeItem();
        _popups[0].GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(dataManager.AutoSelect); // text is check

        // Update global mode UI + buttons
        SetStarImageModeLevel();
        UpdateModeButtons();
    }
    private void SetTakeItem()
    {
        Transform itemButton1 = _popups[0].GetChild(0).GetChild(3); // item equip 1
        Transform itemButton2 = _popups[0].GetChild(0).GetChild(4); // item equip 2
        if (dataManager.AutoSelect)
        {
            if (dataManager.idItem1 == 99 || dataManager.idItem2 == 99)
            {
                // Auto-equip rules:
                // - Prefer items that exist in warehouse (CountItem > 0)
                // - Do NOT equip the same item into both slots unless CountItem[item] >= 2
                int GetCountSafe(int id)
                {
                    if (dataManager.warehouse == null || dataManager.warehouse.CountItem == null) return 0;
                    if (id < 0 || id >= dataManager.warehouse.CountItem.Count) return 0;
                    return dataManager.warehouse.CountItem[id];
                }

                int PickForSlot(int otherEquippedId)
                {
                    if (dataManager.warehouse == null || dataManager.warehouse.ListItems == null) return 99;
                    var list = dataManager.warehouse.ListItems;
                    if (list.Count == 0) return 99;

                    List<int> candidates = new List<int>(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        int id = list[i];
                        int count = GetCountSafe(id);
                        if (count <= 0) continue;

                        if (otherEquippedId != 99 && id == otherEquippedId && count < 2)
                            continue; // only 1 copy, cannot equip into both slots

                        candidates.Add(id);
                    }

                    if (candidates.Count == 0) return 99;
                    return candidates[Random.Range(0, candidates.Count)];
                }

                if (dataManager.idItem1 == 99)
                    dataManager.idItem1 = PickForSlot(dataManager.idItem2);
                if (dataManager.idItem2 == 99)
                    dataManager.idItem2 = PickForSlot(dataManager.idItem1);
            }
        }

        itemButton1.GetChild(1).gameObject.SetActive(dataManager.idItem1 != 99);
        itemButton2.GetChild(1).gameObject.SetActive(dataManager.idItem2 != 99);
        itemButton1.GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99);
        itemButton2.GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99);
        if (dataManager.idItem1 != 99)
        {
            itemButton1.GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem1];
        }
        if (dataManager.idItem2 != 99)
        {
            itemButton2.GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem2];
        }

        //back up
        //Transform itemButton1 = _popups[0].GetChild(0).GetChild(3); // item equip 1
        //Transform itemButton2 = _popups[0].GetChild(0).GetChild(4); // item equip 2
        //if (dataManager.AutoSelect)
        //{
        //    if (dataManager.idItem1 == 99 || dataManager.idItem2 == 99)
        //    {
        //        if (dataManager.warehouse.ListItems.Count > 2)
        //        {
        //            if (dataManager.idItem1 == 99)
        //            {
        //                int RandomId = Random.Range(0, dataManager.warehouse.ListItems.Count);
        //                dataManager.idItem1 = dataManager.warehouse.ListItems[RandomId];
        //            }
        //            if (dataManager.idItem2 == 99)
        //            {
        //                int RandomId2 = Random.Range(0, dataManager.warehouse.ListItems.Count);
        //                dataManager.idItem2 = dataManager.warehouse.ListItems[RandomId2];
        //            }
        //        }
        //        else
        //        {
        //            if (dataManager.warehouse.ListItems.Count > 0 && dataManager.idItem1 == 99)
        //            {
        //                dataManager.idItem1 = dataManager.warehouse.ListItems[0];
        //            }
        //            if (dataManager.warehouse.ListItems.Count > 0 && dataManager.idItem2 == 99)
        //            {
        //                dataManager.idItem2 = dataManager.warehouse.ListItems[dataManager.warehouse.ListItems.Count == 2 ? 1 : 0];
        //            }

        //        }
        //    }
        //}
    }
     
    public void BtnMode(int id)
    {
        //AudioBase.Instance.SetAudioUI(0);
        //if (id <= dataManager.levelDatas[level].Star)
        //{
        //    mode = id;
        //    dataManager.LevelMode = id;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (i == id)
        //            _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[1];
        //        else
        //            _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[0];
        //    }
        //}
        AudioBase.Instance.SetAudioUI(0);

        int starCount = dataManager.levelDatas[level].Star;

        // Allow change only if this mode is unlocked for the current level
        if (id <= starCount)
        {
            mode = id;
            dataManager.LevelMode = id;

            // Refresh popup stars to match earned stars (show off sprite for locked/unearned)
            Transform popupStarContainer = _popups[0].GetChild(0).GetChild(1).GetChild(1);
            for (int i = 0; i < 3; i++)
            {
                popupStarContainer.GetChild(i).GetComponent<Image>().sprite = (i < starCount) ? _sprStarLevel[1] : _sprStarLevel[0];
                popupStarContainer.GetChild(i).gameObject.SetActive(true);
            }

            // Update global mode UI + buttons
            SetStarImageModeLevel();
            UpdateModeButtons();
        }
        else
        {
            // feedback for locked mode (optional)
            AudioBase.Instance.SetAudioUI(1);
        }
    }

    public void ButtonNextMode()
    {
        mode++;
        if (mode > 2)
            mode = 2;
        dataManager.LevelMode = mode;
        SetStarImageModeLevel();
        UpdateModeButtons();
    }

    public void ButtonPreviousMode()
    {
        mode--;
        if (mode < 0)
            mode = 0;
        dataManager.LevelMode = mode;
        SetStarImageModeLevel();
        UpdateModeButtons();
    }

    [SerializeField] private Button btnNextMode;
    [SerializeField] private Button btnPreviousMode;

    // Call this after selecting a level or changing mode
    private void UpdateModeButtons()
    {
        int unlockedModes = dataManager.levelDatas[level].Star + 1; // Modes unlocked for this level

        btnPreviousMode.interactable = mode > 0;
        btnNextMode.interactable = mode < unlockedModes - 1;

        //update img reward
        UpdateImageReward(level);
    }

    void UpdateImageReward(int _level)
    {
        if (dataManager.levelDatas[_level].starRewards[dataManager.LevelMode].isPassed)
        {
            _popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(true); 
            _popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            _popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(_level, 0);
            _popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(_level, 1);

            _popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(1).gameObject.SetActive(false);
            _popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(1).gameObject.SetActive(false);
        }
    }

    void SetStarImageModeLevel()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i <= mode)
            {
                _modeSelect.GetChild(i).GetComponent<Image>().sprite = starOn;
            }
            else
            {
                _modeSelect.GetChild(i).GetComponent<Image>().sprite = starOff;
            }
        }
        textModeLevel.text = mode == 0 ? "EASY" : mode == 1 ? "MEDIUM" : "HARD";
    }

    private Sprite GetSprLoot(int level , int typeOfSlot)
    {
        int type1 = dataManager.levelDatas[level].starRewards[dataManager.LevelMode].rewards[0].itemId; // item 1 id as loot
        int type2 = dataManager.levelDatas[level].starRewards[dataManager.LevelMode].rewards[1].itemType; // item 2 type as loot
        Sprite _sprite = null;

        if (type1 > 66 && typeOfSlot == 0) // if get item is coin/gem/key
        {
            if (type1 == 666)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprItemCommon[0];
            }
            else if (type1 == 661)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprItemCommon[1];
            }
            else if(type1 == 662)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprItemCommon[2];
            }
        }

        if (typeOfSlot == 1 && type2 != 666) // if get item 2 and it's not coin/gem/key
        {

            if (type2 == 0)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[dataManager.levelDatas[level].starRewards[dataManager.LevelMode].rewards[1].itemId]; // player level up
            }
            else if (type2 == 1)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[dataManager.levelDatas[level].starRewards[dataManager.LevelMode].rewards[1].itemId]; // player evolve
            }
            else if (type2 == 2)
            {
                _sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.levelDatas[level].starRewards[dataManager.LevelMode].rewards[1].itemId]; // item booster
            }
        }

        return _sprite;

        //switch (type1)
        //{
        //    case 0:
        //        return dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[dataManager.levelDatas[level].idItem];
        //    case 1:
        //        return dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[dataManager.levelDatas[level].idItem];
        //    case 2:
        //        return dataManager.dataBase.imgEquipItems.sprItem[dataManager.levelDatas[level].idItem];
        //    default:
        //        return dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[dataManager.levelDatas[level].idItem];
        //}
    }
    private void SetListBtn()
    {
        int highestUnlockedIndex = (dataManager.LevelCurren > 0) ? Mathf.Min(dataManager.LevelCurren - 1, _btnLevels.Length - 1) : -1;
        for (int i = 0; i < _btnLevels.Length; i++)
        {
            if (i < dataManager.LevelCurren) // unlocked
            {
                //// set state button isUnLocked ( not select level )
                //if (i != dataManager.LevelSelect)
                //{
                //    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[0]; // base button 
                //    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[1]; // top base button
                //    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(true); // text level
                //    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(false); // text level selected
                //    _btnLevels[i].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); // text level
                //    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock
                //}
                //else
                //{
                //    // set state button isUnLocked ( select level )
                //    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[2]; // base button 
                //    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[3]; // top base button
                //    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(false); // text level
                //    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(true); // text level selected
                //    _btnLevels[i].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); // text level selected
                //    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock
                //}
                bool isSelectedVisual = (i == highestUnlockedIndex);

                // set state button for unlocked (either selected visual or normal unlocked)
                if (!isSelectedVisual)
                {
                    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[0]; // base button 
                    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[1]; // top base button
                    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(true); // text level
                    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(false); // text level selected
                    _btnLevels[i].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); // text level
                    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock
                }
                else
                {
                    // visual selected state applied to the highest unlocked level
                    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[2]; // base button 
                    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[3]; // top base button
                    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(false); // text level
                    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(true); // text level selected
                    _btnLevels[i].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); // text level selected
                    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock
                }

                // show star indicators and set on/off based on earned stars
                int starCount = dataManager.levelDatas[i].Star; // 0..3
                for (int y = 1; y <= 3; y++)


                {
                    var starTransform = _btnLevels[i].GetChild(y);
                    starTransform.gameObject.SetActive(true); // always visible for unlocked levels
                    starTransform.GetComponent<Image>().sprite = (y <= starCount) ? _sprStarLevel[1] : _sprStarLevel[0];
                }
            }
            else // locked
            {
                _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[4]; // base button 
                _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[5]; // top base button
                _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(false); // text level
                _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(false); // text level
                _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(true); // icon lock

                // show stars as "off" for locked levels
                for (int y = 1; y <= 3; y++)
                {
                    var starTransform = _btnLevels[i].GetChild(y);
                    starTransform.gameObject.SetActive(true);
                    starTransform.GetComponent<Image>().sprite = _sprStarLevel[0];
                }
            }
        }
    }
    private int CountStarUnlockLevel(int level)
    {
        if (level < 9)
            return 0;
        else if (level < 19)
            return 1;
        else if (level < 28)
            return 2;
        else if (level < 37)
            return 3;
        else
            return 4;
    }
    public void BtnItem(int item)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (item == 0)
        {
            if (dataManager.idItem1 == 99)
            {
                OpenWarehouse(item);
            }
            else
            {
                //TitleItem(dataManager.idItem1);
                //_popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                //_popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
                //{
                //    BtnClosePopup(2);
                //});
                _popUpChangeItem1.SetActive(true);
                _popUpChangeItem1.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.titleAttributeItems[dataManager.idItem1];

            }
        }
        if (item == 1)
        {
            if (dataManager.idItem2 == 99)
            {
                OpenWarehouse(item);
            }
            else
            {
                //TitleItem(dataManager.idItem2 );
                //_popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                //_popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
                //{
                //    BtnClosePopup(2);
                //});
                _popUpChangeItem2.SetActive(true);
                _popUpChangeItem2.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.titleAttributeItems[dataManager.idItem2];
            }
        }

        StartCoroutine(ClosePopChange());
    }

    IEnumerator ClosePopChange()
    {
        if(_popUpChangeItem1.activeSelf || _popUpChangeItem2.activeSelf)
        {
            yield return new WaitForSeconds(2f);
            _popUpChangeItem1.SetActive(false);
            _popUpChangeItem2.SetActive(false);

        }
        yield return null;
    }

    public void BtnXItem(int item)
    {
        AudioBase.Instance.SetAudioUI(0);

        if (item == 0)
        {
            _popUpChangeItem1.SetActive(false);
            dataManager.idItem1 = 99;
        }
        else
        {
            _popUpChangeItem2.SetActive(false);
            dataManager.idItem2 = 99;
        }
        _popups[0].GetChild(0).GetChild(3).GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99); // item 1 icon
        _popups[0].GetChild(0).GetChild(3).GetChild(1).gameObject.SetActive(dataManager.idItem1 != 99); // item 1 icon remove
        _popups[0].GetChild(0).GetChild(4).GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99); //item 2 icon
        _popups[0].GetChild(0).GetChild(4).GetChild(1).gameObject.SetActive(dataManager.idItem2 != 99); // item 2 icon remove
    }

    public void ButtonChange(int item)
    {
        OpenWarehouse(item);
    }

    public void BtnAutoSelect()
    {
        AudioBase.Instance.SetAudioUI(0);
        dataManager.AutoSelect = !dataManager.AutoSelect;
        //_popups[0].GetChild(1).GetChild(2).GetChild(2).GetComponent<Image>().sprite = dataManager.AutoSelect ? _sprAutoSelect[1] : _sprAutoSelect[0];
        _popups[0].GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(dataManager.AutoSelect); // text is check
        dataManager.SaveFile();
    }
    public void BtnClosePopup(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        _popups[id].gameObject.SetActive(false);
    }
    private void OpenWarehouse(int item)
    {
        _popups[1].gameObject.SetActive(true);

        Debug.Log(" count of list:" + dataManager.warehouse.ListItems.Count);
        for (int i = 0; i < dataManager.warehouse.ListItems.Count; i++)
        {
            Debug.Log("name of iem" + dataManager.warehouse.ListItems[i].ToString());
        }

        int totalPages = 3;

        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
        {
            Transform page = _content.transform.GetChild(pageIndex);
            for (int i = 0; i < page.childCount; i++)
            {
                int globalItemIndex = pageIndex * page.childCount + i;

                if (globalItemIndex < dataManager.warehouse.ListItems.Count)
                {
                    int idItem = dataManager.warehouse.ListItems[globalItemIndex];

                    // Re-enable UI elements in case this slot was previously hidden
                    page.GetChild(i).GetChild(0).gameObject.SetActive(true); // icon item
                    page.GetChild(i).GetChild(1).gameObject.SetActive(idItem == dataManager.idItem1 || idItem == dataManager.idItem2); // icon equip
                    page.GetChild(i).GetChild(2).gameObject.SetActive(true); // stars
                    page.GetChild(i).GetChild(3).gameObject.SetActive(true); // count
                    page.GetChild(i).gameObject.SetActive(true);

                    // Set image icon
                    page.GetChild(i).GetChild(0).GetComponent<Image>().sprite =
                        dataManager.dataBase.imgEquipItems.sprItem[idItem];
                    page.GetChild(i).GetChild(0).GetComponent<Image>().preserveAspect = true;
                    // Set star icons
                    for (int y = 0; y < page.GetChild(i).GetChild(2).childCount; y++)
                    {
                        page.GetChild(i).GetChild(2).GetChild(y).gameObject.SetActive(
                            y < dataManager.dataBase.imgEquipItems.StarItems[idItem] +1);
                    }

                    // Set item count
                    page.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text =
                        dataManager.warehouse.CountItem[idItem].ToString();

                    // Set button event
                    Button btn = page.GetChild(i).GetComponent<Button>();
                    btn.interactable = true;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        _indexItemSelected = globalItemIndex >=9 ? globalItemIndex - 9 : globalItemIndex;
                        TitleItem(idItem);
                        Transform confirmBtn = _popups[2].GetChild(0).GetChild(4);
                        confirmBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                        confirmBtn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            BtnActiveItem(item, idItem);
                        });
                    });
                }
                else
                {
                    // Hide UI elements for empty slot
                    page.GetChild(i).GetChild(0).gameObject.SetActive(false); // icon item
                    page.GetChild(i).GetChild(1).gameObject.SetActive(false); // icon equip
                    page.GetChild(i).GetChild(2).gameObject.SetActive(false); // stars
                    page.GetChild(i).GetChild(3).gameObject.SetActive(false); // count
                    page.GetChild(i).GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    //private void OpenWarehouse(int item)
    //{
    //    _popups[1].gameObject.SetActive(true);
    //    ///
    //    Transform _page0 = _content.transform.GetChild(0);

    //    Debug.Log("item is list warehoause: " + dataManager.warehouse.ListItems + "count of list:" + dataManager.warehouse.ListItems.Count);

    //    for (int i = 0; i < _page0.childCount; i++)
    //    {
    //        if(i < dataManager.warehouse.ListItems.Count) // have item in list
    //        {
    //            //set image icon or item
    //            _page0.GetChild(i).GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.warehouse.ListItems[i]];// icon item
    //            for (int y = 0; y < _page0.GetChild(i).GetChild(2).childCount; y++) // check stars
    //            {
    //                _page0.GetChild(i).GetChild(2).GetChild(y).gameObject.SetActive(y < dataManager.dataBase.imgEquipItems.StarItems[dataManager.warehouse.ListItems[i]]); // active star item
    //            }
    //            _page0.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text = dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[i]].ToString(); // count item

    //            //set event for button
    //            _page0.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
    //            _page0.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate
    //            {
    //                int idItem = dataManager.warehouse.ListItems[i];
    //                TitleItem(idItem);
    //                _popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
    //                _popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
    //                {
    //                    BtnActiveItem(item, idItem);
    //                });
    //            });

    //        }
    //        else // list item empty or not enough item
    //        {
    //            // deactive item
    //            _page0.GetChild(i).GetChild(0).gameObject.SetActive(false); // icon item
    //            _page0.GetChild(i).GetChild(1).gameObject.SetActive(false); // icon equip item
    //            _page0.GetChild(i).GetChild(2).gameObject.SetActive(false); // icon star item
    //            _page0.GetChild(i).GetChild(3).gameObject.SetActive(false); // count item
    //        }
    //    }

    //    //StartCoroutine(SetWarehouse(item));
    //}


    //IEnumerator SetWarehouse(int item)
    //{
    //    int totalItems = dataManager.warehouse.ListItems.Count;
    //    int itemsPerPage = 9;
    //    int totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
    //    if (_content.childCount > 0)
    //        for (int i = 0; i < _content.childCount; i++)
    //            Destroy(_content.GetChild(i).gameObject);
    //    yield return new WaitForSeconds(0.01f);
    //    if (totalPages == 0)
    //    {
    //        Instantiate(_prfListItem, _content.transform.position, Quaternion.identity, _content.transform);
    //        for (int i = 0; i < _content.GetChild(0).childCount; i++)
    //        {
    //            for (int y = 0; y < _content.GetChild(0).GetChild(i).childCount; y++)
    //                _content.GetChild(0).GetChild(i).GetChild(y).gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < totalPages; i++)
    //        {
    //            Instantiate(_prfListItem, _content.transform.position, Quaternion.identity, _content.transform);
    //        }
    //        for (int j = 0; j < totalPages * 9; j++)
    //        {
    //            _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetComponent<Button>().onClick.RemoveAllListeners();
    //            if (j < dataManager.warehouse.ListItems.Count)
    //            {
    //                int idItem = dataManager.warehouse.ListItems[j];
    //                _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.warehouse.ListItems[j]];
    //                _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetChild(1).GetComponent<Image>().sprite = _sprStarItem[dataManager.dataBase.imgEquipItems.StarItems[dataManager.warehouse.ListItems[j]]];
    //                _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetChild(2).GetComponent<TextMeshProUGUI>().text = dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[j]].ToString();
    //                _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetComponent<Button>().onClick.AddListener(delegate
    //                {
    //                    TitleItem(idItem);
    //                    _popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
    //                    _popups[2].GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
    //                    {
    //                        BtnActiveItem(item, idItem);
    //                    });
    //                });
    //            }
    //            else
    //            {
    //                for (int y = 0; y < _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).childCount; y++)
    //                    _content.GetChild(GetIdPage(j)).GetChild(j - (GetIdPage(j) * 9)).GetChild(y).gameObject.SetActive(false);
    //            }
    //        }
    //    }
    //    SetSnapScrollview(totalPages);
    //}


    public void BtnActiveItem(int btn, int item)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (btn == 0)
        {
            dataManager.idItem1 = item;
        }
        else
        {
            dataManager.idItem2 = item;
        }
        GetItemActive();
        _popups[1].gameObject.SetActive(false);
        _popups[2].gameObject.SetActive(false);
    }
    private void GetItemActive()
    {
        //Transform item1 = _popups[0].GetChild(1).GetChild(2).GetChild(0);
        //Transform item2 = _popups[0].GetChild(1).GetChild(2).GetChild(1);

        Transform item1 = _popups[0].GetChild(0).GetChild(3); // item equip 1
        Transform item2 = _popups[0].GetChild(0).GetChild(4); // item equip 2

        item1.GetChild(1).gameObject.SetActive(dataManager.idItem1 != 99);
        item2.GetChild(1).gameObject.SetActive(dataManager.idItem2 != 99);

        item1.GetChild(0).gameObject.SetActive(dataManager.idItem1 != 99);
        item2.GetChild(0).gameObject.SetActive(dataManager.idItem2 != 99);
        if (dataManager.idItem1 != 99)
        {
            item1.GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem1];
        }
        if (dataManager.idItem2 != 99)
        {
            item2.GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[dataManager.idItem2];
        }
    }

    //private int GetIdPage(int id)
    //{
    //    if (id < 9) return 0;
    //    else if (id < 18) return 1;
    //    else if (id < 27) return 2;
    //    else return 3;
    //}

    //private void SetSnapScrollview(int count)
    //{
    //    _scrollSnapPagination.totalPages = count;
    //    List<Image> tempList = new List<Image>(_scrollSnapPagination.pageIndicators);
    //    tempList.Clear();
    //    for (int i = 0; i < _popups[1].GetChild(2).childCount; i++)
    //    {
    //        if (i < count)
    //        {
    //            _popups[1].GetChild(2).GetChild(i).gameObject.SetActive(true);
    //            tempList.Add(_popups[1].GetChild(2).GetChild(i).GetComponent<Image>());
    //        }
    //        else
    //        {
    //            _popups[1].GetChild(2).GetChild(i).gameObject.SetActive(false);
    //        }
    //    }
    //    _scrollSnapPagination.pageIndicators = tempList.ToArray();
    //    _scrollSnapPagination.Start();
    //}

    private void TitleItem(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        _popups[2].gameObject.SetActive(true);
        int _page = (int)(_indexItemSelected / 9);

        Vector3 pos = _content.transform.GetChild(_page).GetChild(_indexItemSelected).localPosition;
        _popups[2].gameObject.transform.localPosition =new Vector3(pos.x + 25, pos.y - 220f, 0);
        //_popups[2].GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = dataManager.dataBase.imgEquipItems.sprItem[id];
        //_popups[2].GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>().sprite = _sprStarItem[dataManager.dataBase.imgEquipItems.StarItems[id]];
        //_popups[2].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.nameItems[id];

        _popups[2].GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = dataManager.dataBase.imgEquipItems.titleAttributeItems[id]; //text attribute item
    }
    public void BtnPlay()
    {
        AudioBase.Instance.SetAudioUI(0);
        //if (PlayerPrefs.GetInt("Energy") >= 2)
        //{
        //    PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - 2); 
        //    // Play Game
        //    dataManager.LevelSelect = level;
        //    _popups[4].gameObject.SetActive(true);
        //    Invoke(nameof(PlayGame), 0.5f);
        //}
        //else
        //{
        //    _popups[3].gameObject.SetActive(true);
        //}
        // Play Game
        dataManager.LevelSelect = level;
        _popups[4].gameObject.SetActive(true);
        Invoke(nameof(PlayGame), 0.5f);
    }
    private void PlayGame()
    {
        //SceneManager.LoadSceneAsync("2_GamePlay");
        SceneManager.LoadSceneAsync("2_GamePlay");
    }
    public void BtnBuyEnergy()
    {
        AudioBase.Instance.SetAudioUI(0);
        if (PlayerPrefs.GetInt("Diamont") >= 40)
        {
            AudioBase.Instance.SetAudioUI(2);
            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") - 40);
            PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") + 20);
            MainManager.Instance.SetMission(0, 40);
            MainManager.Instance.SetTopBar();
            _popups[3].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Unlock toàn bộ level để phục vụ test.
    /// Gọi hàm này từ nút debug / editor button.
    /// </summary>
    public void UnlockAllLevelsForTest()
    {
        // Mở khóa tất cả level tương ứng với số nút level đang có
        dataManager.LevelCurren = _btnLevels.Length;
        //Debug.Log("All levels unlocked for testing. Current level count: " + dataManager.LevelCurren);
        // Lưu lại dữ liệu và cập nhật lại UI nút level
        dataManager.SaveFile();
        SetListBtn();
    }
}
