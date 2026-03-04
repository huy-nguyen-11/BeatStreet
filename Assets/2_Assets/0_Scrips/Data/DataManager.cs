using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public DataBase dataBase;
    private string dataPath;
    // GamePlay
    public List<LevelData> levelDatas;
    public int LevelCurren;
    public int LevelSelect;
    public int LevelMode;
    public int IdPlayerCurren;
    public bool isRotateScreen;
    // Vip
    public bool isCheckVip;
    // Spin
    public bool isFreeAdsSpin;
    public int priceSpin = 100;
    // Quests
    public QuestsCurrent questsCurrent;
    // Deal
    public List<DealsCurrent> dealsCurrent;
    // Kho đồ
    public Warehouse warehouse;
    public int idItem1, idItem2 = 99;
    public bool AutoSelect;
    // UpGrade
    public List<PlayerData> playerData;
    public List<EnemyData> enemyData;
    public List<PetData> petData;
    public int idPlayer;
    public int idPet1, idPet2 = 99;
    public bool isCheckIdPet;
    public bool isActivePet1, isActivePet2;

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
    }
    private void OnEnable()
    {
        dataPath = Application.persistentDataPath + "/data.json";
        Debug.Log("Data Path: " + dataPath);
        if (PlayerPrefs.GetInt("DataDefault") == 0)
        {
            PlayerPrefs.SetInt("DataDefault", 1);
            PlayerPrefs.SetInt("Coin", 100);
            PlayerPrefs.SetInt("Diamont", 100);
            PlayerPrefs.SetInt("Key", 0);
            PlayerPrefs.SetInt("Energy", 20);
            PlayerPrefs.SetFloat("Sound", 1);
            PlayerPrefs.SetFloat("Music", 1);
            SaveFile();
        }
        else
        {
            LoadFile();
        }
        DontDestroyOnLoad(gameObject);
    }
    public void SaveFile()
    {
        string jsonData = JsonUtility.ToJson(DataManager.Instance);
        File.WriteAllText(dataPath, jsonData);
    }
    public void LoadFile()
    {
        var backupDataBase = dataBase;

        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            JsonUtility.FromJsonOverwrite(jsonData, DataManager.Instance);
        }

        dataBase = backupDataBase;
    }
}
[System.Serializable]
public class QuestsCurrent
{
    public int IdQuest1;
    public int IdQuest2;
    public int ProgressQuest1;
    public int ProgressQuest2;
    public int ProgressQuest3;
    public int ProgressQuest4;
    public List<int> checkQuest = new List<int>();
}
[System.Serializable]
public class DealsCurrent
{
    public int IdType;
    public int IdPiece;
    public int Quantily;
    public int PriceDeal;
    public bool TypePrice;
}
[System.Serializable]
public class Warehouse
{
    public List<int> CountPiecePlayerLevelUp;
    public List<int> CountPiecePlayerEvolve;
    public List<int> CountPieceEnemy;
    public List<int> CountItem;
    public List<int> ListItems;
}
[System.Serializable]
public class PlayerData
{
    public float Hp;
    public float Dame;
    public float Mana;
    public int Level;
    public int LevelEvolve;
    public int LevelUnlockCharacter;
    public int pieceUnlock;
    public bool CheckVip;
    public bool isUnlock;
}
[System.Serializable]
public class EnemyData
{
    public float Hp;
    public float Dame;
    public int Level;
}
[System.Serializable]
public class PetData
{
    public float petAttribute;
    public int Level;
    public int countPieceUnlock;
    public bool isUnlock;
}
[System.Serializable]
public class LevelData
{
    public int Star;
    public int Mode;
    public int Type;
    public int idItem;
}

