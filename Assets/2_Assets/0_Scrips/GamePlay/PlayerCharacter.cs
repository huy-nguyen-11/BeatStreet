using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        Catch,
        Hit,
        Punch,
        SpeedUp,
        StandUp,
        Walk,
        Wingame,
        Skill1,
        Skill2,
        Ulti,
    }
    public State state = State.Idle;
    public int id;
    public RuntimeAnimatorController[] _anims;
    public FillBarPlayer fillBar;
    [SerializeField] public Animator animator;
    [SerializeField] public SkeletonAnimation skeletonAnimation;
    public float Hp;
    public float Mana;
    public float Dame;
    public List<string> keyAnim = new List<string>() { "Idle","Run","Walk","Jump","JumpKick","SpeedUp",
        "StandUp","Hit","Punch","Change","Throw","Combo1","Combo2","Combo3","Skill1","Skill2","Ulti"};

     //Play an animation using Spine if available; otherwise fallback to Animator
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
        else if (animator != null)
        {
            animator.Play(animName);
        }
    }

    // Play animation on Spine and attach event handler. onEnd will be called once when either
    //  - a Spine event with name "Attack_end" / "End_attack" / "End_<animName>" fires for this TrackEntry
    //  - or the entry is not interrupted and a fallback timeout expires (one loop duration)
    //public void PlayAnimWithEventHandler(string animName, bool loop, Action<TrackEntry> onEnd)
    //{
    //    if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
    //    {
    //        var animState = skeletonAnimation.AnimationState;
    //        Spine.TrackEntry entry = animState.SetAnimation(0, animName, loop);

    //        if (entry == null)
    //        {
    //            return;
    //        }

    //        bool eventFired = false;
    //        Spine.AnimationState.TrackEntryEventDelegate eventHandler = null;

    //        eventHandler = (trackEntry, e) =>
    //        {
    //            try
    //            {
    //                if (trackEntry != entry) return;
    //                if (e == null || e.Data == null) return;
    //                string evtName = e.Data.Name;
    //                if (evtName == "Attack_end" || evtName == "End_attack" || evtName == ("End_" + animName))
    //                {
    //                    if (eventFired) return;
    //                    eventFired = true;
    //                    // Unsubscribe
    //                    animState.Event -= eventHandler;
    //                    onEnd?.Invoke(entry);
    //                }
    //            }
    //            catch { }
    //        };

    //        // subscribe to event timelines
    //        animState.Event += eventHandler;

    //        // start a coroutine to wait for completion fallback
    //        float duration = entry != null && entry.Animation != null ? entry.Animation.Duration : 0f;
    //        // if loop, duration may not indicate intent; still use event first, else wait one loop
    //        float waitTime = Mathf.Max(0.05f, duration);
    //        StartCoroutine(WaitForEntryTimeout(entry, waitTime, () =>
    //        {
    //            if (eventFired) return;
    //            // ensure same entry still active
    //            var current = animState.GetCurrent(0);
    //            if (current != entry) return; // interrupted
    //            // unsubscribe event
    //            animState.Event -= eventHandler;
    //            eventFired = true;
    //            onEnd?.Invoke(entry);
    //        }));

    //        //return;
    //    }
    //}

    //private IEnumerator WaitForEntryTimeout(TrackEntry entry, float waitTime, Action onTimeout)
    //{
    //    float timer = 0f;
    //    while (timer < waitTime)
    //    {
    //        if (entry == null) yield break;
    //        timer += Time.deltaTime * (entry.TimeScale);
    //        yield return null;
    //    }
    //    onTimeout?.Invoke();
    //}

    public void PlayAnimWithEventHandler(string animName, bool loop,
    System.Action<Spine.TrackEntry> onEventFired = null)
    {
        if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
        {
            Spine.TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);

            if (onEventFired != null && entry != null)
            {
   
                Spine.AnimationState.TrackEntryEventDelegate handler = null;
                 handler = (trackEntry, e) =>
                {
                    try
                    {
                        if (trackEntry == entry && e != null && e.Data != null)
                        {
                            if (e.Data.Name == "Attack_end" || e.Data.Name == "End_attack")
                            {
                                onEventFired?.Invoke(entry);
                                // Unsubscribe sau khi fire
                                skeletonAnimation.AnimationState.Event -= handler;
                            }
                        }
                    }
                    catch { }
                };

                skeletonAnimation.AnimationState.Event += handler;
            }
        }
        else if (animator != null)
        {
            animator.Play(animName);
        }
    }

    // Get current spine track entry for track 0, or null
    public Spine.TrackEntry GetCurrentSpineTrackEntry()
    {
        if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
        {
            return skeletonAnimation.AnimationState.GetCurrent(0);
        }
        return null;
    }

    // Coroutine helper to wait until an animation finishes (supports Spine and Animator)
    public IEnumerator WaitForAnimationEnd(string animName)
    {
        // If using Spine
        if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
        {
            Spine.TrackEntry entry = null;
            // Wait until the requested animation is playing on track 0
            while (true)
            {
                entry = skeletonAnimation.AnimationState.GetCurrent(0);
                if (entry != null && entry.Animation != null && entry.Animation.Name == animName)
                    break;
                yield return null;
            }

            // Wait until the animation completes at least once
            float duration = entry.Animation != null && entry.Animation.Duration > 0 ? entry.Animation.Duration : 0f;
            // If duration is zero, just wait a frame to avoid infinite loop
            if (duration <= 0f)
            {
                yield return null;
            }
            else
            {
                // Wait until track time >= duration
                while (entry.TrackTime < duration)
                {
                    entry = skeletonAnimation.AnimationState.GetCurrent(0);
                    if (entry == null || entry.Animation == null || entry.Animation.Name != animName)
                        break;
                    yield return null;
                }
            }

            yield break;
        }

        // Fallback to Animator
        if (animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            // Wait until animator enters the state
            int attempts = 0;
            while ((!state.IsName(animName) || state.normalizedTime < 1f) && attempts < 10000)
            {
                state = animator.GetCurrentAnimatorStateInfo(0);
                attempts++;
                yield return null;
            }
        }
    }
}
