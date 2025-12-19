using UnityEngine;

public class PlayerRun : PlayerStateManager
{
    public PlayerRun(PlayerController player) : base(player) { }
    private float holdThreshold = 0.15f;
    public override void Enter()
    {

        playerController.PlayAnim("Run", true);
        playerController.state = PlayerController.State.Run;
    }
    public override void Update()
    {
        playerController.SetMovePlayer(3f);
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
