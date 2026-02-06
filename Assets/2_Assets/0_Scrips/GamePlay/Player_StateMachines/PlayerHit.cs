using UnityEngine;

public class PlayerHit : PlayerStateManager
{
    public PlayerHit(PlayerController player) : base(player) { }
    //[SerializeField] private float jumpHeight = 1f;
    //[SerializeField] private float jumpDistance = 1.65f;
    //[SerializeField] private float jumpDuration = 1f;
    //[SerializeField] private int jumpCount = 1;
    bool _isFall;
    Coroutine _coroutine;
    public override void Enter()
    {
        playerController.state = PlayerController.State.Hit;
        playerController.rb.linearVelocity = Vector2.zero;

        bool shouldFaceRight = !playerController.HitDirection;
        playerController.SetFacingDirection(shouldFaceRight);

        if (playerController.HitCount < 3)
        {
            playerController.PlayAnim2("Hit");
        }
        else
        {
            playerController.isFall = true;
            playerController.isImmortal = true;
            playerController.PlayAnim("Dead", false);
            playerController.velocity = 8;
            _coroutine = playerController.StartCoroutine(playerController.FallCoroutine());
            _isFall = true;
        }
    }
    public override void Update()
    {
        if (_isFall)
        {
            playerController.ProcessGravity();
            playerController.SetFall();
        }
    }

    public override void Exit()
    {
        _isFall = false;
        playerController.isFall = false;
        if (_coroutine != null)
        {
            playerController.StopCoroutine(_coroutine);
        }
    }
    public override void FixedUpdate()
    {
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
