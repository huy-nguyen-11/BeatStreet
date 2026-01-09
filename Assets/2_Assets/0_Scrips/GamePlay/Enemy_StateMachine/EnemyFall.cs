using System.Collections;
using UnityEngine;

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
        fallAnim = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? "Death" : "Dead";
        numFall = enemyController.typeOfEnemy == TypeOfEnemy.Boss ? 1.5f : 5f;
        enemyController.PlayAnim(fallAnim, false);
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
        // Only set jump velocity if not waking up (to prevent movement during wakeup)
        if (!isWakingUp)
        {
            SetFall();
        }
    }
    private void SetFall()
    {
        //bool Direction = enemyController.player.position.x > enemyController.Char.position.x ? false : true;
        //bool Direction = enemyController.playerController.isFacingRight ? true : false;
        if (!_direction)
            enemyController.rb.linearVelocity = -Vector2.right * numFall;
        else
            enemyController.rb.linearVelocity = Vector2.right * numFall;
    }
    private IEnumerator WakeUpCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        while (enemyController.transform.localPosition.y > 0)
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
        
        enemyController.PlayAnim(enemyController.wakeUpAnim, false);
        yield return new WaitForSeconds(0.2f);
        enemyController.SwitchToRunState(enemyController.enemyIdle);
    }
    public override void Exit()
    {
        // Stop the coroutine to prevent state switching
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
