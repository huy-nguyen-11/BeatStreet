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
    [SerializeField] Sprite[] _sprFrame;
    [SerializeField] Sprite[] _sprAutoSelect;
    [SerializeField] Sprite[] _sprStarItem;
    // Inventory
    [SerializeField] GameObject _prfListItem ;
    [SerializeField] Transform _content;
    ScrollSnapPagination _scrollSnapPagination;
    int level;
    int mode;
    int countStar;

    [SerializeField] GameObject _popUpChangeItem1 , _popUpChangeItem2;
    private int _indexItemSelected = 0;

    void Start()
    {
        dataManager = DataManager.Instance;
        _scrollSnapPagination = transform.GetComponent<ScrollSnapPagination>();
        OnInit();
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

    private void Update()
    {
    }

    public void OnInit()
    {
        //SetObjUnlockLevel();
        SetListBtn();
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
        AudioBase.Instance.SetAudioUI(0);
        if (id >= dataManager.LevelCurren) return;
        //if (id > 14)
        //{
        //    _popups[5].gameObject.SetActive(true);
        //    return;
        //}
        level = id;
        _popups[0].gameObject.SetActive(true); //pop level
        _popups[0].GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "LEVEL " + (id + 1).ToString(); // level text

        // Set reward
        _popups[0].GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id); // reward icon
        _popups[0].GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetSprLoot(id); // reward icon

        // select mode
        GameObject _modeGo = _popups[0].GetChild(0).GetChild(1).GetChild(1).gameObject; // mode game object
        TextMeshProUGUI _modeText = _popups[0].GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(); // text mode
        for (int i = 0; i < 3; i++)
        {
            _modeGo.transform.GetChild(i).GetComponent<Image>().sprite = i < dataManager.levelDatas[i].Star ? _sprStarLevel[1] : _sprStarLevel[0]; // set star mode
            //_popups[0].GetChild(1).GetChild(1).GetChild(i).GetChild(2).gameObject.SetActive(i > dataManager.levelDatas[id].Star);
            //if (i == 2 && dataManager.levelDatas[id].Star == 3)
            //{
            //    dataManager.LevelMode = i;
            //    _popups[0].GetChild(1).GetChild(1).GetChild(2).GetComponent<Image>().sprite = _sprFrame[1];
            //}
            //else if (i == dataManager.levelDatas[id].Star)
            //{
            //    dataManager.LevelMode = i;
            //    _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[1];
            //}
            //else
            //    _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[0];
        }
        // Take Item
        SetTakeItem();
        //_popups[0].GetChild(0).GetChild(5).GetComponent<Image>().sprite = dataManager.AutoSelect ? _sprAutoSelect[1] : _sprAutoSelect[0]; // icon is check
        _popups[0].GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(dataManager.AutoSelect); // text is check
    }
    private void SetTakeItem()
    {
        Transform itemButton1 = _popups[0].GetChild(0).GetChild(3); // item equip 1
        Transform itemButton2 = _popups[0].GetChild(0).GetChild(4); // item equip 2
        if (dataManager.AutoSelect)
        {
            if (dataManager.idItem1 == 99 || dataManager.idItem2 == 99)
            {
                if (dataManager.warehouse.ListItems.Count > 2)
                {
                    if (dataManager.idItem1 == 99)
                    {
                        int RandomId = Random.Range(0, dataManager.warehouse.ListItems.Count);
                        dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[RandomId]]--;
                        dataManager.idItem1 = dataManager.warehouse.ListItems[RandomId];
                        if (dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[RandomId]] <= 0)
                            dataManager.warehouse.ListItems.Remove(dataManager.warehouse.ListItems[RandomId]);
                    }
                    if (dataManager.idItem2 == 99)
                    {
                        int RandomId2 = Random.Range(0, dataManager.warehouse.ListItems.Count);
                        dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[RandomId2]]--;
                        dataManager.idItem2 = dataManager.warehouse.ListItems[RandomId2];
                        if (dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[RandomId2]] <= 0)
                            dataManager.warehouse.ListItems.Remove(dataManager.warehouse.ListItems[RandomId2]);
                    }
                }
                else
                {
                    if (dataManager.warehouse.ListItems.Count > 0 && dataManager.idItem1 == 99)
                    {
                        dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[0]]--;
                        dataManager.idItem1 = dataManager.warehouse.ListItems[0];
                        if (dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[0]] <= 0)
                            dataManager.warehouse.ListItems.Remove(dataManager.warehouse.ListItems[0]);
                    }
                    if (dataManager.warehouse.ListItems.Count > 0 && dataManager.idItem2 == 99)
                    {
                        dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[dataManager.warehouse.ListItems.Count == 2 ? 1 : 0]]--;
                        dataManager.idItem1 = dataManager.warehouse.ListItems[dataManager.warehouse.ListItems.Count == 2 ? 1 : 0];
                        if (dataManager.warehouse.CountItem[dataManager.warehouse.ListItems[dataManager.warehouse.ListItems.Count == 2 ? 1 : 0]] <= 0)
                            dataManager.warehouse.ListItems.Remove(dataManager.warehouse.ListItems.Count == 2 ? dataManager.warehouse.ListItems[1] : dataManager.warehouse.ListItems[0]);
                    }

                }
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
    }

    public void BtnMode(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (id <= dataManager.levelDatas[level].Star)
        {
            mode = id;
            dataManager.LevelMode = id;
            for (int i = 0; i < 3; i++)
            {
                if (i == id)
                    _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[1];
                else
                    _popups[0].GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().sprite = _sprFrame[0];
            }
        }
    }
    private Sprite GetSprLoot(int level)
    {
        int type = dataManager.levelDatas[level].Type;
        switch (type)
        {
            case 0:
                return dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[dataManager.levelDatas[level].idItem];
            case 1:
                return dataManager.dataBase.imgEquipItems.sprPiecePlayerEvolve[dataManager.levelDatas[level].idItem];
            case 2:
                return dataManager.dataBase.imgEquipItems.sprPieceEnemy[dataManager.levelDatas[level].idItem];
            case 3:
                return dataManager.dataBase.imgEquipItems.sprItem[dataManager.levelDatas[level].idItem];
            default:
                return dataManager.dataBase.imgEquipItems.sprPiecePlayerLevelUp[dataManager.levelDatas[level].idItem];
        }
    }
    private void SetListBtn()
    {
        for (int i = 0; i < _btnLevels.Length; i++) 
        {
            if (i < dataManager.LevelCurren)//is unlock
            {
                //if (CountStarUnlockLevel(i) > 0 && _UnlockLevel[CountStarUnlockLevel(i) - 1].gameObject.activeSelf
                //    )
                //{
                //    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[0];
                //    for (int y = 0; y < 4; y++) // 
                //        _btnLevels[i].GetChild(y).gameObject.SetActive(false);
                //}
                //else
                //{
                //    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[1];
                //    _btnLevels[i].GetChild(3).gameObject.SetActive(true);
                //    _btnLevels[i].GetChild(3).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                //    if (dataManager.levelDatas[i].Star == 0)
                //        for (int y = 0; y < 3; y++)
                //            _btnLevels[i].GetChild(y).gameObject.SetActive(false);
                //    else
                //        for (int y = 0; y < 3; y++)
                //        {
                //            _btnLevels[i].GetChild(y).gameObject.SetActive(true);
                //            _btnLevels[i].GetChild(y).GetComponent<Image>().sprite = y < dataManager.levelDatas[i].Star ? _sprStarLevel[1] : _sprStarLevel[0];
                //        }
                //}

                //set state button isUnLocked ( not select level )
                if(i != dataManager.LevelSelect)
                {
                    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[0];//base button 
                    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[1];//top base button
                    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(true); //text level
                    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(false); //text level selected
                    _btnLevels[i].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); //text level
                    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock
                }
                else
                {
                    //set state button isUnLocked ( select level )
                    _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[2];//base button 
                    _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[3];//top base button
                    _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(false); //text level
                    _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(true); //text level
                    _btnLevels[i].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); //text level selected
                    _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(false); // icon lock

                }
                //set star of level unloked
                if (dataManager.levelDatas[i].Star == 0)
                    for (int y = 1; y <= 3; y++)
                        _btnLevels[i].GetChild(y).GetComponent<Image>().sprite = _sprStarLevel[0]; // off all star
                else
                    for (int y = 1; y <=3; y++)
                    {
                        _btnLevels[i].GetChild(y).gameObject.SetActive(true);
                        _btnLevels[i].GetChild(y).GetComponent<Image>().sprite = y < dataManager.levelDatas[i].Star ? _sprStarLevel[1] : _sprStarLevel[0];
                    }
            }
            else //is lock
            {
                _btnLevels[i].GetComponent<Image>().sprite = _sprBtn[4];//base button 
                _btnLevels[i].transform.GetChild(0).GetComponent<Image>().sprite = _sprBtn[5];//top base button
                _btnLevels[i].GetChild(0).GetChild(0).gameObject.SetActive(false); //text level
                _btnLevels[i].GetChild(0).GetChild(1).gameObject.SetActive(false); //text level
                _btnLevels[i].GetChild(0).GetChild(2).gameObject.SetActive(true); // icon lock
                for (int y = 1; y <=3; y++)
                    _btnLevels[i].GetChild(y).GetComponent<Image>().sprite = _sprStarLevel[0];// off all star
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
            if (!dataManager.warehouse.ListItems.Contains(dataManager.idItem1))
                dataManager.warehouse.ListItems.Add(dataManager.idItem1);
            dataManager.warehouse.CountItem[dataManager.idItem1]++;
            dataManager.idItem1 = 99;
        }
        else
        {
            _popUpChangeItem2.SetActive(false);
            if (!dataManager.warehouse.ListItems.Contains(dataManager.idItem2))
                dataManager.warehouse.ListItems.Add(dataManager.idItem2);
            dataManager.warehouse.CountItem[dataManager.idItem2]++;
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

        Debug.Log("item is list warehouse: " + dataManager.warehouse.ListItems + " count of list:" + dataManager.warehouse.ListItems.Count);

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

                    // Set image icon
                    page.GetChild(i).GetChild(0).GetComponent<Image>().sprite =
                        dataManager.dataBase.imgEquipItems.sprItem[idItem];

                    // Set star icons
                    for (int y = 0; y < page.GetChild(i).GetChild(2).childCount; y++)
                    {
                        page.GetChild(i).GetChild(2).GetChild(y).gameObject.SetActive(
                            y < dataManager.dataBase.imgEquipItems.StarItems[idItem]);
                    }

                    // Set item count
                    page.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text =
                        dataManager.warehouse.CountItem[idItem].ToString();

                    // Set button event
                    Button btn = page.GetChild(i).GetComponent<Button>();
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
            dataManager.warehouse.CountItem[item]--;
            if (dataManager.warehouse.CountItem[item] <= 0 && dataManager.warehouse.ListItems.Contains(item))
                dataManager.warehouse.ListItems.Remove(item);
            dataManager.idItem1 = item;
        }
        else
        {
            dataManager.warehouse.CountItem[item]--;
            if (dataManager.warehouse.CountItem[item] <= 0 && dataManager.warehouse.ListItems.Contains(item))
                dataManager.warehouse.ListItems.Remove(item);
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
    private int GetIdPage(int id)
    {
        if (id < 9) return 0;
        else if (id < 18) return 1;
        else if (id < 27) return 2;
        else return 3;
    }
    private void SetSnapScrollview(int count)
    {
        _scrollSnapPagination.totalPages = count;
        List<Image> tempList = new List<Image>(_scrollSnapPagination.pageIndicators);
        tempList.Clear();
        for (int i = 0; i < _popups[1].GetChild(2).childCount; i++)
        {
            if (i < count)
            {
                _popups[1].GetChild(2).GetChild(i).gameObject.SetActive(true);
                tempList.Add(_popups[1].GetChild(2).GetChild(i).GetComponent<Image>());
            }
            else
            {
                _popups[1].GetChild(2).GetChild(i).gameObject.SetActive(false);
            }
        }
        _scrollSnapPagination.pageIndicators = tempList.ToArray();
        _scrollSnapPagination.Start();
    }

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
        if (PlayerPrefs.GetInt("Energy") >= 2)
        {
            PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - 2); 
            // Play Game
            dataManager.LevelSelect = level;
            _popups[4].gameObject.SetActive(true);
            Invoke(nameof(PlayGame), 0.5f);
        }
        else
        {
            _popups[3].gameObject.SetActive(true);
        }
    }
    private void PlayGame()
    {
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
}
