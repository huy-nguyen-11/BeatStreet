using Spine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : EnemyStateMachine
{
    public EnemyAttack(EnemyController enemy) : base(enemy) { }
    private TrackEntry attackEntry;
    private string attackAnimationName;

    public override void Enter()
    {
        Debug.Log("Enter Attack State"+ Time.time);
        enemyController.state = EnemyController.State.Attack;
        if(enemyController.typeOfEnemy == TypeOfEnemy.Boss && enemyController.idEnemy == 1)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.5f)
            {
                attackAnimationName = "Attack1";
            }
            else
            {
                attackAnimationName = "Attack2";
            }
        }
        else
        {
            attackAnimationName = "Attack";
        }
        // Play Attack animation with Spine
        attackEntry = enemyController.skeletonAnimation.AnimationState.SetAnimation(0, attackAnimationName, false);
        attackEntry.Complete += OnAttackComplete;
    }

    private void OnAttackComplete(TrackEntry trackEntry)
    {
        attackEntry.Complete -= OnAttackComplete;

        enemyController.ResetState();
    }


    public override void Update()
    {
    }

    public override void Exit()
    {
        enemyController.isAttack = false;

        if (attackEntry != null)
            attackEntry.Complete -= OnAttackComplete;
    }

    public override void FixedUpdate() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnTriggerEnter(Collider2D collision) { }
    public override void OnTriggerExit(Collider2D collision) { }
    public override void OnTriggerStay(Collider2D collision) { }
}
