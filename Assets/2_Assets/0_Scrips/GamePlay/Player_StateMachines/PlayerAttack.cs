using UnityEngine;
using System.Collections;
using Spine;

public class PlayerAttack : PlayerStateManager
{
    public PlayerAttack(PlayerController player) : base(player) { }
    private float holdThreshold = 0.07f;
    Coroutine coroutine;

    public override void Enter()
    {
        playerController.state = PlayerController.State.Attack;
        playerController.rb.linearVelocity = Vector2.zero;

        Transform target = playerController.Char ?? playerController.transform;
        float yRot = target.localEulerAngles.y;
        bool facingRight = Mathf.Abs(Mathf.DeltaAngle(yRot, 0f)) < 90f;
        playerController.SetFacingDirection(facingRight);

        // Start first attack in combo
        //StartCombo();
        PlayComboAttackWithEvent();
        AudioBase.Instance.AudioPlayer(0);
    }
    public override void Update()
    {
    }

    // Public entry point for controller to start/continue a combo attack
    //public void StartCombo()
    //{
    //    int idx = playerController.comboAttack;
    //    string animName = "Attack_1";
    //    switch (idx)
    //    {
    //        case 0: animName = "Attack_1"; break;
    //        case 1: animName = "Attack_1_2"; break;
    //        case 2: animName = "Attack_1_3"; break;
    //    }

    //    // Use PlayerCharacter helper that attaches event handler directly to the TrackEntry
    //    playerController.PlayAnimWithEventHandler(animName, false, OnAttackEnd);

    //    // apply small lunge
    //    playerController.rb.linearVelocity = Vector2.left * (playerController.transform.rotation.y != 0 ? 0.1f : -0.1f);
    //}

    //// Callback from PlayAnimWithEventHandler when animation signals end (via Spine event or timeout)
    //private void OnAttackEnd(TrackEntry entry)
    //{
    //    if (playerController.state != PlayerController.State.Attack) return;

    //    // increment combo index and wrap
    //    playerController.comboAttack++;
    //    if (playerController.comboAttack > 2) playerController.comboAttack = 0;

    //    if (playerController.queuedComboAttack)
    //    {
    //        playerController.queuedComboAttack = false;
    //        Debug.Log("Continuing combo attack " + playerController.comboAttack + " time:" + Time.time);
    //        StartCombo();
    //    }
    //    else
    //    {
    //        Debug.Log("Attack ended, returning to idle" + Time.time);
    //        playerController.ResetStatus();
    //    }
    //}

    private void PlayComboAttackWithEvent()
    {
        int idx = playerController.comboIndex % playerController.comboAttackAnims.Count;
        string animName = playerController.comboAttackAnims[idx];

        playerController.PlayAnimWithEventHandler(animName, false, OnAttackEventFired);

        // Apply velocity using isFacingRight (reliable, not rotation.y)
        float lunge = playerController.isFacingRight ? 0.1f : -0.1f;
        playerController.rb.linearVelocity = Vector2.left * lunge;
    }

    /// <summary>
    /// Callback khi event "Attack_end" được kích hoạt
    /// </summary>
    private void OnAttackEventFired(Spine.TrackEntry entry)
    {
        if (playerController.state != PlayerController.State.Attack)
            return;
        playerController.comboIndex++;
        if (playerController.comboIndex >= playerController.comboAttackAnims.Count)
            playerController.comboIndex = 0;

        if (playerController.queuedComboAttack)
        {
            playerController.queuedComboAttack = false;
            PlayComboAttackWithEvent();
        }
        else
        {
            playerController.ResetStatus();
        }
    }


    public override void Exit()
    {
        //if (coroutine != null)
        //    playerController.StopCoroutine(coroutine);
    }
    public override void FixedUpdate()
    {
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
    }
    public override void OnTriggerEnter(Collider2D collision)
    {
    }
    public override void OnTriggerExit(Collider2D collision)
    {
    }
    public override void OnTriggerStay(Collider2D collision)
    {
    }
}
