using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Floating_bl_Joystick : bl_Joystick
{
    [Header("Floating Joystick Settings")]
    [SerializeField] private bool resetPositionOnRelease = true; // Reset joystick position when released

    [SerializeField] private RectTransform canvasRect;

    private Vector3 initialPosition;

    /// <summary>
    /// Initialize the floating joystick.
    /// </summary>
    void Start()
    {
        base.Start();

        if (floatingBackground != null)
        {
            initialPosition = floatingBackground.position;
            floatingBackground.gameObject.SetActive(false); // Hide joystick initially
        }
    }

    /// <summary>
    /// Handle pointer down event to activate and reposition the joystick.
    /// </summary>
    /// <param name="data"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, playerPos.position);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
             screenPosition,
            Camera.main,
            out localPoint
        );

        floatingBackground.anchoredPosition = localPoint;

        floatingBackground.gameObject.SetActive(true);

        base.OnPointerDown(eventData);
    }

    /// <summary>
    /// Handle pointer up event to deactivate the joystick.
    /// </summary>
    /// <param name="data"></param>
    public override void OnPointerUp(PointerEventData data)
    {
        base.OnPointerUp(data); // Call base functionality

        if (floatingBackground != null && resetPositionOnRelease)
        {
            floatingBackground.gameObject.SetActive(false); // Hide joystick
            floatingBackground.position = initialPosition; // Reset to initial position
        }
    }
}
