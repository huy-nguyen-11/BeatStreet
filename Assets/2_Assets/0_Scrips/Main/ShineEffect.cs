using DG.Tweening;
using UnityEngine;

public class ShineEffect : MonoBehaviour
{
    [SerializeField] private float targetX = 500f; 
    [SerializeField] private float duration = 1f;  
    [SerializeField] private float delay = 2f;    
    private Sequence shineSequence;

    void OnEnable()
    {
        StartShineAnimation();
    }

    private void OnDisable()
    {
        shineSequence?.Kill();
    }

    void StartShineAnimation()
    {
        Vector3 startPosition = transform.localPosition;

        shineSequence = DOTween.Sequence();

        shineSequence.Append(
            transform.DOLocalMoveX(targetX, duration)
                .SetEase(Ease.Linear)
        )
        .AppendInterval(delay)
        .OnStepComplete(() => {
            
            transform.localPosition = startPosition;
        })
        .SetLoops(-1); 
    }
}
