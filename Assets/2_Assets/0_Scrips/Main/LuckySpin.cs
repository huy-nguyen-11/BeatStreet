using I2.MiniGames;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpin : MonoBehaviour
{
    public GameObject[] _btnSpin;
    public GameObject popupRefreshTime;
    public MiniGame_Controller controllerSpin;
    private const string LAST_REWARD_TIME = "LastSpinTime";
    private TimeSpan rewardInterval = new TimeSpan(24, 0, 0);
    private DateTime lastRewardTime;
    private DateTime currentTime;
    public bool isRun;
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
    }
    public void CheckRewardStatus()
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
            popupRefreshTime.SetActive(false);
            DataManager.Instance.priceSpin = 100;
            DataManager.Instance.isFreeAdsSpin = true;
            SetBtnSpin(0);
        }
        else
        {
            popupRefreshTime.SetActive(true);
            if (DataManager.Instance.isFreeAdsSpin)
            {
                SetBtnSpin(1);
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
            }
            else
            {
                //_btnSpin[i].SetActive(true);
                _btnSpin[i].GetComponent<Button>().interactable = false;
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
            controllerSpin.ValidateRoundFree();
            StartCoroutine(SetTimeRun());
            DataManager.Instance.SaveFile();
            MainManager.Instance.SetTopBar();
        }
    }
    public void BtnFreeSpin()
    {
        if (isRun) return;
        AudioBase.Instance.SetAudioUI(0);
        MainManager.Instance.SetMission(3, 1);
        isRun = true;
        lastRewardTime = DateTime.Now;
        PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString());
        PlayerPrefs.Save();
        CheckRewardStatus();
        controllerSpin.ValidateRoundFree();
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
                MainManager.Instance.SetMission(3, 1);
                CheckRewardStatus();
                controllerSpin.ValidateRoundFree();
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
