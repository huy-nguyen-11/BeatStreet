using System.Collections;
using UnityEngine;

public class PlayerSkill1 : PlayerStateManager
{
    public PlayerSkill1(PlayerController player) : base(player) { }
    private Coroutine coroutine;
    public override void Enter()
    {
        playerController.animator.Play("Skill1");
        playerController.state = PlayerController.State.Skill1;
        GamePlayManager.Instance.SetMission(6, 1);
        AudioBase.Instance.AudioPlayer(0);
        AudioBase.Instance.AudioPlayer(4);
        SetSkill1();
    }
    public override void Update()
    {

    }
    public void SetSkill1()
    {
        float point = playerController.transform.rotation.y == 0 ? 4f : -4f;
        playerController.rb.velocity = Vector3.right * point;
        coroutine = playerController.StartCoroutine(ResetStatus());
    }
    IEnumerator ResetStatus()
    {
        yield return new WaitForSeconds(0.55f);
        playerController.ResetStatus();
    }
    public override void Exit()
    {
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
