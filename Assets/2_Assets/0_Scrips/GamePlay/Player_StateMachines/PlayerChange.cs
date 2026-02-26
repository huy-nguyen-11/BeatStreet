using System.Collections;
using UnityEngine;

public class PlayerChange : PlayerStateManager
{
    public PlayerChange(PlayerController player) : base(player) { }
    private float holdThreshold = 0.25f;
    Coroutine _coroutine;
    public override void Enter()
    {
        playerController.state = PlayerController.State.Change;
    
        playerController.PlayAnim("Strength", false);
        _coroutine = playerController.StartCoroutine(WaitForChangeAnimEnd());
        //playerController.PlayAnimHaveEffect("Strength", false, "Strength2");
        AudioBase.Instance.AudioPlayer(7);
        playerController.rb.linearVelocity = Vector2.zero;
    }
    public override void Update()
    {
        playerController.holdTime += Time.deltaTime;
    }
    public override void Exit()
    {
        playerController.holdTime = 0f;
  
        if (_coroutine != null)
        {
            playerController.StopCoroutine(_coroutine);
            _coroutine = null;
        }
        if(playerController.fxStrength != null)
            playerController.fxStrength.SetActive(false);
    }

    IEnumerator WaitForChangeAnimEnd()
    {
        yield return new WaitForSeconds(0.3f);
        if(playerController.fxStrength != null)
            playerController.fxStrength.SetActive(true);
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
