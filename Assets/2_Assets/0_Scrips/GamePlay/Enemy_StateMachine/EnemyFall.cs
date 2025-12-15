using System.Collections;
using UnityEngine;

public class EnemyFall : EnemyStateMachine
{
    public EnemyFall(EnemyController enemy) : base(enemy) { }
    Coroutine coroutine;
    public override void Enter()
    {
        enemyController.PlayAnim("Dead", false);
        enemyController.state = EnemyController.State.Fall;
        if (coroutine != null)
            enemyController.StopCoroutine(coroutine);
        coroutine = enemyController.StartCoroutine(JumpCoroutine());
        enemyController.currentHitIndex = 0;
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {
        SetJump();
    }
    private void SetJump()
    {
        bool Direction = enemyController.player.position.x > enemyController.Char.position.x ? false : true;
        if (!Direction)
            enemyController.rb.linearVelocity = -Vector2.right * 5f;
        else
            enemyController.rb.linearVelocity = Vector2.right * 5f;
    }
    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        while (enemyController.transform.localPosition.y > 0)
        {
            yield return null;
        }
        enemyController.SwitchToRunState(enemyController.enemyIdle);
    }
    public override void Exit()
    {
        enemyController.transform.position = new Vector3(enemyController.transform.position.x, enemyController.Char.transform.GetChild(3).position.y);
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
