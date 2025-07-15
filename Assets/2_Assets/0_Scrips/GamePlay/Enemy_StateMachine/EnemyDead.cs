using UnityEngine;

public class EnemyDead : EnemyStateMachine
{
    public EnemyDead(EnemyController enemy) : base(enemy) { }
    public override void Enter()
    {
        enemyController.animator.Play("Dead");
        AudioBase.Instance.AudioPlayerAtkHit();
        enemyController.state = EnemyController.State.Dead;
    }
    public override void Update()
    {

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
