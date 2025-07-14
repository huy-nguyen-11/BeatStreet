using System.Collections;
using UnityEngine;

public class EnemyAttack : EnemyStateMachine
{
    public EnemyAttack(EnemyController enemy) : base(enemy) { }
    Coroutine coroutine;
    public override void Enter()
    {
        enemyController.state = EnemyController.State.Attack;
        enemyController.animator.Play("Attack1");
        coroutine = enemyController.StartCoroutine(CheckAnimationAndTriggerEvent());
    }
    public IEnumerator CheckAnimationAndTriggerEvent()
    {
        AnimatorStateInfo state = enemyController.animator.GetCurrentAnimatorStateInfo(0);
        while (state.normalizedTime < 1f)
        {
            state = enemyController.animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        enemyController.ResetState();
    }
    public override void Update()
    {

    }
    public void SetSwithJump()
    {
    }
    public override void Exit()
    {
        enemyController.isAttack = false;
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
