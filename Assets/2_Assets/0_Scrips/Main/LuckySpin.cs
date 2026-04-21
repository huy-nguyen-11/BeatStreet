using I2.MiniGames;
using MysticDev;
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpin : MonoBehaviour
{
    public GameObject[] _btnSpin;
    public GameObject popupRefreshTime;
    public MiniGame_Controller controllerSpin;
    private const string LAST_REWARD_TIME = "LastSpinTime";
    private TimeSpan rewardInterval = new TimeSpan(0, 15, 0);
    private DateTime lastRewardTime;
    private DateTime currentTime;
    private const string LAST_FREE_SPIN_DATE = "LastFreeSpinDate";
    private DateTime lastFreeSpinDate;
    public bool isRun;
    public WheelLedManager ledManager;

    public Sprite spYellowOn, spYellowOff, spGreenOn, spGreenOff;
    private void OnEnable()
    {
        CheckRewardStatus();
    }
    void Update()
    {
        currentTime = DateTime.Now;
        TimeSpan timeElapsed = currentTime - lastRewardTime;
        if (timeElapsed < rewardInterval)
        {
            TimeSpan timeRemaining = rewardInterval - (currentTime - lastRewardTime);
            popupRefreshTime.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{timeRemaining.Hours}:{timeRemaining.Minutes}:{timeRemaining.Seconds}";
        }
        else
        {
            if (popupRefreshTime != null && popupRefreshTime.activeSelf)
                CheckRewardStatus();
        }
    }
    public void CheckRewardStatus()
    {
        // Load last free-spin date (yyyy-MM-dd)
        Debug.Log("Checking reward status...");
        if (PlayerPrefs.HasKey(LAST_FREE_SPIN_DATE))
        {
            string lastFreeStr = PlayerPrefs.GetString(LAST_FREE_SPIN_DATE);
            if (!DateTime.TryParseExact(lastFreeStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out lastFreeSpinDate))
                lastFreeSpinDate = DateTime.MinValue;
        }
        else
        {
            lastFreeSpinDate = DateTime.MinValue;
        }

        if (lastFreeSpinDate != DateTime.MinValue && lastFreeSpinDate.Date == DateTime.Now.Date)
        {
            if (_btnSpin != null && _btnSpin.Length > 1 && _btnSpin[0] != null)
            {
                var go = _btnSpin[0];
                var btn = go.GetComponent<Button>();
                if (btn != null) btn.interactable = false;
                var img = go.GetComponent<Image>();
                if (img != null) img.sprite = spYellowOff;
                go.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.GetChild(1).gameObject.SetActive(true);
            }
            return;
        }


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
            popupRefreshTime.SetActive(false);
            DataManager.Instance.priceSpin = 100;
            DataManager.Instance.isFreeAdsSpin = true;
            //SetBtnSpin(0);
        }
        else
        {
            popupRefreshTime.SetActive(true);
            if (DataManager.Instance.isFreeAdsSpin)
            {
                //SetBtnSpin(1);
                DataManager.Instance.isFreeAdsSpin = false;
            }
            else
            {
                //_btnSpin[2].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = DataManager.Instance.priceSpin.ToString();
                //SetBtnSpin(2);
            }
        }
    }
    private void SetBtnSpin(int id)
    {
        for (int i = 0; i < _btnSpin.Length; i++)
            if (i == id)
            {
                //_btnSpin[i].SetActive(true);
                _btnSpin[i].GetComponent<Button>().interactable = true;
                _btnSpin[i].transform.GetChild(0).gameObject.SetActive(true); //text active
                _btnSpin[i].transform.GetChild(1).gameObject.SetActive(false); // text deactive
                _btnSpin[i].GetComponent<Image>().sprite = (id == 0) ? spGreenOn : spYellowOn;
            }
            else
            {
                //_btnSpin[i].SetActive(true);
                _btnSpin[i].GetComponent<Button>().interactable = false;
                _btnSpin[i].transform.GetChild(0).gameObject.SetActive(false); //text active
                _btnSpin[i].transform.GetChild(1).gameObject.SetActive(true); // text deactive
                _btnSpin[i].GetComponent<Image>().sprite = (id == 0) ? spGreenOff : spYellowOff;
            }     
    }
    public void BtnCoinSpin()
    {
        if (isRun) return;
        if (PlayerPrefs.GetInt("Coin") >= DataManager.Instance.priceSpin)
        {
            AudioBase.Instance.SetAudioUI(0);
            isRun = true;
            MainManager.Instance.SetMission(3, 1);
            MainManager.Instance.SetMission(1, DataManager.Instance.priceSpin);
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") - DataManager.Instance.priceSpin);
            DataManager.Instance.priceSpin += 100;
            CheckRewardStatus();
            //controllerSpin.ValidateRoundFree();
            controllerSpin.ValidateRound();
            StartCoroutine(SetTimeRun());
            DataManager.Instance.SaveFile();
            MainManager.Instance.SetTopBar();
        }
    }
    public void BtnFreeSpin()
    {
        if (isRun) return;
        AudioBase.Instance.SetAudioUI(0);
        MainManager.Instance.SetMission(12, 1); // mission 12: perform 1 free spin
        ledManager.SpinLed(0.02f);
        isRun = true;
        //lastRewardTime = DateTime.Now;
        //PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString());
        var todayStr = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        PlayerPrefs.SetString(LAST_FREE_SPIN_DATE, todayStr);
        PlayerPrefs.Save();
        CheckRewardStatus();
        //controllerSpin.ValidateRoundFree();
        controllerSpin.ValidateRound();
        StartCoroutine(SetTimeRun());
        DataManager.Instance.SaveFile();
        MainManager.Instance.SetTopBar();
    }
    public void BtnAdsSpin()
    {
        if (isRun) return;
        isRun = true;
        AudioBase.Instance.SetAudioUI(0);
        MG_Interface.Current.Reward_Show((bool result) =>
        {
            if (result)
            {
                MainManager.Instance.SetMission(12, 1);// mission 12: perform 1 free spin
                ledManager.SpinLed(0.02f);
                lastRewardTime = DateTime.Now;
                PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString());
                PlayerPrefs.Save();
                popupRefreshTime.SetActive(true);
                CheckRewardStatus();
                controllerSpin.ValidateRound();
                StartCoroutine(SetTimeRun());
                DataManager.Instance.SaveFile();
                MainManager.Instance.SetTopBar();
            }
        });
    }
    public void BtnBack()
    {
        if (isRun) return;
        MainManager.Instance.OpenPanel(3);
    }
    IEnumerator SetTimeRun()
    {
        yield return new WaitForSeconds(4f);
        isRun = false;
    }
}
