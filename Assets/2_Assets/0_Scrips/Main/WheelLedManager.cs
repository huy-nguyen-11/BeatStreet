using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WheelLedManager : MonoBehaviour
{
    [Header("Led Setup")]
    public Image[] leds;

    public Sprite ledOnSprite;
    public Sprite ledOffSprite;

    [Header("Settings")]
    public float idleBlinkSpeed = 0.5f; 
    public float rewardBlinkSpeed = 0.15f; 

    private Coroutine currentLedCoroutine;

    private void OnEnable()
    {
        IdleLed();
    }

    public void IdleLed()
    {
        StopCurrentAnimation();
        currentLedCoroutine = StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine()
    {
        bool isEvenLit = true; 

        while (true) 
        {
            for (int i = 0; i < leds.Length; i++)
            {
                bool turnOn = (i % 2 == 0) ? isEvenLit : !isEvenLit;
                leds[i].sprite = turnOn ? ledOnSprite : ledOffSprite;
            }

            isEvenLit = !isEvenLit;
            yield return new WaitForSeconds(idleBlinkSpeed);
        }
    }


    public void SpinLed(float delayBetweenLeds = 0.03f)
    {
        StopCurrentAnimation();
        currentLedCoroutine = StartCoroutine(SpinRoutine(delayBetweenLeds));
    }

    private IEnumerator SpinRoutine(float delay)
    {
        int headIndex = 0;
        int trailLength = 3;

        while (true)
        {
            SetAllLeds(false);
            for (int i = 0; i < trailLength; i++)
            {
                int indexToLight = headIndex - i;
                if (indexToLight < 0)
                    indexToLight += leds.Length;

                leds[indexToLight].sprite = ledOnSprite;
            }

            headIndex = (headIndex + 1) % leds.Length;

            yield return new WaitForSeconds(delay);
        }
    }

    public void RewardLed()
    {
        StopCurrentAnimation();
        currentLedCoroutine = StartCoroutine(RewardRoutine());
    }

    private IEnumerator RewardRoutine()
    {
        int blinkAmount = 4;

        for (int i = 0; i < blinkAmount; i++)
        {
            SetAllLeds(true); 
            yield return new WaitForSeconds(rewardBlinkSpeed);

            SetAllLeds(false); 
            yield return new WaitForSeconds(rewardBlinkSpeed);
        }

        IdleLed();
    }

    private void StopCurrentAnimation()
    {
        if (currentLedCoroutine != null)
        {
            StopCoroutine(currentLedCoroutine);
        }
    }

    private void SetAllLeds(bool isOn)
    {
        Sprite targetSprite = isOn ? ledOnSprite : ledOffSprite;
        for (int i = 0; i < leds.Length; i++)
        {
            leds[i].sprite = targetSprite;
        }
    }
}
