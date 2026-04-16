using UnityEngine;
using Spine;
using Spine.Unity;

public class ShowPopEffect : MonoBehaviour
{
    public SkeletonGraphic skeletonGraphic;
    public string nameAim1, nameAim2;

    private void OnEnable()
    {
        if (skeletonGraphic != null)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, nameAim1, false);
            skeletonGraphic.AnimationState.AddAnimation(0, nameAim2, true, 1.867f);
        }
    }
}
