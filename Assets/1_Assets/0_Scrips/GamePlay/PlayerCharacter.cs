using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        Attack,
        Dead,
        Change,
        Hit,
        Punch,
        SpeedUp,
        StandUp,
        Throw,
        Walk,
        Wingame,
        Combo1,
        Combo3,
        Skill1,
        Skill2,
        Ulti,
    }
    public State state = State.Idle;
    public int id;
    public RuntimeAnimatorController[] _anims;
    public FillBarPlayer fillBar;
    [SerializeField] public Animator animator;
    public float Hp;
    public float Mana;
    public float Dame;
    public List<string> keyAnim = new List<string>() { "Idle","Run","Walk","Jump","JumpKick","SpeedUp",
        "StandUp","Hit","Punch","Change","Throw","Combo1","Combo2","Combo3","Skill1","Skill2","Ulti"};
}
