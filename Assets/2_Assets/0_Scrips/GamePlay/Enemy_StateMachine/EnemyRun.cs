using UnityEngine;

public class EnemyRun : EnemyStateMachine
{
    public EnemyRun(EnemyController enemy) : base(enemy) { }
    public override void Enter()
    {
        enemyController.state = EnemyController.State.Run;
        
        // Reset move animation to ensure proper animation is set when entering Run state
        // This is especially important when transitioning from Fall/Idle states
        enemyController.ResetMoveAnimation();
    }
    public override void Update()
    {
        if (enemyController.playerController.IsDead) enemyController.SwitchToRunState(enemyController.enemyIdle);
        // Stop execution if grabbed
        if (enemyController.isGrabbed)
        {
            return;
        }
        if (!GamePlayManager.Instance.isCheckUlti)
            if (enemyController.state != EnemyController.State.Hit)
                enemyController.Movement();

        //var current = enemyController.skeletonAnimation.AnimationState.GetCurrent(0);

        //if (!enemyController.isStopping)
        //{
        //    if (current == null || current.Animation.Name != "Run")
        //    {
        //        enemyController.PlayAnim("Run", true);
        //    }
        //}
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
