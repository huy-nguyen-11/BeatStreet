using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    [SerializeField] GameObject[] _quests;
    [SerializeField] Sprite[] _sprIcon;
    public int idQuets1;
    public int idQuets2;

    DataManager dataManager;
    DataBase dataBase;
    private void Start()
    {

    }
    private void OnEnable()
    {
        dataManager = DataManager.Instance;
        dataBase = dataManager.dataBase;
        CheckAndSaveDate();
    }
    void CheckAndSaveDate()
    {
        string savedDate = PlayerPrefs.GetString("SavedDateQuest", "");
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        if (string.IsNullOrEmpty(savedDate))
        {
            PlayerPrefs.SetString("SavedDateQuest", today);
            PlayerPrefs.Save();
            RefreshQuest();
        }
        else
        {
            System.DateTime savedDateTime = System.DateTime.Parse(savedDate);
            System.DateTime todayDateTime = System.DateTime.Parse(today);
            if (todayDateTime > savedDateTime)
            {
                PlayerPrefs.SetString("SavedDateQuest", today);
                PlayerPrefs.Save();
                RefreshQuest();
            }
            else
            {
                SetQuest();
            }
        }
    }
    public void RefreshQuest()
    {
        dataManager.questsCurrent.ProgressQuest1 = 0;
        dataManager.questsCurrent.ProgressQuest2 = 0;
        dataManager.questsCurrent.ProgressQuest3 = 0;
        dataManager.questsCurrent.ProgressQuest4 = 0;
        idQuets1 = Random.Range(0, dataBase.ListQuests.Count);
        idQuets2 = RandomIdQuest();
        dataManager.questsCurrent.IdQuest1 = idQuets1;
        dataManager.questsCurrent.IdQuest2 = idQuets2;
        SetQuest();
    }
    private int RandomIdQuest()
    {
        int id = Random.Range(0, dataBase.ListQuests.Count);
        if (idQuets1 != id)
            return id;
        else
            return RandomIdQuest();
    }
    public void SetQuest()
    {
        for (int i = 0; i < _quests.Length; i++)
        {
            if (!dataManager.questsCurrent.checkQuest.Contains(i))
            {
                _quests[i].transform.GetChild(4).gameObject.SetActive(false);
                if (i == 0)
                    SetQuest1();
                if (i == 1)
                    SetQuest2();
                if (i == 2)
                    SetQuest3();
                if (i == 3)
                    SetQuest4();
            }
            else
            {
                _quests[i].transform.GetChild(1).gameObject.SetActive(false);
                _quests[i].transform.GetChild(2).gameObject.SetActive(false);
                _quests[i].transform.GetChild(3).gameObject.SetActive(false);
                _quests[i].transform.GetChild(4).gameObject.SetActive(true);
                if (i == 2)
                    _quests[i].transform.GetChild(5).gameObject.SetActive(false);
            }
        }
    }
    private void SetQuest1()
    {
        if (dataManager.questsCurrent.ProgressQuest1 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].NameQuests;
            _quests[0].transform.GetChild(1).GetComponent<Image>().sprite = spr;
            _quests[0].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].price.ToString();
            _quests[0].transform.GetChild(3).gameObject.SetActive(false);
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest1;
            _quests[0].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[0].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
        }
        else
        {
            _quests[0].transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    private void SetQuest2()
    {
        if (dataManager.questsCurrent.ProgressQuest2 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].NameQuests;
            _quests[1].transform.GetChild(1).GetComponent<Image>().sprite = spr;
            _quests[1].transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].price.ToString();
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest2;
            _quests[1].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[1].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
            _quests[1].transform.GetChild(3).gameObject.SetActive(false);

        }
        else
        {
            _quests[1].transform.GetChild(3).gameObject.SetActive(true);
        }
    }
    private void SetQuest3()
    {
        if (dataManager.questsCurrent.ProgressQuest3 < 5)
        {
            _quests[2].transform.GetChild(3).gameObject.SetActive(false);
            _quests[2].transform.GetChild(5).gameObject.SetActive(true);
        }
        else
        {
            _quests[2].transform.GetChild(3).gameObject.SetActive(true);
            _quests[2].transform.GetChild(5).gameObject.SetActive(false);
        }
        float progress = dataManager.questsCurrent.ProgressQuest3;
        _quests[2].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / 5, 0);
        _quests[2].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + 5;
    }
    private void SetQuest4()
    {
        if (dataManager.questsCurrent.ProgressQuest4 < 3)
            _quests[3].transform.GetChild(3).gameObject.SetActive(false);
        else
            _quests[3].transform.GetChild(3).gameObject.SetActive(true);
        float progress = dataManager.questsCurrent.ProgressQuest4;
        _quests[3].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / 3, 0);
        _quests[3].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + 3;
    }
    public void BtnClaimQuest(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (!dataManager.questsCurrent.checkQuest.Contains(id))
        {
            AudioBase.Instance.SetAudioUI(1);
            dataManager.questsCurrent.ProgressQuest4++;
            dataManager.questsCurrent.checkQuest.Add(id);
            BonusQuest(id);
            DataManager.Instance.SaveFile();
            SetQuest();
        }
    }
    private void BonusQuest(int id)
    {
        switch (id)
        {
            case 0:
                int idQuest = dataManager.questsCurrent.IdQuest1;
                int price = dataBase.ListQuests[idQuest].price;
                string keyPrice = dataBase.ListQuests[idQuest].checkPrice ? "Diamont" : "Coin";
                PlayerPrefs.SetInt(keyPrice, PlayerPrefs.GetInt(keyPrice) + price);
                break;
            case 1:
                int idQuest2 = dataManager.questsCurrent.IdQuest2;
                int price2 = dataBase.ListQuests[idQuest2].price;
                string keyPrice2 = dataBase.ListQuests[idQuest2].checkPrice ? "Diamont" : "Coin";
                PlayerPrefs.SetInt(keyPrice2, PlayerPrefs.GetInt(keyPrice2) + price2);
                break;
            case 2:
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 5);
                break;
            case 3:
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 10);
                break;
            default:
                break;
        }
        MainManager.Instance.SetTopBar();
    }
    public void BtnAds()
    {
        AudioBase.Instance.SetAudioUI(0);
        MG_Interface.Current.Reward_Show((bool result) =>
        {
            if (result)
            {
                dataManager.questsCurrent.ProgressQuest3++;
                SetQuest3();
            }
        });
    }
}
