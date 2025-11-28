using Spine;
using Spine.Unity;
using UnityEngine;

public class DemoTouch : MonoBehaviour
{
    private float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody2D rb;
    public Transform joyPos;

    // thresholds for walk/run decisions (use raw input magnitude for immediate response)
    [SerializeField] private float walkThreshold;
    [SerializeField] private float runThreshold;

    public void FixedUpdate()
    {
        if (variableJoystick == null || rb == null)
            return;

        // use raw magnitude (no smoothing) to decide walk vs run so it's responsive
        Vector2 rawInput = variableJoystick.Direction;
        float rawMag = rawInput.magnitude;

        if (rawMag > 0.1f)
        {
            float currentSpeed = 0f;
            if (rawMag >= runThreshold)
                currentSpeed = 4f; // run
            else if (rawMag >= walkThreshold)
                currentSpeed = 1.5f; // walk

            if (currentSpeed > 0f)
                rb.linearVelocity = rawInput.normalized * currentSpeed;
            else
                rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
