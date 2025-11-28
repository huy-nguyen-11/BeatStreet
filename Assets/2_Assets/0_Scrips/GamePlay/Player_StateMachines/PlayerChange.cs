using UnityEngine;

public class PlayerChange : PlayerStateManager
{
    public PlayerChange(PlayerController player) : base(player) { }
    private float holdThreshold = 0.25f;
    public override void Enter()
    {
        playerController.state = PlayerController.State.Change;
        playerController.animator.Play("Change");
        AudioBase.Instance.AudioPlayer(7);
        playerController.rb.linearVelocity = Vector2.zero;
    }
    public override void Update()
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
