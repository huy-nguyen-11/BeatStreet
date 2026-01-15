using Spine.Unity;
using System.Collections;
using UnityEngine;

public class PlayerJump : PlayerStateManager
{
    public PlayerJump(PlayerController player) : base(player) { }
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDistance = 1.65f;
    //[SerializeField] private float jumpDuration = 1f;
    [SerializeField] private int jumpCount = 1;
    private GameObject fx;

    private Coroutine _coroutine;

    public override void Enter()
    {
        if (playerController.fillBar == null) return;
        playerController.isJumping = true;
        playerController.state = PlayerController.State.Jump;
        jumpDistance = playerController.fillBar.mana >= 5 ? 4f : 4.6f;
        if (playerController.fillBar.mana >= 5)
        {
            playerController.velocity = 4;
            GamePlayManager.Instance.SetMission(6, 1);
            playerController.SetMana(-5);
            playerController.PlayAnim2("Jump_Attack");
            playerController.idAttackArea = 2;// set id attack area == 2
            fx = ObjectPooler.Instance.SpawnFromPool("FxJump_Attack", playerController.posFxJump.position,Quaternion.identity);
            fx.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "jump_Attack_Fx", false);
        }
        else
        {
            playerController.velocity = 5;
            playerController.PlayAnim2("Jump");
            fx =ObjectPooler.Instance.SpawnFromPool("FxJump", playerController.posFxJump.position, Quaternion.identity);
            fx.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "jump", false);
        }
    }

    public override void Update()
    {
        playerController.ProcessGravity();

        SetJump();
    }

    private void SetJump()
    {

        if (playerController.isResettingFromJump)
        {
            return;
        }
        
        //bool Direction = playerController.transform.rotation.y != 0 ? false : true;
        bool Direction = playerController.isFacingRight;
        if (!Direction)
            playerController.rb.linearVelocity = -Vector2.right * jumpDistance;
        else
            playerController.rb.linearVelocity = Vector2.right * jumpDistance;
    }
    public override void Exit()
    {
        if (_coroutine != null)
        {
            //playerController.StopCoroutine(_coroutine);
        }
        fx.SetActive(false);
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
