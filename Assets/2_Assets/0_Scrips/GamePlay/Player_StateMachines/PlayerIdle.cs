using UnityEngine;

public class PlayerIdle : PlayerStateManager
{
    public PlayerIdle(PlayerController player) : base(player) { }
    private float holdThreshold = 0.15f;
    public override void Enter()
    {
        playerController.state = PlayerController.State.Idle;
        playerController.rb.linearVelocity = Vector2.zero;

        playerController.PlayAnim("Idle", true);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        //playerController.TimeCheckChange = 0;
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
