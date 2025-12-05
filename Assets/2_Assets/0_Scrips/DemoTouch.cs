using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class DemoTouch : MonoBehaviour
{
    [Header("Tap / Hold")]
    public float tapMaxTime = 0.25f;
    public float strengthChargeTime = 0.3f;
    public float heavyReadyExtra = 0.4f;
    private float heavyReadyTime;

    private bool isHolding = false;
    private float holdStartTime = 0f;
    private bool strengthPlayed = false;

    [Header("Swipe / Jump")]
    public float swipeThreshold = 120f;
    private Vector2 dragStartPos;

    [Header("Spine")]
    public SkeletonAnimation spine;
    public List<string> attackAnimations;
    private Spine.AnimationState anim;

    // combo state
    private int comboIndex = 0;
    private bool isAttacking = false;
    private bool queuedAttack = false;

    [Header("Joystick Movement")]
    public Joystick joystick;
    public Rigidbody2D rb;
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;
    private bool isJoystickMoving = false;

    void Start()
    {
        anim = spine.AnimationState;
        anim.SetAnimation(0, "Idle", true);
        heavyReadyTime = strengthChargeTime + heavyReadyExtra;

        anim.Event += HandleSpineEvent;

    }

    void Update()
    {
        //#if UNITY_EDITOR
        //        HandleMouseInput();          // vẫn giữ để test trong Editor
        //#else
        //        HandleTouchInput();          // build mobile → dùng touch
        //#endif
        HandleTouchInput();
        HandleJoystickMovement();
    }

    void HandleJoystickMovement()
    {
        if (joystick == null || rb == null) return;

        Vector2 dir = joystick.SmoothedDirection;
        float mag = joystick.SmoothedMagnitude;

        // 1. Không chạm → idle
        if (mag <= 0.01f)
        {
            if (isJoystickMoving)
            {
                anim.SetAnimation(0, "Idle", true);
                isJoystickMoving = false;
            }

            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 2. Không cho di chuyển nếu đang: combo, charge, heavy, swipe, jump
        if (isAttacking || isHolding || strengthPlayed)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 3. Chọn tốc độ run/walk
        float speed = (mag >= 0.7f) ? runSpeed : walkSpeed;

        rb.linearVelocity = dir * speed;

        // 4. Animation tương ứng
        if (!isJoystickMoving)
            isJoystickMoving = true;

        if (mag >= 0.7f)
            anim.SetAnimation(0, "Run", true);
        else
            anim.SetAnimation(0, "Walk", true);
    }


    // ============================================================
    // TOUCH INPUT (CHUẨN MOBILE - KHÔNG DELAY)
    // ============================================================
    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        // ==========================
        // TOUCH BEGAN
        // ==========================
        if (t.phase == TouchPhase.Began)
        {
            //allowAnimationUpdateFromEvent = false;
            dragStartPos = t.position;

            isHolding = true;
            holdStartTime = Time.time;
            strengthPlayed = false;
        }

        // ==========================
        // HOLD CHECK (CHARGE)
        // ==========================
        if (isHolding && !strengthPlayed)
        {
            float holdTime = Time.time - holdStartTime;

            if (holdTime >= strengthChargeTime)
            {
                strengthPlayed = true;
                PlayStrength();
            }
        }

        // ==========================

        // TOUCH ENDED (QUAN TRỌNG – TAP NHANH KHÔNG DELAY)
        // ==========================
        if (t.phase == TouchPhase.Ended)
        {
            //allowAnimationUpdateFromEvent = true;

            float holdTime = Time.time - holdStartTime;
            Vector2 diff = t.position - dragStartPos;

            isHolding = false;

            // 1. Strength bắt đầu nhưng chưa đủ Heavy → hủy charge
            if (strengthPlayed && holdTime < heavyReadyTime)
            {
                strengthPlayed = false;
                anim.SetAnimation(0, "Idle", true);
                return;
            }

            // 2. Heavy attack
            if (strengthPlayed && holdTime >= heavyReadyTime)
            {
                PlayHeavyAttack();
                return;
            }

            // 3. Swipe up → Jump
            if (diff.magnitude > swipeThreshold && diff.y > Mathf.Abs(diff.x))
            {
                PlayJump();
                return;
            }

            // 4. Swipe ngang → Speed_Run
            if (diff.magnitude > swipeThreshold && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                PlaySpeedRun();
                return;
            }

            // 5. TAP (Tap chuẩn trên touch là check Ended)
            if (holdTime < tapMaxTime)
            {
                PlayComboAttack();
                return;
            }
        }
    }

    // ===================================================================
    // (GIỮ NGUYÊN) MOUSE INPUT – ĐỂ TEST TRÊN EDITOR
    // ===================================================================
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;

            isHolding = true;
            holdStartTime = Time.time;
            strengthPlayed = false;
        }

        if (isHolding && !strengthPlayed)
        {
            float holdTime = Time.time - holdStartTime;
            if (holdTime >= strengthChargeTime)
            {
                strengthPlayed = true;
                PlayStrength();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            float holdTime = Time.time - holdStartTime;
            Vector2 diff = (Vector2)Input.mousePosition - dragStartPos;

            isHolding = false;

            if (strengthPlayed && holdTime < heavyReadyTime)
            {
                strengthPlayed = false;
                anim.SetAnimation(0, "Idle", true);
                return;
            }

            if (strengthPlayed && holdTime >= heavyReadyTime)
            {
                PlayHeavyAttack();
                return;
            }

            if (diff.magnitude > swipeThreshold && diff.y > Mathf.Abs(diff.x))
            {
                PlayJump();
                return;
            }

            if (diff.magnitude > swipeThreshold && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                PlaySpeedRun();
                return;
            }

            if (holdTime < tapMaxTime)
            {
                PlayComboAttack();
                return;
            }
        }
    }

    // ============================================================
    // COMBAT / MOVE / SPINE (GIỮ NGUYÊN)
    // ============================================================
    void PlayComboAttack()
    {
        if (isAttacking)
        {
            queuedAttack = true;
            return;
        }

        isAttacking = true;
        queuedAttack = false;

        string animName = attackAnimations[comboIndex];
        anim.SetAnimation(0, animName, false);
    }

    void PlayHeavyAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        comboIndex = 0;
        queuedAttack = false;

        var entry = anim.SetAnimation(0, "Strength_Attack", false);
        entry.Complete += (t) =>
        {
            isAttacking = false;
            anim.SetAnimation(0, "Idle", true);
        };
    }

    void PlayStrength()
    {
        if (isAttacking) return;
        anim.SetAnimation(0, "Strength", false);
    }

    void PlayJump()
    {
        if (isAttacking) return;

        var entry = anim.SetAnimation(0, "Jump", false);
        entry.Complete += (t) =>
        {
            anim.SetAnimation(0, "Idle", true);
        };
    }

    void PlaySpeedRun()
    {
        if (isAttacking) return;

        var entry = anim.SetAnimation(0, "Speed_Run", false);
        entry.Complete += (t) =>
        {
            anim.SetAnimation(0, "Idle", true);
        };
    }

    void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "End_attack")
        {
            isAttacking = false;

            comboIndex++;
            if (comboIndex >= attackAnimations.Count)
                comboIndex = 0;

            if (queuedAttack)
            {
                queuedAttack = false;
                PlayComboAttack();
            }
            else
            {
                anim.SetAnimation(0, "Idle", true);
            }
        }
    }
}
