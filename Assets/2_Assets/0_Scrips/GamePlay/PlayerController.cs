using DG.Tweening;
using PinePie.SimpleJoystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerCharacter
{
    public static PlayerController Instance { get; private set; }
    DataManager dataManager;
    // Joystick
    public JoystickController joystick;
    public float speedThreshold;
    public Transform Char;
    private bool isGetJoy = false;
    // Attribute
    public List<float> _attributesPet = new List<float>();
    public List<float> _attributesItem = new List<float>();
    public Rigidbody2D rb;
    public AttackArea attackArea;
    public AttackArea attackSkill2;
    public AttackArea attackJumKickL;
    public AttackArea attackJumKickR;
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
    [SerializeField] private float detectionRange = 4f;
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
    private float maxTapTime = 0.25f;
    public float changeHoldTime = 0.3f;
    private Dictionary<int, Vector2> touchStartPositions = new Dictionary<int, Vector2>();
    private Vector2 touchStartPositionsRun;
    private Dictionary<int, float> touchStartTimes = new Dictionary<int, float>();
    public int idTounchRun;
    private float holdTimer;

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
        Char = transform.parent;
        rb = transform.parent.GetComponent<Rigidbody2D>();
        dataManager = DataManager.Instance;
        OnInit();
    }
    private void OnEnable()
    {
    }
    public void OnInit()
    {
        id = dataManager.idPlayer;
        animator.runtimeAnimatorController = _anims[id];
        stateManager = playerIdle;
        stateManager.Enter();
        SetAttributePet();
        countCancelDamage = (int)_attributesPet[17];
        Hp = DataManager.Instance.playerData[id].Hp + (DataManager.Instance.playerData[id].Hp * _attributesPet[0]);
        Mana = _attributesPet[8] > 100 * (_attributesPet[8] / 100) ? 1 : 0;
        Dame = DataManager.Instance.playerData[id].Dame + (DataManager.Instance.playerData[id].Dame * _attributesPet[1]);
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
        stateManager.Update();
        if (IsDead
            || state == State.Dead
            || state == State.Wingame
            || state == State.Ulti
            || GamePlayManager.Instance.isCheckUlti
            ) return;
        if (state == State.Hit) return;

        CheckTouchInput();
        //GetJoy();

        comboTimer += Time.deltaTime;
        HitTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
            ResetCombo();
        if (HitTimer > HitResetTime)
            ResetHit();
    }
    private void GetJoy()
    {
        //if (GamePlayManager.Instance.isCheckUlti
        //    && state != State.Skill1
        //    && state != State.Skill2) return;
        //Vector2 direction = joystick.Direction;
        //if (Mathf.Abs(joystick.Direction.x) > 0.25f || Mathf.Abs(joystick.Direction.y) > 0.25f
        //    && (state == State.Idle
        //    || state == State.SpeedUp
        //    || state == State.Change
        //    || state == State.Walk || state == State.Run) && !isJumping)
        //{
        //    if (Mathf.Abs(joystick.Direction.x) > speedThreshold || Mathf.Abs(joystick.Direction.y) > speedThreshold)
        //    {
        //        if (state != State.Run)
        //        {
        //            SwitchToRunState(playerRun);
        //        }

        //    }
        //    else
        //    {
        //        if (state != State.Walk)
        //        {
        //            SwitchToRunState(playerWalk);
        //        }

        //    }
        //}
        //else
        //{
        //    if (state != State.Idle && state != State.Change && state != State.SpeedUp)
        //    {
        //        SwitchToRunState(playerIdle);
        //    }
        //}
        if (GamePlayManager.Instance.isCheckUlti
       && state != State.Skill1
       && state != State.Skill2) return;

        Vector2 direction = joystick.Direction;
        float magnitude = direction.magnitude;

        // Trạng thái có thể xử lý di chuyển
        bool canMoveState = (state == State.Idle || state == State.SpeedUp || state == State.Change || state == State.Walk || state == State.Run);

        if (!isJumping && canMoveState)
        {
            // RUN nếu joystick mạnh
            if (magnitude > speedThreshold) // bạn có thể điều chỉnh ngưỡng này
            {
                //Debug.Log("Run");
                if (state != State.Run)
                {
                    Debug.Log("run"+ magnitude);
                    SwitchToRunState(playerRun);
                }
                    
            }
            // WALK nếu joystick vừa
            //else if (magnitude > 0.3f && magnitude < speedThreshold)
            //{
            //    if (state != State.Walk)
            //    {
            //        Debug.Log("Walk"+ magnitude);
            //        SwitchToRunState(playerWalk);
            //    }
                   
            //}
            // IDLE nếu joystick yếu (và không phải đang Idle sẵn)
            else
            {
                if (state != State.Idle)
                {
                    SwitchToRunState(playerIdle);
                    Debug.Log("Idle" + magnitude);
                }

            }
        }
       
    }

    //todo wlak or running
    public void SetMovePlayer()
    {
        if (IsDead)
            return;
        Vector2 direction = joystick.Direction;
        if (Mathf.Abs(joystick.Direction.x) > 0f)
            transform.rotation = Quaternion.Euler(new Vector3(0, joystick.Direction.x > 0 ? 0 : -180, 0));

        moveSpeed = (Mathf.Abs(joystick.Direction.x) > speedThreshold || Mathf.Abs(joystick.Direction.y) > speedThreshold) ? 3.5f : 1.75f;
        Vector2 movement = direction.normalized * moveSpeed;
        rb.velocity = movement;
    }


    private void CheckTouchInput()
    {
        Vector2 direction = joystick.Direction;
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
                        return;
                    GetJoy();
                    isGetJoy = true;
                    break;
                case TouchPhase.Stationary:
                    if (isJumping)
                        return;
                    if (touchStartPositions.ContainsKey(touchId))
                    {
                        holdTimer += Time.deltaTime;
                        if ((Mathf.Abs(joystick.Direction.x) <= 0.3f || Mathf.Abs(joystick.Direction.y) <= 0.3f)
                            && holdTimer > changeHoldTime && state != State.Change
                            && (state == State.Idle || state == State.Attack) && !isGetJoy)
                        {
                            holdTimer = 0f;
                            SwitchToRunState(playerChange);
                        }
                    }

                    if (direction.magnitude <= 0.3f && state != State.Change) // gần như không kéo
                    {
                        if (state != State.Idle)
                        {
                            SwitchToRunState(playerIdle);
                            rb.velocity = Vector2.zero; // ngắt di chuyển
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

                        if (Vector2.Distance(startPosition, endPosition) <= 0.2f) // 0.2f is tabThreshold
                        {
                            if (state == State.Change)
                            {
                                if (isCheckSkill2)
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
                                    if (!GamePlayManager.Instance.isCheckUlti
                                        //&& state != State.Attack
                                        && state != State.Jump
                                        && state != State.Skill2
                                        && state != State.Skill1)
                                    {
                                        //Debug.Log("attack1");
                                        //SwitchToRunState(playerAttack);
                                        attackQueue.Enqueue(1);
                                        ProcessAttackQueue();
                                    }
                                    else
                                    {
                                        if (state != State.Jump
                                            && state != State.Skill1
                                            && state != State.Skill2)
                                            SwitchToRunState(playerIdle);
                                    }
                                }
                            }
                            else if (state == State.SpeedUp)
                            {
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
                                //&& state != State.Attack
                                && state != State.Jump
                                && state != State.Skill2
                                && state != State.Skill1)
                            {
                                attackQueue.Enqueue(1); 
                                ProcessAttackQueue();
                                //Debug.Log("attack 2");
                                //SwitchToRunState(playerAttack);
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
                                if (delta.magnitude > swipeThreshold)
                                {

                                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && (state == State.Run || state == State.Walk) && !isJumping)
                                    {
                                        SpeedupDirection = delta.x > 0 ? true : false;
                                        SwitchToRunState(playerSpeedUp);
                                    }
                                    else if (delta.y > 0 && state != State.Jump && (state == State.Run || state == State.Walk))
                                    {
                                        isJumping = true;
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
        if (stateManager != null)
            stateManager.Exit();
        stateManager = player;
        stateManager.Enter();
    }
    public void SetAttack(int id)
    {
        if (id != 4)
        {
            if (id == 3)
            {
                attackArea.SetAttack(Dame * SetFatal(13), id);
            }
            else
                attackArea.SetAttack(Dame, id);
        }
        else
        {
            if (transform.rotation.y < 0)
                attackJumKickL.SetAttackSkill(Dame * (state == State.Jump ? SetFatal(11) : SetFatal(10)), id);
            else
                attackJumKickR.SetAttackSkill(Dame * (state == State.Jump ? SetFatal(11) : SetFatal(10)), id);
        }
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
    public void SetRunSkill2()
    {
        StartCoroutine(attackSkill2.SetAttackSkill2Player(Dame * SetFatal(12)));
    }
    public void SetUltiPlayer()
    {
        SwitchToRunState(playerUlti);
    }
    public void SetFalseChangeUlti()
    {
        AnimUlti.SetActive(false);
        if (id == 0)
            animator.Play("Summary");
        else if (id == 1)
            animator.Play("Ulti");
    }
    public void SetImmortal()
    {
        isImmortal = true;
    }
    public IEnumerator SetImmortalHitBox()
    {
        isFall = false;
        transform.GetComponent<SpriteRenderer>().DOFade(0, 0.05f).SetLoops(-1, LoopType.Yoyo);
        isImmortal = true;
        yield return new WaitForSeconds(_attributesPet[16]);
        isImmortal = false;
        transform.GetComponent<SpriteRenderer>().DOKill();
        transform.GetComponent<SpriteRenderer>().DOFade(1, 0);
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
                if (state != State.Jump && state != State.Skill1 && state != State.Skill2)
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
            rb.velocity = -Vector2.right * 5f;
        else
            rb.velocity = Vector2.right * 5f;
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
        isJumping = false;
        SwitchToRunState(playerIdle);
    }
    public IEnumerator CheckAnimationAndTriggerEvent(string name)
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        while (state.normalizedTime < 1f || !state.IsName(name))
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSecondsRealtime(0f);
        }
        ResetStatus();
    }
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}