using DG.Tweening;
using PinePie.SimpleJoystick;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensure PlayerController updates early so touch is processed promptly (before other scripts like DemoTouch)
[DefaultExecutionOrder(-1000)]
public class PlayerController : PlayerCharacter
{
    #region fields
    public static PlayerController Instance { get; private set; }
    DataManager dataManager;
    // Joystick
    //public JoystickController joystick;
    public VariableJoystick joystick;
    public float speedThreshold;
    public Transform Char;
    private bool isGetJoy = false;

    //jump distance kick
    public float jumpKickDistance;

    // Attribute
    public List<float> _attributesPet = new List<float>();
    public List<float> _attributesItem = new List<float>();
    public Rigidbody2D rb;
    public AttackArea attackArea;
    public List<AttackArea> attackAreas = new List<AttackArea>();
    public int idAttackArea = 0;
    public int countCancelDamage;
    // Combo
    [SerializeField] Transform _pointSpawnTxtHit;
    [SerializeField] GameObject _prfTxtHit;
    public int comboCount = 0;
    public int HitCount = 0;
    public int comboAttack = 0;
    public float comboTimer = 0f;
    public float HitTimer = 0f;
    public float comboResetTime = 0.65f;
    public float HitResetTime = 1f;
    //for effect fx
    public GameObject fxStrength;
    public Transform posFxJump, posFxFootStep, posFxFall;

    //for attack
    private Queue<int> attackQueue = new Queue<int>();
    private bool isProcessingAttack = false;

