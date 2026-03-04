using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyFall : EnemyStateMachine
{
    public EnemyFall(EnemyController enemy) : base(enemy) { }
    Coroutine coroutine;
    private bool isWakingUp = false; // Flag to prevent SetJump() during wakeup sequence
    bool _direction;
    private string fallAnim;
    float numFall;

    public override void Enter()
    {
        //fallAnim = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? "Death" : "Dead";
        numFall = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 1.2f : 2.5f;
        enemyController.velocity = 8f;
        //enemyController.PlayAnim(fallAnim, false);
        if(enemyController.typeOfEnemy == TypeOfEnemy.Boss)
            enemyController.PlayAnim("Death", false);
        else
            enemyController.PlayAnimFall("Dead_2", "Dead");
        enemyController.state = EnemyController.State.Fall;
        if (enemyController.isGetHitStrengthMax)
        {
            _direction = enemyController.Char.position.x < enemyController.player.position.x ? false : true;
        }
        else
        {
            _direction = enemyController.playerController.isFacingRight ? true : false;
        }

        isWakingUp = false; // Reset flag
        if (coroutine != null)
            enemyController.StopCoroutine(coroutine);
        coroutine = enemyController.StartCoroutine(WakeUpCoroutine());
        enemyController.currentHitIndex = 0;

    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {
        //// Only set jump velocity if not waking up (to prevent movement during wakeup)
        if (!isWakingUp)
        {
            // fallback: move horizontally using old approach when no Rigidbody present
            SetFall();
        }
        enemyController.ProcessGravity();
    }

    private void SetFall()
    {
        if (!_direction)
            enemyController.rb.linearVelocity = -Vector2.right * numFall;
        else
            enemyController.rb.linearVelocity = Vector2.right * numFall;
    }


    private IEnumerator WakeUpCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (enemyController.rb.linearVelocity.y > 0)
        {
            yield return null;
        }
        // Stop SetJump() from running during wakeup
        isWakingUp = true;

        // Clear velocity before wakeup to prevent drifting
        if (enemyController.rb != null)
        {
            enemyController.rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.5f);
        enemyController.PlayAnim(enemyController.wakeUpAnim, false);
        yield return new WaitForSeconds(1.135f);
        enemyController.SwitchToRunState(enemyController.enemyIdle);

    }
    public override void Exit()
    {
        //// Stop the coroutine to prevent state switching
        if (coroutine != null)
        {
            enemyController.StopCoroutine(coroutine);
            coroutine = null;
        }

        // Clear velocity to stop any movement from SetJump()
        if (enemyController.rb != null)
        {
            enemyController.rb.linearVelocity = Vector2.zero;
        }

        // Reset position
        enemyController.transform.position = new Vector3(enemyController.transform.position.x, enemyController.Char.transform.GetChild(3).position.y);

        // Reset move animation to ensure new animation is set when transitioning to Idle/Run
        enemyController.ResetMoveAnimation();

        // Ensure isGrabbed is cleared (in case it wasn't cleared properly)
        enemyController.isGrabbed = false;
        enemyController.isGetHitStrengthMax = false;
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
