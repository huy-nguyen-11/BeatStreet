using UnityEngine;

public class PlayerCombo3 : PlayerStateManager
{
    public PlayerCombo3(PlayerController player) : base(player) { }
    Coroutine coroutine;
    public override void Enter()
    {
        playerController.state = PlayerController.State.Attack;
        playerController.animator.Play("Combo3");
        AudioBase.Instance.AudioPlayer(0);
        if (coroutine != null)
            playerController.StopCoroutine(coroutine);
        coroutine = playerController.StartCoroutine(playerController.CheckAnimationAndTriggerEvent("Combo3"));
    }
    public override void Update()
    {

    }
    public void SetSwithJump()
    {
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
