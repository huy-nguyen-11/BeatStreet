using System.Collections;
using UnityEngine;

public class EnemyGrabed : EnemyStateMachine
{
    private Collider2D[] grabbedColliders;

    public EnemyGrabed(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Mark state
        enemyController.state = EnemyController.State.Grabed;

        enemyController.skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 6;
        // Stop ongoing coroutines that may change state (DelayAttack, SetStateIdle, etc.)
        try
        {
            // stop coroutine referenced on controller
            if (enemyController.coroutine != null)
            {
                enemyController.StopCoroutine(enemyController.coroutine);
                enemyController.coroutine = null;
            }
            // stop any other coroutines running on the enemyController
            enemyController.StopAllCoroutines();
        }
        catch (System.Exception e)
        {
           
        }

        // Set grabbed flag to prevent all movement and AI logic
        enemyController.isGrabbed = true;

        // clear movement flags/timers to avoid auto transitions
        enemyController.isStopping = false;
        enemyController.stopTimer = 0f;
        enemyController.patrolTimer = 0f;
        enemyController.isAttack = false;

        // Stop rigidbody
        if (enemyController.rb != null)
        {
            enemyController.rb.linearVelocity = Vector2.zero;
            enemyController.rb.angularVelocity = 0f;
        }

        // Make colliders triggers to avoid unintended collision events
        if (enemyController.Char != null)
        {
            grabbedColliders = enemyController.Char.GetComponentsInChildren<Collider2D>(true);
            if (grabbedColliders != null)
            {
                foreach (var c in grabbedColliders)
                {
                    c.isTrigger = true;
                }
            }
        }

        // Play grab animation
        enemyController.PlayAnim("Grab", false);
    }

    public override void Update()
    {
        // Intentionally empty - enemy is completely frozen while grabbed
    }

    public override void FixedUpdate()
    {
        // Ensure physics stays stable while grabbed
        if (enemyController.rb != null)
        {
            enemyController.rb.linearVelocity = Vector2.zero;
        }
    }

    public override void Exit()
    {
        enemyController.skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 4;
        // Clear grabbed flag to resume normal AI
        enemyController.isGrabbed = false;

        // Restore colliders to normal
        if (grabbedColliders != null)
        {
            foreach (var c in grabbedColliders)
            {
                if (c != null) c.isTrigger = false;
            }
        }
        grabbedColliders = null;

        enemyController.PlayAnim("Idle", true);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // Collisions ignored while grabbed
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
