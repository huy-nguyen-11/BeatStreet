using UnityEngine;

public class PlayerSpeedUp : PlayerStateManager
{
    public PlayerSpeedUp(PlayerController player) : base(player) { }
    public override void Enter()
    {
        playerController.animator.Play("SpeedUp");
        playerController.state = PlayerController.State.SpeedUp;
        AudioBase.Instance.AudioPlayer(2);
    }
    public override void Update()
    {
        SetSpeedUp();
    }
    public void SetSpeedUp()
    {
        Vector2 direction = playerController.joystick.Direction;
        playerController.rb.linearVelocity = Vector2.right * (!playerController.SpeedupDirection ? -4f : 4);
        playerController.transform.rotation = Quaternion.Euler(new Vector3(0, playerController.SpeedupDirection ? 0 : -180, 0));
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
