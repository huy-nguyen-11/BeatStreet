using DG.Tweening;
using UnityEngine;

public class PlayerUlti : PlayerStateManager
{
    public PlayerUlti(PlayerController player) : base(player) { }
    public override void Enter()
    {
        //playerController.animator.Play("Ulti");
        playerController.state = PlayerController.State.Ulti;
        AudioBase.Instance.AudioPlayer(8);
        UltiVal();
    }
    public void UltiVal()
    {
        switch (playerController.id)
        {
            case 0:
                playerController.animator.Play("Walk");
                playerController.Char.DOMoveY(GamePlayManager.Instance._Enemy.transform.position.y, 0.5f);
                playerController.Char.DOMoveX(GamePlayManager.Instance._Enemy.transform.position.x
                + (playerController.Char.position.x < GamePlayManager.Instance._Enemy.transform.position.x ? -2.5f : 2.5f), 0.5f).OnComplete(() =>
                {
                    playerController.transform.rotation = Quaternion.Euler(0, playerController.Char.position.x
                        < GamePlayManager.Instance._Enemy.transform.position.x ? 0 : 180, 0);
                    playerController.animator.Play("ChangeUlti");
                    playerController.AnimUlti.SetActive(true);
                    playerController.Char.DOMoveX(GamePlayManager.Instance._Enemy.Char.position.x
            + (playerController.Char.position.x < GamePlayManager.Instance._Enemy.transform.position.x ? -0.5f : 0.5f), 0.45f).OnComplete(() =>
            {
                AudioBase.Instance.AudioPlayer(3);
                GamePlayManager.Instance.SetStopFollowCamera();
                playerController.transform.DOMoveY(playerController.transform.position.y + 15f, 0.9f)
                .SetLoops(2, LoopType.Yoyo);
                GamePlayManager.Instance._Enemy.transform.DOMoveY(GamePlayManager.Instance._Enemy.transform.position.y + 15f, 0.9f)
                .SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    AudioBase.Instance.AudioPlayer(6);
                    AudioBase.Instance.AudioPlayer(10);
                    GamePlayManager.Instance.isCheckUlti = false;
                    GamePlayManager.Instance._Enemy.SetHit(playerController.Dame * 15);
                    playerController.SwitchToRunState(playerController.playerIdle);
                });
                GamePlayManager.Instance.Invoke(nameof(GamePlayManager.Instance.SetFollowCamera), 2f);
            }).SetDelay(1f);
                });
                break;
            case 1:
                playerController.animator.Play("Walk");
                playerController.Char.DOMoveY(GamePlayManager.Instance._Enemy.transform.position.y, 0.5f);
                playerController.Char.DOMoveX(GamePlayManager.Instance._Enemy.transform.position.x
                + (playerController.Char.position.x < GamePlayManager.Instance._Enemy.transform.position.x ? -2.5f : 2.5f), 0.5f).OnComplete(() =>
                {
                    playerController.transform.rotation = Quaternion.Euler(0, playerController.Char.position.x
                        < GamePlayManager.Instance._Enemy.transform.position.x ? 0 : 180, 0);
                    playerController.animator.Play("ChangeUlti");
                    playerController.AnimUlti.SetActive(true);
                    playerController.Char.DOMoveX(GamePlayManager.Instance._Enemy.Char.position.x
            + (playerController.Char.position.x < GamePlayManager.Instance._Enemy.transform.position.x ? -0.5f : 0.5f), 0.45f).OnComplete(() =>
            {
                AudioBase.Instance.AudioPlayer(3);
                GamePlayManager.Instance.SetStopFollowCamera();
                playerController.transform.DOMoveY(playerController.transform.position.y + 15f, 0.9f)
                .SetLoops(2, LoopType.Yoyo);
                GamePlayManager.Instance._Enemy.transform.DOMoveY(GamePlayManager.Instance._Enemy.transform.position.y + 15f, 0.9f)
                .SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    AudioBase.Instance.AudioPlayer(6);
                    AudioBase.Instance.AudioPlayer(10);
                    GamePlayManager.Instance.isCheckUlti = false;
                    GamePlayManager.Instance._Enemy.SetHit(playerController.Dame * 15);
                    playerController.SwitchToRunState(playerController.playerIdle);
                });
                GamePlayManager.Instance.Invoke(nameof(GamePlayManager.Instance.SetFollowCamera), 2f);
            }).SetDelay(1f);
                });
                break;
            case 2:
                break;
            default:
                break;
        }
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        playerController.AnimUlti.SetActive(false);
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
