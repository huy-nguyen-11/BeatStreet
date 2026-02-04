using System;
using UnityEngine;

public class EnergyRecovery : MonoBehaviour
{
    private const string ENERGY_KEY = "Energy";
    private const string LAST_TIME_KEY = "last_timeEnergy";

    public int maxEnergy = 20;
    public float recoveryTime = 300f;

    private float timer = 0f;

    void Start()
    {
        CheckOfflineRecovery();
    }

    void Update()
    {
        //RecoverEnergy();
    }

    void CheckOfflineRecovery()
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, 0);
        if (currentEnergy >= maxEnergy) return;

        string lastTimeString = PlayerPrefs.GetString(LAST_TIME_KEY, "");
        if (!string.IsNullOrEmpty(lastTimeString))
        {
            DateTime lastTime = DateTime.Parse(lastTimeString);
            TimeSpan timePassed = DateTime.Now - lastTime;
            int energyRecovered = (int)(timePassed.TotalSeconds / recoveryTime);
            currentEnergy = Mathf.Min(currentEnergy + energyRecovered, maxEnergy);
            PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);
        }
        PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    void RecoverEnergy()
    {
        int currentEnergy = PlayerPrefs.GetInt(ENERGY_KEY, 0);

        if (currentEnergy >= maxEnergy)
        {
            timer = 0f;
            PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString());
            PlayerPrefs.Save();
            return;
        }
        timer += Time.deltaTime;
        if (timer >= recoveryTime)
        {
            currentEnergy++;
            PlayerPrefs.SetInt(ENERGY_KEY, currentEnergy);
            PlayerPrefs.Save();
            timer = 0f;
            if (MainManager.Instance != null)
                MainManager.Instance.SetTopBar();
            if (GamePlayManager.Instance != null)
                GamePlayManager.Instance.gameOver.SetTopBar();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString(LAST_TIME_KEY, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }
}
