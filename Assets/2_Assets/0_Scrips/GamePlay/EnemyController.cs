using DG.Tweening;
using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeOfEnemy
{
    Enemy,
    EliteEnemy,
    Boss,
}

public class EnemyController : EnemyCharacter
{
    public TypeOfEnemy typeOfEnemy;
    public Transform player;
    public PlayerController playerController;
    DataManager dataManager;
    public TextMeshPro txtLevel;
    public Transform Char;
    public Rigidbody2D rb;
    public AttackArea attackArea;
    public float rangeAttack = 1f;
    public bool isBoss;
    public int Level;
    public int idEnemy;
    public float Hp = 3;
    public float dame = 3;
    public LayerMask playerLayer;
    public EnemyStateMachine stateManager;
    public EnemyRun enemyRun;
    public EnemyIdle enemyIdle;
    public EnemyHit enemyHit;
    public EnemyAttack enemyAttack;
    public EnemyDead enemyDead;
    public EnemyFall enemyFall;
    public EnemyGrabed enemyGrabed;
    public EnemySpawn enemySpawn;
    public EnemyChasePlayer enemyChasePlayer;
    public GameObject prfCoin;
    // NEW: Flag to indicate grabbed state - prevents all movement and state changes
    public bool isGrabbed = false;
    public bool isActiveRun = false;
    public bool isSpawned = false; // Flag to indicate if enemy was spawned (setactive true)
    // Hit
    [SerializeField] Transform _pointTxtHit;
    [SerializeField] GameObject _prfTxtHit;
    public int currentHitIndex = 0;
    public float HitTimeout = 5.0f;
    public bool isCheckAnimAttack = false;
    public bool isGetHitStrengthMax = false;
    public Coroutine coroutine;
    //public Coroutine coroutineAttack;

    // Enemy AI
    public float patrolRadius = 3f;
    public float moveSpeed = 2.5f;

    public float moveSpeedRun = 2.6f;
    public float moveSpeedWalk = 1.2f;
    public float moveSpeedRunBack = 2.2f;
    public float moveSpeedWalkBack = 0.9f;

    public float attackRadius = 1.5f;
    public float attackCooldown = 2f;
    public bool isWall;
    public bool canCheckWall;
    public bool isAttack;
    public bool isAttacking = false;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    private Vector3 lastDirection;
    public Vector3 randomTarget;
    private Vector3 lastRandomTarget;
    private bool isTooClosePlayer = false;
    public bool isCheckingPlayer = false;

    //private float patrolDuration = 1.5f;
    public float patrolTimer = 0f; 
    public float stopTimer = 0f; 
    public bool isStopping = false;
    // Duration to stay idle when stopping
    public float stopDuration = 1f;
    // Flag to indicate a tween/throw is active — prevents Movement from overriding tween
    public bool isBeingThrown = false;
    //private float stopDuration = 1.25f;

    // --- Movement animation helpers ---
    // animation names (adjust to your Spine/Animator naming)
    private const string ANIM_RUN = "Run";
    private const string ANIM_WALK = "Walk";
    private const string ANIM_RUN_BACK = "Run"; // adjust if your project uses different naming
    private const string ANIM_WALK_BACK = "Run_Back"; // adjust if your project uses different naming
                                                      // private const string ANIM_IDLE = "Idle"; // minimal: ensure Idle exists
    [HideInInspector]
    public bool shouldDirectChase = false;

    // thresholds (tweak to taste)
    public float runThreshold = 1.0f;
    public float walkThreshold = 0.25f;
    // small threshold used to decide "already at target" -> Idle
    public float idleThreshold = 0.05f;
    // hysteresis to avoid flicker between idle and move
    public float animationHysteresis = 0.08f;

    private string currentMoveAnim = null;
    public Transform posBullet , posKnife , posWave , posThrower;
    public float rangeThrower;
    public float timerCheckThrower = 3f;

    // for all enemies tracking
    private static readonly List<EnemyController> s_AllEnemies = new List<EnemyController>();

    //aura boss
    [SerializeField] private GameObject auraBoss;

    private void OnEnable()
    {
        if (!s_AllEnemies.Contains(this))
            s_AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        s_AllEnemies.Remove(this);
    }

    // Public method to reset move animation (useful when transitioning from states like Fall)
    public void ResetMoveAnimation()
    {
        currentMoveAnim = null;
    }

    private void Awake()
    {
        //playerController = FindObjectOfType<PlayerController>();
        playerController = GamePlayManager.Instance._Player;
        player = playerController.transform.parent;
        enemyIdle = new EnemyIdle(this);
        enemyRun = new EnemyRun(this);
        enemyHit = new EnemyHit(this);
        enemyFall = new EnemyFall(this);
        enemyAttack = new EnemyAttack(this);
        enemyDead = new EnemyDead(this);
        enemyGrabed = new EnemyGrabed(this);
        enemySpawn = new EnemySpawn(this);
        enemyChasePlayer = new EnemyChasePlayer(this);
    }
    void Start()
    {
        isActiveRun = false;
        dataManager = DataManager.Instance;
        Char = transform.parent.GetComponent<Transform>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
        if (!isSpawned)
        {
            stateManager = enemyIdle;
            stateManager.Enter();
        }
        Level = dataManager.dataBase.listModeLevelEnemyMaps[dataManager.LevelSelect].Mode[dataManager.LevelMode];
        Hp = dataManager.dataBase.listLevelEnemyUpgrades[idEnemy].HP[Level] * (isBoss ? 2f : 1f);
        dame = dataManager.dataBase.listLevelEnemyUpgrades[idEnemy].Dame[Level] * (isBoss ? 1.5f : 1f);
        fillBar.OnInit(Hp);
        SetLevel();

        skeletonAnimation.AnimationState.Event += HandleAttackEvent;

        if(auraBoss != null)
        {
            auraBoss.SetActive(false);
        }
    }

