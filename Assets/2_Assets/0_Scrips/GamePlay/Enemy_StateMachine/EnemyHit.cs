using System.Collections;
using UnityEngine;

public class EnemyHit : EnemyStateMachine
{
    public EnemyHit(EnemyController enemy) : base(enemy) { }
    Coroutine coroutine;
    private string hitAnim;
    public override void Enter()
    {
        enemyController.state = EnemyController.State.Hit;
        AudioBase.Instance.AudioPlayerAtkHit();
        hitAnim = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? "Damaged" : "Hit";
        SetAnimHit();
    }
    public override void Update()
    {

    }
    public void SetAnimHit()
    {
        if(enemyController.typeOfEnemy == TypeOfEnemy.Boss)
        {
            SetHitbox();
            enemyController.PlayAnim(hitAnim, false);
            if (coroutine != null)
                enemyController.StopCoroutine(coroutine);
            coroutine = enemyController.StartCoroutine(SetStateIdle());
        }
        else
        {
            if (enemyController.currentHitIndex >= 0 && enemyController.currentHitIndex <= 4)
            {
                SetHitbox();
                enemyController.PlayAnim(hitAnim, false);
                if (coroutine != null)
                    enemyController.StopCoroutine(coroutine);
                coroutine = enemyController.StartCoroutine(SetStateIdle());
            }
            else
            {
                if (coroutine != null)
                    enemyController.StopCoroutine(coroutine);
                if (enemyController.state != EnemyController.State.Fall)
                    enemyController.SwitchToRunState(enemyController.enemyFall);
            }
        }
    }
    private void SetHitbox()
    {
        float hit = GamePlayManager.Instance._Player.Char.position.x
            <= enemyController.Char.position.x ? -0.15f : 0.15f;
        int num = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 10 : 1;
        enemyController.rb.linearVelocity = Vector2.left * hit * num;
    }
    IEnumerator SetStateIdle()
    {
        float num = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 1.2f : 0.5f;
        yield return new WaitForSeconds(num);
        enemyController.SwitchToRunState(enemyController.enemyIdle);
    }
    public override void Exit()
    {
        if (coroutine != null)
        {
            enemyController.StopCoroutine(coroutine);
        }
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