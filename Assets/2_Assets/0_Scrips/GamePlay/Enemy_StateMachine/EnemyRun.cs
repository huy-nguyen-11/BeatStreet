using UnityEngine;

public class EnemyRun : EnemyStateMachine
{
    public EnemyRun(EnemyController enemy) : base(enemy) { }
    public override void Enter()
    {
        if (!enemyController.isStopping)
        {
            enemyController.animator.Play("Run");
            enemyController.state = EnemyController.State.Run;
        }
    }
    public override void Update()
    {
        if (enemyController.playerController.IsDead) enemyController.SwitchToRunState(enemyController.enemyIdle);
        if (!GamePlayManager.Instance.isCheckUlti)
            if (enemyController.state != EnemyController.State.Hit)
                enemyController.Movement();
        AnimatorStateInfo state = enemyController.animator.GetCurrentAnimatorStateInfo(0);
        if (!state.IsName("Run")&& !enemyController.isStopping)
            enemyController.animator.Play("Run");
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
