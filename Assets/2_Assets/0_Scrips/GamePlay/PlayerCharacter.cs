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
        Grab,
        Hit,
        Punch,
        SpeedUp,
        StandUp,
        Walk,
        Wingame,
        Skill1,
        Skill2,
        Combo3,
        Ulti,
        Throw
    }
    public State state = State.Idle;
    public int id;
    public RuntimeAnimatorController[] _anims;
   
    [SerializeField] public Animator animator;
    [SerializeField] public SkeletonAnimation skeletonAnimation;
    [SerializeField] public PlayerController playerController;
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
    }

    public void PlayAnimHaveEffect(string animName, bool loop = true, string fxAnim = null)
    {
        if (skeletonAnimation == null) return;

        var state = skeletonAnimation.AnimationState;
        if (state == null)
        {
            skeletonAnimation.AnimationName = animName;
            return;
        }

        state.SetAnimation(0, animName, loop);

        if (!string.IsNullOrEmpty(fxAnim))
        {
            var fxEntry = state.SetAnimation(1, fxAnim, false);
            fxEntry.MixDuration = 0;
            fxEntry.Complete += _ => state.ClearTrack(1);
        }
    }

    public void PlayAnimAttack(string animName)
    {
        var entry = skeletonAnimation.AnimationState.SetAnimation(0,animName, false);
        entry.Complete += (t) =>
        {
            playerController.SwitchToRunState(playerController.playerIdle);
        };
    }

    public void PlayAnim2(string anim)
    {
        var entry = skeletonAnimation.AnimationState.SetAnimation(0, anim, false);
        entry.Complete += (t) =>
        {
            if (playerController == null || playerController.skeletonAnimation == null || playerController.skeletonAnimation.AnimationState == null)
                return;
            var current = playerController.skeletonAnimation.AnimationState.GetCurrent(0);
            if (current != t)
                return;
            playerController.ResetStatus();
        };
    }

    public void PlayAnimWithEventHandler(string animName, bool loop,
    System.Action<Spine.TrackEntry> onEventFired = null)
    {
        if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
        {
            Spine.TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);

            if (onEventFired != null && entry != null)
            {
                bool eventCalled = false;
                Spine.AnimationState.TrackEntryEventDelegate handler = null;
                 handler = (trackEntry, e) =>
                {
                    try
                    {
                        if (trackEntry == entry && e != null && e.Data != null)
                        {
                            if (e.Data.Name == "Attack_end" || e.Data.Name == "End_attack")
                            {
                                if (!eventCalled)
                                {
                                    eventCalled = true;
                                    onEventFired?.Invoke(entry);
                                }
                                skeletonAnimation.AnimationState.Event -= handler;
                            }
                        }
                    }
                    catch { }
                };

                skeletonAnimation.AnimationState.Event += handler;

                // Fallback: listen for Complete event to ensure state machine transitions even if Spine event is missed
                entry.Complete += (t) =>
                {
                    if (!eventCalled)
                    {
                        eventCalled = true;
                        onEventFired?.Invoke(entry);
                    }
                    skeletonAnimation.AnimationState.Event -= handler;
                };
            }
        }
    }

    /// <summary>
    /// Queue an animation on track 0 (AddAnimation) and invoke callback when end-attack event fires for that queued entry.
    /// This avoids hard-cutting the current animation and helps combos look smoother.
    /// </summary>
    public Spine.TrackEntry AddAnimWithEventHandler(string animName, bool loop, float delay,
        System.Action<Spine.TrackEntry> onEventFired = null)
    {
        if (skeletonAnimation == null || skeletonAnimation.AnimationState == null)
            return null;

        Spine.TrackEntry entry = skeletonAnimation.AnimationState.AddAnimation(0, animName, loop, delay);
        if (onEventFired == null || entry == null)
            return entry;

        bool eventCalled = false;
        Spine.AnimationState.TrackEntryEventDelegate handler = null;
        handler = (trackEntry, e) =>
        {
            try
            {
                if (trackEntry == entry && e != null && e.Data != null)
                {
                    if (e.Data.Name == "Attack_end" || e.Data.Name == "End_attack")
                    {
                        if (!eventCalled)
                        {
                            eventCalled = true;
                            onEventFired?.Invoke(entry);
                        }
                        skeletonAnimation.AnimationState.Event -= handler;
                    }
                }
            }
            catch { }
        };
        skeletonAnimation.AnimationState.Event += handler;

        // Fallback: listen for Complete event to ensure state machine transitions even if Spine event is missed
        entry.Complete += (t) =>
        {
            if (!eventCalled)
            {
                eventCalled = true;
                onEventFired?.Invoke(entry);
            }
            skeletonAnimation.AnimationState.Event -= handler;
        };

        return entry;
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
    }
}
