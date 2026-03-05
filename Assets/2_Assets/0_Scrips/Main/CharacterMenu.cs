using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMenu : MonoBehaviour
{
    [Header("Spine")]
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private List<SkeletonDataAsset> skeletonOptions = new List<SkeletonDataAsset>();

    [Header("Jump movement")]
    [Tooltip("Optional target transform. If null, Target Offset is used relative to start position.")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform startPosTransform;
    [SerializeField] private Vector3 targetOffset = new Vector3(3f, 0f, 0f);
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float jumpHeight = 2.0f;

    [Header("Scale")]
    [SerializeField] private float startScale = 3f;
    [SerializeField] private float endScale = 6f;

    [Header("Animation names")]
    [SerializeField] private string jumpAnim = "Jump";
    [SerializeField] private string idleAnim = "Idle";

    private bool isActive = false;

    private void OnEnable()
    {
        if (skeletonGraphic == null)
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        SetDataChar();
        if (!isActive)
            StartJump();
    }

    void SetDataChar()
    {
        if (DataManager.Instance == null || skeletonOptions == null || skeletonOptions.Count == 0 || skeletonGraphic == null)
            return;

        int id = Mathf.Clamp(DataManager.Instance.idPlayer, 0, skeletonOptions.Count - 1);
        var asset = skeletonOptions[id];
        if (asset == null) return;

        // assign the selected SkeletonDataAsset to the SkeletonGraphic and reinitialize
        skeletonGraphic.skeletonDataAsset = asset;
        skeletonGraphic.Initialize(true);
    }

    public void StartJump()
    {
        StartCoroutine(PlayJumpToTarget());
    }

    private IEnumerator PlayJumpToTarget()
    {
        // ensure starting scale
        transform.localScale = Vector3.one * startScale;
        Vector3 startPos = startPosTransform.position; // Fixed start position
        Vector3 endPos = (targetTransform != null) ? targetTransform.position : startPos + targetOffset;

        // Play jump animation (Spine)
        if (skeletonGraphic != null)
        {
            var state = skeletonGraphic.AnimationState;
            if (state != null)
                state.SetAnimation(0, jumpAnim, false);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);

            // horizontal interpolation
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            // vertical offset: peak early and gradually reduce height as it moves
            float verticalOffset = Mathf.Sin(Mathf.PI * t) * jumpHeight * (1f - t);
            pos.y += verticalOffset;

            transform.position = pos;

            // scale from startScale -> endScale
            float s = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = Vector3.one * s;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ensure final values
        transform.position = endPos;
        transform.localScale = Vector3.one * endScale;

        // Switch to idle animation
        if (skeletonGraphic != null)
        {
            var state = skeletonGraphic.AnimationState;
            if (state != null)
                state.SetAnimation(0, idleAnim, true);
        }

        isActive = true;
    }
}
