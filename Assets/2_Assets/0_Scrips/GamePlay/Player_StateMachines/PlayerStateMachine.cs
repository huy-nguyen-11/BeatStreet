using UnityEngine;

public abstract class PlayerStateManager
{
    protected PlayerController playerController;
    public PlayerStateManager(PlayerController player)
    {
        playerController = player;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Exit();
    public abstract void OnTriggerEnter(Collider2D collision);
    public abstract void OnTriggerStay(Collider2D collision);
    public abstract void OnTriggerExit(Collider2D collision);
    public abstract void OnCollisionEnter2D(Collision2D collision);
}
