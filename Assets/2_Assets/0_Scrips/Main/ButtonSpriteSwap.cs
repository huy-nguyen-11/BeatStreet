using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSpriteSwap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (image != null && pressedSprite != null && GetComponent<Button>().interactable)
        {
            image.sprite = pressedSprite;
            transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 4.3f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image != null && normalSprite != null && GetComponent<Button>().interactable)
        {
            image.sprite = normalSprite;
            transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 12.7f);
        }
    }
}