    // Ulti
    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] public LayerMask obstacleLayer;
    [SerializeField] private GameObject EnemyDistance;
    [SerializeField] public GameObject AnimUlti;
    // StateMachine
    public PlayerStateManager stateManager;
    public PlayerIdle playerIdle;
    public PlayerRun playerRun;
    public PlayerAttack playerAttack;
    public PlayerJump playerJump;
    public PlayerChange playerChange;
    public PlayerGrab playerGrab;
    public PlayerDead playerDead;
    public PlayerHit playerHit;
    public PlayerPunch playerPunch;
    public PlayerSpeedUp playerSpeedUp;
    public PlayerStandUp playerStandUp;
    public PlayerThrow playerThrow;
    public PlayerWalk playerWalk;
    public PlayerWingame playerWingame;
    public PlayerCombo1 playerCombo1;
    public PlayerCombo3 playerCombo3;
    public PlayerSkill1 playerSkill1;
    public PlayerSkill2 playerSkill2;
    public PlayerUlti playerUlti;

    // Swipe
    public Vector2 startTouchPosition; // Vị trí bắt đầu chạm
    public Vector2 endTouchPosition;   // Vị trí kết thúc chạm
    private Vector2 endTouchPositionChange;   // Vị trí kết thúc chạm
    public float swipeThreshold = 50f;
    public float swipeTimeLimit = 0.3f;
    public float swipeStartTime;

    //check state Jump
    public bool isJumping = false;

    public bool SpeedupDirection = false;
    public bool isButtonDown = false;
    public bool isCheckSkill2 = false;
    public bool isImmortal = false;
    public bool isFall = false;
    public float TimeCheckChange = 0;
    public float holdTime;
    private float touchDuration;

    [SerializeField] public float moveSpeed = 5f;
    public bool IsDead = false;
    public bool HitDirection = false;
    // Fire/DoT damage: take damage without hit-stun/knockback, and if dead keep position.
    public bool deathStayInPlace = false;
    private bool suppressHitReactionThisDamage = false;
    // Tounch
    public float moveThreshold = 50f;
    public float jumpThreshold = 100f;
    public float speedUpThreshold = 100f;
    // Match DemoTouch timing to reduce perceived tap delay
    private float maxTapTime = 0.2f;
    public float changeHoldTime = 0.3f;
    private Dictionary<int, Vector2> touchStartPositions = new Dictionary<int, Vector2>();
    private Dictionary<int, Vector2> touchLastPositions = new Dictionary<int, Vector2>();
    private Vector2 touchStartPositionsRun;
    private Dictionary<int, float> touchStartTimes = new Dictionary<int, float>();
    public int idTounchRun;
    private float holdTimer;
    [SerializeField] private float holdMoveTolerancePixels = 25f; 
    [SerializeField] private float holdJoystickTolerance = 0.5f; 

    // Giảm giật state khi đổi hướng (hysteresis cho Walk/Run/Idle)
    [SerializeField] private float runToWalkGrace = 0.15f;
    [SerializeField] private float walkToIdleGrace = 0.15f;
    private float lastRunStrengthTime = -999f;
    private float lastWalkStrengthTime = -999f;

    // Facing state to avoid repeated flips/snapping
    public bool isFacingRight = true;
    
    // track last processed entry to avoid duplicate handling
    private TrackEntry lastProcessedAttackEntry = null;
    
    public int comboIndex = 0;           // 0, 1, 2 ,3,4
    public bool lastAttackHadHit = false;
    // Timestamp of the last confirmed hit from player's attacks (more reliable than a bool flag).
    [HideInInspector] public float lastHitTime = -999f;
    // How long after a hit we still consider it as "this attack hit" for combo advancing.
    [SerializeField] public float hitConfirmWindow = 0.35f;
    public bool isAttackingCombo = false;  // Flag đang trong combo
    public bool queuedComboAttack = false; // Flag spam tap
    public List<string> comboAttackAnims;
    public string animSlideAttack1, animSlideAttack2 , animUlti;
    private float grabCooldown = 2;
    public bool canGrab = false;
    public bool isSpeedUpAttack = false;
    public bool isResettingFromJump = false;
    private PlayerStateManager pendingStateAfterReset = null;
    [SerializeField] private float joystickMoveThreshold = 12f;

    // Normal / global attack cooldown
    public bool isAttackCooldown = false;
    public float attackCooldownTimer = 0f;
    public float attackCooldownDuration = 2f;
    // Có thể dùng attackCooldownDuration hoặc truyền thời gian riêng cho từng loại tấn công
    
    // Foot step effect
    private float footStepEffectTimer = 0f;
    [SerializeField] private float footStepEffectInterval = 0.3f; // Thời gian giữa các lần spawn effect

    // near other public fields
    [HideInInspector] public bool isInputBlocked = false;

    // --- Combo input buffer / grace window ---
    // Allow the player to tap slightly late (after end_attack) and still continue the combo.
    [SerializeField] private float comboGraceDuration = 0.12f;
    private float comboGraceUntil = -1f;
    private Coroutine comboGraceCoroutine;

    //for attack grab enemy
    public bool isThrowEnemy = false;

    public FillBarPlayer fillBar;

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        playerIdle = new PlayerIdle(this);
        playerRun = new PlayerRun(this);
        playerAttack = new PlayerAttack(this);
        playerJump = new PlayerJump(this);
        playerChange = new PlayerChange(this);
        playerGrab = new PlayerGrab(this);
        playerDead = new PlayerDead(this);
        playerHit = new PlayerHit(this);
        playerPunch = new PlayerPunch(this);
        playerSpeedUp = new PlayerSpeedUp(this);
        playerStandUp = new PlayerStandUp(this);
        playerThrow = new PlayerThrow(this);
        playerWalk = new PlayerWalk(this);
        playerWingame = new PlayerWingame(this);
        playerCombo1 = new PlayerCombo1(this);
        playerCombo3 = new PlayerCombo3(this);
        playerSkill1 = new PlayerSkill1(this);
        playerSkill2 = new PlayerSkill2(this);
        playerUlti = new PlayerUlti(this);
    }
    void Start()
    {
        canGrab = false;
        Char = transform.parent;
        rb = transform.parent.GetComponent<Rigidbody2D>();

        // Initialize facing from Char's local rotation (tolerant)
        Transform t = Char ?? transform;
        float y = t.localEulerAngles.y;
        isFacingRight = Mathf.Abs(Mathf.DeltaAngle(y, 0f)) < 90f;

        dataManager = DataManager.Instance;
        OnInit();

        if (joystick != null)
        {
            joystick.OnSmoothedDirectionChanged += OnJoystickDirectionChanged;
        }

        if(skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Event += HandleAttackEvent;
        }
        isSpeedUpAttack = false;
        if(fxStrength != null)
            fxStrength.SetActive(false);
    }
    
    private void OnDestroy()
    {

        if (joystick != null)
        {
            joystick.OnSmoothedDirectionChanged -= OnJoystickDirectionChanged;
        }

    }
    
    private void OnJoystickDirectionChanged(Vector2 direction, float magnitude)
    {
        //UpdateAnimationStateFromJoystick();
    }
    
    private void UpdateAnimationStateFromJoystick()
    {
        //if (!allowAnimationUpdateFromEvent) return;
        
        if (GamePlayManager.Instance.isCheckUlti
            && state != State.Skill1
            && state != State.Skill2) return;
            
        if (IsDead || state == State.Dead || state == State.Wingame || state == State.Ulti || state == State.Hit)
            return;
            
        if (isJumping) return;

        if (isGetJoy) return;
        
        bool canMoveState = (state == State.Idle || state == State.Change || state == State.Walk || state == State.Run);
        if (!canMoveState) return;

        Vector2 smoothDir = joystick != null ? joystick.RawDirection : Vector2.zero;
        float magnitude = smoothDir.magnitude;

        float walkThreshold = 0.2f;
        float runThreshold = Mathf.Clamp(speedThreshold, 0f, 1f);

        if (magnitude >= runThreshold)
        {
            if (state != State.Run)
                SwitchToRunState(playerRun);
        }
        else if (magnitude >= walkThreshold && magnitude < runThreshold)
        {
            if (state != State.Walk)
                SwitchToRunState(playerWalk);
        }
        else
        {
            if (state != State.Idle)
            {
                SwitchToRunState(playerIdle);
            }
        }
    }

    public void OnInit()
    {
        id = dataManager.idPlayer;
        //animator.runtimeAnimatorController = _anims[id];
        stateManager = playerIdle;
        stateManager.Enter();
        SetAttributePet();
        countCancelDamage = (int)_attributesPet[17];
        Hp = DataManager.Instance.playerData[id].Hp + (DataManager.Instance.playerData[id].Hp * _attributesPet[0]);
        Mana = _attributesPet[8] > 100 * (_attributesPet[8] / 100) ? 1 : 0;
        Dame = DataManager.Instance.playerData[id].Dame + (DataManager.Instance.playerData[id].Dame * _attributesPet[1]); // demo test
        fillBar.OnInit(Hp, Mana);
    }
    private void SetAttributePet()
    {
        int idPet1 = dataManager.idPet1;
        int idPet2 = dataManager.idPet2;
        if (idPet1 != 99)
            _attributesPet[idPet1] += GetAttributePet(idPet1);
        if (idPet2 != 99)
            _attributesPet[idPet2] += GetAttributePet(idPet2);
    }
    private float GetAttributePet(int id)
    {
        List<int> idPet1 = new List<int>() { 0, 1, 3, 5, 7, 8, 9, 10, 11, 12, 13, 14 };
        float coefficient = 1;
        if (idPet1.Contains(id))
            coefficient = 100;
        else
            coefficient = 1;
        return dataManager.petData[id].petAttribute / coefficient;
    }
    void Update()
    {
        if (isInputBlocked || (GamePlayManager.Instance != null && GamePlayManager.Instance.isShowingBoss))
            return;

        //update timmer grab cooldown
        if (!canGrab)
        {
            grabCooldown -= Time.deltaTime;
            if (grabCooldown <= 0)
            {
                canGrab = true;
                grabCooldown = 2;
            }
        }

        // Update normal attack cooldown
        if (isAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                isAttackCooldown = false;
                attackCooldownTimer = 0f;
            }
        }

        stateManager?.Update();
        if (IsDead
            || state == State.Dead
            || state == State.Wingame
            || state == State.Ulti
            || GamePlayManager.Instance.isCheckUlti
            ) return;

        //if (state == State.Hit) return;
        //if(isFall) return;

        if(joystick != null)
            CheckTouchInput();

        comboTimer += Time.deltaTime;
        HitTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
            ResetCombo();
        if (HitTimer > HitResetTime)
            ResetHit();
    }

    public bool IsRecentHit()
    {
        return (Time.time - lastHitTime) <= hitConfirmWindow;
    }

    public bool IsInComboGraceWindow()
    {
        return Time.time <= comboGraceUntil;
    }

    public void BeginComboGraceWindow()
    {
        comboGraceUntil = Time.time + comboGraceDuration;
        if (comboGraceCoroutine != null)
            StopCoroutine(comboGraceCoroutine);
        comboGraceCoroutine = StartCoroutine(ComboGraceRoutine(comboGraceUntil));
    }

    private IEnumerator ComboGraceRoutine(float until)
    {
        // Wait until grace expires or we start chaining.
        while (Time.time < until)
        {
            if (queuedComboAttack)
                yield break;
            yield return null;
        }

        // If still in Attack state and nothing queued, return to idle smoothly.
        if (state == State.Attack && !queuedComboAttack && !IsDead && !isJumping && !isResettingFromJump)
        {
            // Don't use ResetStatus() here (it has jump/ground reset semantics).
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            SwitchToRunState(playerIdle);
        }
    }
    public void GetJoy()
    {
        if (GamePlayManager.Instance.isCheckUlti
      && state != State.Skill1
      && state != State.Skill2) return;


        float smoothMag = joystick != null ? joystick.RawMagnitude : 0f;

        // Configure thresholds (walkThreshold < speedThreshold)
        float walkThreshold = 0.15f;
        float runThreshold = Mathf.Clamp(speedThreshold, 0f, 1f);

        bool canMoveState = (state == State.Idle || state == State.SpeedUp || state == State.Change || state == State.Walk || state == State.Run);

        if (!isJumping && canMoveState && !isFall)
        {

            if (smoothMag >= runThreshold)
            {
                lastRunStrengthTime = Time.time;
                lastWalkStrengthTime = Time.time;
                if (state != State.Run)
                {
                    SwitchToRunState(playerRun);
                }
            }

            else if (smoothMag >= walkThreshold)
            {
                lastWalkStrengthTime = Time.time;
                if (state == State.Run)
                {
                    if (Time.time - lastRunStrengthTime > runToWalkGrace)
                    {
                        SwitchToRunState(playerWalk);
                    }
                }
                else if (state != State.Walk)
                {
                    SwitchToRunState(playerWalk);
                }
            }

            else
            {
                bool canFallToIdle =
                    (state == State.Run || state == State.Walk) &&
                    (Time.time - lastWalkStrengthTime > walkToIdleGrace);

                if (canFallToIdle)
                {
                    if (state != State.Idle && state != State.Change && state != State.SpeedUp)
                    {
                        SwitchToRunState(playerIdle);
                    }
                }
     
            }
        }
    }

    //todo wlak or running
    public void SetMovePlayer(float speed)
    {
        if (IsDead)
        {
            return;
        }
        
        //Vector2 smoothDir = joystick != null ? joystick.SmoothedDirection : Vector2.zero;
        Vector2 rawDir = joystick != null ? joystick.RawDirection : Vector2.zero;
        //float rawMag = joystick.RawMagnitude;

        // Use smoothed magnitude for speed calculation
        //float smoothMag = Mathf.Clamp01(smoothDir.magnitude);
        //float runThreshold = Mathf.Clamp(speedThreshold, 0f, 1f);

        //float walkSpeed = 1.2f;
        //float runSpeed = 3f;

        //float baseSpeed = rawMag >= runThreshold ? runSpeed : walkSpeed;
        float baseSpeed = speed;

        Vector2 movement = rawDir.sqrMagnitude > 0f ? rawDir.normalized * baseSpeed : Vector2.zero;
        
        if (rb != null)
        {
            Vector2 velocityBefore = rb.linearVelocity;
            rb.linearVelocity = movement;
        }

        if (Mathf.Abs(rawDir.x) > 0.1f)
        {
            bool wantRight = rawDir.x > 0f;
            if (wantRight != isFacingRight)
            {
                isFacingRight = wantRight;
                Transform target = Char ?? transform;
                float yRot = isFacingRight ? 0f : 180f;
                Vector3 angles = target.localEulerAngles;
                angles.y = yRot;
                target.localEulerAngles = angles;
            }
        }
        
        // Spawn foot step effect when player is moving
        if (movement.sqrMagnitude > 0.1f && (state == State.Run))
        {
            // Adjust interval based on speed (run faster = spawn more frequently)
            float currentInterval = state == State.Run ? footStepEffectInterval * 0.7f : footStepEffectInterval;
            
            footStepEffectTimer += Time.deltaTime;
            if (footStepEffectTimer >= currentInterval)
            {
                SpawnFootStepEffect();
                footStepEffectTimer = 0f;
            }
        }
        else
        {
            footStepEffectTimer = 0f; // Reset timer when not moving
        }
    }
    
    private void SpawnFootStepEffect()
    {
        if (posFxFootStep != null && ObjectPooler.Instance != null)
        {
            GameObject fx = ObjectPooler.Instance.SpawnFromPool("FxFoot_Step", posFxFootStep.position, Quaternion.identity);
            if (fx != null)
            {
                // Play animation if it's a SkeletonAnimation
                SkeletonAnimation skeletonAnim = fx.GetComponent<SkeletonAnimation>();
                if (skeletonAnim != null)
                {
                    skeletonAnim.AnimationState.SetAnimation(0, "smoke", false);
                }
            }
        }
    }
    
    public void ResetFootStepTimer()
    {
        footStepEffectTimer = 0f;
    }
    
    public void CheckGrabEnemy()
    {
        // Check grab condition
        var (enemy, distance) = GetNearestEnemy();
        if (enemy != null && enemy.enemyController.typeOfEnemy != TypeOfEnemy.Boss)
        {
            Vector2 playerPos = transform.position;
            Vector2 enemyPos = enemy.transform.position;

            float distX = Mathf.Abs(enemyPos.x - playerPos.x);
            float distY = Mathf.Abs(enemyPos.y - playerPos.y);

            if (distX < 0.75f && distY < 0.25f)
            {
                bool enemyInFront = isFacingRight ? enemyPos.x > playerPos.x : enemyPos.x < playerPos.x;
                if (enemyInFront && enemy.enemyController.state != EnemyCharacter.State.Dead &&
                    enemy.enemyController.state != EnemyCharacter.State.Fall && state != State.Grab)
                {
                    SwitchToRunState(playerGrab);
                }
            }
        }
    }

    private void CheckTouchInput()
    {
        if (isInputBlocked || (GamePlayManager.Instance != null && GamePlayManager.Instance.isShowingBoss))
        {
            // clear any in-progress touch state so no leftover inputs
            ClearTouch();
            return;
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int touchId = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isJumping || isFall)
                        return;

                    touchStartPositions[touchId] = touch.position;
                    touchLastPositions[touchId] = touch.position;
                    touchStartTimes[touchId] = Time.time;
                    holdTimer = 0f;


                    break;
                case TouchPhase.Moved:
                    if (isJumping || isFall)
                    {
                        return;
                    }

                    if (!touchStartTimes.ContainsKey(touchId))
                        return;
                    // tích lũy thời gian giữ nếu ngón tay chỉ rung nhẹ
                    Vector2 lastPos = touchLastPositions.TryGetValue(touchId, out var lp) ? lp : touch.position;
                    float frameMove = Vector2.Distance(touch.position, lastPos);
                    touchLastPositions[touchId] = touch.position;
                    bool nearlyStationary = frameMove <= holdMoveTolerancePixels;
                    if (nearlyStationary && joystick.HandleNormalizedMagnitude <= 0.3f && state != State.Change && state != State.SpeedUp && state != State.Hit 
                        && !isAttackCooldown && state != State.Skill2 && state != State.Combo3)
                    {
                        holdTimer += Time.deltaTime;
                        if (holdTimer > changeHoldTime && !isGetJoy && GamePlayManager.Instance.isEnableAttack)
                        {
                            holdTimer = 0f;
                            SwitchToRunState(playerChange);
                        }
                    }
                    else
                    {
                        holdTimer = 0f;
                    }
                    GetJoy();
                    //grab enemy
                    if ((state == State.Walk || state == State.Run) && canGrab)
                    {
                        CheckGrabEnemy();
                    }
                    isGetJoy = true;
                    break;
                case TouchPhase.Stationary:
                    if (isJumping || isFall)
                        return;

                    if (touchStartPositions.ContainsKey(touchId))
                    {
                        holdTimer += Time.deltaTime;
           
                        if ((joystick.HandleNormalizedMagnitude <= 0.3f) && holdTimer > changeHoldTime && state != State.Change && state 
                            != State.SpeedUp && state != State.Hit && !isAttackCooldown && state != State.Skill2 && state != State.Combo3 && GamePlayManager.Instance.isEnableAttack)
                        {
                            holdTimer = 0f;
                            SwitchToRunState(playerChange);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isJumping || isFall)
                        return;
                    if (touchStartPositions.ContainsKey(touchId))
                    {
                        isGetJoy = false;
                        Vector2 startPosition = touchStartPositions[touchId];
                        Vector2 endPosition = touch.position;
                        Vector2 delta = endPosition - startPosition;
                        float deltaTime = Time.time - touchStartTimes[touchId];
 
                        // Use a fixed tap pixel threshold (match DemoTouch behavior)
                        float tapThresholdPixels = 40f;
                        if (Vector2.Distance(startPosition, endPosition) <= tapThresholdPixels) // tap threshold in pixels
                        {
                            //grab attack enemy
                            if (playerGrab != null && playerGrab.IsGrabActive() && !isThrowEnemy)
                            {
                                // queue a grab-attack (will play Grab_Attack on spine track 1 and queue taps while previous anim runs)
                                playerGrab.QueueGrabAttack();
                            }

                            if (state == State.Change)
                            {
                                if(holdTime> 0.3)
                                {
                                    if (fillBar.mana >= 20) 
                                    {
                                        SetMana(-20);
                                        SwitchToRunState(playerSkill2);
                                    }
                                    else
                                    {
                                        if (!GamePlayManager.Instance.isCheckUlti)
                                            SwitchToRunState(playerCombo3);
                                        else
                                        {
                                            if (state != State.Jump
                                                && state != State.Skill1
                                                && state != State.Skill2)
                                                SwitchToRunState(playerIdle);
                                        }
                                    }
                                }
                                else
                                {
                                    SwitchToRunState(playerIdle);
                                }
                            }
                            else if (state == State.SpeedUp)
                            {
                                isSpeedUpAttack = true;
                                if (fillBar.mana >= 7)
                                {
                                    SetMana(-7);
                                    SwitchToRunState(playerSkill1);
                                }
                                else
                                {
                                    SwitchToRunState(playerCombo1);
                                }
                            }
                            else if (deltaTime <= maxTapTime
                                && state != State.Attack
                                && state != State.Jump

                                && state != State.Skill2
                                && state != State.Skill1)
                            {
                                // If we just ended an attack, allow late tap to continue combo instead of resetting.
                                if (IsInComboGraceWindow())
                                {
                                    queuedComboAttack = true;
                                    // ensure we're in Attack state so PlayerAttack can continue chaining
                                    SwitchToRunState(playerAttack);
                                }
                                else
                                {
                                    TriggerAttack();
                                }
                            }
                            else if (state == State.Attack && deltaTime <= maxTapTime)
                            {
                                queuedComboAttack = true;
                            }
                            else
                            {
                                if (state != State.Jump
                                    && state != State.Skill1
                                    && state != State.Skill2)
                                    SwitchToRunState(playerIdle);
                            }
                        }
                        else
                        {
                            if (Time.time - touchStartTimes[touchId] > 0.1f && Time.time - touchStartTimes[touchId] <= swipeTimeLimit)
                            {
                                //grab throw enemy
                                if (playerGrab.IsGrabActive() && Mathf.Abs(delta.x) > 40f)
                                {
                                    float throwDir = delta.x > 0 ? 1f : -1f;
                                    playerGrab.StartThrow(throwDir);
                                }

                                if (delta.magnitude > swipeThreshold)
                                {
                                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) /*&& (state == State.Run || state == State.Walk)*/ && !isJumping)
                                    {
                                        bool isMovingInput = joystick != null && joystick.HandleNormalizedMagnitude > 0.3f;
   
                                        if (state == State.Run || state == State.Walk || isMovingInput)
                                        {
                                            //SpeedupDirection = delta.x > 0;
                                            isFacingRight = delta.x > 0;
                                            SwitchToRunState(playerSpeedUp);
                                        }
                                        else
                                        {
                                            // not moving: treat as idle fallback
                                            if (state != State.Jump
                                                && state != State.Skill1
                                                && state != State.Skill2)
                                                SwitchToRunState(playerIdle);
                                        }
                                    }
                                    else if (delta.y > 0 && state != State.Jump /*&& (state == State.Run || state == State.Walk)*/)
                                    {
                                        SwitchToRunState(playerJump);
                                    }
                                    else
                                    {
                                        if (state != State.Jump
                                            && state != State.Skill1
                                            && state != State.Skill2)
                                            SwitchToRunState(playerIdle);
                                    }
                                }
                                else
                                {
                                    if (state != State.Jump
                                        && state != State.Skill1
                                        && state != State.Skill2)
                                        SwitchToRunState(playerIdle);
                                }
                            }
                            else
                            {
                                if (state != State.Jump
                                    && state != State.Skill1
                                    && state != State.Skill2)
                                    SwitchToRunState(playerIdle);
                            }
                        }
                        touchStartPositions.Remove(touchId);
                        touchStartTimes.Remove(touchId);
                        touchLastPositions.Remove(touchId);
                    }
                    break;
            }
        }
    }

    public void TriggerAttack()
    {
        if (isAttackCooldown || !GamePlayManager.Instance.isEnableAttack)
        {
            return;
        }

        if (state == State.Attack)
        {
            queuedComboAttack = true;
            return;
        }
        lastAttackHadHit = false;
        lastHitTime = -999f;
        comboAttack = 0; // Reset combo
        comboIndex = 0;  // Reset combo animation index
        queuedComboAttack = false;

        if (stateManager == playerGrab && playerGrab != null && playerGrab.IsGrabActive())
        {
            playerGrab.QueueGrabAttack();
            return;
        }


        AimToNearestEnemyOnAttack();

        SwitchToRunState(playerAttack);
    }

    private void ProcessAttackQueue()
    {
        if (isProcessingAttack || attackQueue.Count == 0)
            return;
        isProcessingAttack = true;
        int attack = attackQueue.Dequeue();

        //attack
        SwitchToRunState(playerAttack);

        StartCoroutine(WaitAndProcessNextAttack());
    }

    private IEnumerator WaitAndProcessNextAttack()
    {
        yield return new WaitForSeconds(0.2f);

        isProcessingAttack = false;

        if (attackQueue.Count > 0)
        {
            ProcessAttackQueue();
        }
    }

    public void ClearTouch()
    {
        touchStartPositions.Clear();
    }
    public void SetRevive()
    {
        StartCoroutine(SetImmortalHitBox());
        SwitchToRunState(playerIdle);
        Hp = DataManager.Instance.playerData[id].Hp;
        fillBar.OnInit(Hp, Mana);
        IsDead = false;
        // Reset local Y về 0 (giữ nguyên X)
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, 0f, pos.z);
    }

    float gravity = -18f;
    public float velocity = 0;
    public bool isCheckGravity;

    public void ProcessGravity()
    {
        isCheckGravity = transform.localPosition.y <= 0;
        if (isCheckGravity && velocity < 0)
        {
            velocity = -2f;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime * Vector3.up;
        }
    }

    private void FixedUpdate()
    {
        stateManager?.FixedUpdate();
    }

    public void SwitchToRunState(PlayerStateManager player)
    {
        // Prevent leaving Grab state while grab is active
        if ((stateManager == playerGrab && playerGrab != null && playerGrab.IsGrabActive()) && !(player == playerDead))
        {
            return;
        }

        // Allow some critical states to interrupt jump/reset immediately
        if (player == playerDead || player == playerHit || player == playerUlti)
        {
            if (stateManager != null) stateManager.Exit();
            stateManager = player;
            stateManager.Enter();
            return;
        }

        // Block normal attack + charge (Change) during attack cooldown (speedrun is unaffected)
        if ((isAttackCooldown || (GamePlayManager.Instance != null && !GamePlayManager.Instance.isEnableAttack)) && (player == playerAttack || player == playerChange))
        {
            return;
        }

        // If currently resetting from jump, queue the requested state instead of switching immediately
        if (isJumping || isResettingFromJump)
        {
            pendingStateAfterReset = player;
            return;
        }


        if (stateManager != null)
            stateManager.Exit();
        stateManager = player;
        stateManager.Enter();
    }

    void HandleAttackEvent(TrackEntry trackEntry , Spine.Event e)
    {
        if(GamePlayManager.Instance.isCheckUlti) return;

        if (e.Data.Name == "Hit" )
        {
            SetAttack(id);
        }
        else if (e.Data.Name == "max_hit" || e.Data.Name == "Hit_Max" || e.Data.Name == "Hit_Jump")
        {
            AttackArea currentAttackArea = GetAttackAreaByComboIndex();
            if (currentAttackArea != null)
            {
                SetAttack(id, currentAttackArea);
                currentAttackArea.SetMaxHit(true);
            }
        }else if(e.Data.Name == "End_Hit")
        {
            AttackArea currentAttackArea = attackAreas[2];// for skill jump attack
            if (currentAttackArea != null)
            {
                currentAttackArea.EndHitJump();
            }
        }
        else if (e.Data.Name == "Impact" && idAttackArea == 0)
        {
            int _dir = isFacingRight ? 1 : 12;
            ObjectPooler.Instance.SpawnFromPool("HitWukong", new Vector3(transform.position.x - (_dir * 0.3f), transform.position.y - 0.3f, 0f), Quaternion.Euler(0, 0, 0));
        }
    }

    public void SetAttack(int id)
    {
        AttackArea currentAttackArea = GetAttackAreaByComboIndex();
        if (currentAttackArea != null)
        {
            SetAttack(id, currentAttackArea);
        }
    }

    public void SetAttack(int id, AttackArea attackArea)
    {
        if(idAttackArea == 5)
        {
            if (attackArea != null)
            {
                attackArea.SetAttackSkill(Dame, id); // attack all around player
                int _dir = isFacingRight ? 1 : 8;
                if(id == 2)
                {
                    ObjectPooler.Instance.SpawnFromPool("HitMaxWukong", new Vector3(attackArea.transform.position.x - (_dir * 0.5f),
                        attackArea.transform.position.y - 0.6f, 0f), Quaternion.Euler(0, 0, 0));
                }
            }
        }
        else if(idAttackArea == 4)
        {
            if (attackArea != null)
            {
                attackArea.SetAttackSkillSpeedUp(Dame, id); // attack speed up limmit lane y , allow attack back player
            }
        }
        else if(idAttackArea == 2)
        {
            if (attackArea != null)
            {
                //attackArea.SetAttackSkillJump(Dame, id); // attack jump area
                attackArea.StartHitJump(Dame);
            }
        }
        else
        {
            if (attackArea != null)
            {
                attackArea.SetAttack(Dame, id); // normal attack : online attack font of player and limmit lane y
            }
        }

    }

    private AttackArea GetAttackAreaByComboIndex()
    {
        if (attackAreas == null || attackAreas.Count == 0)
            return null;

        int index = idAttackArea;
        return attackAreas[index];
    }

    public float SetFatal(int id)
    {
        float Fatal = _attributesPet[id];
        float check = Random.Range(0f, 1f);
        return check <= Fatal ? 2 : 1;
    }

    private void ResetCombo()
    {
        SetManaCountCombo();
        comboAttack = 0;
        comboCount = 0;
    }
    private void ResetHit()
    {
        HitCount = 0;
    }
    public void PerformAttack()
    {
        comboTimer = 0f;
        comboAttack++;
        if (comboAttack > 4)
        {
            comboAttack = 0;
        }
    }
    public void CountCombo()
    {
        comboCount++;
        GamePlayManager.Instance.SetAnimCombo(comboCount);
    }

    public void PerformHit()
    {
        HitTimer = 0f;
        HitCount++;
        if (HitCount > 3)
        {
            HitCount = 0;
        }
    }
    public void SetManaCountCombo()
    {
        if (comboCount <= 1) return;
        else if (comboCount == 2) SetMana(5 + _attributesPet[15]);
        else if (comboCount >= 3 && comboCount <= 9) SetMana(10 + _attributesPet[15]);
        else if (comboCount >= 10 && comboCount <= 20) SetMana(30 + _attributesPet[15]);
        else SetMana(50 + _attributesPet[15]);
    }

    public void SetActiveSkill2()
    {
        isCheckSkill2 = true;
    }
    //public void SetRunSkill2()
    //{
    //    StartCoroutine(attackSkill2.SetAttackSkill2Player(Dame * SetFatal(12)));
    //}
    public void SetUltiPlayer()
    {
        SwitchToRunState(playerUlti);
    }
    public void SetFalseChangeUlti()
    {
        AnimUlti.SetActive(false);
        if (id == 0)
            playerController.PlayAnim("Strength", false);
        else if (id == 1)
            playerController.PlayAnim("Strength", false);
    }
    public void SetImmortal()
    {
        isImmortal = true;
    }
    public IEnumerator SetImmortalHitBox()
    {
        isFall = false;
        FlashKnockBack(0.1f, 10);
        isImmortal = true;
        yield return new WaitForSeconds(/*_attributesPet[16]*/2f);
        isImmortal = false;
    }

    public (EnemyChar enemy, float distance) GetNearestEnemy()
    {
        float nearestDistance = float.MaxValue;
        EnemyChar nearestEnemy = null;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

        foreach (Collider2D collider in hitColliders)
        {
            EnemyChar enemy = collider.GetComponent<EnemyChar>();
            if (enemy != null)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }
        return (nearestEnemy, nearestDistance == float.MaxValue ? -1 : nearestDistance);
    }

    public void AimToNearestEnemyOnAttack()
    {
        var (enemy, distance) = GetNearestEnemy();
        if (enemy == null || distance < 0)
            return;

        bool facingRight = enemy.transform.position.x > transform.position.x;
        SetFacingDirection(facingRight);
    }

    /// <summary>
    /// Di chuyển player tới vị trí lý tưởng để đánh Ulti:
    /// - Cách enemy ~1.0 đơn vị theo trục X (trong khoảng 0.85 - 1.2), bên trái hoặc phải.
    /// - Đảm bảo |deltaY| với enemy không vượt quá 0.2.
    /// - Nếu hướng đang chọn bị vướng tường thì thử di chuyển sang hướng còn lại.
    /// </summary>
    public void MoveToIdealUltiPosition(Transform enemyTransform)
    {
        if (enemyTransform == null)
            return;

        // Dùng Char làm gốc nếu có, vì Rigidbody2D thường nằm ở Char
        Vector3 playerPos = Char != null ? Char.position : transform.position;
        Vector3 enemyPos = enemyTransform.position;

        // Các ngưỡng khoảng cách mong muốn theo trục X so với enemy
        const float minXDist = 0.85f;
        const float maxXDist = 1.2f;
        const float idealXDist = 1.0f; // nằm trong [min, max]

        // Ưu tiên hướng hiện tại (đang đứng bên trái hay bên phải enemy)
        bool isLeftOfEnemy = playerPos.x < enemyPos.x;
        int[] dirOrder = isLeftOfEnemy
            ? new int[] { -1, 1 }  // đang ở bên trái -> thử trái trước rồi phải
            : new int[] { 1, -1 }; // đang ở bên phải -> thử phải trước rồi trái

        var gm = GamePlayManager.Instance;
        Vector3? chosenPos = null;

        // Thử lần lượt 2 phía quanh enemy
        foreach (int dir in dirOrder)
        {
            // Vị trí mục tiêu lý tưởng theo trục X: cách enemy ~1 đơn vị
            float targetX = enemyPos.x + dir * idealXDist;

            // Vị trí mục tiêu theo trục Y: giới hạn sao cho |targetY - enemyY| <= 0.2
            float targetY = Mathf.Clamp(playerPos.y, enemyPos.y - 0.2f, enemyPos.y + 0.2f);

            Vector3 candidate = new Vector3(targetX, targetY, playerPos.z);

            // Nếu có layer tường: không chọn hướng mà đường đi bị chắn bởi tường
            bool blockedByWall = false;
            if (obstacleLayer.value != 0)
            {
                RaycastHit2D hit = Physics2D.Linecast(playerPos, candidate, obstacleLayer);
                if (hit.collider != null)
                {
                    blockedByWall = true;
                }
            }
            if (blockedByWall)
                continue;

            // Giới hạn trong biên bản đồ/camera nếu có thiết lập
            if (gm != null)
            {
                candidate.x = Mathf.Clamp(candidate.x, gm.minPosX, gm.maxPosX);

                // Chỉ clamp Y nếu min/max Y được cấu hình hợp lệ
                if (gm.maxPosY > gm.minPosY)
                    candidate.y = Mathf.Clamp(candidate.y, gm.minPosY, gm.maxPosY);
            }

            // Kiểm tra lại điều kiện khoảng cách sau khi clamp
            float dx = Mathf.Abs(candidate.x - enemyPos.x);
            float dy = Mathf.Abs(candidate.y - enemyPos.y);
            if (dx < minXDist || dx > maxXDist)
                continue;
            if (dy > 0.2f)
                continue;

            // Tìm được vị trí phù hợp
            chosenPos = candidate;
            break;
        }

        // Nếu cả hai phía đều không thỏa mãn (do tường / biên bản đồ),
        // fallback: chỉ chỉnh lại trục Y nếu khoảng cách X hiện tại đã tương đối phù hợp.
        if (!chosenPos.HasValue)
        {
            float currentDx = Mathf.Abs(playerPos.x - enemyPos.x);
            if (currentDx >= minXDist && currentDx <= maxXDist)
            {
                float newY = Mathf.Clamp(playerPos.y, enemyPos.y - 0.2f, enemyPos.y + 0.2f);
                Vector3 fallback = new Vector3(playerPos.x, newY, playerPos.z);

                if (gm != null)
                {
                    fallback.x = Mathf.Clamp(fallback.x, gm.minPosX, gm.maxPosX);
                    if (gm.maxPosY > gm.minPosY)
                        fallback.y = Mathf.Clamp(fallback.y, gm.minPosY, gm.maxPosY);
                }

                chosenPos = fallback;
            }
        }

        if (!chosenPos.HasValue)
            return;

        if (Char != null)
            Char.position = chosenPos.Value;
        else
            transform.position = chosenPos.Value;
    }

    public void SetHit(float dameHit)
    {
        if (IsDead) return;
        if (isImmortal) return;
        if (countCancelDamage > 0)
        {
            countCancelDamage--;
            return;
        }
        if (GamePlayManager.Instance.isActiveDefence)
        {
            dameHit = dameHit * (GamePlayManager.Instance.attributePlayer[1] / 100f);
        }
        if (GamePlayManager.Instance.isActiveSkill)
        {
            float check = Random.Range(0f, 1f);
            if (check <= GamePlayManager.Instance.attributePlayer[1])
            {
                dameHit = dameHit - dataManager.playerData[id].Mana < 0 ? 0 : dameHit - dataManager.playerData[id].Mana;
            }
        }
        if (dameHit <= 0) return;
        Hp = Hp - dameHit < 0 ? 0 : Hp - dameHit;
        SpawnTxtHit(dameHit);
        if (Hp > 0)
        {
            if (!isFall)
            {
                if (!suppressHitReactionThisDamage)
                {
                    PerformHit();
                    if (state != State.Jump && state != State.Skill1 && state != State.Skill2)
                    {
                        if (playerGrab != null && playerGrab.IsGrabActive())
                        {
                            playerGrab.CancelGrab();
                        }
                        SwitchToRunState(playerHit);
                    }
                }
                else
                {
                    if (state != State.Jump && state != State.Skill1 && state != State.Skill2)
                    {
                        if (playerGrab != null && playerGrab.IsGrabActive())
                        {
                            playerGrab.CancelGrab();
                        }
                        SwitchToRunState(playerHit);
                    }
                }

            }
        }
        else
        {
            Hp = 0;
            if (playerGrab != null && playerGrab.IsGrabActive())
            {
                playerGrab.CancelGrab();
            }
            SwitchToRunState(playerDead);
            IsDead = true;
            StartCoroutine(GamePlayManager.Instance.OpenPopupGameOver(2));
        }
        fillBar.SetNewHp(Hp);
    }

    /// <summary>
    /// Damage from fire/DoT while stepping on hazards:
    /// - Không tăng HitCount / không vào PlayerHit (không bị bay ra khi đủ hit)
    /// - Nếu chết: chết ngay tại chỗ (không SetFall)
    /// </summary>
    public void SetHitFromFire(float dameHit)
    {
        suppressHitReactionThisDamage = true;
        deathStayInPlace = true;
        SetHit(dameHit);
        // If still alive, only suppress reaction for this tick.
        if (!IsDead)
            deathStayInPlace = false;
        suppressHitReactionThisDamage = false;
    }
    public void SpawnTxtHit(float dame)
    {
        //GameObject txt = Instantiate(_prfTxtHit, _pointSpawnTxtHit.transform.position, Quaternion.identity);
        GameObject txt = ObjectPooler.Instance.SpawnFromPool("Text", _pointSpawnTxtHit.transform.position, Quaternion.identity);
        txt.GetComponent<TxtHit>().SetTxt(dame, false);
    }
    public IEnumerator FallCoroutine()
    {
        while (!isCheckGravity ||
            (velocity > 0 && isCheckGravity))
        {
            yield return null;
        }
        // Reset local Y về 0 sau khi ngã chạm đất
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, 0f, pos.z);

        GameObject fx = ObjectPooler.Instance.SpawnFromPool("Fx_Fall", posFxFall.position, Quaternion.identity);
        fx.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "fall", false);
        yield return new WaitForSeconds(0.12f);
        fx.SetActive(false);
        SwitchToRunState(playerStandUp);
    }
    public IEnumerator JumpCoroutine()
    {
        while (!isCheckGravity ||
            (velocity > 0 && isCheckGravity))
        {
            yield return null;
        }
        ResetStatus();
    }
    public void SetFall()
    {
        // When falling/dead, knock the player in the direction of received damage.
        // HitDirection is set by the attacker before calling SetHit().
        if (rb == null) return;
        float dir = HitDirection ? 1f : -1f;
        rb.linearVelocity = Vector2.right * (3.5f * dir);
    }

    // Public helper to set facing consistently (updates flag + visual rotation)
    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
        Transform target = Char ?? transform;
        float yRot = isFacingRight ? 0f : 180f;
        Vector3 angles = target.localEulerAngles;
        angles.y = yRot;
        target.localEulerAngles = angles;
    }

    public void SetMana(float mana)
    {
        Mana += mana;
        if (Mana > 100)
            Mana = 100;
        fillBar.SetNewMana(Mana);
    }
    public void SetHp(float hp)
    {
        Hp += hp;
        if (Hp > fillBar.maxHp)
            Hp = fillBar.maxHp;
        fillBar.SetNewHp(Hp);
    }
    public void ResetStatus()
    {
        //isJumping = false;
        //SwitchToRunState(playerIdle);

        //isJumping = false;
        //if (rb != null)
        //{
        //    rb.linearVelocity = Vector2.zero;
        //}
        //SwitchToRunState(playerIdle);
        // Start coroutine only if not already running

        // If already resetting, ignore duplicate calls
        if (isResettingFromJump)
        {
            return;
        }

        // Set flag to prevent duplicate calls
        isResettingFromJump = true;
        // stop horizontal movement immediately to avoid sliding / race with later state
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Start coroutine to wait for ground and then finalize reset
        StartCoroutine(DoResetStatus());
    }

    private IEnumerator DoResetStatus()
    {
        // stop horizontal movement to avoid sliding
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Reset local Y về 0 sau khi nhảy/ngã (playerController con trong PlayerChar)
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, 0f, pos.z);

        // wait one frame so any in-flight state changes or animation events settle
        yield return null;

        // default landing -> Idle
        if (stateManager != null)
            stateManager.Exit();
        stateManager = playerIdle;
        stateManager.Enter();

        // CHỈ reset isJumping SAU KHI đã chuyển state xong
        isJumping = false;
        isResettingFromJump = false;

        // apply any queued state requested while we were resetting
        if (pendingStateAfterReset != null)
        {
            var toApply = pendingStateAfterReset;
            pendingStateAfterReset = null;
            // this will run SwitchToRunState again (isResettingFromJump == false now)
            SwitchToRunState(toApply);
        }
    }

    //public IEnumerator CheckAnimationAndTriggerEvent(string name)
    //{
    //    AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
    //    while (state.normalizedTime < 1f || !state.IsName(name))
    //    {
    //        state = animator.GetCurrentAnimatorStateInfo(0);
    //        yield return new WaitForSecondsRealtime(0f);
    //    }
    //    ResetStatus();
    //}
    public IEnumerator setPlayerNextTurn()
    {
        yield return new WaitForSeconds(0.5f);
        GamePlayManager.Instance._levelMap.SetCamera();
        Char.position = new Vector2(Char.position.x + 3, Char.position.y);
        yield return new WaitForSeconds(0.75f);
        if (GamePlayManager.Instance._levelMap.TurnEnemy >= GamePlayManager.Instance._levelMap.listTurnEnemy.childCount - 1)
            AudioBase.Instance.SetMusicGPL(1);
        GamePlayManager.Instance.SetOffDarkScene();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
        }
        stateManager.OnTriggerEnter(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        stateManager.OnTriggerStay(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        stateManager.OnTriggerExit(collision);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        stateManager.OnCollisionEnter2D(collision);
    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //}

    private Coroutine flashCoroutine;
    Color hitColor = HexToColor("#FFCD45");

    public void FlashKnockBack(float flashTime = 0.1f, int flashCount = 3)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine(flashTime, flashCount));
    }

    private IEnumerator FlashCoroutine(float flashTime, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            skeletonAnimation.skeleton.SetColor(hitColor);
            yield return new WaitForSeconds(flashTime);

            skeletonAnimation.skeleton.SetColor(Color.white);
            yield return new WaitForSeconds(flashTime);
        }
    }

    public static Color HexToColor(string hex)
    {
        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.white;
    }
}