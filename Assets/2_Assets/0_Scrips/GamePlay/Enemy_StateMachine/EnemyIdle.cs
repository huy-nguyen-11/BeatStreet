using System.Collections;
using UnityEngine;

public class EnemyIdle : EnemyStateMachine
{
    public EnemyIdle(EnemyController enemy) : base(enemy) { }
    private bool _isActiveRun = false;

    Coroutine _coroutine;
    public override void Enter()
    {
        Debug.Log("[EnemyIdle] Enter");
        enemyController.PlayAnim("Idle", true);
        enemyController.state = EnemyController.State.Idle;
        enemyController.rb.linearVelocity = Vector2.zero;
        // Skip attack logic if grabbed
        if (enemyController.isGrabbed) return;
        if (enemyController.isStopping)
        {
            return;
        }
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
            if (_coroutine == null)
            {
                _coroutine = enemyController.StartCoroutine(DelayAttack());
            }
        }
    }
    public override void Update()
    {
        if (enemyController.playerController.IsDead) return;
        if (GamePlayManager.Instance.isCheckUlti) return;
        // Stop all logic if grabbed
        if (enemyController.isGrabbed)
        {
            return;
        }

        // If đang trong pha dừng, chỉ đếm thời gian và thoát
        if (enemyController.isStopping)
        {
            enemyController.TickStopTimer();
            // Khi hết dừng, rời Idle để quay lại Run
            if (!enemyController.isStopping)
            {
                enemyController.SetRun();
            }
            return;
        }

        if (!enemyController.isAttack)
        {
            if (!_isActiveRun && Vector2.Distance(enemyController.transform.position, enemyController.player.position) <= 2.5f)
            {
                enemyController.SetRandomPatrolTarget();
                Debug.Log("[EnemyIdle] Setting Run state in Update (first time):");
                enemyController.SetRun();
                _isActiveRun = true;
            }

            //if (_isActiveRun
            //   && !enemyController.playerController.IsDead
            //   && !GamePlayManager.Instance.isCheckUlti)
            //{
            //    Debug.Log("[EnemyIdle] Setting Run state in Update: {0}");
            //    enemyController.SetRun();
            //}
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
        {
            enemyController.StopCoroutine(_coroutine);
            _coroutine = null;
        }
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
