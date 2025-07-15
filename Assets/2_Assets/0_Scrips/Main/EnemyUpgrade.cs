using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUpgrade : MonoBehaviour
{
    [SerializeField] Sprite[] _sprAvatarLock;
    [SerializeField] Sprite[] _sprAvatarUnLock;
    [SerializeField] Sprite[] _sprFrame;
    [SerializeField] Transform[] _btnEnemy;
    [SerializeField] Sprite[] _sprLock;
    [SerializeField] Sprite[] _sprPrice;
    [SerializeField] ScrollSnapPagination scrollSnap;
    DataManager _dataManager;
    int idPet;
    int piece;
    int pieceLevelUp;
    int price;
    private void OnEnable()
    {
        _dataManager = DataManager.Instance;
        GetIdPet();
        SetBtnUpgrade(idPet);
        SetBarFill(idPet);
        SetParameter(idPet);
        transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _sprAvatarUnLock[idPet];
        transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
        if (_dataManager.petData[idPet].Level < 20)
        {
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
            {
                BtnUpGradePet(idPet);
            });
        }
        SetBtnPet();
        if (scrollSnap != null)
            Invoke(nameof(PagePlayerSellect), 0.01f);
    }
    private void GetIdPet()
    {
        bool check = _dataManager.isActivePet1 ? false : true;
        if (!check)
        {
            if (_dataManager.idPet1 == 99)
                idPet = 0;
            else
                idPet = _dataManager.idPet1;
        }
        else
        {
            if (_dataManager.idPet2 == 99)
                idPet = 0;
            else
                idPet = _dataManager.idPet2;
        }
    }
    public void BtnActivce()
    {
        AudioBase.Instance.SetAudioUI(0);
        bool check = _dataManager.isActivePet1 ? false : true;
        if (_dataManager.petData[idPet].isUnlock)
        {
            if (!check)
            {
                if (idPet == _dataManager.idPet2)
                    _dataManager.idPet2 = _dataManager.idPet1 == 99 ? 99 : _dataManager.idPet1;
                else
                {
                    if (idPet == 4 && _dataManager.idPet1 != 4)
                    {
                        SetAttributeBlockless(true);
                    }
                    else if (idPet != 4 && _dataManager.idPet1 == 4)
                    {
                        SetAttributeBlockless(false);
                    }
                }
                _dataManager.idPet1 = idPet;
            }
            else
            {
                if (idPet == _dataManager.idPet1)
                    _dataManager.idPet1 = _dataManager.idPet2 == 99 ? 99 : _dataManager.idPet2;
                else
                {
                    if (idPet == 4 && _dataManager.idPet2 != 4)
                    {
                        SetAttributeBlockless(true);
                    }
                    else if (idPet != 4 && _dataManager.idPet2 == 4)
                    {
                        SetAttributeBlockless(false);
                    }
                }
                _dataManager.idPet2 = idPet;
            }
        }
        _dataManager.SaveFile();
        MainManager.Instance.OpenPanel(3);
    }
    private void SetAttributeBlockless(bool blockless)
    {
        if (!blockless)
            _dataManager.playerData[_dataManager.idPlayer].Mana -= _dataManager.petData[4].petAttribute;
        else
            _dataManager.playerData[_dataManager.idPlayer].Mana += _dataManager.petData[4].petAttribute;
        _dataManager.SaveFile();
    }
    private void SetBtnPet()
    {
        for (int i = 0; i < _btnEnemy.Length; i++)
        {
            if (_dataManager.petData[i].isUnlock)
            {
                _btnEnemy[i].GetChild(0).GetComponent<Image>().sprite = _sprAvatarUnLock[i];
                _btnEnemy[i].GetChild(1).gameObject.SetActive(false);
                _btnEnemy[i].GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                _btnEnemy[i].GetChild(0).GetComponent<Image>().sprite = _sprAvatarLock[i];
                _btnEnemy[i].GetChild(1).gameObject.SetActive(true);
                _btnEnemy[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = _dataManager.warehouse.CountPieceEnemy[i] + "/" + _dataManager.petData[i].countPieceUnlock;
                if (_dataManager.warehouse.CountPieceEnemy[i] >= _dataManager.petData[i].countPieceUnlock)
                {
                    _btnEnemy[i].GetChild(2).GetComponent<Image>().sprite = _sprLock[1];
                }
                else
                {
                    _btnEnemy[i].GetChild(2).GetComponent<Image>().sprite = _sprLock[0];
                }
            }
            if (i == idPet)
                _btnEnemy[i].GetComponent<Image>().sprite = _sprFrame[1];
            else
                _btnEnemy[i].GetComponent<Image>().sprite = _sprFrame[0];
        }
    }
    public void BtnEnemy(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        idPet = id;
        SetBtnUpgrade(id);
        SetBarFill(id);
        SetParameter(id);
        transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _sprAvatarUnLock[id];
        transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
        if (_dataManager.petData[id].Level < 20)
        {
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
            {
                BtnUpGradePet(id);
            });
        }
        SetBtnPet();
    }
    private void SetBtnUpgrade(int id)
    {
        if (_dataManager.petData[id].Level < 20)
        {
            string txtUpgrade = !_dataManager.petData[id].isUnlock ? "Unlock" : "Level Up";
            transform.GetChild(4).GetChild(0).gameObject.SetActive(_dataManager.petData[id].isUnlock);
            transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = txtUpgrade;
            transform.GetChild(4).GetChild(2).gameObject.SetActive(!_dataManager.petData[id].isUnlock);
            transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
            transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
            if (_dataManager.petData[id].isUnlock)
            {
                price = 50 + (75 * _dataManager.petData[id].Level);
                transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = price.ToString();
            }
        }
        else
        {
            transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
            transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
            transform.GetChild(4).GetChild(3).gameObject.SetActive(true);
        }
    }
    private void SetParameter(int id)
    {
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _dataManager.dataBase.imgEquipItems.nameEnemy[id].ToString();
        transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Level " + _dataManager.petData[id].Level;
        transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = SetAttibute(id);
    }
    private string SetAttibute(int id)
    {
        switch (id)
        {
            case 0:
                return "Increase max health by " + _dataManager.petData[id].petAttribute + "%";
            case 1:
                return "Raise attack by " + _dataManager.petData[id].petAttribute + "%";
            case 2:
                return "Power ups last longer with " + _dataManager.petData[id].petAttribute + "Seconds";
            case 3:
                return "Thrown weapon damage increases with " + _dataManager.petData[id].petAttribute + "%";
            case 4:
                return "Increase skill by " + _dataManager.petData[id].petAttribute + " (Enemies block less)";
            case 5:
                return "Weapon durability increases with " + _dataManager.petData[id].petAttribute + "%";
            case 6:
                return "Receive " + _dataManager.petData[id].petAttribute + " extra coins on mission complete";
            case 7:
                return "Take less damage from obstacles and projectiles with " + _dataManager.petData[id].petAttribute + "%";
            case 8:
                return "Start mission with " + _dataManager.petData[id].petAttribute + "% Combo meter filled";
            case 9:
                return "Increase weapon power by " + _dataManager.petData[id].petAttribute + "%";
            case 10:
                return "Gives " + _dataManager.petData[id].petAttribute + "% chance of a critical hit for dash attack";
            case 11:
                return "Gives " + _dataManager.petData[id].petAttribute + "% chance of a critical hit for jump attack";
            case 12:
                return "Gives " + _dataManager.petData[id].petAttribute + "% chance of a critical hit for charge attack";
            case 13:
                return "Gives " + _dataManager.petData[id].petAttribute + "% chance of a critical hit for final combo attack";
            case 14:
                return _dataManager.petData[id].petAttribute + "% chance of getting up after o death once";
            case 15:
                return "Combo meter fills up extra with " + _dataManager.petData[id].petAttribute + " combo";
            case 16:
                return _dataManager.petData[id].petAttribute + " seconds longer invincibility period after getting up.";
            case 17:
                return "Cancel any damage " + _dataManager.petData[id].petAttribute + "x";
            default:
                return "";
        }
    }
    private void SetBarFill(int id)
    {
        piece = _dataManager.warehouse.CountPieceEnemy[id];
        if (_dataManager.petData[id].isUnlock)
            pieceLevelUp = GetPieceLevelUp(id);
        else
            pieceLevelUp = _dataManager.petData[id].countPieceUnlock;
        transform.GetChild(3).GetChild(0).GetComponent<Image>().DOFillAmount((float)piece / (float)pieceLevelUp, 0);
        transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = piece + "/" + pieceLevelUp;
        transform.GetChild(3).GetChild(2).GetComponent<Image>().sprite = _dataManager.dataBase.imgEquipItems.sprPieceEnemy[id];
    }
    private int GetPieceLevelUp(int id)
    {
        int levelUp = _dataManager.petData[id].Level + 1;
        if (levelUp > 2)
            return 3;
        else
            return 2;
    }
    public void BtnUpGradePet(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (piece >= pieceLevelUp && PlayerPrefs.GetInt("Coin") >= price)
        {
            _dataManager.warehouse.CountPieceEnemy[id] -= pieceLevelUp;
            if (!_dataManager.petData[id].isUnlock)
            {
                AudioBase.Instance.SetAudioUI(3);
                _dataManager.petData[id].isUnlock = true;
            }
            else
            {
                AudioBase.Instance.SetAudioUI(4);
                PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") - price);
                MainManager.Instance.SetMission(1, pieceLevelUp);
                _dataManager.petData[id].Level++;
                SetAttribute(id);
                MainManager.Instance.SetTopBar();
                _dataManager.SaveFile();
            }
            idPet = id;
            SetBtnUpgrade(id);
            SetBarFill(id);
            SetParameter(id);
            transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = _sprAvatarUnLock[id];
            transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
            if (_dataManager.petData[id].Level < 20)
            {
                transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
                {
                    BtnUpGradePet(id);
                });
            }
            SetBtnPet();
        }
    }
    private void SetAttribute(int id)
    {
        List<int> idPet1 = new List<int>() { 0, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 17 };
        List<int> idPet2 = new List<int>() { 1, 3, 9 };
        if (idPet1.Contains(id))
            _dataManager.petData[id].petAttribute++;
        else if (idPet2.Contains(id))
            _dataManager.petData[id].petAttribute += GetAttribute(id);
        else
            _dataManager.petData[id].petAttribute += 0.5f;
    }
    private int GetAttribute(int id)
    {
        int level = _dataManager.petData[id].Level + 1;
        if (level == 2)
            return 5;
        else if (level == 3)
            return 4;
        else if (level == 4)
            return 3;
        else if (level == 5)
            return 2;
        else
            return 1;
    }
    private void PagePlayerSellect()
    {
        if (idPet < 6)
            scrollSnap.PagesSellect(0);
        else if (idPet > 5 && idPet < 12)
            scrollSnap.PagesSellect(1);
        else
            scrollSnap.PagesSellect(2);
    }
}
