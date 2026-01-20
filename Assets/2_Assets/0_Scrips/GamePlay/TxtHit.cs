using TMPro;
using UnityEngine;
using DG.Tweening;

public class TxtHit : MonoBehaviour,IpooledObject
{
    TextMeshPro _txt;
    [SerializeField] Color[] _txtColors;

    bool isCheck;
    bool isRun;

    [Header("Random X")]
    float randomXRange = 0.165f;

    Tween autoDisableTween; // ⏱ auto tắt
    Sequence moveSeq;

    public void OnObjectSpawn()
    {
        if (_txt == null)
            _txt = GetComponent<TextMeshPro>();

        // reset trạng thái khi lấy từ pool
        isRun = false;
        transform.DOKill();
        autoDisableTween?.Kill();

        // reset alpha về 1
        Color c = _txt.color;
        c.a = 1f;
        _txt.color = c;
    }

    public void SetTxt(float Dame, bool isCheck)
    {
        this.isCheck = isCheck;

        _txt.color = _txtColors[!isCheck ? 0 : 1];
        _txt.text = ((int)Dame).ToString();

        if (!isRun)
            PlayTween();

        // ⏱ Auto tắt sau 1 giây kể từ lúc xuất hiện
        autoDisableTween = DOVirtual.DelayedCall(3f, () =>
        {
            gameObject.SetActive(false);
        });
    }

    //void PlayTween()
    //{
    //    isRun = true;

    //    Vector3 startPos = transform.position;

    //    float randomX = Random.Range(-randomXRange, randomXRange);

    //    Vector3 fastUpPos = startPos + new Vector3(randomX * 0.3f, 0.5f, 0);
    //    Vector3 slowUpPos = startPos + new Vector3(randomX, 1.5f, 0);

    //    moveSeq = DOTween.Sequence();

    //    // 🚀 Nhịp 1
    //    moveSeq.Append(
    //        transform.DOMove(fastUpPos, 0.2f)
    //                 .SetEase(Ease.OutQuad)
    //    );

    //    // 🌬️ Nhịp 2
    //    moveSeq.Append(
    //        transform.DOMove(slowUpPos, 0.4f)
    //                 .SetEase(Ease.OutCubic)
    //    );

    //    // 🌫️ Fade
    //    moveSeq.Join(
    //        _txt.DOFade(0f, 0.4f)
    //            .SetDelay(0.25f)
    //    );

    //    // kết thúc sớm → cũng tắt
    //    moveSeq.OnComplete(() =>
    //    {
    //        gameObject.SetActive(false);
    //    });
    //}
    void PlayTween()
    {
        isRun = true;

        Vector3 startPos = transform.position;

        float randomX = Random.Range(-randomXRange, randomXRange);

        Vector3 fastUpPos = startPos + new Vector3(randomX * 0.4f, 1f, 0);
        Vector3 slowUpPos = startPos + new Vector3(randomX, 2.5f, 0);

        moveSeq = DOTween.Sequence();

        // 🚀 Nhịp 1: bay lên nhanh
        moveSeq.Append(
            transform.DOMove(fastUpPos, 0.3f)
                     .SetEase(Ease.OutQuad)
        );

        // ⏸️ Dừng lại 1 nhịp ngắn
        moveSeq.AppendInterval(0.05f);

        // 🌬️ Nhịp 2: bay chậm + mờ dần song song
        moveSeq.Append(
            transform.DOMove(slowUpPos, 1f)
                     .SetEase(Ease.Linear)
        );

        moveSeq.Join(
            _txt.DOFade(0f, 1f
        ));
        moveSeq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

}
