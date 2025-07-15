using UnityEngine;

public class PlayerWingame : PlayerStateManager
{
    public PlayerWingame(PlayerController player) : base(player) { }
    public override void Enter()
    {
        playerController.state = PlayerController.State.Wingame;
        playerController.rb.velocity = Vector2.zero;
        playerController.animator.Play("WinGame");
    }
    public override void Update()
    {

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
