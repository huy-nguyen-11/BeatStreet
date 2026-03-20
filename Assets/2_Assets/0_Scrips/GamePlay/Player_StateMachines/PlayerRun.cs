using UnityEngine;

public class PlayerRun : PlayerStateManager
{
    public PlayerRun(PlayerController player) : base(player) { }
    private float holdThreshold = 0.15f;
    public override void Enter()
    {
        playerController.PlayAnim("Run", true);
        playerController.state = PlayerController.State.Run;

        if (playerController.joystick != null && playerController.rb != null)
        {
            Vector2 rawDir = playerController.joystick.RawDirection;
            if (rawDir.sqrMagnitude > 0f)
            {
                Vector2 movement = rawDir.normalized * 4f;
                playerController.rb.linearVelocity = movement;
            }
        }
    }
    public override void Update()
    {
        playerController.SetMovePlayer(2.5f);

        if (playerController.canGrab)
        {
            playerController.CheckGrabEnemy();
        }
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
