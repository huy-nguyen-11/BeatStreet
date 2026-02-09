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
        

        if (!enemyController.isGrabbed 
            && !enemyController.playerController.IsDead 
            && !GamePlayManager.Instance.isCheckUlti
            && enemyController.state != EnemyController.State.Hit
            && enemyController.state != EnemyController.State.Fall
            && enemyController.state != EnemyController.State.Attack)
        {
            enemyController.CheckAttack();
        }
    }
    public override void Update()
    {
        //if (enemyController.playerController.IsDead) enemyController.SwitchToRunState(enemyController.enemyIdle);
        //// Stop execution if grabbed
        //if (enemyController.isGrabbed)
        //{
        //    return;
        //}
        //if (!GamePlayManager.Instance.isCheckUlti)
        //    if (enemyController.state != EnemyController.State.Hit)
        //        enemyController.Movement();
        if (enemyController.playerController.IsDead) enemyController.SwitchToRunState(enemyController.enemyIdle);
        // Stop execution if grabbed
        if (enemyController.isGrabbed)
        {
            return;
        }
        

        if (!GamePlayManager.Instance.isCheckUlti)
        {
            if (enemyController.state != EnemyController.State.Hit
                && enemyController.state != EnemyController.State.Fall
                && enemyController.state != EnemyController.State.Attack)
            {
                enemyController.CheckAttack();
            }
            

            if (enemyController.state == EnemyController.State.Run)
            {
                if (enemyController.typeOfEnemy == TypeOfEnemy.Boss && enemyController.idEnemy == 2 && enemyController.player != null)
                {
                    float dx = Mathf.Abs(enemyController.Char.position.x - enemyController.player.position.x);
                    float dy = Mathf.Abs(enemyController.Char.position.y - enemyController.player.position.y);
                    bool farEnough = (dy > 2f && dx > 1.5f) || dx > 2f;
                    //if (farEnough && Random.value <= enemyController.directChaseRatio)
                    //    enemyController.PrepareDirectChase();
                }
                enemyController.Movement();
            }
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
