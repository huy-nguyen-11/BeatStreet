using UnityEngine;

public class DemoTouch : MonoBehaviour
{
    private void Update()
    {
        TouchDemo();
    }

    void TouchDemo()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int touchId = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("touch bagan" + Time.time);
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Stationary:

                    break;
                case TouchPhase.Ended:
                    Debug.Log("attack:" + Time.time);
                    break;
            }
        }
    }
}
