using System.Collections;
using UnityEngine;

public class EnemyIdle : EnemyStateMachine
{
    public EnemyIdle(EnemyController enemy) : base(enemy) { }
    private bool _isActiveRun = false;
    private bool _isCheck = false;
    Coroutine _coroutine;
    public override void Enter()
    {
        //enemyController.PlayAnim("Idle", true);
        enemyController.state = EnemyController.State.Idle;
        enemyController.rb.linearVelocity = Vector2.zero;
        // Skip attack logic if grabbed
        if (enemyController.isGrabbed) return;
        
        if (!enemyController.isAttack)
        {
            if (_isActiveRun
                && !enemyController.playerController.IsDead
                && !GamePlayManager.Instance.isCheckUlti)
            {
                enemyController.SetRun();
            }
        }
        else
        {
            _coroutine = enemyController.StartCoroutine(DelayAttack());
        }
    }
    public override void Update()
    {
        if (enemyController.playerController.IsDead) return;
        if (GamePlayManager.Instance.isCheckUlti) return;
        // Stop all logic if grabbed
        if (enemyController.isGrabbed)
        {
            Debug.LogWarning($"[EnemyIdle.Update] ?? Attempted Update while isGrabbed=true. Returning early.");
            return;
        }

        if (!enemyController.isAttack)
        {
            if (!_isActiveRun && Vector2.Distance(enemyController.transform.position, enemyController.player.position) <= 2.5f)
            {
                enemyController.SetRandomPatrolTarget();
                enemyController.SetRun();
                _isActiveRun = true;
            }
            if (_isActiveRun
               && !enemyController.playerController.IsDead
               && !GamePlayManager.Instance.isCheckUlti)
            {
                enemyController.SetRun(); 
            }
        }
    }
    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(0.5f);
        if (Mathf.Abs(enemyController.Char.position.x - enemyController.player.position.x) <= 1f
           && Mathf.Abs(enemyController.Char.position.x - enemyController.player.position.x) >= 0.15f
           && Mathf.Abs(enemyController.Char.position.y - enemyController.player.position.y) <= 0.3f)
        {
            int idEnemy = enemyController.Char.GetComponent<EnemyChar>().idEnemy;

            if (enemyController.Char.position.x <= enemyController.player.position.x)
            {
                GamePlayManager.Instance.isEnemyOnLeft[idEnemy] = true;
            }
            else
            {
                GamePlayManager.Instance.isEnemyOnRight[idEnemy] = true;
            }

            enemyController.SwitchToRunState(enemyController.enemyAttack);
        }
        else
        {
            enemyController.SwitchToRunState(enemyController.enemyRun);
        }
    }
    public override void Exit()
    {
        if (_coroutine != null)
            enemyController.StopCoroutine(_coroutine);
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
