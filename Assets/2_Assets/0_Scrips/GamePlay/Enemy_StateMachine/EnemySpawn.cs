using Spine;
using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemySpawn : EnemyStateMachine
{
    private bool hasJumped = false;
    private TrackEntry spawnAnimEntry;

    public EnemySpawn(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemyController.state = EnemyController.State.Spawn;
        hasJumped = false;

        // Play spawn animation once and detect when it completes
        if (enemyController.skeletonAnimation != null)
        {
            // Ensure skeleton is active & initialized when object is re-enabled
            if (!enemyController.skeletonAnimation.isActiveAndEnabled)
            {
                enemyController.skeletonAnimation.gameObject.SetActive(true);
            }
            enemyController.skeletonAnimation.Initialize(false);

            if (enemyController.skeletonAnimation.AnimationState != null)
            {
                Debug.Log("Playing Spawn animation.");
                spawnAnimEntry = enemyController.skeletonAnimation.AnimationState.SetAnimation(0, "Jump", false);
                spawnAnimEntry.Complete += OnSpawnAnimationComplete;
                return;
            }
            else
            {
                Debug.LogWarning("AnimationState is null on EnemyController skeletonAnimation.");
            }
        }

        // Fallback: try play via helper to at least trigger the anim
        Debug.LogWarning("SkeletonAnimation or AnimationState is null on EnemyController.");
        enemyController.PlayAnim("Jump", false);
    }

    private void OnSpawnAnimationComplete(TrackEntry trackEntry)
    {
        Debug.Log("Spawn animation complete event received.");
        // When spawn animation completes, switch to idle state
        if (enemyController.state == EnemyController.State.Spawn)
        {
            Debug.Log("Spawn animation complete, switching to Run state.");
            enemyController.SwitchToRunState(enemyController.enemyIdle);
        }
    }

    public override void Update()
    {
        // Animation complete event handler will switch to idle state
        // No additional logic needed here
    }

    public override void Exit()
    {
        // Unsubscribe from animation complete event
        if (spawnAnimEntry != null)
        {
            spawnAnimEntry.Complete -= OnSpawnAnimationComplete;
            spawnAnimEntry = null;
        }
    }

    public override void FixedUpdate()
    {
        // Make enemy jump up with magnitude = 2, distance = 3 towards player (only once)
        if (!hasJumped && enemyController.rb != null && enemyController.player != null)
        {
            Vector2 directionToPlayer = (enemyController.player.position - enemyController.Char.position).normalized;
            float yRotation = enemyController.player.position.x > enemyController.Char.position.x ? 0f : 180f;
            enemyController.Char.rotation = Quaternion.Euler(0f, yRotation, 0f);
            // Jump up with magnitude = 2 (vertical)
            float jumpUpMagnitude = 2f;
            // Jump forward with distance = 3 (horizontal towards player)
            float jumpDistance = 3f;

            Vector2 jumpVelocity = new Vector2(directionToPlayer.x * jumpDistance, jumpUpMagnitude);
            enemyController.rb.linearVelocity = jumpVelocity;

            hasJumped = true;
        }
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
