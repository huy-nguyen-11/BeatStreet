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
        
        // Check attack ngay khi vào Run state để tránh animation Run không cần thiết
        // Đặc biệt quan trọng với Boss sau khi tấn công xong
        // Gọi CheckAttack() trực tiếp để tận dụng logic đã có sẵn
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
        
        // Check attack trước khi di chuyển để tránh animation Run không cần thiết
        if (!GamePlayManager.Instance.isCheckUlti)
        {
            // Check attack trước khi di chuyển - nếu có thể attack sẽ chuyển state ngay
            // Điều này đặc biệt quan trọng với Boss sau khi tấn công xong
            if (enemyController.state != EnemyController.State.Hit
                && enemyController.state != EnemyController.State.Fall
                && enemyController.state != EnemyController.State.Attack)
            {
                enemyController.CheckAttack();
            }
            
            // Chỉ di chuyển nếu vẫn còn ở Run state (chưa chuyển sang Attack)
            if (enemyController.state == EnemyController.State.Run)
            {
                // <-- ADDED DEBUG: trace caller of Movement -> will lead to MoveToPlayer
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
