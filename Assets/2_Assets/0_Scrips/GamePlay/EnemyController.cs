using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : EnemyCharacter
{
    public Transform player;
    public PlayerController playerController;
    DataManager dataManager;
    public TextMeshPro txtLevel;
    public Transform Char;
    public Rigidbody2D rb;
    public AttackArea attackArea;
    public bool isBoss;
    public int Level;
    public int idEnemy;
    public float Hp = 3;
    public float dame = 3;
    private Vector3 directionToPlayer;
    private float zigZagAmplitude;
    private float zigZagFrequency;
    private float nextChangeTime;
    public float raycastDistance = 0.25f;
    public LayerMask playerLayer;
    public EnemyStateMachine stateManager;
    public EnemyRun enemyRun;
    public EnemyIdle enemyIdle;
    public EnemyHit enemyHit;
    public EnemyAttack enemyAttack;
    public EnemyDead enemyDead;
    public EnemyFall enemyFall;
    public GameObject prfCoin;
    // Hit
    [SerializeField] Transform _pointTxtHit;
    [SerializeField] GameObject _prfTxtHit;
    public int currentHitIndex = 0;
    public float HitTimeout = 5.0f;
    public bool isCheckAnimAttack = false;

    public Coroutine coroutine;
    //public Coroutine coroutineAttack;

    // Enemy AI
    public float patrolRadius = 3f;
    public float moveSpeed = 2f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 2f;
    public bool isWall;
    public bool canCheckWall;
    public bool isAttack;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    private Vector3 lastDirection;
    public Vector3 randomTarget;
    private Vector3 lastRandomTarget;
    private bool isTooClosePlayer = false;
    private bool isCheckingPlayer = false;

    private float patrolDuration = 1.5f;
    private float patrolTimer = 0f; 
    private float stopTimer = 0f; 
    public bool isStopping = false;
    //private float stopDuration = 1.25f;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.transform.parent;
        enemyIdle = new EnemyIdle(this);
        enemyRun = new EnemyRun(this);
        enemyHit = new EnemyHit(this);
        enemyFall = new EnemyFall(this);
        enemyAttack = new EnemyAttack(this);
        enemyDead = new EnemyDead(this);
    }
    void Start()
    {
        dataManager = DataManager.Instance;
        Char = transform.parent.GetComponent<Transform>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
        stateManager = enemyIdle;
        stateManager.Enter();
        Level = dataManager.dataBase.listModeLevelEnemyMaps[dataManager.LevelSelect].Mode[dataManager.LevelMode];
        Hp = dataManager.dataBase.listLevelEnemyUpgrades[idEnemy].HP[Level] * (isBoss ? 2f : 1f);
        dame = dataManager.dataBase.listLevelEnemyUpgrades[idEnemy].Dame[Level] * (isBoss ? 1.5f : 1f);
        fillBar.OnInit(Hp);
        SetLevel();
    }
    void Update()
    {
        stateManager.Update();
        if (state == State.Dead) return;
        if (playerController.IsDead) return;
        if (state != State.Idle && GamePlayManager.Instance.isCheckUlti)
            SwitchToRunState(enemyIdle);
        if (GamePlayManager.Instance.isCheckUlti) return;
        if (state != State.Hit && state != State.Fall && state != State.Attack)
            CheckAttack();
    }

    private void FixedUpdate()
    {
        fillBar.transform.rotation = Quaternion.Euler(new Vector3(0, transform.position.y, 0));
        stateManager.FixedUpdate();
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

    private void SeparateFromOtherEnemies()
    {
        float minDist = 1f; // Khoảng cách tối thiểu giữa các enemy
        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            if (enemy == this || enemy.state == State.Dead) continue;
            float dist = Vector3.Distance(Char.position, enemy.Char.position);
            if (dist < minDist && dist > 0.01f)
            {
                // Đẩy enemy này ra xa enemy khác
                separation += (Char.position - enemy.Char.position).normalized * (minDist - dist);
                count++;
            }
        }

        if (count > 0)
        {
            Char.position += separation / count * 0.7f;
        }
    }

    public void Movement()
    {
        //UpdateEnemyRotation(); 
        if (player == null) return;

        if (isStopping)
        {
            stopTimer += Time.deltaTime;
            float _stopDuration = 1f;
            if (stopTimer >= _stopDuration)
            {
                isStopping = false;
                stopTimer = 0f;
                isPatrolling = Random.value < 0.5f; //random 50% patrol, 50% move to player
                isAvoidingPlayer = false;
                if (isPatrolling)
                    SetRandomPatrolTarget();
            }
            SwitchToRunState(enemyIdle);
        }
        else
        {
            // Nếu enemy đang tránh player thì ưu tiên tránh
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

            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolDuration)
            {
                isStopping = true;
                patrolTimer = 0f;
            }
        }

        UpdateEnemyRotation();
        SeparateFromOtherEnemies();
    }

    private void MoveToPlayer()
    {
        float targetOffset = 0.5f;
        Vector3 leftTarget = player.position + Vector3.left * targetOffset;
        Vector3 rightTarget = player.position + Vector3.right * targetOffset;

        bool leftOccupied = IsTargetOccupiedByOtherEnemy(leftTarget);
        bool rightOccupied = IsTargetOccupiedByOtherEnemy(rightTarget);

        Vector3 targetPos;

        // Nếu enemy đang ở gần vị trí tấn công thì giữ vị trí đó, không chuyển sang tuần tra
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
            // Cả hai bên đều có enemy, chuyển sang tuần tra ngẫu nhiên
            isPatrolling = true;
            SetRandomPatrolTarget();
            PatrolRandomly();
            return;
        }

        Vector3 direction = (targetPos - Char.position).normalized;
        lastDirection = direction;
        Char.position += direction * moveSpeed * Time.deltaTime;

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
        Vector3 direction = (randomTarget - Char.position).normalized;
        lastDirection = direction;
        Char.position += direction * moveSpeed * Time.deltaTime;

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

    private bool IsTargetOccupiedByOtherEnemy(Vector3 target, EnemyController ignore = null)
    {
        float minDist = 1f;
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            if (enemy == this || enemy == ignore || enemy.state == State.Dead) continue;
            if (Vector3.Distance(enemy.Char.position, target) < minDist)
                return true;
        }
        return false;
    }

    public Vector3 GetRandomPositionInRect(float minX, float maxX, float minY, float maxY, float z = 0f)
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector3(x, y, z);
    }


    public void SetRandomPatrolTarget() // ramdom target when start patrol
    {
        int attempts = 10;
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
        //Debug.Log("ne player");
        //int attempts = 10;
        //for (int i = 0; i < attempts; i++)
        //{
        //    float directionMultiplier = Char.position.x > player.position.x ? 1 : -1;

        //    Vector3 randomDirection = Random.insideUnitSphere * 3;
        //    randomDirection.z = 0;
        //    randomDirection.y = -randomDirection.y;
        //    randomDirection.x = Mathf.Abs(randomDirection.x) * directionMultiplier;
        //    Vector3 newTarget = Char.position + randomDirection;

        //    if (Physics.OverlapSphere(newTarget, 1.2f, playerLayer).Length == 0)
        //    {
        //        randomTarget = newTarget;
        //        return;
        //    }
        //}
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
        return distanceX <= 0.6f && distanceY <= 1.1f;
    }

    IEnumerator CheckPlayerCollisionRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 targetPosition = new Vector3(
        player.position.x + (Char.position.x > player.position.x ? 1f : -1f),
        player.position.y,
        Char.position.z);

        Vector3 direction = (targetPosition - Char.position).normalized;
        Char.position += direction * moveSpeed / 1.3f * Time.deltaTime;

        if (Vector3.Distance(Char.position, targetPosition) <= 0.1f)
        {
            SwitchToRunState(enemyIdle);
        }
        isCheckingPlayer = false;
    }

    bool CheckWallCollision()
    {
        Vector3 startPos = Char.position + Vector3.up * 0.3f;
        return Physics2D.OverlapCircle(startPos, 0.5f, wallLayer) != null;
    }

    float gravity = -29f;
    public float velocity = 0;
    public bool isCheckGravity;

    IEnumerator CheckWallCollisionRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        canCheckWall = false;
    }


    private void UpdateEnemyRotation()
    {
        float yRotation = player.position.x > Char.position.x ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void ProcessGravity()
    {
        isCheckGravity = transform.localPosition.y < 0;
        if (isCheckGravity && (velocity < 0 || transform.localPosition.y > 3))
        {
            velocity = -2f;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
            transform.localPosition += velocity * Time.deltaTime * Vector3.up;
        }
    }
    public void SetYPosition(float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
    public void CheckAttack()
    {
        float distanceX = Mathf.Abs(Char.position.x - player.position.x);
        float distanceY = Mathf.Abs(Char.position.y - player.position.y);

        // Xác định vị trí tấn công lý tưởng của enemy này (bên trái hoặc phải player)
        Vector3 myTarget = Char.position.x < player.position.x
            ? player.position + Vector3.left * 0.5f
            : player.position + Vector3.right * 0.5f;

        // Nếu enemy đã ở rất gần player (<= 0.15f) nhưng vị trí tấn công chưa bị chiếm, hãy di chuyển đến vị trí tấn công
        if (distanceX <= 0.15f && distanceY <= 0.2f)
        {
            if (!IsTargetOccupiedByOtherEnemy(myTarget, this))
            {
                // Di chuyển đến vị trí tấn công lý tưởng
                Vector3 direction = (myTarget - Char.position).normalized;
                Char.position += direction * moveSpeed * Time.deltaTime;
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
        if (distanceX <= 0.75f && distanceY <= 0.2f)
        {
            // Nếu enemy này là người chiếm vị trí tấn công (hoặc vị trí chưa bị chiếm)
            if (!IsTargetOccupiedByOtherEnemy(myTarget, this) ||
                Vector3.Distance(Char.position, myTarget) < 0.2f)
            {
                if (state != State.Idle)
                {
                    isAttack = true;
                    SwitchToRunState(enemyIdle);
                }
            }
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
    public void SetHit(float dameHit)
    {
        if (state == State.Dead)
            return;
        float hp = Hp - dameHit < 0 ? 0 : Hp - dameHit;
        SpawnTxtHit(dameHit);
        if (hp <= 0)
            SetDead();
        currentHitIndex++;
        if (state != State.Fall)
            SwitchToRunState(enemyHit);
        if (coroutine == null)
            coroutine = StartCoroutine(SetTimeHit());
        Hp = hp;
        fillBar.SetNewHp(Hp);
    }
    public void SpawnTxtHit(float dame)
    {
        GameObject txt = Instantiate(_prfTxtHit, _pointTxtHit.transform.position, Quaternion.identity);
        txt.GetComponent<TxtHit>().SetTxt(dame, true);
    }
    public void SetUltiPlayer()
    {
        SwitchToRunState(enemyIdle);
    }
    public void SetDead()
    {
        DropCoin();
        SwitchToRunState(enemyDead);
        GamePlayManager.Instance.CheckEnemyDead();
        Vector2 upwardDirection = new Vector2(0, 0.5f);
        Vector3 jumpDirection = player.transform.right;
        float horizontalDirection = player.transform.rotation.y != 0 ? -1 : 1;
        Vector2 moveDirection = new Vector2(horizontalDirection, upwardDirection.y).normalized;
        Vector2 targetPosition = (Vector2)transform.parent.position + moveDirection * 5;
        transform.parent.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
                 {
                     Char.gameObject.SetActive(false);
                 });
    }
    public void DropCoin()
    {
        int number = Random.Range(3, 6);
        for (int i = 0; i < number; i++)
        {
            Instantiate(prfCoin, Char.position, Quaternion.identity);
        }
    }
    public void ResetState()
    {
        SwitchToRunState(enemyIdle);
    }

    public void SwitchToRunState(EnemyStateMachine enemy)
    {
        if (state == State.Dead) return;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPos, 0.5f);

        // Vẽ điểm target
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(randomTarget, 0.5f);
    }
}
