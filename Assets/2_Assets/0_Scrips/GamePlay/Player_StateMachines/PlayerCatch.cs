using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatch : PlayerStateManager
{
    public PlayerCatch(PlayerController player) : base(player) { }
    public override void Enter()
    {
        //playerController.animator.Play("Catch");
    }
    public override void Update()
    {

    }
    public void SetCatchEnemy()
    {
    }
    public override void Exit()
    {
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
