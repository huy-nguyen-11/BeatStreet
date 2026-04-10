using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowUIEffect : MonoBehaviour
{
    public RectTransform icon;
    public RectTransform box;

    private void Start()
    {
        PlayEffect();
        PlayLoop();
    }

    public void PlayEffect()
    {
        icon.DOKill(); 

        Sequence seq = DOTween.Sequence();
        seq.Append(
            icon.DOScale(1.15f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(8, LoopType.Yoyo)
        );

        seq.Append(icon.DORotate(new Vector3(0, 0, 15), 0.1f))
           .Append(icon.DORotate(new Vector3(0, 0, -15), 0.1f))
           .Append(icon.DORotate(Vector3.zero, 0.1f));

        seq.SetLoops(-1, LoopType.Restart);
    }

    void PlayLoop()
    {
        box.DOKill();

        Sequence seq = DOTween.Sequence();

        // 1. Idle (thở nhẹ)
        seq.Append(box.DOScale(1.05f, 0.8f).SetEase(Ease.InOutSine))
           .Append(box.DOScale(1f, 0.8f).SetEase(Ease.InOutSine));

        // 2. Nhún nhẹ (anticipation)
        seq.Append(box.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack));

        // 3. Rung + xoay (giả vờ sắp mở)
        seq.Append(box.DOShakeRotation(0.4f, new Vector3(0, 0, 10), 15, 90, false));

        // 4. Bung nhẹ rồi thu lại (fake open feeling)
        seq.Append(box.DOScale(1.2f, 0.15f).SetEase(Ease.OutQuad))
           .Append(box.DOScale(1f, 0.2f).SetEase(Ease.InQuad));

        // Delay nhỏ để "nghỉ"
        seq.AppendInterval(1f);

        // Loop lại
        seq.SetLoops(-1, LoopType.Restart);
    }
}
