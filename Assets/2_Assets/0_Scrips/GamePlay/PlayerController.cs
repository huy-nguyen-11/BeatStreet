using DG.Tweening;
using PinePie.SimpleJoystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

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
    private float swipeThreshold = 70f;
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
    // Tounch
    public float moveThreshold = 50f;
    public float jumpThreshold = 100f;
    public float speedUpThreshold = 100f;
    // Match DemoTouch timing to reduce perceived tap delay
    private float maxTapTime = 0.2f;
    public float changeHoldTime = 0.3f;
    private Dictionary<int, Vector2> touchStartPositions = new Dictionary<int, Vector2>();
    private Vector2 touchStartPositionsRun;
    private Dictionary<int, float> touchStartTimes = new Dictionary<int, float>();
    public int idTounchRun;
    private float holdTimer;

    // Facing state to avoid repeated flips/snapping
    public bool isFacingRight = true;

    // track last processed entry to avoid duplicate handling
    private TrackEntry lastProcessedAttackEntry = null;

    public int comboIndex = 0;           // 0, 1, 2
    public bool isAttackingCombo = false;  // Flag đang trong combo
    public bool queuedComboAttack = false; // Flag spam tap
    public List<string> comboAttackAnims = new List<string>
{
    "Attack_1", "Attack_1_2", "Attack_1_3" // Điều chỉnh tên animation
};
    private float grabCooldown = 2;
    public bool canGrab = false;
    public bool isSpeedUpAttack = false;
    public bool isResettingFromJump = false;
    private PlayerStateManager pendingStateAfterReset = null;
    [SerializeField] private float joystickMoveThreshold = 12f;
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

        stateManager.Update();
        if (IsDead
            || state == State.Dead
            || state == State.Wingame
            || state == State.Ulti
            || GamePlayManager.Instance.isCheckUlti
            ) return;
        if (state == State.Hit) return;

        CheckTouchInput();

        comboTimer += Time.deltaTime;
        HitTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
            ResetCombo();
        if (HitTimer > HitResetTime)
            ResetHit();
    }
    public void GetJoy()
    {
        if (GamePlayManager.Instance.isCheckUlti
      && state != State.Skill1
      && state != State.Skill2) return;

        // Use both raw and smoothed magnitude
        //Vector2 smoothDir = joystick != null ? joystick.RawDirection : Vector2.zero;
        //float smoothMag = smoothDir.magnitude;
        //float smoothMag = joystick != null ? joystick.RawMagnitude : 0f;
        float smoothMag = joystick != null ? joystick.HandleNormalizedMagnitude : 0f;

        // Configure thresholds (walkThreshold < speedThreshold)
        float walkThreshold = 0.2f;
        float runThreshold = Mathf.Clamp(speedThreshold, 0f, 1f);

        bool canMoveState = (state == State.Idle || state == State.SpeedUp || state == State.Change || state == State.Walk || state == State.Run);


        if (!isJumping && canMoveState)
        {
            if (smoothMag >= runThreshold)
            {
                if (state != State.Run)
                {
                    SwitchToRunState(playerRun);
                }

            }
            else if (smoothMag >= walkThreshold && smoothMag < runThreshold)
            {
                if (state != State.Walk)
                {
                    SwitchToRunState(playerWalk);
                }
            }
            else
            {
                if (state != State.Idle && state != State.Change && state != State.SpeedUp)
                {
                    SwitchToRunState(playerIdle);
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

            if (distX < 0.75f && distY < 0.5f)
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

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int touchId = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isJumping)
                        return;

                    touchStartPositions[touchId] = touch.position;
                    touchStartTimes[touchId] = Time.time;
                    holdTimer = 0f;


                    break;
                case TouchPhase.Moved:
                    if (isJumping)
                    {
                        return;
                    }

                    if (!touchStartTimes.ContainsKey(touchId))
                        return;
                    GetJoy();
                    //grab enemy
                    if ((state == State.Walk || state == State.Run) && canGrab)
                    {
                        //// Check grab condition
                        //var (enemy, distance) = GetNearestEnemy();
                        //if (enemy != null && enemy.enemyController.typeOfEnemy != TypeOfEnemy.Boss)
                        //{
                        //    Vector2 playerPos = transform.position;
                        //    Vector2 enemyPos = enemy.transform.position;

                        //    float distX = Mathf.Abs(enemyPos.x - playerPos.x);
                        //    float distY = Mathf.Abs(enemyPos.y - playerPos.y);

                        //    if (distX < 0.75f && distY < 0.5f)
                        //    {
                        //        bool enemyInFront = isFacingRight ? enemyPos.x > playerPos.x : enemyPos.x < playerPos.x;
                        //        if (enemyInFront && enemy.enemyController.state != EnemyCharacter.State.Dead &&
                        //            enemy.enemyController.state != EnemyCharacter.State.Fall)
                        //        {
                        //            SwitchToRunState(playerGrab);
                        //        }
                        //    }
                        //}
                        CheckGrabEnemy();
                    }
                    isGetJoy = true;
                    break;
                case TouchPhase.Stationary:
                    if (isJumping)
                        return;

                    if (touchStartPositions.ContainsKey(touchId))
                    {
                        holdTimer += Time.deltaTime;
                        if (/*(Mathf.Abs(joystick.SmoothedDirection.x) <= 0.3f || Mathf.Abs(joystick.SmoothedDirection.y) <= 0.3f*/joystick.HandleNormalizedMagnitude <= 0.3f
                            && holdTimer > changeHoldTime && state != State.Change
                            && (state == State.Idle || state == State.Attack) && !isGetJoy)
                        {
                            holdTimer = 0f;
                            SwitchToRunState(playerChange);
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (isJumping)
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
                            if (playerGrab != null && playerGrab.IsGrabActive())
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
                                TriggerAttack();
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
                    }
                    break;
            }
        }
    }

    public void TriggerAttack()
    {
        if (state == State.Attack)
        {
            queuedComboAttack = true;
            return;
        }

        comboAttack = 0; // Reset combo
        queuedComboAttack = false;

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
        transform.localPosition = new Vector2(transform.localRotation.x, 0);
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
        stateManager.FixedUpdate();
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
        if (e.Data.Name == "Hit" )
        {
            SetAttack(id);
        }
        else if (e.Data.Name == "max_hit" || e.Data.Name == "Hit_Max" || e.Data.Name == "Hit_Jump")
        {
            //if (attackArea != null)
            //{
            //    SetAttack(id);
            //    attackArea.SetMaxHit(true);
            //}
            AttackArea currentAttackArea = GetAttackAreaByComboIndex();
            if (currentAttackArea != null)
            {
                SetAttack(id, currentAttackArea);
                currentAttackArea.SetMaxHit(true);
            }
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
        if(idAttackArea != 5)
        {
            if (attackArea != null)
            {
                attackArea.SetAttack(Dame, id);
            }
        }
        else
        {
            if (attackArea != null)
            {
                attackArea.SetAttackSkill(Dame, id);
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
        if (comboAttack > 2)
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
        //transform.GetComponent<SpriteRenderer>().DOFade(0, 0.05f).SetLoops(-1, LoopType.Yoyo);
        isImmortal = true;
        yield return new WaitForSeconds(/*_attributesPet[16]*/0f);
        isImmortal = false;
        //transform.GetComponent<SpriteRenderer>().DOKill();
        //transform.GetComponent<SpriteRenderer>().DOFade(1, 0);
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
                PerformHit();
                if (state != State.Jump && state != State.Skill1 && state != State.Skill2 && state != State.Change)
                {
                    if (isCheckSkill2)
                        isCheckSkill2 = false;
                    SwitchToRunState(playerHit);
                }
            }
        }
        else
        {
            Hp = 0;
            SwitchToRunState(playerDead);
            IsDead = true;
            StartCoroutine(GamePlayManager.Instance.OpenPopupGameOver(2));
        }
        fillBar.SetNewHp(Hp);
    }
    public void SpawnTxtHit(float dame)
    {
        GameObject txt = Instantiate(_prfTxtHit, _pointSpawnTxtHit.transform.position, Quaternion.identity);
        txt.GetComponent<TxtHit>().SetTxt(dame, false);
    }
    public IEnumerator FallCoroutine()
    {
        while (!isCheckGravity ||
            (velocity > 0 && isCheckGravity))
        {
            yield return null;
        }
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
        bool Direction = transform.rotation.y != 0 ? false : true;
        if (!Direction)
            rb.linearVelocity = -Vector2.right * 5f;
        else
            rb.linearVelocity = Vector2.right * 5f;
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

        // wait one frame so any in-flight state changes or animation events settle
        yield return null;

        // wait until grounded or a short timeout (avoid infinite wait)
        float timeout = 0.5f;
        float start = Time.time;
        int waitCount = 0;


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
}