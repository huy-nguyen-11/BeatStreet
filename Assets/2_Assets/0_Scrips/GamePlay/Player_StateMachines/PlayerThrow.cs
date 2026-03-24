using UnityEngine;

public class PlayerThrow : PlayerStateManager
{
    public PlayerThrow(PlayerController player) : base(player) { }
    public override void Enter()
    {
        playerController.state = PlayerController.State.Throw;
        playerController.PlayAnim("Grab_L", false);
        AudioBase.Instance.AudioPlayer(18);
        AudioBase.Instance.AudioPlayer(21);
    }
    public override void Update()
    {

    }
    public void SetThrow()
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
