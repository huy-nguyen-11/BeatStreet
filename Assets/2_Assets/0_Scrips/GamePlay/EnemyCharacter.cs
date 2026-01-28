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
        ChasePlayer,
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

    // Play an animation on a specified Spine track so it layers above track 0 (e.g. track 1).
    // If loop == false, an empty animation is queued to clear the overlay after the animation duration.
    public Spine.TrackEntry PlayAnimOnTrack(string animName, int trackIndex = 1, bool loop = false)
    {
        if (skeletonAnimation == null) return null;
        var state = skeletonAnimation.AnimationState;
        if (state == null)
        {
            // fallback: set as main animation if state not ready
            skeletonAnimation.AnimationName = animName;
            return null;
        }

        Spine.TrackEntry entry = state.SetAnimation(trackIndex, animName, loop);
        if (!loop && entry != null && entry.Animation != null)
        {
            // queue an empty animation after the animation's duration so overlay is cleared
            float duration = entry.Animation.Duration;
            // AddEmptyAnimation(trackIndex, mixDuration, delay)
            state.AddEmptyAnimation(trackIndex, 0f, duration);
        }
        return entry;
    }
}
