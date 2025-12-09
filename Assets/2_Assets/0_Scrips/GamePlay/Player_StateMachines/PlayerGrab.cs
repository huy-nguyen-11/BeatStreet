using UnityEngine;
using System.Collections;

public class PlayerGrab : PlayerStateManager
{
    private EnemyChar grabbedEnemy;
    private EnemyController grabbedEnemyController;
    private float grabDuration = 10f;
    private float grabCooldown = 3f;
    private float grabTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isGrabActive = false;
    private Vector3 grabOffset = new Vector3(0.1f, 0.1f, 0.1f); // offset to position enemy relative to player


    public PlayerGrab(PlayerController player) : base(player) { }

    public override void Enter()
    {
        playerController.state = PlayerController.State.Grab;
        playerController.rb.linearVelocity = Vector2.zero;

        // Detect nearest enemy
        var (enemy, distance) = playerController.GetNearestEnemy();

        if (enemy == null)
        {
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Check if enemy is in front of player
        Vector2 playerPos = playerController.transform.position;
        Vector2 enemyPos = enemy.transform.position;

        float distX = Mathf.Abs(enemyPos.x - playerPos.x);
        float distY = Mathf.Abs(enemyPos.y - playerPos.y);

        // Check distance constraints: X < 0.75, Y < 0.5
        if (distX > 0.75f || distY > 0.5f)
        {
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Check if enemy is in front (facing direction)
        bool enemyInFront = playerController.isFacingRight
            ? enemyPos.x > playerPos.x
            : enemyPos.x < playerPos.x;

        if (!enemyInFront)
        {
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Check enemy state - should not be in certain states
        if (enemy.enemyController.state == EnemyCharacter.State.Dead ||
            enemy.enemyController.state == EnemyCharacter.State.Fall ||
            enemy.enemyController.state == EnemyCharacter.State.Hit)
        {
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Successful grab
        grabbedEnemy = enemy;
        grabbedEnemyController = enemy.enemyController != null ? enemy.enemyController : enemy.transform.GetChild(0).GetComponent<EnemyController>();

        if (grabbedEnemyController != null)
        {
            // instead of disabling the component, switch enemy to Grabed state
            grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyGrabed);

            // zero rigidbody velocity to freeze (enemy state will also keep it stopped)
            if (grabbedEnemyController.rb != null)
            {
                grabbedEnemyController.rb.linearVelocity = Vector2.zero;
                grabbedEnemyController.rb.angularVelocity = 0f;
            }
        }

        // Play grab animation
        playerController.PlayAnim("Grab", true);
        AudioBase.Instance.AudioPlayer(2); // Adjust sound ID as needed

        isGrabActive = true;
        grabTimer = 0f;
    }

    public override void Update()
    {
        if (!isGrabActive)
            return;

        grabTimer += Time.deltaTime;

        // Wait for grab duration to complete
        if (grabTimer >= grabDuration)
        {
            ReleaseGrab();
        }

        // cooldown update (optional)
        UpdateCooldown();
    }

    public override void FixedUpdate()
    {
        if (isGrabActive && grabbedEnemyController != null)
        {
            // Keep enemy root (Char) attached to player during grab so it visually follows
            if (grabbedEnemyController.Char != null && playerController.Char != null)
            {
                grabbedEnemyController.Char.position = playerController.Char.position + grabOffset;
                // match facing with player
                float yRot = playerController.isFacingRight ? 0f : 180f;
                Vector3 angles = grabbedEnemyController.Char.localEulerAngles;
                angles.y = yRot;
                grabbedEnemyController.Char.localEulerAngles = angles;
            }
        }
    }

    private void ReleaseGrab()
    {
        isGrabActive = false;

        if (grabbedEnemyController != null)
        {
            // throw enemy: switch to Fall (or Hit) state and apply velocity
            grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyFall);

            bool isThrowRight = playerController.isFacingRight;
            float throwForce = 5f;
            if (grabbedEnemyController.rb != null)
            {
                grabbedEnemyController.rb.linearVelocity = new Vector2(
                    isThrowRight ? throwForce : -throwForce,
                    2f
                );
            }
        }

        grabbedEnemy = null;
        grabbedEnemyController = null;

        // Return to idle
        playerController.SwitchToRunState(playerController.playerIdle);
    }

    public override void Exit()
    {
        // Safety check: ensure enemy resumes normal AI if grab interrupted
        if (grabbedEnemyController != null)
        {
            grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyIdle);
        }

        isGrabActive = false;
        grabTimer = 0f;
    }
    public override void OnTriggerEnter(Collider2D collision)
    {
    }

    public override void OnTriggerStay(Collider2D collision)
    {
    }

    public override void OnTriggerExit(Collider2D collision)
    {
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public bool CanGrab()
    {
        return cooldownTimer <= 0;
    }

    public void StartGrabCooldown()
    {
        cooldownTimer = grabCooldown;
    }

    public void UpdateCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }
}
