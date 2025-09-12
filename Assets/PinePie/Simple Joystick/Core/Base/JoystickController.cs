using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PinePie.SimpleJoystick
{
    [AddComponentMenu("PinePie/Joystick Controller")]
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        public RectTransform joystickBase;
        public RectTransform handle;
        public float joystickRange;
        [Range(0f, 1f)] public float deadZone;

        [Tooltip("if true, snap handle to center when not using. Use false only when a certain direction is always needed.")]
        public bool snapHandleBack = true;

        private Vector2 inputDirection = Vector2.zero;
        public Vector2 Direction => inputDirection;

        public event Action OnTouchPressed;
        public event Action OnTouchRemoved;
        public event Action OnDirectionChanged;
        private bool dragStartedInside = false;

        //new update
        private Vector2 startPoint;
        private Vector2 previousDirection = Vector2.zero;
        public float reverseThreshold = 0.2f;
        public float dragStartThreshold = 0.15f;
        private bool isDragging = false;

        void Awake()
        {
            handle.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // event for touch pressed
            OnTouchPressed?.Invoke();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out startPoint);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 localPoint
                    );
            dragStartedInside = localPoint.magnitude <= joystickRange;

            previousDirection = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 currentPoint);

            Vector2 delta = currentPoint - startPoint;
            handle.anchoredPosition = delta;
            Vector2 rawInput = delta / joystickRange;

            // Dead zone check
            if (rawInput.magnitude < deadZone)
            {
                inputDirection = Vector2.zero;
                handle.anchoredPosition = Vector2.zero;
                handle.gameObject.SetActive(false);
                isDragging = false;
                return;
            }

            if (!isDragging)
            {
                if (delta.magnitude < joystickRange * dragStartThreshold)
                {

                    inputDirection = Vector2.zero;
                    handle.anchoredPosition = Vector2.zero;
                    handle.gameObject.SetActive(false);
                    return;
                }

                isDragging = true;
            }


            Vector2 newDirection = rawInput.normalized;

            // Reverse threshold check
            if (previousDirection != Vector2.zero)
            {
                float dot = Vector2.Dot(previousDirection, newDirection);
                if (dot < 0f && delta.magnitude < joystickRange * reverseThreshold)
                { 
                    return;
                }
            }

            // Cập nhật hướng và vị trí
            inputDirection = rawInput;
            previousDirection = newDirection;

            handle.anchoredPosition = Vector2.ClampMagnitude(delta, joystickRange);
            handle.gameObject.SetActive(true);

            OnDirectionChanged?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTouchRemoved?.Invoke();

            if (snapHandleBack)
            {
                handle.gameObject.SetActive(false);
                handle.anchoredPosition = Vector2.zero;
                inputDirection = Vector2.zero;
            }
        }
    }
}