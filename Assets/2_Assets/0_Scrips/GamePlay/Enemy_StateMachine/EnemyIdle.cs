using System.Collections;
using UnityEngine;

public class EnemyIdle : EnemyStateMachine
{
    public EnemyIdle(EnemyController enemy) : base(enemy) { }

    Coroutine _coroutine;
    Coroutine _spawnedEnemyCoroutine;

    public override void Enter()
    {
        enemyController.PlayAnim("Idle", true);
        enemyController.state = EnemyController.State.Idle;
        enemyController.rb.linearVelocity = Vector2.zero;
        
        // Skip attack logic if grabbed
        if (enemyController.isGrabbed) return;
        
        // If enemy was spawned, wait 1 second then switch to run (don't wait for player)
        if (enemyController.isSpawned)
        {
            if (_spawnedEnemyCoroutine == null)
            {
                _spawnedEnemyCoroutine = enemyController.StartCoroutine(SpawnedEnemyRunDelay());
            }
            return;
        }

        //if (enemyController.isStopping)
        //{
        //    return;
        //}
        // Allow attack preparation to proceed even when isStopping is true.
        if (enemyController.isStopping)
        {
            return;
        }

        if (!enemyController.isAttack)
        {
            if (enemyController.isActiveRun
                && !enemyController.playerController.IsDead
                && !GamePlayManager.Instance.isCheckUlti
                && !enemyController.isStopping)
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
    
    IEnumerator SpawnedEnemyRunDelay()
    {
        yield return new WaitForSeconds(1f);
        
        // After 1 second, switch to run state
        if (enemyController.state == EnemyController.State.Idle
            && !enemyController.playerController.IsDead
            && !GamePlayManager.Instance.isCheckUlti
            && !enemyController.isGrabbed)
        {
            enemyController.isActiveRun = true;
            enemyController.isSpawned = false; // Reset spawned flag
            enemyController.SetRun();
        }
        
        _spawnedEnemyCoroutine = null;
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

        // For spawned enemies, don't check for player distance - they will run after 1 second delay
        if (enemyController.isSpawned)
        {
            return;
        }


        if (enemyController.isStopping)
        {
            enemyController.TickStopTimer();

            if (!enemyController.isStopping)
            {
                enemyController.SetRun();
            }
            return;
        }

        if (!enemyController.isAttack)
        {
            if (!enemyController.isActiveRun && Vector2.Distance(enemyController.transform.position, enemyController.player.position) <= 2.5f)
            {
                enemyController.SetRandomPatrolTarget();

                enemyController.SetRun();
                enemyController.isActiveRun = true;
            }
        }
    }
    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(0.3f);
        if(enemyController.typeOfEnemy == TypeOfEnemy.Boss && enemyController.idEnemy == 0)
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
            if (Mathf.Abs(enemyController.Char.position.x - enemyController.player.position.x) <= (enemyController.rangeAttack + 0.2f)
               && Mathf.Abs(enemyController.Char.position.x - enemyController.player.position.x) >= 0.15f
               && Mathf.Abs(enemyController.Char.position.y - enemyController.player.position.y) <= 0.2f)
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
    }
    public override void Exit()
    {
        if (_coroutine != null)
        {
            enemyController.StopCoroutine(_coroutine);
            _coroutine = null;
        }
        if (_spawnedEnemyCoroutine != null)
        {
            enemyController.StopCoroutine(_spawnedEnemyCoroutine);
            _spawnedEnemyCoroutine = null;
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
