using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ShowPopUpReward : MonoBehaviour
{
    [Header("Cấu hình Khung")]
    public RectTransform boardRect;
    public float boardTargetHeight = 915f;
    public float boardTargetY = 120f;
    public float boardDuration = 0.5f;

    [Header("Cấu hình Items")]
    public List<RectTransform> rewardItems; // Kéo các ô Item vào đây
    public float flipDuration = 0.4f;
    public float delayBetweenItems = 0.15f; // Thời gian chờ giữa mỗi item
    void OnEnable()
    {
        PlaySequence();
    }

    public void PlaySequence()
    {
        // 1. Khởi tạo trạng thái ban đầu
        boardRect.sizeDelta = new Vector2(boardRect.sizeDelta.x, 0);
        boardRect.anchoredPosition = new Vector2(boardRect.anchoredPosition.x, 0);

        foreach (var item in rewardItems)
        {
            // Xoay trục Y 90 độ (để nó nằm ngang, nhìn không thấy)
            item.localRotation = Quaternion.Euler(0, 90, 0);
            item.localScale = Vector3.zero; // Thêm scale 0 để hiện ra mượt hơn
        }

        // 2. Tạo Sequence (Chuỗi hành động)
        Sequence mainSeq = DOTween.Sequence();

        // Bước A: Mở khung gỗ
        mainSeq.Append(boardRect.DOSizeDelta(new Vector2(boardRect.sizeDelta.x, boardTargetHeight), boardDuration).SetEase(Ease.OutQuart));
        mainSeq.Join(boardRect.DOAnchorPosY(boardTargetY, boardDuration).SetEase(Ease.OutQuart));

        // Khoảng nghỉ ngắn sau khi khung mở xong
        mainSeq.AppendInterval(0.1f);

        // Bước B: Lật từng Item lần lượt
        for (int i = 0; i < rewardItems.Count; i++)
        {
            RectTransform item = rewardItems[i];

            // Join làm các hiệu ứng của cùng 1 item chạy cùng lúc (Xoay + Scale)
            // Append làm các item chạy sau nhau theo vòng lặp
            mainSeq.Append(item.DOScale(1f, flipDuration * 0.5f).SetEase(Ease.OutQuad));
            mainSeq.Join(item.DORotate(new Vector3(0, 0, 0), flipDuration).SetEase(Ease.OutBack));

            // Thêm một chút delay nhỏ giữa các item nếu muốn giãn cách rõ hơn
            if (i < rewardItems.Count - 1)
                mainSeq.AppendInterval(delayBetweenItems);
        }
    }
}
