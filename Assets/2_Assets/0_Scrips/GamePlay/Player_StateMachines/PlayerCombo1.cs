using UnityEngine;
using System.Collections;

public class PlayerCombo1 : PlayerStateManager
{
    public PlayerCombo1(PlayerController player) : base(player) { }
    Coroutine coroutine;

    private const float attackForwardDistance = 1.3f;
    // Maximum time allowed to perform the forward movement (safety timeout)
    private const float attackForwardTimeout = 0.5f;

    public override void Enter()
    {
        playerController.state = PlayerController.State.Attack;
        //playerController.animator.Play("Combo1");
        bool wasRunning = (playerController.state == PlayerController.State.Run);
        playerController.PlayAnimAttack("Attack_1_5");

        playerController.idAttackArea = 4;// set id attack area == 4
                                          //playerController.rb.linearVelocity = Vector3.zero;
                                          // If a previous coroutine is running, stop it
        if (coroutine != null)
        {
            playerController.StopCoroutine(coroutine);
            coroutine = null;
        }

        // Start a short forward-movement then stop
        coroutine = playerController.StartCoroutine(MoveForwardThenStop(attackForwardDistance, attackForwardTimeout));
        AudioBase.Instance.AudioPlayer(0);
        //if (coroutine != null)
        //    playerController.StopCoroutine(coroutine);
        //coroutine = playerController.StartCoroutine(playerController.CheckAnimationAndTriggerEvent("Combo1"));
    }
    public override void Update()
    {

    }
    public void SetSwithJump()
    {
    }
    public override void Exit()
    {
        playerController.isSpeedUpAttack = false;
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

    private IEnumerator MoveForwardThenStop(float distance, float timeout)
    {
        if (playerController == null || playerController.rb == null)
            yield break;

        Vector3 startPos = playerController.transform.position;
        float elapsed = 0f;
        float dir = playerController.isFacingRight ? 1f : -1f;
        // use playerController.moveSpeed as a base; tweak multiplier if you want faster/slower push
        float pushSpeed = playerController.moveSpeed * 1.0f;

        // Keep only horizontal movement while preserving vertical velocity
        while (elapsed < timeout && Vector3.Distance(startPos, playerController.transform.position) < distance)
        {
            var lv = playerController.rb.linearVelocity;
            playerController.rb.linearVelocity = new Vector3(dir * pushSpeed, lv.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop horizontal movement cleanly
        if (playerController.rb != null)
        {
            var lv = playerController.rb.linearVelocity;
            playerController.rb.linearVelocity = new Vector3(0f, lv.y, 0f);
        }

        coroutine = null;
    }
}
