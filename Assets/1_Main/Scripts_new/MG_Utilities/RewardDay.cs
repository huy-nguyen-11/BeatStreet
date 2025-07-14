using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardDay : MonoBehaviour
{
    public string rewardName;
    public int stepTime;
    public Button btn_video;
    public CanvasGroup canvasGroup;
    public GameObject time_text;
    public GameObject btnObj;

    private bool flag_On;
    private int hours, minutes, seconds;
    private System.TimeSpan steptime;
    private double CountdowTime;

    private void OnEnable()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (CountdowTime > 0)
        {
            CountdowTime -= Time.deltaTime;
            hours = (int) (CountdowTime / 3600);
            minutes = (int)(CountdowTime / 60) % 60;
            seconds = (int)CountdowTime % 60;
            time_text.GetComponent<TextMeshProUGUI>().SetText("" + string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds));
        }
        else
        {
            if (!flag_On)
            {
                time_text.SetActive(false);
                btn_video.interactable = true;
                btnObj.SetActive(true);
                flag_On = true;
                // bg.SetActive(false);
            }
        }
    }

    public void Init()
    {
        flag_On = false;
        steptime = System.DateTime.Parse(PlayerPrefs.GetString(rewardName, DateTime.Now.AddDays(-2).ToString())) - UnbiasedTime.Instance.Now();
        CountdowTime = steptime.TotalSeconds;
        if (CountdowTime < 0)
        {
            time_text.SetActive(false);
            btn_video.interactable = true;
            btnObj.SetActive(true);
            canvasGroup.enabled = false;

        }
        else
        {
            time_text.SetActive(true);
            btn_video.interactable = false;
            btnObj.SetActive(false);
            canvasGroup.enabled = true;
        }

    }

    public void OnCallBack()
    {
        PlayerPrefs.SetString(rewardName, UnbiasedTime.Instance.Now().AddDays(1).ToString());
        Init();
    }
}
