using UnityEngine;
using System.Collections;
using Spine;

public class PlayerAttack : PlayerStateManager
{
    public PlayerAttack(PlayerController player) : base(player) { }
    private float holdThreshold = 0.07f;
    Coroutine coroutine;
    private bool _startedFirstAttack = false;

    public override void Enter()
    {
        playerController.CancelComboGraceWindow();
        playerController.state = PlayerController.State.Attack;
        playerController.rb.linearVelocity = Vector2.zero;

        Transform target = playerController.Char ?? playerController.transform;
        float yRot = target.localEulerAngles.y;
        bool facingRight = Mathf.Abs(Mathf.DeltaAngle(yRot, 0f)) < 90f;
        playerController.SetFacingDirection(facingRight);

        // Start first attack in combo
        //StartCombo();
        // When entering Attack state from idle/start, always start the first strike via SetAnimation.
        // Subsequent strikes in the same combo should be queued via AddAnimation for smooth chaining.
        _startedFirstAttack = false;
        PlayComboAttackWithEvent(useAddQueue: false);
        //AudioBase.Instance.AudioPlayer(12);
    }

    public override void Update()
    {
        if (playerController.queuedComboAttack && playerController.IsInsideComboGraceWindow())
        {
            playerController.queuedComboAttack = false;
            playerController.CancelComboGraceWindow(); // Ensure grace window is canceled before starting next attack
            AdvanceComboAndPlayNext();
        }
    }

    private void AdvanceComboAndPlayNext()
    {
        int current = playerController.comboIndex;
        int maxIndex = playerController.comboAttackAnims.Count - 1;

        if (current == maxIndex)
        {
            playerController.isImmortal = false;
            playerController.comboIndex = 0;
            playerController.queuedComboAttack = false;
            playerController.lastAttackHadHit = false;
            playerController.isAttackCooldown = true;
            playerController.attackCooldownTimer = playerController.attackCooldownDuration;
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        bool hasRecentHit = playerController.lastAttackHadHit || playerController.IsRecentHit();
        bool requireHitForThisAdvance = current >= 2;
        bool canAdvance = !requireHitForThisAdvance || hasRecentHit;

        if (canAdvance)
        {
            playerController.comboIndex++;
            if (playerController.comboIndex > maxIndex)
                playerController.comboIndex = 0;
        }
        else
        {
            playerController.comboIndex = 0;
        }

        playerController.lastAttackHadHit = false;
        
        // When advancing from inside grace window or from an event, we play the next attack.
        // If we were in the grace window, useAddQueue=false is safer because the previous animation is already done.
        PlayComboAttackWithEvent(useAddQueue: false);
    }

    public void TriggerQueuedCombo()
    {
        if (playerController.queuedComboAttack)
        {
            playerController.queuedComboAttack = false;
            playerController.CancelComboGraceWindow();
            AdvanceComboAndPlayNext();
        }
    }

    private void OnAttackEventFired(Spine.TrackEntry entry)
    {
        if (playerController.queuedComboAttack)
        {
            playerController.queuedComboAttack = false;
            AdvanceComboAndPlayNext();
        }
        else
        {
            playerController.BeginComboGraceWindow();
        }
    }

    private void PlayComboAttackWithEvent(bool useAddQueue)
    {
        int idx = playerController.comboIndex % playerController.comboAttackAnims.Count;
        string animName = playerController.comboAttackAnims[idx];

        if (idx == playerController.comboAttackAnims.Count - 1)
        {
            playerController.isImmortal = true;
        }

        playerController.idAttackArea = 0;
        AudioBase.Instance.AudioPlayer(playerController.comboIndex);

        if (!useAddQueue)
        {
            _startedFirstAttack = true;
            playerController.PlayAnimWithEventHandler(animName, false, OnAttackEventFired);
        }
        else
        {
            playerController.AddAnimWithEventHandler(animName, false, 0f, OnAttackEventFired);
        }

        if (coroutine != null)
            playerController.StopCoroutine(coroutine);

        float decelDuration = 0.165f;
        coroutine = playerController.StartCoroutine(StopLungeCoroutine(decelDuration));
    }


    private IEnumerator StopLungeCoroutine(float duration/*, float initialLunge*/)
    {
        yield return new WaitForSeconds(0.15f);
        float lunge = playerController.isFacingRight ? 0.36f : -0.36f;
        playerController.rb.linearVelocity = Vector2.right * lunge;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Ease-out curve: remaining = 1 - t^2 (smooth slow-down)
            float remaining = 1f - (t * t);
            if (playerController != null && playerController.rb != null)
            {
                Vector2 v = playerController.rb.linearVelocity;
                playerController.rb.linearVelocity = new Vector2(lunge * remaining, v.y);
            }
            yield return null;
        }

        // ensure fully stopped horizontally if still in Attack state
        if (playerController != null && playerController.state == PlayerController.State.Attack && playerController.rb != null)
        {
            Vector2 v = playerController.rb.linearVelocity;
            playerController.rb.linearVelocity = new Vector2(0f, v.y);
        }

        coroutine = null;
    }

    public override void Exit()
    {
        if (coroutine != null)
            playerController.StopCoroutine(coroutine);
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