    public void InitializeEnemy()
    {
        Char = transform.parent.GetComponent<Transform>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    private float timmerCheckThrower = 0f;
    public bool isEnableThrower = false;

    void Update()
    {
        stateManager.Update();
        if (state == State.Dead) return;
        if (playerController.IsDead) return;
        if (state != State.Idle && GamePlayManager.Instance.isCheckUlti)
            SwitchToRunState(enemyIdle);
        if (GamePlayManager.Instance.isCheckUlti) return;
        // Skip attack check if grabbed
        if (isGrabbed)
        {
            if (state != State.Grabed)
            {
              
            }
            return;
        }
        if (state != State.Hit && state != State.Fall && state != State.Attack)
            CheckAttack();

        if(!isEnableThrower)
        {
            timmerCheckThrower += Time.deltaTime;
            if(timmerCheckThrower >= timerCheckThrower)
            {
                isEnableThrower = true;
                timmerCheckThrower = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        fillBar.transform.rotation = Quaternion.Euler(new Vector3(0, transform.position.y, 0));
        stateManager.FixedUpdate();
    }

    private void LateUpdate()
    {
        //update sorting order
        if(state == State.Dead) return;
        if(state == State.Grabed) return;
        float posY = Char.position.y;
        float posYPlayer = player.position.y;
        float yTolerance = 0.05f;
        if (posY > posYPlayer + yTolerance )
        {
            skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 4;
        }
        else if( posY < posYPlayer - yTolerance)
        {
            skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 10;
        }
    }

    private void SetLevel()
    {
        txtLevel.text = "Level " + Level.ToString();
    }
    public void SetRun()
    {
        SwitchToRunState(enemyRun);
    }

    private bool isAvoidingPlayer = false;
    private bool isPatrolling = true;

    /// <summary>Boss id=2: Bật chase và ngắt patrol/avoid để di chuyển trực tiếp về player.</summary>
    public void PrepareDirectChase()
    {
        shouldDirectChase = true;
        isPatrolling = false;
        isAvoidingPlayer = false;
    }

    private void SeparateFromOtherEnemies()
    {
        // Guard: don't modify position while being thrown
        if (isBeingThrown) return;
        float minDist = 1f; // Khoảng cách tối thiểu giữa các enemy
        Vector3 separation = Vector3.zero;
        int count = 0;

        for (int i = 0; i < s_AllEnemies.Count; i++)
        {
            var enemy = s_AllEnemies[i];
            if (enemy == null) continue;
            if (enemy == this || enemy.state == State.Dead) continue;
            if (enemy.Char == null) continue;

            float dist = Vector3.Distance(Char.position, enemy.Char.position);
            if (dist < minDist && dist > 0.01f)
            {
                separation += (Char.position - enemy.Char.position).normalized * (minDist - dist);
                count++;
            }
        }


        if (count > 0)
        {
            Char.position += separation / count * 0.7f;
        }
    }

    // Helper: pick move animation based on next target
    private float SetMoveAnimationByTarget(Vector3 nextTarget)
    {

        // Safety checks
        if (isGrabbed || isBeingThrown) return 0f;
        if (Char == null || player == null) return moveSpeed;
        if (state == State.Dead || state == State.Attack || state == State.Hit || state == State.Fall) return moveSpeed;

        Vector3 toTarget = nextTarget - Char.position;
        float dist = toTarget.magnitude;
        float absDistX = Mathf.Abs(toTarget.x);

        // If essentially at target, don't try to pick a move animation
        if (dist <= idleThreshold) return 0f;

        // Determine facing using Char's current rotation (avoid race with player movement)
        bool facingRight = Mathf.Abs(Mathf.DeltaAngle(Char.localEulerAngles.y, 0f)) < 90f;
        int moveDirX = (int)Mathf.Sign(toTarget.x);
        bool movingForward = (moveDirX == (facingRight ? 1 : -1));

        // Hysteresis band to avoid flicker around the walk/run threshold
        float lower = Mathf.Max(0f, walkThreshold - animationHysteresis);
        float upper = walkThreshold + animationHysteresis;

        string chosenAnim;
        if (absDistX <= lower)
        {
            chosenAnim = ANIM_WALK;
        }
        else if (absDistX >= upper)
        {
            chosenAnim = ANIM_RUN;
        }
        else
        {
            // inside hysteresis band -> keep previous animation if exists, else pick based on threshold
            chosenAnim = currentMoveAnim ?? (absDistX <= walkThreshold ? ANIM_WALK : ANIM_RUN);
        }

        // Use back variants when moving backwards
        if (!movingForward)
        {
            chosenAnim = (chosenAnim == ANIM_RUN) ? ANIM_RUN_BACK : ANIM_WALK_BACK;
        }

        // Boss special-case: force run
        if (typeOfEnemy == TypeOfEnemy.Boss)
        {
            // Avoid showing run animation when we're essentially at the target.
            if (dist <= idleThreshold)
            {
                return 0f;
            }

            if (currentMoveAnim != ANIM_RUN)
            {
                currentMoveAnim = ANIM_RUN;
                PlayAnim(ANIM_RUN, true);
            }
            return moveSpeedRun;
        }

        // Only switch animation when it actually changes
        if (currentMoveAnim != chosenAnim)
        {
            currentMoveAnim = chosenAnim;
            PlayAnim(chosenAnim, true);
        }

        // Return speed mapped to chosen anim
        if (chosenAnim == ANIM_RUN) return moveSpeedRun;
        if (chosenAnim == ANIM_WALK) return moveSpeedWalk;
        if (chosenAnim == ANIM_RUN_BACK) return moveSpeedRunBack;
        if (chosenAnim == ANIM_WALK_BACK) return moveSpeedWalkBack;
        return moveSpeed;

    }

    // --------------------------------------------------
    // Movement entrypoint: ensure facing is up-to-date before decision
    public void Movement()
    {
        // If grabbed or being thrown, stop all movement immediately
        if (isGrabbed || isBeingThrown)
        {
            return;
        }



        // Keep facing consistent for animation decision
        UpdateEnemyRotation();

        //// Is BOSS: Allway target
        //if (typeOfEnemy == TypeOfEnemy.Boss)
        //{
        //    MoveToPlayer();
        //    UpdateEnemyRotation();
        //    //SeparateFromOtherEnemies();
        //    return;
        //}

        if (player == null) return;

        // Tick stop timer even while in Movement state; if still stopping, stay in Idle
        if (isStopping)
        {
            TickStopTimer();
            if (isStopping)
            {
                if (stateManager != enemyIdle)
                {
                    SwitchToRunState(enemyIdle);
                }
                UpdateEnemyRotation();
                SeparateFromOtherEnemies();
                return;
            }
        }
        else
        {
            if (isAvoidingPlayer)
            {
                PatrolRandomly(); // dùng randomTarget đã được AvoidPlayer() set
                if (Vector3.Distance(Char.position, randomTarget) < 0.2f)
                {
                    isAvoidingPlayer = false;
                    isStopping = true;
                    patrolTimer = 0f;
                }
            }
            else if (!isPatrolling && CheckPlayerTooClose())
            {
                AvoidPlayer();
                isAvoidingPlayer = true;
            }
            else
            {
                if (isPatrolling)
                {
                    PatrolRandomly();
                }
                else
                {
                    MoveToPlayer();
                }
            }

            // FIX: Only increment patrol timer if we're actually moving
            // Check if we have a valid target and are moving towards it
            float preferredRange = (typeOfEnemy == TypeOfEnemy.EliteEnemy || typeOfEnemy == TypeOfEnemy.Boss) ? rangeThrower : rangeAttack;
            Vector3 currentTarget = isPatrolling ? randomTarget :
                (Char.position.x < player.position.x ?
                    player.position + Vector3.left * preferredRange :
                    player.position + Vector3.right * preferredRange);

            float distanceToTarget = Vector3.Distance(Char.position, currentTarget);

            // Only increment timer if we're not already at target
            if (distanceToTarget > 0.2f)
            {
                float patrolDuration = typeOfEnemy == TypeOfEnemy.Boss ? 3.65f : 1.5f;

                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolDuration)
                {
                    isStopping = true;
                    patrolTimer = 0f;
                }
            }
            else
            {
                // FIX: Reset timer if we're at target, don't let it accumulate
                patrolTimer = 0f;
            }

            //patrolTimer += Time.deltaTime;
            //if (patrolTimer >= patrolDuration)
            //{
            //    isStopping = true;
            //    patrolTimer = 0f;
            //}
        }

        UpdateEnemyRotation();
        SeparateFromOtherEnemies();
    }

    //// Handle stop timer so Idle can advance even when state machine isn't running Movement
    public void TickStopTimer()
    {
        if (!isStopping) return;

        stopTimer += Time.deltaTime;
        if (stopTimer >= stopDuration)
        {
            isStopping = false;
            stopTimer = 0f;
            float num = typeOfEnemy == TypeOfEnemy.Boss ? 0.3f : 0.3f;
            isPatrolling = Random.value < num;
            isAvoidingPlayer = false;
            //if (isPatrolling && (typeOfEnemy == TypeOfEnemy.Enemy || typeOfEnemy == TypeOfEnemy.EliteEnemy))
            //    SetRandomPatrolTarget();

            if(isPatrolling)
                SetRandomPatrolTarget();
        }
    }

    private void MoveToPlayer()
    {
        if (isAttacking)
        {
            return;
        }
        if (isBeingThrown || isGrabbed) return;
        // replace existing:
        //float targetOffset = rangeAttack;
        float targetOffset = (typeOfEnemy == TypeOfEnemy.EliteEnemy && typeOfEnemy == TypeOfEnemy.Boss) ? rangeThrower : rangeAttack;
        Vector3 leftTarget = player.position + Vector3.left * targetOffset;
        Vector3 rightTarget = player.position + Vector3.right * targetOffset;

        bool leftOccupied = IsTargetOccupiedByOtherEnemy(leftTarget);
        bool rightOccupied = IsTargetOccupiedByOtherEnemy(rightTarget);

        Vector3 targetPos;
        // replace existing Debug.Log("target");
        if (!leftOccupied && Char.position.x < player.position.x)
            targetPos = leftTarget;
        else if (!rightOccupied && Char.position.x > player.position.x)
            targetPos = rightTarget;
        else if (!leftOccupied)
            targetPos = leftTarget;
        else if (!rightOccupied)
            targetPos = rightTarget;
        else
        {
            isPatrolling = true;
            SetRandomPatrolTarget();
            PatrolRandomly();
            return;
        }

        // BOSS - special chase behavior for idEnemy == 2
        if (typeOfEnemy == TypeOfEnemy.Boss && idEnemy == 2 && shouldDirectChase)
        {
            // Guard: respect grabbed/throw/dead/attack/hit/fall states
            if (isGrabbed || isBeingThrown) return;
            if (Char == null || player == null) return;
            if (state == State.Dead || state == State.Attack || state == State.Hit || state == State.Fall) return;


            // Force chase animation + fixed speed = 1
            const string CHASE_ANIM = "Attack2";
            const float CHASE_SPEED = 2.5f;

            if (currentMoveAnim != CHASE_ANIM)
            {
                currentMoveAnim = CHASE_ANIM;
                PlayAnim(CHASE_ANIM, true);
            }

            Vector3 _targetPos = targetPos;
            Vector3 _direction = (_targetPos - Char.position).normalized;
            lastDirection = _direction;
            Char.position += _direction * CHASE_SPEED * Time.deltaTime;
            // Nếu đã đến target, kết thúc chase
            if (Vector3.Distance(Char.position, targetPos) < 0.2f)
            {
                shouldDirectChase = false; // Tắt chase để tránh AttackArea hit khi boss đã dừng
                isStopping = true;
                patrolTimer = 0f;
                SwitchToRunState(enemyIdle);
            }
            // Check wall collision (avoid wall and set cooldown)
            if (!canCheckWall && CheckWallCollision())
            {
                AvoidWall();
                canCheckWall = true;
                StartCoroutine(CheckWallCollisionRoutine());
            }
            return;
        }


        float speed = SetMoveAnimationByTarget(targetPos);

        Vector3 direction = (targetPos - Char.position).normalized;
        lastDirection = direction;
        Char.position += direction * speed * Time.deltaTime;

        // Nếu đã đến target, giữ vị trí đó
        if (Vector3.Distance(Char.position, targetPos) < 0.2f)
        {
            isStopping = true;
            patrolTimer = 0f;
        }

        if (!canCheckWall && CheckWallCollision())
        {
            AvoidWall();
            canCheckWall = true;
            StartCoroutine(CheckWallCollisionRoutine());
        }
    }

    void PatrolRandomly()
    {
        if (isBeingThrown || isGrabbed) return;
        Vector3 direction = (randomTarget - Char.position).normalized;
        lastDirection = direction;

        // set animation based on random target
        float speed = SetMoveAnimationByTarget(randomTarget);

        Char.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(Char.position, randomTarget) < 0.2f || IsTargetOccupiedByOtherEnemy(randomTarget, this))
        {
            isStopping = true;
            patrolTimer = 0f;
        }

        if (!canCheckWall && CheckWallCollision())
        {
            AvoidWall();
            canCheckWall = true;
            StartCoroutine(CheckWallCollisionRoutine());
        }
    }

    public Vector3 GetRandomPositionInRect(float minX, float maxX, float minY, float maxY, float z = 0f)
    {
        if (isBeingThrown) return Char.position; // don't pick new targets while thrown
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector3(x, y, z);
    }


    public void SetRandomPatrolTarget() // ramdom target when start patrol
    {
        int attempts = 10;

        //if ((typeOfEnemy == TypeOfEnemy.EliteEnemy || typeOfEnemy == TypeOfEnemy.Boss) && player != null)
        //{
        //    for (int i = 0; i < attempts; i++)
        //    {
        //        float side = (Random.value < 0.5f) ? -1f : 1f;
        //        Vector3 candidate = new Vector3(
        //            player.position.x + side * rangeThrower + Random.Range(-0.5f, 0.5f),
        //            player.position.y + Random.Range(-0.8f, 0.8f),
        //            0f);
        //        // clamp vào biên bản đồ
        //        candidate.x = Mathf.Clamp(candidate.x, GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX);
        //        candidate.y = Mathf.Clamp(candidate.y, GamePlayManager.Instance.minPosY, GamePlayManager.Instance.maxPosY);
        //        if (Vector3.Distance(candidate, lastRandomTarget) > 1f && !IsTargetOccupiedByOtherEnemy(candidate))
        //        {
        //            randomTarget = candidate;
        //            return;
        //        }
        //    }
        //}

        for (int i = 0; i < attempts; i++)
        {
            Vector3 noise = GetRandomPositionInRect(GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX, GamePlayManager.Instance.minPosY, GamePlayManager.Instance.maxPosY, 0);
            noise.z = 0;
            if (Vector3.Distance(noise, lastRandomTarget) > 1f && !IsTargetOccupiedByOtherEnemy(noise))
            {
                randomTarget = noise;
                return;
            }
        }
    }

    public void AvoidWall() // random target when hit wall
    {
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomDirection = GetRandomPositionInRect(GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX, -2.8f, 1.4f, 0);
            randomDirection.z = 0;

            randomDirection.y = -randomDirection.y;
            Vector3 newTarget = /*Char.position +*/ randomDirection;
            if (Vector3.Distance(newTarget, lastRandomTarget) > 1f)
            {
                randomTarget = newTarget;
                return;
            }
        }
    }

