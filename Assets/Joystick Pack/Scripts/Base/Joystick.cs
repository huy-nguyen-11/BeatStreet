using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Horizontal { get { return (snapX) ? SnapFloat(smoothedInput.x, AxisOptions.Horizontal) : smoothedInput.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(smoothedInput.y, AxisOptions.Vertical) : smoothedInput.y; } }

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
    [SerializeField] private float deadZone = 0.2f; // DeadZone mặc định 0.2 để tránh input khi chỉ chạm nhẹ
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
    private float handleSmoothTime = 0.1f;
    private float directionSmoothTime = 0.05f;

    private Vector2 handleTargetPosition = Vector2.zero;
    private Vector2 handleVelocity = Vector2.zero;

    private Vector2 smoothedInput = Vector2.zero;
    private Vector2 inputVelocity = Vector2.zero;

    // Add rawInput storage and public accessors
    private Vector2 rawInput = Vector2.zero;

    // Expose raw (immediate) input and magnitudes
    public Vector2 RawDirection { get { return rawInput; } }
    //public float RawMagnitude { get { return rawInput.magnitude; } }
    public float RawMagnitude { get { return Mathf.Min(rawInput.magnitude, 1f); } }

    // Expose smoothed direction vector
    public Vector2 SmoothedDirection { get { return smoothedInput; } }

    // Event for smoothed direction changes
    public event Action<Vector2, float> OnSmoothedDirectionChanged;
    
    // Track previous magnitude to detect significant changes
    private float previousSmoothedMagnitude = 0f;
    [SerializeField, Tooltip("Threshold for detecting significant magnitude changes (0.05 = 5% change)")]
    private float magnitudeChangeThreshold = 0.05f;


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
        followTarget = GamePlayManager.Instance.targetForJoystic;
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
        previousSmoothedMagnitude = smoothedInput.magnitude;

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
            Vector2 calculatedInput = handle.anchoredPosition / (radius * handleRange);
            float calculatedMagnitude = calculatedInput.magnitude;
            
            // Chỉ tính input khi vượt qua deadZone
            if (calculatedMagnitude > deadZone)
            {
                input = calculatedInput;
            }
            else
            {
                input = Vector2.zero;
            }
        }

        // Smooth Direction output
        Vector2 previousSmoothed = smoothedInput;
        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref inputVelocity, directionSmoothTime);
        
        // Đảm bảo smoothedInput cũng tuân theo deadZone
        if (smoothedInput.magnitude <= deadZone)
        {
            smoothedInput = Vector2.zero;
        }
        
        // Check for significant magnitude change and fire event
        float currentMagnitude = smoothedInput.magnitude;
        float magnitudeDelta = Mathf.Abs(currentMagnitude - previousSmoothedMagnitude);
        
        if (magnitudeDelta >= magnitudeChangeThreshold || 
            (previousSmoothedMagnitude == 0f && currentMagnitude > deadZone) ||
            (previousSmoothedMagnitude > deadZone && currentMagnitude <= deadZone))
        {
            previousSmoothedMagnitude = currentMagnitude;
            OnSmoothedDirectionChanged?.Invoke(smoothedInput, currentMagnitude);
        }

       
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

        // Determine center in screen space
        Vector2 centerScreen;
        if (useInitialTouchAsCenter && isDragging)
        {
            centerScreen = dragStartScreenPosition;
        }
        else
        {
            centerScreen = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        }

        // Convert points to background local coordinates (same unit as sizeDelta)
        Vector2 localPoint;
        Vector2 localCenter;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, cam, out localPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, centerScreen, cam, out localCenter);

        Vector2 radius = background.sizeDelta / 2f;

        // rawInput in -1..1 range relative to the background rect
        Vector2 calculatedRawInput = (localPoint - localCenter) / radius;
        float calculatedMagnitude = calculatedRawInput.magnitude;

        // Chỉ tính input khi vượt qua deadZone
        if (calculatedMagnitude > deadZone)
        {
            rawInput = calculatedRawInput;
            input = rawInput;
        }
        else
        {
            // Nếu chưa vượt deadZone, set về zero
            rawInput = Vector2.zero;
            input = Vector2.zero;
        }

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
        rawInput = Vector2.zero;
        // Smoothly return handle to center
        handleTargetPosition = Vector2.zero;

        // Hide handle when released (inside dead zone)
        if (handle != null)
            handle.gameObject.SetActive(false);

        // reset drag state
        isDragging = false;

        dragStartScreenPosition = Vector2.zero;

        // Fire event when joystick is released (magnitude becomes 0)
        if (previousSmoothedMagnitude > 0.01f)
        {
            previousSmoothedMagnitude = 0f;
            OnSmoothedDirectionChanged?.Invoke(Vector2.zero, 0f);
        }

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

    public float HandleNormalizedMagnitude
    {
        get
        {
            if (background == null || handle == null)
                return 0f;

            float radius = Mathf.Min(background.sizeDelta.x, background.sizeDelta.y) * 0.5f * handleRange;
            if (radius <= 0f)
                return 0f;

            return Mathf.Clamp01(handle.anchoredPosition.magnitude / radius);
        }
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }