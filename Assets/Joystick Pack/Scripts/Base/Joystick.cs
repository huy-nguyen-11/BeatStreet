using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Horizontal { get { return (snapX) ? SnapFloat(smoothedInput.x, AxisOptions.Horizontal) : smoothedInput.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(smoothedInput.y, AxisOptions.Vertical) : smoothedInput.y; } }
    //public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    [SerializeField] public Transform followTarget = null;
    [Header("Input Options")]
    [SerializeField, Tooltip("When true, the joystick will use the initial touch position as the input center while dragging.")]
    private bool useInitialTouchAsCenter = true;

    private bool isDragging = false;
    private Vector2 dragStartScreenPosition = Vector2.zero;
    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    private RectTransform canvasRect = null;

    private Vector2 input = Vector2.zero;

    // Smoothing fields
    private float handleSmoothTime = 0.05f;
    private float directionSmoothTime = 0.05f;

    private Vector2 handleTargetPosition = Vector2.zero;
    private Vector2 handleVelocity = Vector2.zero;

    private Vector2 smoothedInput = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    // Add rawInput storage and public accessors
    private Vector2 rawInput = Vector2.zero;

    // Expose raw (immediate) input and magnitudes
    public Vector2 Direction { get { return rawInput; } }
    public float RawMagnitude { get { return rawInput.magnitude; } }
    public float SmoothedMagnitude { get { return smoothedInput.magnitude; } }

    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        canvasRect = canvas.transform as RectTransform;

        // If follow target specified, position the background under it
        if (followTarget != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera ?? Camera.main, followTarget.position);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera ?? Camera.main, out localPoint))
            {
                background.anchoredPosition = localPoint;
            }
        }

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

        // init smoothing
        handleTargetPosition = handle.anchoredPosition;
        smoothedInput = input;

        // Ensure handle is visible at start
        if (handle != null)
            handle.gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        // If follow target specified, update background position to follow the world transform each frame
        if (followTarget != null && canvasRect != null)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera ?? Camera.main, followTarget.position);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera ?? Camera.main, out localPoint))
            {
                background.anchoredPosition = localPoint;
            }
        }

        // Smooth handle movement
        Vector2 currentPos = handle.anchoredPosition;
        Vector2 newPos = Vector2.SmoothDamp(currentPos, handleTargetPosition, ref handleVelocity, handleSmoothTime);
        handle.anchoredPosition = newPos;

        // Update input based on actual handle position so input is synchronized with what the player sees
        Vector2 radius = background.sizeDelta / 2f;
        if (radius.x != 0 && handleRange != 0)
        {
            // invert the earlier relation: handle.anchoredPosition = input * radius * handleRange
            input = handle.anchoredPosition / (radius * handleRange);
        }

        // Smooth Direction output
        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref inputVelocity, directionSmoothTime);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // Record initial touch position; do not calculate input yet. Input will be handled during OnDrag.
        isDragging = true;
        dragStartScreenPosition = eventData.position;
        // Show handle if starting a drag
        if (handle != null && !handle.gameObject.activeSelf)
            handle.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        // Determine the center for input calculations.
        Vector2 position;
        if (useInitialTouchAsCenter && isDragging)
        {
            // use the recorded initial touch screen position as center
            position = dragStartScreenPosition;
        }
        else
        {
            // use background position on screen as center
            position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        }
        Vector2 radius = background.sizeDelta / 2;
        // calculate raw input (immediate, before smoothing/clamping)
        rawInput = (eventData.position - position) / (radius * canvas.scaleFactor);
        input = rawInput;
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        // Set target position for smoothing instead of snapping immediately
        handleTargetPosition = input * radius * handleRange;

        // Hide handle when inside dead zone, show otherwise
        if (handle != null)
        {
            if (input == Vector2.zero)
            {
                // inside dead zone
                handle.gameObject.SetActive(false);
                handleTargetPosition = Vector2.zero;
            }
            else
            {
                if (!handle.gameObject.activeSelf)
                    handle.gameObject.SetActive(true);
            }
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        // Smoothly return handle to center
        handleTargetPosition = Vector2.zero;

        // Hide handle when released (inside dead zone)
        if (handle != null)
            handle.gameObject.SetActive(false);

        // reset drag state
        isDragging = false;
        dragStartScreenPosition = Vector2.zero;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(smoothedInput, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }