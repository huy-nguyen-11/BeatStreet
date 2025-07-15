using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        Attack,
        Dead,
        Hit,
        Punch,
        Fall,
        Ulti,
    }
    public State state = State.Idle;
    public FillBarEnemy fillBar;
    [SerializeField] public Animator animator;
}
