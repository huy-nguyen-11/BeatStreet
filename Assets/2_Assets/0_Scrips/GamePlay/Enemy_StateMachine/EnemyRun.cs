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

        enemyController.patrolTimer = 0f;

        // FIX: Ensure isStopping is false when entering Run state
        // Only set to false if we're not in a forced stop situation
        if (enemyController.isStopping && enemyController.stopTimer < enemyController.stopDuration * 0.5f)
        {
            // If we just started stopping, allow it to complete
            // But if we're transitioning to Run, we should clear the stop flag
            enemyController.isStopping = false;
            enemyController.stopTimer = 0f;
        }
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
