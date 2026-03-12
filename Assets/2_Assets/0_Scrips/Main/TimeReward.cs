using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeReward : MonoBehaviour
{
    public string nameReward;

    [SerializeField] private TextMeshProUGUI myText;
    public float totalTimeInSeconds2;
    private float timeRemaining;
    public GameObject freeButton, freeText , lockButton;
    public Sprite disableSp, activeSp;

    private bool isCounting = false;

    private void OnEnable()
    {
        LoadTimeRemaining();
        if (!isCounting)
        {
            freeButton.GetComponent<Button>().enabled = true;
            freeButton.GetComponent<Image>().sprite = activeSp;
            myText.gameObject.SetActive(false);
            freeText.gameObject.SetActive(true);
            lockButton.gameObject.SetActive(false);
        }
        else
        {
            freeText.gameObject.SetActive(false);
            lockButton.gameObject.SetActive(true);
            freeButton.GetComponent<Button>().enabled = false;
            freeButton.GetComponent<Image>().sprite = disableSp;
            StartCoroutine(Counter());
        }
    }

    private void OnApplicationQuit()
    {
        SaveTimeRemaining();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SaveTimeRemaining();
        }
    }

    private void OnDisable()
    {
        SaveTimeRemaining();
    }

    private void SaveTimeRemaining()
    {
        PlayerPrefs.SetFloat($"TimeRemainingGacha_{nameReward}", timeRemaining);
        PlayerPrefs.SetString($"QuitTimeGacha_{nameReward}", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.SetInt($"IsCountingGacha_{nameReward}", isCounting ? 1 : 0);
    }

    private void LoadTimeRemaining()
    {
        isCounting = PlayerPrefs.GetInt($"IsCountingGacha_{nameReward}", 0) == 1;

        if (PlayerPrefs.HasKey($"QuitTimeGacha_{nameReward}"))
        {
            DateTime quitTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString($"QuitTimeGacha_{nameReward}")));
            TimeSpan duration = DateTime.Now - quitTime;
            timeRemaining = PlayerPrefs.GetFloat($"TimeRemainingGacha_{nameReward}") - (float)duration.TotalSeconds;

            if (timeRemaining <= 0)
            {
                timeRemaining = totalTimeInSeconds2;
                isCounting = false;
            }
        }
        else
        {
            timeRemaining = totalTimeInSeconds2;
            isCounting = false;
        }

        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        myText.gameObject.SetActive(true);
        freeText.gameObject.SetActive(false);
        lockButton.gameObject.SetActive(true);
        myText.text = timeSpan.ToString(@"mm\:ss");
    }

    public void OnFreeButtonClick()
    {
        if (!isCounting)
        {
            freeButton.GetComponent<Button>().enabled = false;
            freeButton.GetComponent<Image>().sprite = disableSp;
            isCounting = true;
            timeRemaining = totalTimeInSeconds2;
            if(gameObject.activeInHierarchy)
                StartCoroutine(Counter());
        }
    }

    private IEnumerator Counter()
    {
        while (timeRemaining > 0f)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            UpdateTimeText();
        }

        PlayerPrefs.SetInt("Done Counting", 1);
        freeButton.GetComponent<Button>().enabled = true;
        freeButton.GetComponent<Image>().sprite = activeSp;
        myText.gameObject.SetActive(false);
        freeText.gameObject.SetActive(true);
        lockButton.gameObject.SetActive(false);
        isCounting = false;
    }
}
