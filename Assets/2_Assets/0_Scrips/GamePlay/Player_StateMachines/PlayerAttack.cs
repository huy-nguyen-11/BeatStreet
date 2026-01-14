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

    private void PlayComboAttackWithEvent()
    {
        int idx = playerController.comboIndex % playerController.comboAttackAnims.Count;
        string animName = playerController.comboAttackAnims[idx];

        //playerController.idAttackArea = playerController.comboIndex;// set id attack area == index combo
        playerController.idAttackArea = 0;// set id attack area == 0

        playerController.PlayAnimWithEventHandler(animName, false, OnAttackEventFired);

        // Apply velocity using isFacingRight (reliable, not rotation.y)
        float lunge = playerController.isFacingRight ? 0.1f : -0.1f;
        playerController.rb.linearVelocity = Vector2.right * lunge;
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
