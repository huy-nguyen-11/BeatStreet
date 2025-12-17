using UnityEngine;

public class PlayerCombo1 : PlayerStateManager
{
    public PlayerCombo1(PlayerController player) : base(player) { }
    Coroutine coroutine;
    public override void Enter()
    {
        Debug.Log($"PlayerCombo1.Enter previousState={playerController.state}");
        playerController.state = PlayerController.State.Attack;
        //playerController.animator.Play("Combo1");
        bool wasRunning = (playerController.state == PlayerController.State.Run);
        playerController.PlayAnimAttack("Attack_1_5");
        playerController.rb.linearVelocity = Vector3.zero;
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
}
