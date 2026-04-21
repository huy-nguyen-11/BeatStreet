using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    private const int DynamicQuestCount = 4;
    private const int AdsQuestIndex = 4;
    private const int CompleteAllQuestIndex = 5;
    private const int AdsQuestMilestone = 5;
    private const int CompleteAllMilestone = 5;

    [SerializeField] GameObject[] _quests;
    [SerializeField] Sprite[] _sprIcon;
    public int idQuets1;
    public int idQuets2;
    public int idQuets3;
    public int idQuets4;

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
        dataManager.questsCurrent.ProgressQuest5 = 0;
        dataManager.questsCurrent.ProgressQuest6 = 0;
        idQuets1 = Random.Range(0, dataBase.ListQuests.Count);
        idQuets2 = RandomIdQuest(idQuets1);
        idQuets3 = RandomIdQuest(idQuets1, idQuets2);
        idQuets4 = RandomIdQuest(idQuets1, idQuets2, idQuets3);
        dataManager.questsCurrent.IdQuest1 = idQuets1;
        dataManager.questsCurrent.IdQuest2 = idQuets2;
        dataManager.questsCurrent.IdQuest3 = idQuets3;
        dataManager.questsCurrent.IdQuest4 = idQuets4;
        SetQuest();
    }
    private int RandomIdQuest(params int[] usedIds)
    {
        int id = Random.Range(0, dataBase.ListQuests.Count);
        for (int i = 0; i < usedIds.Length; i++)
        {
            if (usedIds[i] == id)
                return RandomIdQuest(usedIds);
        }
        return id;
    }
    public void SetQuest()
    {
        int questCount = Mathf.Min(_quests.Length, DynamicQuestCount + 2);
        for (int i = 0; i < questCount; i++)
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
                if (i == 4)
                    SetQuest5();
                if (i == 5)
                    SetQuest6();
            }
            else //is claimed
            {
                if (i == AdsQuestIndex)
                {
                    _quests[AdsQuestIndex].transform.GetChild(4).gameObject.SetActive(false); //button ad
                    _quests[AdsQuestIndex].transform.GetChild(3).gameObject.SetActive(false); //button claim
                    _quests[AdsQuestIndex].transform.GetChild(5).gameObject.SetActive(true); //claimed
                }
                else
                {
                    _quests[i].transform.GetChild(3).gameObject.SetActive(false); //button claim
                    _quests[i].transform.GetChild(4).gameObject.SetActive(true); //claimed
                }
            }
        }
    }
    private void SetQuest1()
    {
        if (dataManager.questsCurrent.ProgressQuest1 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].NameQuests; //name quest
            _quests[0].transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = spr; //icon coins or gem at button claim
            _quests[0].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].price.ToString(); // amount coins or gem at button claim
            _quests[0].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;//button claim
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest1].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest1;
            _quests[0].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[0].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
        }
        else
        {
            _quests[0].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true; //button claim
        }
    }
    private void SetQuest2()
    {
        if (dataManager.questsCurrent.ProgressQuest2 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].NameQuests;
            _quests[1].transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = spr;
            _quests[1].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].price.ToString();
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest2].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest2;
            _quests[1].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[1].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
            _quests[1].transform.GetChild(3).GetComponent<Button>().interactable = false;//button claim

        }
        else
        {
            _quests[1].transform.GetChild(3).GetComponent<Button>().interactable = true;//button claim
        }
    }
    private void SetQuest3()
    {
        if (dataManager.questsCurrent.ProgressQuest3 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest3].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest3].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest3].NameQuests;
            _quests[2].transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = spr;
            _quests[2].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest3].price.ToString();
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest3].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest3;
            _quests[2].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[2].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
            _quests[2].transform.GetChild(3).GetComponent<Button>().interactable = false;//button claim
        }
        else
        {
            _quests[2].transform.GetChild(3).GetComponent<Button>().interactable = true;//button claim
        }
    }
    private void SetQuest4()
    {
        if (dataManager.questsCurrent.ProgressQuest4 < dataBase.ListQuests[dataManager.questsCurrent.IdQuest4].Milestone)
        {
            Sprite spr = dataBase.ListQuests[dataManager.questsCurrent.IdQuest4].checkPrice ? _sprIcon[1] : _sprIcon[0];
            _quests[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest4].NameQuests;
            _quests[3].transform.GetChild(3).GetChild(1).GetComponent<Image>().sprite = spr;
            _quests[3].transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                dataBase.ListQuests[dataManager.questsCurrent.IdQuest4].price.ToString();
            float milestone = dataBase.ListQuests[dataManager.questsCurrent.IdQuest4].Milestone;
            float progress = dataManager.questsCurrent.ProgressQuest4;
            _quests[3].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / milestone, 0);
            _quests[3].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + milestone;
            _quests[3].transform.GetChild(3).GetComponent<Button>().interactable = false;//button claim
        }
        else
        {
            _quests[3].transform.GetChild(3).GetComponent<Button>().interactable = true;//button claim
        }
    }
    private void SetQuest5() //watch 5 ads
    {
        if (dataManager.questsCurrent.ProgressQuest5 < AdsQuestMilestone)
        {
            _quests[AdsQuestIndex].transform.GetChild(3).gameObject.SetActive(false); //button claim
            _quests[AdsQuestIndex].transform.GetChild(4).gameObject.SetActive(true); //button ads
        }
        else
        {
            _quests[AdsQuestIndex].transform.GetChild(3).gameObject.SetActive(true); //button claim
            _quests[AdsQuestIndex].transform.GetChild(4).gameObject.SetActive(false); //button ad
        }
        float progress = dataManager.questsCurrent.ProgressQuest5;
        _quests[AdsQuestIndex].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / AdsQuestMilestone, 0);
        _quests[AdsQuestIndex].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + AdsQuestMilestone;
    }
    private void SetQuest6() //completed all quest above
    {
        if (dataManager.questsCurrent.ProgressQuest6 < CompleteAllMilestone)
            _quests[CompleteAllQuestIndex].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = false;
        else
            _quests[CompleteAllQuestIndex].transform.GetChild(3).gameObject.GetComponent<Button>().interactable = true;
        float progress = dataManager.questsCurrent.ProgressQuest6;
        _quests[CompleteAllQuestIndex].transform.GetChild(2).GetChild(0).GetComponent<Image>().DOFillAmount(progress / CompleteAllMilestone, 0);
        _quests[CompleteAllQuestIndex].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = progress + "/" + CompleteAllMilestone;
    }
    public void BtnClaimQuest(int id)
    {
        AudioBase.Instance.SetAudioUI(0);
        if (!dataManager.questsCurrent.checkQuest.Contains(id))
        {
            AudioBase.Instance.SetAudioUI(1);
            if (id < CompleteAllQuestIndex)
                dataManager.questsCurrent.ProgressQuest6++;
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
                int idQuest3 = dataManager.questsCurrent.IdQuest3;
                int price3 = dataBase.ListQuests[idQuest3].price;
                string keyPrice3 = dataBase.ListQuests[idQuest3].checkPrice ? "Diamont" : "Coin";
                PlayerPrefs.SetInt(keyPrice3, PlayerPrefs.GetInt(keyPrice3) + price3);
                break;
            case 3:
                int idQuest4 = dataManager.questsCurrent.IdQuest4;
                int price4 = dataBase.ListQuests[idQuest4].price;
                string keyPrice4 = dataBase.ListQuests[idQuest4].checkPrice ? "Diamont" : "Coin";
                PlayerPrefs.SetInt(keyPrice4, PlayerPrefs.GetInt(keyPrice4) + price4);
                break;
            case 4:
                PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 5);
                break;
            case 5:
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
                dataManager.questsCurrent.ProgressQuest5++;
                SetQuest5();
            }
        });
    }
}
