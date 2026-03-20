using UnityEngine;


public class PlayerWalk : PlayerStateManager
{
    public PlayerWalk(PlayerController player) : base(player) { }
    private float holdThreshold = 0.15f;
    public override void Enter()
    {
        playerController.PlayAnim("Walk", true);
        playerController.state = PlayerController.State.Walk;
        

        if (playerController.joystick != null && playerController.rb != null)
        {
            Vector2 rawDir = playerController.joystick.RawDirection;
            if (rawDir.sqrMagnitude > 0f)
            {
                Vector2 movement = rawDir.normalized * 1.5f;
                playerController.rb.linearVelocity = movement;
            }
        }
    }
    public override void Update()
    {
        playerController.SetMovePlayer(1.2f);

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
