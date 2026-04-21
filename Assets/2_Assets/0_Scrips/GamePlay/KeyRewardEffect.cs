using DG.Tweening;
using UnityEngine;

public class KeyRewardEffect : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform keyRewardUI;
    public CanvasGroup keyCanvasGroup;

    [Header("Positions")]
    public RectTransform startPos;
    public RectTransform targetPos;

    private Vector3 originalScale;
    public GameObject glowKey;

    // Thêm cờ đánh dấu đã khởi tạo chưa
    private bool isInitialized = false;

    void Awake()
    {
        Init();
    }


    private void Init()
    {
        if (isInitialized) return; 

        originalScale = keyRewardUI.localScale;


        if (originalScale == Vector3.zero)
            originalScale = Vector3.one;

        keyRewardUI.gameObject.SetActive(false);
        if (glowKey != null) glowKey.SetActive(false);

        isInitialized = true;
    }

    public void PlayKeyRewardEffect()
    {
        Init();


        DOTween.Kill(keyRewardUI);


        keyRewardUI.gameObject.SetActive(true);
        keyRewardUI.position = startPos.position;
        keyRewardUI.localScale = Vector3.zero;
        keyCanvasGroup.alpha = 1f;
        keyRewardUI.rotation = Quaternion.identity;


        if (glowKey != null) glowKey.SetActive(true);

        Sequence seq = DOTween.Sequence();


        seq.Append(keyRewardUI.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.OutBack));


        seq.Join(keyRewardUI.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));

        seq.AppendInterval(0.8f);

        seq.AppendCallback(() =>
        {
            if (glowKey != null) glowKey.SetActive(false);
        });

        float flyDuration = 0.6f;

        seq.Append(keyRewardUI.DOMove(targetPos.position, flyDuration).SetEase(Ease.InBack));
        seq.Join(keyRewardUI.DOScale(originalScale * 0.2f, flyDuration).SetEase(Ease.InBack));
        seq.Join(keyCanvasGroup.DOFade(0f, flyDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            keyRewardUI.gameObject.SetActive(false);
            PlayerPrefs.SetInt("Key", PlayerPrefs.GetInt("Key") + 1);
        });
    }
}
