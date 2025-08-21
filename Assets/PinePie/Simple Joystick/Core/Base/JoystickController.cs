using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PinePie.SimpleJoystick
{
    public enum JoystickBaseMode
    {
        Static,
        Dynamic,
        Floating
    }

    [AddComponentMenu("PinePie/Joystick Controller")]
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        public RectTransform joystickBase;
        public RectTransform handle;

        [Header("Settings")]
        public JoystickBaseMode baseMode = JoystickBaseMode.Static;
        public float joystickRange;
        //public float handleRange;
        [Range(0f, 1f)] public float deadZone = 0.1f;

        [Range(0, 16)]
        [Tooltip("Set 0 or 1 for free handle, or set according to number of direction need like 4 or 8")]
        public int directionSnaps = 0;

        [Tooltip("if true, snap handle to center when not using. Use false only when a certain direction is always needed.")]
        public bool snapHandleBack = true;


        private Vector2 inputDirection = Vector2.zero;
        public Vector2 Direction => inputDirection;

        public event Action OnTouchPressed;
        public event Action OnTouchRemoved;
        public event Action OnDirectionChanged;

        private Vector2 baseStartPos;
        private Canvas parentCanvas;
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
            baseStartPos = joystickBase.anchoredPosition;
            parentCanvas = GetComponentInParent<Canvas>();

            if (baseMode != JoystickBaseMode.Static) joystickBase.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // event for touch pressed
            OnTouchPressed?.Invoke();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera,out startPoint);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 localPoint
                    );
            dragStartedInside = localPoint.magnitude <= joystickRange;

            previousDirection = Vector2.zero;
            //OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase,eventData.position,eventData.pressEventCamera,out Vector2 currentPoint);

            Vector2 delta = currentPoint - startPoint;

            // Vẽ handle tại điểm hiện tại (không cần clamp nữa)
            handle.anchoredPosition = delta;

            // Chuẩn hóa đầu vào theo joystickRange nếu cần
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

            // Kiểm tra nếu chưa bắt đầu kéo đủ lớn
            if (!isDragging)
            {
                if (delta.magnitude < joystickRange * dragStartThreshold)
                {
                    // Chưa vượt ngưỡng kéo ban đầu → không cho điều khiển
                    inputDirection = Vector2.zero;
                    handle.anchoredPosition = Vector2.zero;
                    handle.gameObject.SetActive(false); // vẫn hiển thị handle để người dùng thấy
                    return;
                }

                isDragging = true; // Bắt đầu được phép điều khiển từ đây
            }


            Vector2 newDirection = rawInput.normalized;

            // Reverse threshold check
            if (previousDirection != Vector2.zero)
            {
                float dot = Vector2.Dot(previousDirection, newDirection);
                if (dot < 0f && delta.magnitude < joystickRange * reverseThreshold)
                {
                    // Không cho đổi hướng nếu kéo ngược nhẹ
                    return;
                }
            }

            // Cập nhật hướng và vị trí
            inputDirection = rawInput;
            previousDirection = newDirection;

            // Clamp để handle không vượt joystick range
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

            if (baseMode == JoystickBaseMode.Floating || baseMode == JoystickBaseMode.Dynamic)
                joystickBase.anchoredPosition = baseStartPos;

            if (baseMode != JoystickBaseMode.Static) joystickBase.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (joystickBase != null)
            {
                Vector3 baseWorldPos = joystickBase.position;

                float angleStep = 360f / directionSnaps;

                Handles.color = new Color(1f, 1f, 0f, 0.6f);
                for (int i = 0; i < directionSnaps; i++)
                {
                    float angle = i * angleStep;
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;

                    Vector3 start = baseWorldPos + (Vector3)(dir * (joystickRange - 10f));
                    Vector3 end = baseWorldPos + (Vector3)(dir * (joystickRange + 4f));

                    Handles.DrawLine(start, end);
                }

                // handle range
                Color cyanCol = Color.cyan;
                Handles.color = new(cyanCol.r, cyanCol.g, cyanCol.b, 0.03f);
                Handles.DrawSolidDisc(baseWorldPos, Vector3.forward, joystickRange / 80);
                Handles.color = cyanCol;
                Handles.DrawWireDisc(baseWorldPos, Vector3.forward, joystickRange / 80);

                // dead zone
                Handles.color = Color.red;
                Handles.DrawWireDisc(baseWorldPos, Vector3.forward, joystickRange * deadZone / 80);

                // direction output
                Handles.color = Color.white;
                Handles.DrawLine(baseWorldPos, baseWorldPos + ((Vector3)inputDirection * joystickRange / 80));
            }
        }
#endif

    }

}