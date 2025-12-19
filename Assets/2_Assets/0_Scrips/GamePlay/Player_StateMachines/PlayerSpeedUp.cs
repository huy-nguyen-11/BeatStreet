using UnityEngine;

public class PlayerSpeedUp : PlayerStateManager
{
    public PlayerSpeedUp(PlayerController player) : base(player) { }
    public override void Enter()
    {
        Debug.Log("Enter Speed Up State");
        playerController.PlayAnim("Speed_Run", true);
        playerController.state = PlayerController.State.SpeedUp;
        AudioBase.Instance.AudioPlayer(2);

        //var entry = playerController.skeletonAnimation != null ? playerController.skeletonAnimation.AnimationState.GetCurrent(0) : null;
        //Debug.Log("After PlayAnim: spine entry = " + (entry != null && entry.Animation != null ? entry.Animation.Name : "null"));
    }
    public override void Update()
    {
        SetSpeedUp();
    }
    public void SetSpeedUp()
    {
        playerController.rb.linearVelocity = Vector2.right * (!playerController.SpeedupDirection ? -6f : 6);
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
