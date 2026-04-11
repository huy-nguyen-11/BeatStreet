using DG.Tweening;
using UnityEngine;

public class ShineEffect : MonoBehaviour
{
    [SerializeField] private float targetX = 500f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 2f;

    private Sequence shineSequence;
    private Vector3 originalPosition; 

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        StartShineAnimation();
    }

    private void OnDisable()
    {
        shineSequence?.Kill();
        transform.localPosition = originalPosition;
    }

    void StartShineAnimation()
    {
        shineSequence?.Kill();

        transform.localPosition = originalPosition;

        shineSequence = DOTween.Sequence();

        shineSequence.Append(
            transform.DOLocalMoveX(targetX, duration) 
                .SetEase(Ease.Linear)
        )
        .AppendInterval(delay)
        .OnStepComplete(() => {
            transform.localPosition = originalPosition;
        })
        .SetLoops(-1);
    }
}
