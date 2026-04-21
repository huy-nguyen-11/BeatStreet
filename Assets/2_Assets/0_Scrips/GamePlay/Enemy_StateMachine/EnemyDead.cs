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

        switch(enemyController.typeOfEnemy)
        {
            case TypeOfEnemy.Enemy:
                GamePlayManager.Instance.SetMission(0, 1); // mission 0: kills enemy
                break;
            case TypeOfEnemy.Boss:
                GamePlayManager.Instance.SetMission(3, 1); // mission 3: kills boss
                break;
            case TypeOfEnemy.EliteEnemy:
                if (enemyController.idEnemy == 2 || enemyController.idEnemy == 10)
                {
                    GamePlayManager.Instance.SetMission(2, 1); // mission 2: kills elite enemy bomber
                }
                else
                {
                    GamePlayManager.Instance.SetMission(1, 1); // mission 1: kills elite enemy
                }
                break;
        }
        GamePlayManager.Instance.SetMission(0, 1); // mission 0: kill 1 enemy
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
