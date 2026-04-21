using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataObject", menuName = "DataManager/ScripTableObject")]

public class DataBase : ScriptableObject
{
    public ImgEquipItems imgEquipItems;
    // Quests
    public List<Quests> ListQuests = new List<Quests>();
    // DataUpGrade
    public List<int> HpUpgrade = new List<int>() { 0,18,18,18,18,19,20,19,21,20,22,22,23,24,
        24,26,27,28,29,31,33,34,37,38,42,44,47,50,54,58,63,68,73,80,86,94,102,111,122,132};
    public List<int> dameUpgrade = new List<int>() {0,2,1,2,2,2,2,2,1,2,2,3,2,
        2,2,2,3,2,3,3,3,3,3,3,4,4,4,4,5,5,5,6,6,7,7,8,8,9,10,11 };
    public List<ListLevelModeMap> listLevelModeMaps = new List<ListLevelModeMap>();
    public List<ListLevelEnemyUpgrade> listLevelEnemyUpgrades = new List<ListLevelEnemyUpgrade>();
    public List<ListModeLevelEnemyMap> listModeLevelEnemyMaps = new List<ListModeLevelEnemyMap>();
    // Count Star Unlock List Level
    public List<int> CountUnlockListLevel = new List<int>();
}
[System.Serializable]
public class Quests
{
    public string NameQuests;
    public int Milestone;
    public bool checkPrice;
    public int price;
}
[System.Serializable]
public class ImgEquipItems
{
    public List<Sprite> sprPiecePlayerLevelUp;
    public List<Sprite> sprPiecePlayerEvolve;
    public List<Sprite> sprPieceEnemy;
    public List<Sprite> sprItem;
    public List<Sprite> sprItemCommon; // coin , diamont, key
    public List<string> namePlayer;
    public List<string> nameEnemy;
    public List<string> nameItems;
    public List<string> titleAttributeItems;
    public List<int> AttributeItems;
    public List<int> StarItems;
}
[System.Serializable]
public class ListLevelModeMap
{
    public List<int> levelMode = new List<int>();
}
[System.Serializable]
public class ListLevelEnemyUpgrade
{
    public List<float> HP = new List<float>();
    public List<float> Dame = new List<float>();
}
[System.Serializable]
public class ListModeLevelEnemyMap
{
    public List<int> Mode = new List<int>();
}
