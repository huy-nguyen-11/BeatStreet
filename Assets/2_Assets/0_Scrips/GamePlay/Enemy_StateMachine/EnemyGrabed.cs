using System.Collections;
using UnityEngine;

public class EnemyGrabed : EnemyStateMachine
{
    public EnemyGrabed(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemyController.state = EnemyController.State.Grabed;


        // stop movement immediately
        enemyController.isStopping = true;
        enemyController.stopTimer = 0f;
        enemyController.patrolTimer = 0f;
        enemyController.rb.linearVelocity = Vector2.zero;
        enemyController.rb.angularVelocity = 0f;

        if (enemyController.animator != null)
        {
            enemyController.animator.Play("Grabed");
        }
    }
    public override void Update()
    {

    }
   
    public override void Exit()
    {

    }
    public override void FixedUpdate()
    {
        if (enemyController.rb != null)
        {
            enemyController.rb.linearVelocity = Vector2.zero;
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        enemyController.isStopping = false;
        enemyController.stopTimer = 0f;
        enemyController.patrolTimer = 0f;

        // optional: reset animator to idle to avoid staying on Grabed frame
        if (enemyController.animator != null)
        {
            enemyController.SwitchToRunState(enemyController.enemyIdle);
        }
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
