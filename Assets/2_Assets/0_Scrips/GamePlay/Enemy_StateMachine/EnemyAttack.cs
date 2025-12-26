using Spine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : EnemyStateMachine
{
    public EnemyAttack(EnemyController enemy) : base(enemy) { }
    private TrackEntry attackEntry;

    public override void Enter()
    {
        enemyController.state = EnemyController.State.Attack;

        // Play Attack animation with Spine
        attackEntry = enemyController.skeletonAnimation.AnimationState.SetAnimation(0, "Attack", false);
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
