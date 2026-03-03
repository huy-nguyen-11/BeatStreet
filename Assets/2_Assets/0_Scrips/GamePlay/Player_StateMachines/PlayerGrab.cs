using UnityEngine;
using System.Collections;
using Spine;

public class PlayerGrab : PlayerStateManager
{
    private EnemyChar grabbedEnemy;
    private EnemyController grabbedEnemyController;
    private float grabDuration = 3f;
    private float grabTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isGrabActive = false;
    private Vector3 grabOffset = new Vector3(0.63f, 0f, 0f); // offset to position enemy relative to player
                                                            // Add fields near top of class
    private float _pendingThrowDirection = 1f;
    private Spine.AnimationState.TrackEntryEventDelegate _spineThrowHandler;
    // Add fields near top of class (already have some; insert these)
    private Spine.TrackEntry _throwTrackEntry;
    private Spine.AnimationState.TrackEntryDelegate _throwCompleteHandler;
    private bool _throwAnimCompleted = false;
    private bool _throwOccurred = false;

    // attack
    private bool _grabAttackBusy = false;
    private int _grabAttackQueue = 0;
    private Spine.TrackEntry _grabAttackEntry;
    private Spine.AnimationState.TrackEntryDelegate _grabAttackCompleteHandler;
    private int _grabAttackCount = 0; // Đếm số lần tấn công Grab_Attack
    private int _maxGrabAttacks = 0; // Số lần tấn công tối đa (random 3-6)

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
            enemy.enemyController.state == EnemyCharacter.State.Hit ||
            enemy.enemyController.state == EnemyCharacter.State.Grabed)
        {
            playerController.SwitchToRunState(playerController.playerIdle);
            return;
        }

        // Successful grab - switch enemy to Grabed state immediately
        grabbedEnemy = enemy;
        grabbedEnemyController = enemy.enemyController != null 
            ? enemy.enemyController 
            : enemy.transform.GetChild(0).GetComponent<EnemyController>();


        if (grabbedEnemyController != null)
        {
            // Ensure grabbed flag set immediately to avoid race
            grabbedEnemyController.isGrabbed = true;

            // Force enter grabbed state atomically to avoid races
            grabbedEnemyController.ForceEnterGrabbed();

            // Ensure grabbed enemy faces the player (so it never shows its back to player)
            try
            {
                Transform enemyCharT = grabbedEnemyController.Char != null ? grabbedEnemyController.Char : grabbedEnemyController.transform;
                float yRot = playerController.transform.position.x > enemyCharT.position.x ? 0f : 180f;
                Vector3 eAngles = enemyCharT.localEulerAngles;
                eAngles.y = yRot;
                enemyCharT.localEulerAngles = eAngles;
                // keep root transform consistent
                grabbedEnemyController.Char.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
            }
            catch { }
        }

        // Play grab animation
        playerController.PlayAnim("Grab", true);
        AudioBase.Instance.AudioPlayer(2);

        _grabAttackCount = 0;
        //_maxGrabAttacks = Random.Range(2, 4);
        _maxGrabAttacks = 2;

        isGrabActive = true;
        grabTimer = 0f;
    }

    public override void Update()
    {
        if (!isGrabActive)
            return;

        // If grabbed enemy died or controller lost, cancel grab immediately
        if (grabbedEnemyController == null || grabbedEnemyController.state == EnemyCharacter.State.Dead || playerController.state == PlayerController.State.Dead)
        {
            CancelGrabAndIdle();
            return;
        }

        grabTimer += Time.deltaTime;

        // Wait for grab duration to complete
        if (grabTimer >= grabDuration)
        {
          
            //ReleaseGrab();
            CancelGrab();
        }

        // cooldown update (optional)
        //UpdateCooldown();
    }

    public override void FixedUpdate()
    {
        if (isGrabActive && grabbedEnemyController != null)
        {
            // Nếu enemy đã chết, không còn ép vị trí/rotation nữa để giữ hiệu ứng chết (ném, xoay Z, v.v.)
            if (grabbedEnemyController.state == EnemyCharacter.State.Dead)
            {
                return;
            }
            //// Keep enemy root (Char) attached to player during grab so it visually follows
            //if (grabbedEnemyController.Char != null && playerController.Char != null)
            //{
            //    float dirOffset = playerController.isFacingRight ? 1f : -1f;
            //    Vector3 offset = new Vector3(dirOffset * grabOffset.x, grabOffset.y, grabOffset.z);
            //    grabbedEnemyController.Char.position = playerController.Char.position + offset;

            //    // match facing with player
            //    float currentY = grabbedEnemyController.transform.localEulerAngles.y;
            //    bool enemyFacingRight = (currentY == 0f);
            //    if(enemyFacingRight == playerController.isFacingRight)
            //    {
            //        float yRot = playerController.isFacingRight ? 180f : 0f;
            //        Vector3 angles = grabbedEnemyController.transform.localEulerAngles;
            //        angles.y = yRot;
            //        grabbedEnemyController.transform.localEulerAngles = angles;
            //    }
            //}
            // Keep enemy root (Char) attached to player during grab so it visually follows
            if (grabbedEnemyController.Char != null && playerController.Char != null)
            {
                float dirOffset = playerController.isFacingRight ? 1f : -1f;
                Vector3 offset = new Vector3(dirOffset * grabOffset.x, grabOffset.y, grabOffset.z);
                grabbedEnemyController.Char.position = playerController.Char.position + offset;

                // Ensure enemy always faces the player (use world positions so "behind" grab is handled)
                float yRot = playerController.Char.position.x > grabbedEnemyController.Char.position.x ? 0f : 180f;
                Vector3 angles = grabbedEnemyController.Char.localEulerAngles;
                angles.y = yRot;
                grabbedEnemyController.Char.localEulerAngles = angles;
                // keep root transform consistent as well
                grabbedEnemyController.Char.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
            }
        }
    }

    //private void ReleaseGrab()
    //{
    //    isGrabActive = false;

    //    if (grabbedEnemyController != null)
    //    {
    //        // Switch to Fall state and apply throw velocity
    //        grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyFall);

    //        // Apply throw force
    //        bool isThrowRight = playerController.isFacingRight;
    //        float throwForce = 5f;
    //        if (grabbedEnemyController.rb != null)
    //        {
    //            grabbedEnemyController.rb.linearVelocity = new Vector2(
    //                isThrowRight ? throwForce : -throwForce,
    //                2f
    //            );
    //        }
    //    }

    //    grabbedEnemy = null;
    //    grabbedEnemyController = null;

    //    // Return to idle
    //    playerController.SwitchToRunState(playerController.playerIdle);
    //}

    // Call this from PlayerController when detecting swipe while grabbing
    public void StartThrow(float direction)
    {
        if (!isGrabActive || grabbedEnemyController == null) return;

        // if enemy died between tap and throw, cancel
        if (grabbedEnemyController.state == EnemyCharacter.State.Dead)
        {
            CancelGrabAndIdle();
            return;
        }
        _pendingThrowDirection = Mathf.Sign(direction);
        _throwAnimCompleted = false;
        _throwOccurred = false;

        // Ensure player faces swipe direction immediately
        bool faceRight = _pendingThrowDirection > 0f;
        playerController.SetFacingDirection(faceRight);

        // Also orient grabbed enemy visually to match throw direction
        if (grabbedEnemyController != null && grabbedEnemyController.Char != null)
        {
            float enemyY = faceRight ? 0f : 180f;
            Vector3 eAngles = grabbedEnemyController.Char.localEulerAngles;
            eAngles.y = enemyY;
            grabbedEnemyController.Char.localEulerAngles = eAngles;
            // Also set root rotation to keep consistency
            grabbedEnemyController.Char.transform.rotation = Quaternion.Euler(0f, enemyY, 0f);
        }

        // unsubscribe existing (safety)
        UnsubscribeThrowEvent();

        // create event handler for the Spine 'Throw' event (moment when enemy should be pushed)
        _spineThrowHandler = (trackEntry, e) =>
        {
            if (e == null || e.Data == null) return;
            if (string.Equals(e.Data.Name, "Throw", System.StringComparison.OrdinalIgnoreCase))
            {
                // perform actual enemy push on the event
                PerformThrowImmediate(_pendingThrowDirection);
                _throwOccurred = true;

                // unsubscribe after firing once
                UnsubscribeThrowEvent();
            }
        };

        // subscribe to Spine event
        if (playerController != null && playerController.skeletonAnimation != null)
        {
            playerController.skeletonAnimation.AnimationState.Event += _spineThrowHandler;

            // play animation and capture TrackEntry to know when animation completes
            _throwTrackEntry = playerController.skeletonAnimation.AnimationState.SetAnimation(0, "Grab_L", false);
            if (_throwTrackEntry != null)
            {
                // store handler so we can unsubscribe later
                _throwCompleteHandler = (te) =>
                {
                    _throwAnimCompleted = true;
                    OnThrowAnimComplete();
                };
                _throwTrackEntry.Complete += _throwCompleteHandler;
            }
            else
            {
                // fallback: just play via PlayAnim if SetAnimation not available
                playerController.PlayAnim("Grab_L", false);
            }
        }
        else
        {
            // fallback if skeleton not available
            playerController.PlayAnim("Grab_L", false);
        }
    }

    // Perform the physics push and release the grabbed enemy (do NOT change player state here)
    private void PerformThrowImmediate(float direction)
    {
        if (!isGrabActive || grabbedEnemyController == null) return;

        if (grabbedEnemyController.state == EnemyCharacter.State.Dead)
        {

            CancelGrabAndIdle();
            return;
        }

        // Switch enemy to Fall state
        grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyFall);

        // Clear grabbed references (player remains in this state until animation completes)
        isGrabActive = false;
        grabbedEnemy = null;
        grabbedEnemyController = null;
    }

    // Called when throw animation finishes
    private void OnThrowAnimComplete()
    {
        // ensure event unsubscribed
        UnsubscribeThrowEvent();

        // if animation finished but throw event never fired, do the throw now
        if (!_throwOccurred)
        {
            PerformThrowImmediate(_pendingThrowDirection);
            _throwOccurred = true;
        }

        // cleanup track entry handlers
        if (_throwTrackEntry != null && _throwCompleteHandler != null)
        {
            _throwTrackEntry.Complete -= _throwCompleteHandler;
            _throwCompleteHandler = null;
            _throwTrackEntry = null;
        }

        // Now it's safe to return player to idle
        if (playerController != null)
            playerController.SwitchToRunState(playerController.playerIdle);
    }


    // Preserve public method but keep it as a simple wrapper if you ever call directly
    public void ThrowEnemy(float direction)
    {
        // call perform immediate and rely on animation completion to switch player to idle
        PerformThrowImmediate(direction);
    }

    // Unsubscribe helper (call in Exit too)
    private void UnsubscribeThrowEvent()
    {
        if (_spineThrowHandler != null && playerController != null && playerController.skeletonAnimation != null)
        {
            playerController.skeletonAnimation.AnimationState.Event -= _spineThrowHandler;
            _spineThrowHandler = null;
        }
    }


    /// <summary>
    ///  attack
    /// </summary>
    public void QueueGrabAttack()
    {
        if (!isGrabActive) return;

        if (!_grabAttackBusy)
            PlayGrabAttack();
        else
            _grabAttackQueue++;
    }

    private void PlayGrabAttack()
    {
        if (!isGrabActive) return;
        if (playerController == null) return;

        // if enemy died, cancel grab instead of attacking
        if (grabbedEnemyController == null || grabbedEnemyController.state == EnemyCharacter.State.Dead)
        {
            CancelGrabAndIdle();
            return;
        }

        _grabAttackBusy = true;

        // Play on Spine track 1 so we don't override main grab pose on track 0
        if (playerController.skeletonAnimation != null && playerController.skeletonAnimation.AnimationState != null)
        {
            _grabAttackEntry = playerController.skeletonAnimation.AnimationState.SetAnimation(0, "Grab_Attack", false);
            playerController.idAttackArea = 6;// set id attack area == 2
            if (_grabAttackEntry != null)
            {
                _grabAttackCompleteHandler = (entry) =>
                {
                    OnGrabAttackComplete(entry);
                };
                _grabAttackEntry.Complete += _grabAttackCompleteHandler;
                return;
            }
        }

        // Fallback: if Spine not available or SetAnimation returned null, use PlayAnim and fallback timing
        playerController.PlayAnim("Grab_Attack", false);
        float fallbackDuration = 0.33f;
        // try to read animation duration if possible
        try
        {
            var anim = playerController.skeletonAnimation?.Skeleton?.Data?.FindAnimation("Grab_Attack");
            if (anim != null) fallbackDuration = anim.Duration;
        }
        catch { }
        playerController.StartCoroutine(GrabAttackFallback(fallbackDuration));
    }

    private IEnumerator GrabAttackFallback(float wait)
    {
        yield return new WaitForSeconds(wait);
        OnGrabAttackComplete(null);
    }

    private void OnGrabAttackComplete(Spine.TrackEntry entry)
    {
        // detach handler
        try
        {
            if (_grabAttackEntry != null && _grabAttackCompleteHandler != null)
                _grabAttackEntry.Complete -= _grabAttackCompleteHandler;
        }
        catch { }

        _grabAttackEntry = null;
        _grabAttackCompleteHandler = null;
        _grabAttackBusy = false;

        // Tăng biến đếm sau mỗi lần tấn công
        _grabAttackCount++;

        // Kiểm tra nếu số lần tấn công vượt quá ngưỡng, tự động ném enemy
        if (_grabAttackCount > _maxGrabAttacks && isGrabActive && grabbedEnemyController != null)
        {
            float throwDirection = playerController.isFacingRight ? 1f : -1f;
            playerController.isThrowEnemy = true;
            StartThrow(throwDirection);
            return;
        }

        // if queued taps exist, play next
        if (_grabAttackQueue > 0)
        {
            _grabAttackQueue--;
            PlayGrabAttack();
        }
    }

    private void CancelGrabAndIdle()
    {
        // Unsubscribe any pending throw/attack handlers
        UnsubscribeThrowEvent();

        // cleanup grab-attack handlers/queue
        try
        {
            if (_grabAttackEntry != null && _grabAttackCompleteHandler != null)
                _grabAttackEntry.Complete -= _grabAttackCompleteHandler;
        }
        catch { }
        _grabAttackEntry = null;
        _grabAttackCompleteHandler = null;
        _grabAttackBusy = false;
        _grabAttackQueue = 0;
        _grabAttackCount = 0; // Reset biến đếm
        _maxGrabAttacks = 0; // Reset ngưỡng

        // If enemy controller still exists, clear its grabbed flag and set idle
        if (grabbedEnemyController != null)
        {
            try
            {
                // Nếu enemy đã chết thì không ép chuyển state/rotation nữa,
                // để EnemyDead/SetDead tự xử lý (bao gồm cả xoay Z khi bị ném).
                if (grabbedEnemyController.state != EnemyCharacter.State.Dead)
                {
                    grabbedEnemyController.isGrabbed = false;
                    grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyIdle);
                }
            }
            catch { }
        }

        // clear references and timers
        grabbedEnemy = null;
        grabbedEnemyController = null;
        isGrabActive = false;
        grabTimer = 0f;

        // return player to idle safely
        if (playerController != null)
            playerController.SwitchToRunState(playerController.playerIdle);
    }


    public override void Exit()
    {
        try
        {
            if (_grabAttackEntry != null && _grabAttackCompleteHandler != null)
                _grabAttackEntry.Complete -= _grabAttackCompleteHandler;
        }
        catch { }
        _grabAttackEntry = null;
        _grabAttackCompleteHandler = null;
        _grabAttackBusy = false;
        _grabAttackQueue = 0;
        _grabAttackCount = 0; // Reset biến đếm
        _maxGrabAttacks = 0; // Reset ngưỡng

        UnsubscribeThrowEvent();

        // detach complete handler if still registered
        if (_throwTrackEntry != null && _throwCompleteHandler != null)
        {
            _throwTrackEntry.Complete -= _throwCompleteHandler;
            _throwCompleteHandler = null;
            _throwTrackEntry = null;
        }


        // Safety: if somehow we exit grab state while still holding enemy, release it
        if (grabbedEnemyController != null && grabbedEnemyController.isGrabbed)
        {
            //grabbedEnemyController.isGrabbed = false;
            grabbedEnemyController.SwitchToRunState(grabbedEnemyController.enemyIdle);
        }

        isGrabActive = false;
        grabTimer = 0f;
        playerController.canGrab = false;
        playerController.isThrowEnemy = false;
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

    // add public getter
    public bool IsGrabActive()
    {
        return isGrabActive;
    }

    public void CancelGrab()
    {
        CancelGrabAndIdle();
    }
}
