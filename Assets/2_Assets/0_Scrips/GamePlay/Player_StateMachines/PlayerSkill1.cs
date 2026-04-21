using System.Collections;
using System.Drawing;
using UnityEngine;

public class PlayerSkill1 : PlayerStateManager
{
    public PlayerSkill1(PlayerController player) : base(player) { }
    private Coroutine coroutine;

    private const float attackForwardDistance = 1.35f;
    // Maximum time allowed to perform the forward movement (safety timeout)
    private const float attackForwardTimeout = 0.25f;

    public override void Enter()
    {
        //playerController.animator.Play("Skill1");
        bool wasRunning = (playerController.state == PlayerController.State.Run);
        playerController.PlayAnimAttack(playerController.animSlideAttack2);
        AudioBase.Instance.AudioPlayer(14);
        AudioBase.Instance.AudioPlayer(20);
        playerController.idAttackArea = 4;// set id attack area == 4
        //playerController.rb.linearVelocity = Vector3.zero;
        coroutine = playerController.StartCoroutine(MoveForwardThenStop(attackForwardDistance, attackForwardTimeout));
        playerController.state = PlayerController.State.Skill1;
        GamePlayManager.Instance.SetMission(7, 1); // mission 7: perform 1 dash attack
        //AudioBase.Instance.AudioPlayer(0);
        //AudioBase.Instance.AudioPlayer(4);
        //SetSkill1();
    }
    public override void Update()
    {

    }
    public void SetSkill1()
    {
        float point = playerController.transform.rotation.y == 0 ? 4f : -4f;
        playerController.rb.linearVelocity = Vector3.right * point;
        coroutine = playerController.StartCoroutine(ResetStatus());
    }
    IEnumerator ResetStatus()
    {
        yield return new WaitForSeconds(0.55f);
        playerController.ResetStatus();
    }
    public override void Exit()
    {
        //playerController.StopCoroutine(coroutine);
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

        yield return new WaitForSeconds(0.1f); // slight delay to sync with animation start

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
