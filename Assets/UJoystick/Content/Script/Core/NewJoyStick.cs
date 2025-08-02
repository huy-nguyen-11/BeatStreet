using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class NewJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    [SerializeField, Range(0.1f, 1)] private float Radio = 0.5f;
    [SerializeField, Range(0.01f, 1f)] private float SmoothTime = 0.5f;
    [SerializeField, Range(0.01f, 1f)] private float DeadZoneThreshold = 0.05f; // Ngưỡng tối thiểu để tính joystick
    [SerializeField, Range(0.1f, 5)] private float ScaleDuration = 0.2f;

    [Header("Reference")]
    [SerializeField] private RectTransform StickRect;
    [SerializeField] private RectTransform CenterReference;
    [SerializeField] protected RectTransform floatingBackground;
    public Transform playerPos;

    // Private
    private Vector3 DeathArea;
    private int lastId = -2;
    private Canvas m_Canvas;
    private bool isDragging = false;
    private bool isActive = false;
    private float diff;
    private Vector3 defaultScale;

    protected virtual void Start()
    {
        if (StickRect == null || CenterReference == null || floatingBackground == null)
        {
            Debug.LogError("Joystick references are not set!");
            this.enabled = false;
            return;
        }

        m_Canvas = GetComponentInParent<Canvas>();
        if (m_Canvas == null)
        {
            Debug.LogError("Canvas is missing in hierarchy!");
            this.enabled = false;
            return;
        }

        DeathArea = CenterReference.position;
        diff = CenterReference.position.magnitude;
        defaultScale = Vector3.one;
        HideStick();
    }

    void Update()
    {
        // Follow player position
        if (playerPos != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, playerPos.position);
            RectTransform canvasRect = m_Canvas.transform as RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out localPoint);
            floatingBackground.anchoredPosition = localPoint;
        }

        DeathArea = CenterReference.position;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (lastId == -2)
        {
            lastId = data.pointerId;
            isDragging = true;
            ShowStick();
            OnDrag(data);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId != lastId) return;

        Vector3 position = bl_JoystickUtils.TouchPosition(m_Canvas, GetTouchID);
        float distance = Vector2.Distance(DeathArea, position);

        if (distance >= DeadZoneThreshold * radio)
        {
            isActive = true;
            Vector3 newPos = (distance < radio) ? position : DeathArea + (position - DeathArea).normalized * radio;
            StickRect.position = newPos;
            StickRect.localScale = Vector3.Lerp(StickRect.localScale, defaultScale, Time.deltaTime * 15f); // Smooth scale in
        }
        else
        {
            isActive = false;
            HideStick();
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.pointerId == lastId)
        {
            lastId = -2;
            isDragging = false;
            isActive = false;
            HideStick();
        }
    }

    private void ShowStick()
    {
        StickRect.localScale = Vector3.zero;
        StickRect.gameObject.SetActive(true);
        StickRect.DOScale(defaultScale, ScaleDuration).SetEase(Ease.OutBack);
    }

    private void HideStick()
    {
        StickRect.gameObject.SetActive(false);
        StickRect.localScale = Vector3.zero;
    }

    private float radio { get { return (Radio * 5 + Mathf.Abs((diff - CenterReference.position.magnitude))); } }

    public int GetTouchID
    {
        get
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == lastId)
                    return i;
            }
            return -1;
        }
    }

    public float Horizontal
    {
        get
        {
            if (!isActive) return 0f;
            return Mathf.Clamp((StickRect.position.x - DeathArea.x) / (Radio + 0.5f), -1f, 1f);
        }
    }

    public float Vertical
    {
        get
        {
            if (!isActive) return 0f;
            return Mathf.Clamp((StickRect.position.y - DeathArea.y) / (Radio + 0.5f), -1f, 1f);
        }
    }

    public Vector2 Direction => isActive ? new Vector2(Horizontal, Vertical) : Vector2.zero;
}
