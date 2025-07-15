using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TggleSetting : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform , bgRectransform;
    [SerializeField] Sprite backgroundActiveSprite; // Changed to Sprite type for background image
    [SerializeField] Sprite backgroundInactiveSprite; // Added for inactive background image
    [SerializeField] Sprite handleOn, handleOff;
    [SerializeField] int index;

    Image backgroundImage, handleImage;

    Toggle toggle;

    Vector2 handlePosition;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundImage = bgRectransform.GetComponent<Image>();
        handleImage = uiHandleRectTransform.GetComponent<Image>();

        toggle.onValueChanged.AddListener(OnSwitch);

        switch (index)
        {
            case 0:
                toggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
                break;
            case 1:
                toggle.isOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
                break;
        }

        SetSwitchState(toggle.isOn);
    }

    void OnSwitch(bool on)
    {
        Debug.Log("open!");
        uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack).SetUpdate(true).OnComplete(() =>
        {
            backgroundImage.sprite = on ? backgroundActiveSprite : backgroundInactiveSprite; // Change background image
            handleImage.sprite = on ? handleOn : handleOff; // Change handle image

            switch (index)
            {
                case 0:
                    //AudioManager.instance.ToggleMusic(on);
                    break;
                case 1:
                    //AudioManager.instance.ToggleSFX(on);
                    break;
            }
        });
    }

    void SetSwitchState(bool on)
    {
        uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        backgroundImage.sprite = on ? backgroundActiveSprite : backgroundInactiveSprite;
        handleImage.sprite = on ? handleOn : handleOff;
        //textToggle.text = on ? "On" : "Off";
    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

}