using Spine.Unity;
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
        Grabed,
        Spawn
    }
    public State state = State.Idle;
    public FillBarEnemy fillBar;
    [SerializeField] public SkeletonAnimation skeletonAnimation;
    [SerializeField] public string wakeUpAnim;

    public void PlayAnim(string animName, bool loop = true)
    {
        if (skeletonAnimation != null)
        {
            // Use AnimationState to set the animation on track 0
            if (skeletonAnimation.AnimationState != null)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            }
            else
            {
                // Fallback: set property which will apply when initialized
                skeletonAnimation.AnimationName = animName;
            }
        }
    }
}
