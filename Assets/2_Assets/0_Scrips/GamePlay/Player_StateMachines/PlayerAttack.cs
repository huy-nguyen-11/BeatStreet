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
    }

    private void PlayComboAttackWithEvent(bool useAddQueue)
    {
        //playerController.lastAttackHadHit = false;

        int idx = playerController.comboIndex % playerController.comboAttackAnims.Count;
        string animName = playerController.comboAttackAnims[idx];

        // If this is the last animation in the combo list, make player temporarily immortal
        if (idx == playerController.comboAttackAnims.Count - 1)
        {
            playerController.isImmortal = true;
        }

        //playerController.idAttackArea = playerController.comboIndex;// set id attack area == index combo
        playerController.idAttackArea = 0;// set id attack area == 0
        //Debug.Log($"Playing attack anim: {animName} (combo index: {playerController.comboIndex}, useAddQueue: {useAddQueue})");
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

        // Apply velocity using isFacingRight (reliable, not rotation.y)
        //float lunge = playerController.isFacingRight ? 0.5f : -0.5f;
        //playerController.rb.linearVelocity = Vector2.right * lunge;

        // Stop any previous lunge-stopper coroutine and start a new short stopper so the player
        // doesn't keep sliding through the rest of the animation.
        if (coroutine != null)
            playerController.StopCoroutine(coroutine);

        // Adjust this duration to taste (0.06 - 0.14 works well). Shorter -> sharper stop.
        float decelDuration = 0.165f;
        coroutine = playerController.StartCoroutine(StopLungeCoroutine(decelDuration/*, lunge*/));
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

    /// <summary>
    /// Callback khi event "Attack_end" được kích hoạt
    /// </summary>
    private void OnAttackEventFired(Spine.TrackEntry entry)
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

            // Finished full combo chain -> go back to idle
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Decide whether this strike "counts as hit" for advancing.
        // Use a timestamp window to avoid race conditions where collision detection happens a few frames late.
        bool hasRecentHit = playerController.lastAttackHadHit || playerController.IsRecentHit();

        // Preserve your original intent: require a real hit to continue into finishers (from index >= 2).
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
            // no confirmed hit: reset combo to start
            playerController.comboIndex = 0;
        }

        // consume/reset the flag
        playerController.lastAttackHadHit = false;

        if (playerController.queuedComboAttack)
        {
            playerController.queuedComboAttack = false;

            // Queue next strike smoothly (AddAnimation) to avoid hard-cut jitter.
            PlayComboAttackWithEvent(useAddQueue: true);
        }
        else
        {
            // No tap queued: allow a short grace window for late taps before returning to idle.
            playerController.BeginComboGraceWindow();
        }
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
