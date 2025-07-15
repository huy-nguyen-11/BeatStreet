using UnityEngine;
public class PlayerAttack : PlayerStateManager
{
    public PlayerAttack(PlayerController player) : base(player) { }
    private float holdThreshold = 0.07f;
    Coroutine coroutine;

    public override void Enter()
    {
        playerController.state = PlayerController.State.Attack;
        playerController.rb.velocity = Vector2.zero;
        PlayComboAnimation();
        AudioBase.Instance.AudioPlayer(0);
    }
    public override void Update()
    {
    }

    private void PlayComboAnimation()
    {
        switch (playerController.comboAttack)
        {
            case 0:
                playerController.animator.Play("Combo1");
                ResetAnim("Combo1");
                break;
            case 1:
                if (playerController.id == 0)
                {
                    playerController.animator.Play("Combo1");
                    ResetAnim("Combo1");
                }
                else
                {
                    playerController.animator.Play("Combo2");
                    ResetAnim("Combo2");
                }
                break;
            case 2:
                AudioBase.Instance.AudioPlayer(3);
                if (playerController.id == 0)
                {
                    playerController.animator.Play("Combo2");
                    ResetAnim("Combo2");
                }
                else
                {
                    playerController.animator.Play("Combo3");
                    ResetAnim("Combo3");
                }
                break;
            case 3:
                playerController.animator.Play("Combo1");
                ResetAnim("Combo1");
                break;
            case 4:
                AudioBase.Instance.AudioPlayer(4);
                if (playerController.id == 0)
                {
                    playerController.animator.Play("Combo3");
                    ResetAnim("Combo3");
                }
                else
                {
                    playerController.animator.Play("Combo4");
                    ResetAnim("Combo4");
                }
                break;
            default:
                playerController.animator.Play("Combo1");
                ResetAnim("Combo1");
                break;
        }
        playerController.rb.velocity = Vector2.left * (playerController.transform.rotation.y != 0 ? 0.1f : -0.1f);
    }
    private void ResetAnim(string name)
    {
        coroutine = playerController.StartCoroutine(playerController.CheckAnimationAndTriggerEvent(name));
    }
    public override void Exit()
    {
        if (coroutine != null)
            playerController.StopCoroutine(coroutine);
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
