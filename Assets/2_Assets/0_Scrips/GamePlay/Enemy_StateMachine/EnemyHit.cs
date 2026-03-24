using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHit : EnemyStateMachine
{
    public EnemyHit(EnemyController enemy) : base(enemy) { }
    Coroutine coroutine;
    private string hitAnim;
    // Add this field at class level
    private Coroutine knockbackStopCoroutine;
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
            //if (enemyController.currentHitIndex >= 0 && enemyController.currentHitIndex <= 4)
            //{
            //    SetHitbox();
            //    enemyController.PlayAnim(hitAnim, false);
            //    if (coroutine != null)
            //        enemyController.StopCoroutine(coroutine);
            //    coroutine = enemyController.StartCoroutine(SetStateIdle());
            //}
            //else
            //{
            //    if (coroutine != null)
            //        enemyController.StopCoroutine(coroutine);
            //    if (enemyController.state != EnemyController.State.Fall)
            //        enemyController.SwitchToRunState(enemyController.enemyFall);
            //}
            SetHitbox();
            enemyController.PlayAnim(hitAnim, false);
            if (coroutine != null)
                enemyController.StopCoroutine(coroutine);
            coroutine = enemyController.StartCoroutine(SetStateIdle());
        }
    }
    private void SetHitbox()
    {
        float hit = GamePlayManager.Instance._Player.Char.position.x
            <= enemyController.Char.position.x ? -0.15f : 0.15f;
        int num = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 10 : 1;
        enemyController.rb.linearVelocity = Vector2.left * hit * num;

        // Stop any previous knockback stopper and start a short one so enemy doesn't slide too long
        if (knockbackStopCoroutine != null)
            enemyController.StopCoroutine(knockbackStopCoroutine);

        // Tune duration to taste (0.08 - 0.18). Shorter = snappier.
        knockbackStopCoroutine = enemyController.StartCoroutine(StopKnockbackCoroutine(0.12f));
    }

    private IEnumerator StopKnockbackCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (enemyController != null && enemyController.state == EnemyController.State.Hit && enemyController.rb != null)
        {
            Vector2 v = enemyController.rb.linearVelocity;
            enemyController.rb.linearVelocity = new Vector2(0f, v.y);
        }

        knockbackStopCoroutine = null;
    }

    IEnumerator SetStateIdle()
    {
        float num = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 1.2f : 0.8f;
        yield return new WaitForSeconds(num);
        enemyController.SwitchToRunState(enemyController.enemyIdle);
    }
    public override void Exit()
    {
        if (coroutine != null)
        {
            enemyController.StopCoroutine(coroutine);
        }

        if (knockbackStopCoroutine != null)
        {
            try { enemyController.StopCoroutine(knockbackStopCoroutine); } catch { }
            knockbackStopCoroutine = null;
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