using UnityEngine;

public class EnemyDead : EnemyStateMachine
{
    public EnemyDead(EnemyController enemy) : base(enemy) { }
    private string deadAnim;
    public override void Enter()
    {
        deadAnim = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? "Death" : "Dead_2";
        enemyController.PlayAnim(deadAnim , false);
        AudioBase.Instance.AudioPlayerAtkHit();
        enemyController.state = EnemyController.State.Dead;

        if (enemyController.fillBar != null)
            enemyController.fillBar.SetNewHp(0f);
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