    public void AvoidPlayer()
    {
        float rand = Random.value;
        if (rand < 0.7f)
        {
            // 80%: Né về vị trí cách player 1 đơn vị, cùng trục Y, phía hiện tại
            float direction = Char.position.x > player.position.x ? 1f : -1f;
            Vector3 newTarget = new Vector3(
                player.position.x + direction * 1f,
                Char.position.y,
                Char.position.z
            );

            // Đảm bảo không va chạm player
            if (Physics.OverlapSphere(newTarget, 1.2f, playerLayer).Length == 0)
            {
                randomTarget = newTarget;
                return;
            }
            // Nếu vị trí này bị chiếm, fallback sang random như bên dưới
        }

        // 20% còn lại hoặc fallback nếu vị trí trên bị chiếm
        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            float directionMultiplier = Char.position.x > player.position.x ? 1 : -1;
            Vector3 randomDirection = Random.insideUnitSphere * 3;
            randomDirection.z = 0;
            randomDirection.y = -randomDirection.y;
            randomDirection.x = Mathf.Abs(randomDirection.x) * directionMultiplier;
            Vector3 newTarget = Char.position + randomDirection;

            if (Physics.OverlapSphere(newTarget, 1.2f, playerLayer).Length == 0)
            {
                randomTarget = newTarget;
                return;
            }
        }
    } 

    bool CheckPlayerTooClose()
    {
        float distanceX = Mathf.Abs(Char.position.x - player.position.x);
        float distanceY = Mathf.Abs(Char.position.y - player.position.y);
        return distanceX <= 0.6f && distanceY <= 0.2f;
    }

    IEnumerator CheckPlayerCollisionRoutine()
    {
        if (isBeingThrown || isGrabbed) yield break;
        yield return new WaitForSeconds(0.5f);
        Vector3 targetPosition = new Vector3(
        player.position.x + (Char.position.x > player.position.x ? 1f : -1f),
        player.position.y,
        Char.position.z);

        // set animation for this micro-move
        float speed = SetMoveAnimationByTarget(targetPosition);

        Vector3 direction = (targetPosition - Char.position).normalized;
        Char.position += direction * (speed / 1.3f) * Time.deltaTime;

        if (Vector3.Distance(Char.position, targetPosition) <= 0.1f)
        {
            SwitchToRunState(enemyIdle);
        }
        isCheckingPlayer = false;
    }

    public void CheckAttack()
    {
        if (isBeingThrown) return;
        if (isAttack || isAttacking || !isActiveRun) return;

        // check player in front
        bool facingRight = Mathf.Abs(Mathf.DeltaAngle(Char.localEulerAngles.y, 0f)) < 90f;
        bool playerInFront = facingRight ? (player.position.x > Char.position.x) : (player.position.x < Char.position.x);
        if (!playerInFront) return;

        float distanceX = Mathf.Abs(Char.position.x - player.position.x);
        float distanceY = Mathf.Abs(Char.position.y - player.position.y);

        

        //is boss
        if (typeOfEnemy == TypeOfEnemy.Boss &&( idEnemy == 0 || idEnemy == 1))
        {
            if (distanceX <= rangeThrower + 0.5f && distanceY <= 0.2f && distanceX >= rangeAttack + 0.35f && isEnableThrower) //distanceX <= 3.75f && distanceY <= 0.2f && distanceX >= 1.5f
            {
                if (!isAttack)
                {
                    isStopping = false;
                    stopTimer = 0f;
                    patrolTimer = 0f;
                    isPatrolling = false;

                    isAttack = true;
                    //SwitchToRunState(enemyIdle);
                    enemyAttack.nameBossAttack = "Attack1";
                    //SwitchToRunState(enemyAttack);
                    if (state != State.Idle)
                    {
                        SwitchToRunState(enemyIdle);
                    }
                    if (state != State.Idle)
                    {
                        // immediately request attack state
                        SwitchToRunState(enemyAttack);
                    }
                    else
                    {
                        // if already Idle, also start Attack immediately
                        SwitchToRunState(enemyAttack);
                    }

                    return;
                }
            }
            //else if(distanceX < 1.5f && distanceY <= 0.2f)
            //{
            //    if (!isAttack)
            //    {
            //        isStopping = false;
            //        stopTimer = 0f;
            //        patrolTimer = 0f;
            //        isPatrolling = false;
            //        isAttack = true;
                  
            //        enemyAttack.nameBossAttack = "Attack2";
            //        //SwitchToRunState(enemyAttack);
            //        if (state != State.Idle)
            //        {
            //            SwitchToRunState(enemyIdle);
            //        }
            //        if (state != State.Idle)
            //        {
            //            // immediately request attack state
            //            SwitchToRunState(enemyAttack);
            //        }
            //        else
            //        {
            //            // if already Idle, also start Attack immediately
            //            SwitchToRunState(enemyAttack);
            //        }
            //    }
            //}
            //return;
        }
        //else if (typeOfEnemy == TypeOfEnemy.Boss && idEnemy == 2 && distanceX <= 1.2f && distanceY <= 0.2f)
        //{
        //    if (!isAttack)
        //    {
        //        isStopping = false;
        //        stopTimer = 0f;
        //        patrolTimer = 0f;
        //        isPatrolling = false;
        //        isAttack = true;
        //        enemyAttack.nameBossAttack = "Attack1";
        //        //SwitchToRunState(enemyAttack);
        //        if (state != State.Idle)
        //        {
        //            SwitchToRunState(enemyIdle);
        //        }
        //        if (state != State.Idle)
        //        {
        //            // immediately request attack state
        //            SwitchToRunState(enemyAttack);
        //        }
        //        else
        //        {
        //            // if already Idle, also start Attack immediately
        //            SwitchToRunState(enemyAttack);
        //        }
        //    }

        //    return;
        //}

        //is elite enemy
        if(typeOfEnemy == TypeOfEnemy.EliteEnemy)
        {
            if (distanceX <= rangeThrower + 0.5f && distanceY <= 0.35f && distanceX > rangeAttack + 0.35f && isEnableThrower)
            {
                if (!isAttack)
                {
                    isStopping = false;
                    stopTimer = 0f;
                    patrolTimer = 0f;
                    isPatrolling = false;
                    isEnableThrower = false;
                    enemyAttack.isEliteEnemyAttack = true;
                    isAttack = true;
                    //SwitchToRunState(enemyAttack);
                    if (state != State.Idle)
                    {
                        SwitchToRunState(enemyIdle);
                    }
                    if (state != State.Idle)
                    {
                        // immediately request attack state
                        SwitchToRunState(enemyAttack);
                    }
                    else
                    {
                        // if already Idle, also start Attack immediately
                        SwitchToRunState(enemyAttack);
                    }
                    return;
                }
            }
        }

        // Xác định vị trí tấn công lý tưởng của enemy này (bên trái hoặc phải player)
        Vector3 myTarget = Char.position.x < player.position.x
            ? player.position + Vector3.left * rangeAttack
            : player.position + Vector3.right * rangeAttack;

        // Nếu enemy đã ở rất gần player (<= 0.15f) nhưng vị trí tấn công chưa bị chiếm, hãy di chuyển đến vị trí tấn công
        if (distanceX <= 0.15f && distanceY <= 0.2f)
        {
            if (!IsTargetOccupiedByOtherEnemy(myTarget, this))
            {
                // Di chuyển đến vị trí tấn công lý tưởng
                Vector3 direction = (myTarget - Char.position).normalized;
                float speed = SetMoveAnimationByTarget(myTarget);
                Char.position += direction * speed * Time.deltaTime;
            }
            else
            {
                // Nếu vị trí đã bị enemy khác chiếm, mới lùi lại
                if (!isCheckingPlayer)
                {
                    isCheckingPlayer = true;
                    StartCoroutine(CheckPlayerCollisionRoutine());
                }
            }
            return;
        }

        // Điều kiện tấn công
        if (distanceX <= (rangeAttack+ 0.2f) && distanceY <= 0.2f)
        {
            bool slotFree = !IsTargetOccupiedByOtherEnemy(myTarget, this) ||
                            Vector3.Distance(Char.position, myTarget) < 0.2f;

            if (slotFree)
            {
                if (!isAttack)
                {
                    isStopping = false;
                    stopTimer = 0f;
                    patrolTimer = 0f;
                    isPatrolling = false;
                    if (typeOfEnemy == TypeOfEnemy.Boss)
                    {
                        if (idEnemy == 0 || idEnemy == 1)
                        {
                            enemyAttack.nameBossAttack = "Attack2";
                        }
                        else
                        {
                            enemyAttack.nameBossAttack = "Attack1";
                        }
                    }
                    isAttack = true;
                    if (state != State.Idle)
                    {
                        SwitchToRunState(enemyIdle);
                    }
                    if (state != State.Idle)
                    {
                        // immediately request attack state
                        SwitchToRunState(enemyAttack);
                    }
                    else
                    {
                        // if already Idle, also start Attack immediately
                        SwitchToRunState(enemyAttack);
                    }
                }
            }
        }
    }

    void HandleAttackEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (typeOfEnemy == TypeOfEnemy.Boss)
        {
            if (idEnemy == 0)
            {
                if (e.Data.Name == "hit" && enemyAttack.nameBossAttack == "Attack1")
                {
                    StartCoroutine(ShootBulletsDelayed(3, 0.05f));
                }
                else if (e.Data.Name == "hit" && enemyAttack.nameBossAttack == "Attack2")
                {
                    SetAttack(idEnemy);
                }
            }
            else if (idEnemy == 1)
            {
                if (e.Data.Name == "hit" && enemyAttack.nameBossAttack == "Attack1")
                {
                    GameObject _knife = ObjectPooler.Instance.SpawnFromPool("Knife", posKnife.position, transform.rotation);
                    _knife.GetComponent<KnifeBoss>().centerPos = Char.transform;
                    _knife.GetComponent<KnifeBoss>().direction = Char.transform.rotation.y < 0 ? -1 : 1;
                }
                else if (e.Data.Name == "hit" && enemyAttack.nameBossAttack == "Attack2")
                {
                    SetAttack(idEnemy);
                }
            }
            else if (idEnemy == 2)
            {
                if (e.Data.Name == "hit" && enemyAttack.nameBossAttack == "Attack1")
                {
                    SetAttack(idEnemy);
                    GamePlayManager.Instance._CameraFollow.Shake();
                    int dir = Char.transform.rotation.y < 0 ? -1 : 1;
                    GameObject go = ObjectPooler.Instance.SpawnFromPool("WaveBoss", posWave.transform.position, Quaternion.Euler(60, 0, -90 * dir));
                    go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, 0);
                    go.GetComponent<WaveBoss>().direction = dir;
                }

                //todo handle other attack event attack 2 of enemey id 2
            }
        }
        else if (typeOfEnemy == TypeOfEnemy.EliteEnemy)
        {
            if (e.Data.Name == "Shoot" || e.Data.Name == "shoot")
            {
                if (idEnemy == 2)
                {
                    GameObject bomb = ObjectPooler.Instance.SpawnFromPool("Bomb", posThrower.position, transform.rotation);
                    BomEnemy bomEnemy = bomb.GetComponent<BomEnemy>();
                    int dir = Char.transform.rotation.y < 0 ? -1 : 1;
                    bomEnemy.Throw(throwDir: new Vector2(dir, 0f), throwSpeed: 3f, heightForce: 6f);
                }
                else if (idEnemy == 10)
                {
                    GameObject molotov = ObjectPooler.Instance.SpawnFromPool("Molotov", posThrower.position, transform.rotation);
                    BomEnemy bomEnemy = molotov.GetComponent<BomEnemy>();
                    int dir = Char.transform.rotation.y < 0 ? -1 : 1;
                    bomEnemy.Throw(throwDir: new Vector2(dir, 0f), throwSpeed: 4f, heightForce: 6f);
                }
                else if (idEnemy == 11)
                {
                    GameObject bone = ObjectPooler.Instance.SpawnFromPool("Bone", posThrower.position, transform.rotation);
                    BomEnemy bomEnemy = bone.GetComponent<BomEnemy>();
                    int dir = Char.transform.rotation.y < 0 ? -1 : 1;
                    bomEnemy.Throw(throwDir: new Vector2(dir, 0f), throwSpeed: 8f, heightForce: 3f);
                }
                else if (idEnemy == 4 || idEnemy == 5)
                {
                    GameObject spoon = ObjectPooler.Instance.SpawnFromPool("Spoon", posThrower.position, transform.rotation);
                    BomEnemy bomEnemy = spoon.GetComponent<BomEnemy>();
                    int dir = Char.transform.rotation.y < 0 ? -1 : 1;
                    bomEnemy.Throw(throwDir: new Vector2(dir, 0f), throwSpeed: 7f, heightForce: 3f);
                }
            }
            else if (e.Data.Name == "Hit" || e.Data.Name == "hit")
            {
                SetAttack(idEnemy);
            }
            else if (e.Data.Name == "TongueHit")
            {
                attackArea.StartEnemyTongueHit(dame);
            }
            else if (e.Data.Name == "end_hit" && idEnemy == 6)
            {
                attackArea.EndEnemyTongueHit();
            }
        }
        else
        {
            if (e.Data.Name == "Hit" || e.Data.Name == "hit")
            {
                SetAttack(idEnemy);
            }
        }
    }

    IEnumerator ShootBulletsDelayed(int count, float delayBetween)
    {
        for (int i = 0; i < count; i++)
        {
            ObjectPooler.Instance.SpawnFromPool("Bullet", posBullet.position, transform.rotation);
            if (i < count - 1)
                yield return new WaitForSeconds(delayBetween);
        }
    }

    public void SetAttack(int id)
    {
        attackArea.SetAttack(dame, id);
    }

    public IEnumerator SetTimeHit()
    {
        yield return new WaitForSeconds(HitTimeout);
        currentHitIndex = 0;
    }
    public void SetHit(float dameHit, bool isMaxHit = false)
    {
        if (state == State.Dead)
            return;
        float hp = Hp - dameHit < 0 ? 0 : Hp - dameHit;
        SpawnTxtHit(dameHit);
        if (hp <= 0)
        {
            SetDead();
            return;
        }
        currentHitIndex++;
        if(typeOfEnemy == TypeOfEnemy.Boss)
        {
            FlashHit();
        }

        if (isMaxHit && typeOfEnemy != TypeOfEnemy.Boss)
        {
            GamePlayManager.Instance._CameraFollow.Shake2();
            ApplyMaxHitKnockback();
        }
        else
        {
            if (state != State.Fall/* && typeOfEnemy != TypeOfEnemy.Boss*/)// demo for test boss not hit state
                SwitchToRunState(enemyHit);
        }
        
        if (coroutine == null)
            coroutine = StartCoroutine(SetTimeHit());
        Hp = hp;
        fillBar.SetNewHp(Hp);
    }
    
    private void ApplyMaxHitKnockback()
    {
        if (state == State.Dead || isGrabbed || rb == null)
            return;

        if (state != State.Fall)
        {
            SwitchToRunState(enemyFall);
        }
    }
    public void SpawnTxtHit(float dame)
    {
        //GameObject txt = Instantiate(_prfTxtHit, _pointTxtHit.transform.position, Quaternion.identity);
        GameObject txt = ObjectPooler.Instance.SpawnFromPool("Text", _pointTxtHit.transform.position, Quaternion.identity);
        txt.GetComponent<TxtHit>().SetTxt(dame, true);
    }
    public void SetUltiPlayer()
    {
        SwitchToRunState(enemyIdle);
    }
    public void SetDead()
    {
        DropCoin();
        // Force state to Dead and enter EnemyDead state immediately to stop AI
        if (stateManager != null)
            stateManager.Exit();
        state = State.Dead;
        stateManager = enemyDead;
        stateManager.Enter();
        GamePlayManager.Instance._CameraFollow.Shake2();
        GamePlayManager.Instance.CheckEnemyDead();
        Vector2 upwardDirection = new Vector2(0, 0.85f);
        Vector3 jumpDirection = player.transform.right;
        float horizontalDirection0 = player.transform.rotation.y != 0 ? -1 : 1;
        float horizontalDirection1 = player.transform.position.x > Char.transform.position.x ? -1 : 1;
        float horizontalDirection = 0;
        if (isGetHitStrengthMax)
        {
            horizontalDirection = horizontalDirection1;
        }
        else
        {
            horizontalDirection = horizontalDirection0;
        }
        Vector2 moveDirection = new Vector2(horizontalDirection, upwardDirection.y).normalized;
        Vector2 targetPosition = (Vector2)transform.parent.position + moveDirection * 3f;

        // prevent Movement() and other AI from modifying position while the tween runs
        isBeingThrown = true;

        // stop physics to avoid physics pushing back
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // disable physics simulation for the duration
        }
        if(typeOfEnemy == TypeOfEnemy.Boss)
        {
            //to do handle boss dead : effect, sound ...
        }
        else
        {
            transform.parent.DOMove(targetPosition, 0.6f).SetEase(Ease.Linear)
           .OnComplete(() =>
           {
               // mark thrown finished and deactivate
               isBeingThrown = false;
               // keep physics disabled since deactivating
               if (rb != null)
               {
                   rb.simulated = false;
               }

               GamePlayManager.Instance._CameraFollow.Shake();
               Char.gameObject.SetActive(false);
           });
        }
    }
    public void DropCoin()
    {
        int number = Random.Range(3, 6);
        for (int i = 0; i < number; i++)
        {
            //Instantiate(prfCoin, Char.position, Quaternion.identity);
            ObjectPooler.Instance.SpawnFromPool("Coin", Char.position, Quaternion.identity);
        }
    }
    public void ResetState()
    {
        SwitchToRunState(enemyIdle);
    }


    public void SwitchToRunState(EnemyStateMachine enemy)
    {
        if (state == State.Dead) return;
        if (stateManager == enemy)
        {
            return;
        }

        // If we're switching INTO Grabed, set isGrabbed immediately to block other callers
        if (enemy == enemyGrabed)
        {
            if (!isGrabbed)
            {
                isGrabbed = true;
            }
        }
        // Prevent other systems from overwriting Grabed state while enemy is grabbed
        if (isGrabbed)
        {
            // allow transition only to states that release the grab (Fall/Dead) or keep Grabed
            if (enemy != enemyGrabed && enemy != enemyFall && enemy != enemyDead)
            {

                return;
            }
        }

        if (stateManager != null)
            stateManager.Exit();
        stateManager = enemy;
        stateManager.Enter();
    }
    void OnDrawGizmos()
    {
        if (Char == null) return;

        Vector3 startPos = Char.position + Vector3.up * 0.3f;
        Vector3 direction = (randomTarget - Char.position).normalized;

        // Vẽ đường ray từ vị trí nâng cao hơn
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(startPos, direction * patrolRadius);

        // Vẽ đường tròn hiển thị phạm vi check va chạm
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(startPos, 0.5f);

        // Vẽ điểm target
        Gizmos.color = Color.greenYellow;
        Gizmos.DrawWireSphere(randomTarget, 0.5f);
    }

    // Forcefully set Grabbed state immediately (atomic) to avoid races
    public void ForceEnterGrabbed()
    {
        // Exit current state safely
        if (stateManager != null)
        {
            try { stateManager.Exit(); } catch { }
        }

        // Set flag early to block other transitions
        isGrabbed = true;

        // reset movement/attack timers/flags
        isStopping = false;
        stopTimer = 0f;
        patrolTimer = 0f;
        isAttack = false;
        isCheckingPlayer = false;

        // assign and enter grab state
        stateManager = enemyGrabed;
        if (stateManager != null)
            stateManager.Enter();
    }

    // Helper: check if a target position is occupied by another alive enemy
    private bool IsTargetOccupiedByOtherEnemy(Vector3 target, EnemyController ignore = null)
    {
        float minDist = 1f;

        for (int i = 0; i < s_AllEnemies.Count; i++)
        {
            var enemy = s_AllEnemies[i];
            if (enemy == null) continue;
            if (enemy == this || enemy == ignore || enemy.state == State.Dead) continue;
            if (enemy.Char == null) continue;
            if (Vector3.Distance(enemy.Char.position, target) < minDist)
                return true;
        }
        return false;
    }

    // Wall collision helpers
    private bool CheckWallCollision()
    {
        if (Char == null) return false;
        Vector3 startPos = Char.position + Vector3.up * 0.3f;
        return Physics2D.OverlapCircle(startPos, 0.5f, wallLayer) != null;
    }

    IEnumerator CheckWallCollisionRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        canCheckWall = false;
    }

    // Ensure facing is correct based on player position
    private void UpdateEnemyRotation()
    {
        if (player == null || Char == null || state == State.Grabed) return;
        float yRotation = player.position.x > Char.position.x ? 0f : 180f;
        //float yRotation = player.position.x > Char.position.x ? 0f : 180f;
        Char.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    float gravity = -22f;
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
}
