using System.Collections;
using UnityEngine;

public class PlayerDead : PlayerStateManager
{
    public PlayerDead(PlayerController player) : base(player) { }
    bool isFall;
    Coroutine _coroutine;
    public override void Enter()
    {
        playerController.PlayAnim("Dead" , false);
        playerController.state = PlayerController.State.Dead;
        playerController.rb.linearVelocity = Vector2.zero;
        playerController.isFall = true;
        playerController.velocity = 8;
        AudioBase.Instance.AudioPlayer(5);
        _coroutine = playerController.StartCoroutine(FallCoroutine());
        isFall = true;
    }
    public override void Update()
    {
        if (isFall)
        {
            playerController.ProcessGravity();
            playerController.SetFall();
        }
    }
    public IEnumerator FallCoroutine()
    {
        while (!playerController.isCheckGravity ||
            (playerController.velocity > 0 && playerController.isCheckGravity))
        {
            yield return null;
        }
        isFall = false;
        playerController.rb.linearVelocity = Vector2.zero;
    }
    public void SetSwithJump()
    {
    }
    public override void Exit()
    {
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
