using System.Collections;
using UnityEngine;

public class PlayerJump : PlayerStateManager
{
    public PlayerJump(PlayerController player) : base(player) { }
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDistance = 1.65f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private int jumpCount = 1;

    private Coroutine _coroutine;

    public override void Enter()
    {
        if (playerController.fillBar == null) return;
        playerController.state = PlayerController.State.Jump;
        jumpDistance = playerController.fillBar.mana >= 20 ? 2f : 1.65f;
        jumpDuration = playerController.fillBar.mana >= 20 ? 0.5f : 0.65f;
        if (playerController.fillBar.mana >= 5)
        {
            playerController.velocity = 5;
            GamePlayManager.Instance.SetMission(6, 1);
            playerController.SetMana(-5);
            playerController.PlayAnim2("Jump_Attack");
            /*if (playerController.id != 0)
                playerController.SetRunSkill2();*/
        }
        else
        {
            playerController.velocity = 4;
            playerController.PlayAnim2("Jump");
        }
        //_coroutine = playerController.StartCoroutine(playerController.JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        while (!playerController.isCheckGravity ||
            (playerController.velocity > 0 && playerController.isCheckGravity))
        {
            yield return null;
        }
        playerController.ResetStatus();
    }

    public override void Update()
    {
        playerController.ProcessGravity();
        SetJump();
    }

    private void SetJump()
    {
        bool Direction = playerController.transform.rotation.y != 0 ? false : true;
        if (!Direction)
            playerController.rb.linearVelocity = -Vector2.right * 5f;
        else
            playerController.rb.linearVelocity = Vector2.right * 5f;
    }
    public override void Exit()
    {
        if (_coroutine != null)
        {
            //playerController.StopCoroutine(_coroutine);
        }

        //playerController.ClearFxTrack();
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
