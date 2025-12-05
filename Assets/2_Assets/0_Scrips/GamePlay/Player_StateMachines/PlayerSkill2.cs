using UnityEngine;

public class PlayerSkill2 : PlayerStateManager
{
    public PlayerSkill2(PlayerController player) : base(player) { }
    private float moveSpeed = 0.08f; // Tốc độ di chuyển
    private float moveDuration1 = 0.5f; // Thời gian di chuyển sang trái
    private float moveDuration2 = 1f; // Thời gian di chuyển sang trái
    private float moveTimer = 0f; // Bộ đếm thời gian
    private bool isReturning = false;
    private Vector3 moveDirection;
    private Coroutine _coroutine;
    Vector3 PlayerDirection;
    private float lastXPosition;

    bool isCheckRunSkillTurn;
    public override void Enter()
    {
        lastXPosition = playerController.Char.position.x;
        playerController.PlayAnim("Strength_Attack_Max", false);
        playerController.state = PlayerController.State.Skill2;
        GamePlayManager.Instance.SetMission(6, 1);
        //if (playerController.id == 0)
        //{
        //    moveTimer = 0;
        //    PlayerDirection = playerController.transform.rotation.y == 0 ? Vector3.right : Vector3.left;
        //    isCheckRunSkillTurn = true;
        //    playerController.SetRunSkill2();
        //}
        //else
        //{
        //    playerController.velocity = 8;
        //    _coroutine = playerController.StartCoroutine(playerController.JumpCoroutine());
        //    playerController.SetAttack(4);
        //}
        playerController.isCheckSkill2 = false;
    }
    public override void Update()
    {
        playerController.ProcessGravity();
    }
    public override void FixedUpdate()
    {
        if (playerController.id == 0 && isCheckRunSkillTurn)
            MoveLeftAndReturn();
    }

    private void MoveLeftAndReturn()
    {
        moveTimer += Time.deltaTime;
        if (!isReturning)
        {
            playerController.Char.position += PlayerDirection * moveSpeed;
            if (moveTimer >= moveDuration1)
            {
                PlayerDirection = playerController.transform.rotation.y == 0 ? Vector3.left : Vector3.right;
                isReturning = true;
            }
            if (playerController.transform.rotation.y != 0 && playerController.Char.position.x >= lastXPosition ||
                 playerController.transform.rotation.y == 0 && playerController.Char.position.x <= lastXPosition)
            {
                PlayerDirection = playerController.transform.rotation.y == 0 ? Vector3.left : Vector3.right;
                isReturning = true;
                moveTimer = 0.5f;
            }
            lastXPosition = playerController.Char.position.x;
        }
        else
        {
            playerController.Char.position += PlayerDirection * moveSpeed;
            if (moveTimer >= moveDuration2)
            {
                isReturning = false;
                moveTimer = 0f;
                playerController.ResetStatus();
            }
        }
    }

    public override void Exit()
    {
        if (_coroutine != null)
        {
            playerController.StopCoroutine(_coroutine);
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
