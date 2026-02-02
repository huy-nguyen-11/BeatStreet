using Spine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : EnemyStateMachine
{
    //public EnemyAttack(EnemyController enemy) : base(enemy) { }
    //private TrackEntry attackEntry;
    //private string attackAnimationName;

    //public override void Enter()
    //{
    //    Debug.Log("Enter Attack State"+ Time.time);
    //    enemyController.state = EnemyController.State.Attack;
    //    // Mark active attack so other transitions are blocked
    //    enemyController.isAttacking = true;
    //    if (enemyController.typeOfEnemy == TypeOfEnemy.Boss && enemyController.idEnemy == 1)
    //    {
    //        float randomValue = Random.Range(0f, 1f);
    //        if (randomValue <= 0.5f)
    //        {
    //            attackAnimationName = "Attack1";
    //        }
    //        else
    //        {
    //            attackAnimationName = "Attack2";
    //        }
    //    }
    //    else
    //    {
    //        attackAnimationName = "Attack";
    //    }
    //    // Play Attack animation with Spine
    //    attackEntry = enemyController.skeletonAnimation.AnimationState.SetAnimation(0, attackAnimationName, false);
    //    attackEntry.Complete += OnAttackComplete;
    //}

    //private void OnAttackComplete(TrackEntry trackEntry)
    //{
    //    attackEntry.Complete -= OnAttackComplete;
    //    enemyController.isAttacking = false;
    //    enemyController.ResetState();
    //}


    //public override void Update()
    //{
    //}

    //public override void Exit()
    //{
    //    // mark not attacking anymore
    //    enemyController.isAttacking = false;

    //    enemyController.isAttack = false;

    //    if (attackEntry != null)
    //        attackEntry.Complete -= OnAttackComplete;
    //}

    //public override void FixedUpdate() { }
    //public override void OnCollisionEnter2D(Collision2D collision) { }
    //public override void OnTriggerEnter(Collider2D collision) { }
    //public override void OnTriggerExit(Collider2D collision) { }
    //public override void OnTriggerStay(Collider2D collision) { }
    public EnemyAttack(EnemyController enemy) : base(enemy) { }
    private TrackEntry attackEntry;
    private string attackAnimationName;

    // tuning
    private float preAttackDelay = 1f;     // idle before first attack
    private float interAttackDelay = 0.12f;  // delay between successive attacks

    private Coroutine _attackRoutine;

    public string nameBossAttack;
    public bool isEliteEnemyAttack;

    public override void Enter()
    {
        // mark state and block other transitions
        enemyController.state = EnemyController.State.Attack;
        enemyController.isAttacking = true;

        // ensure we don't move while preparing to attack
        // keep isAttack true so Movement early-return applies
        // reset movement timers so Idle/Run don't interfere
        enemyController.isStopping = false;
        enemyController.stopTimer = 0f;
        enemyController.patrolTimer = 0f;

        // choose animation
        if (enemyController.typeOfEnemy == TypeOfEnemy.Boss)
        {
            float randomValue = Random.Range(0f, 1f);
            //attackAnimationName = (randomValue <= 0.5f) ? "Attack1" : "Attack2";
            attackAnimationName = nameBossAttack;
        }
        else if( enemyController.typeOfEnemy == TypeOfEnemy.EliteEnemy && isEliteEnemyAttack)
        {
            attackAnimationName = "Attack_2";
        }
        else
        {
            attackAnimationName = "Attack";
        }

        // start attack sequence coroutine (pre-idle + 1..2 attacks)
        if (_attackRoutine != null)
            enemyController.StopCoroutine(_attackRoutine);
        _attackRoutine = enemyController.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        // short idle before attacking
        yield return new WaitForSeconds(preAttackDelay);

        int attacks;
        if (nameBossAttack == "Attack1" || isEliteEnemyAttack)
        {
            attacks = 1;
        }
        else
        {
            attacks = (Random.value <= 0.25f) ? 2 : 1;
        }

        for (int i = 0; i < attacks; i++)
        {
            bool completed = false;

            // play attack animation and wait for completion
            attackEntry = enemyController.skeletonAnimation.AnimationState.SetAnimation(0, attackAnimationName, false);
            bool isFacingRight = enemyController.Char.rotation.y < 0f;
            if (enemyController.typeOfEnemy != TypeOfEnemy.Boss && enemyController.typeOfEnemy != TypeOfEnemy.EliteEnemy)
            {
                enemyController.rb.linearVelocity = Vector2.right * (isFacingRight ? -0.2f : 0.2f);
            }

            Spine.AnimationState.TrackEntryDelegate handler = null;
            handler = (TrackEntry te) =>
            {
                completed = true;
                if (attackEntry != null)
                {
                    try { attackEntry.Complete -= handler; } catch { }
                    attackEntry = null;
                }
            };

            if (attackEntry != null)
                attackEntry.Complete += handler;
            else
            {
                // fallback: if SetAnimation failed, avoid infinite wait
                completed = true;
            }

            // wait until animation complete
            yield return new WaitUntil(() => completed);

            // small pause between attacks
            if (i < attacks - 1)
                yield return new WaitForSeconds(interAttackDelay);
        }

        _attackRoutine = null;

        // finished attacking: allow transitions and go to Idle to re-evaluate player
        enemyController.isAttacking = false;
        enemyController.isAttack = false;
        isEliteEnemyAttack = false;
        //if(enemyController.isEnableThrower)
        //{
        //    enemyController.isEnableThrower = false;
        //}
        // after attack, return to Idle so DelayAttack can handle timing before next attack
        enemyController.SwitchToRunState(enemyController.enemyIdle);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        // stop coroutine if still running
        if (_attackRoutine != null)
        {
            try { enemyController.StopCoroutine(_attackRoutine); } catch { }
            _attackRoutine = null;
        }

        // cleanup track entry handler if any
        if (attackEntry != null)
        {
            try { attackEntry.Complete -= OnAttackCompleteFallback; } catch { }
            attackEntry = null;
        }

        enemyController.isAttacking = false;
        enemyController.isAttack = false;
    }

    // small fallback handler used only for safe removal (no logic here)
    private void OnAttackCompleteFallback(TrackEntry trackEntry)
    {
    }

    public override void FixedUpdate() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnTriggerEnter(Collider2D collision) { }
    public override void OnTriggerExit(Collider2D collision) { }
    public override void OnTriggerStay(Collider2D collision) { }
}
