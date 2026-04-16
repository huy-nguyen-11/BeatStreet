using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowPopUpReward : MonoBehaviour
{
    public Transform contentTransform;
    public RectTransform boardRect;
    public float boardTargetHeight = 915f;
    public float boardTargetY = 120f;
    public float boardDuration = 0.5f;
    //for bonus item
    [SerializeField] private List<Vector2> listBonusItemsPos;
    [SerializeField] private List<GameObject> listBonusItems;

    public List<RectTransform> rewardItems;
    public float flipDuration = 0.4f;
    public float delayBetweenItems = 0.15f;

    public RectTransform claimTarget;
    public float flyDuration = 0.5f;
    public Ease flyEase = Ease.InBack;
    public float flyDelayBetween = 0.05f;
    public GameObject popUp;

    private Sequence currentSequence;
    public GameObject buttonClaim;
    private Tween buttonTween;

    public int rewardAmount; // Example reward amount

    void OnEnable()
    {
        ResetState();
        PlaySequence();

        if(listBonusItemsPos == null || listBonusItemsPos.Count == 0 &&  listBonusItems != null)
        {
            for (int i = 0; i < listBonusItems.Count; i++)
            {
                listBonusItemsPos.Add(new Vector2(listBonusItems[i].GetComponent<RectTransform>().anchoredPosition.x , listBonusItems[i].GetComponent<RectTransform>().anchoredPosition.y));
            }
        }

        if(listBonusItemsPos != null && listBonusItemsPos.Count > 0 && listBonusItems != null)
        {
            for (int i = 0; i < listBonusItems.Count; i++)
            {
                if(i < listBonusItemsPos.Count)
                {
                    listBonusItems[i].GetComponent<RectTransform>().anchoredPosition = listBonusItemsPos[i];
                }
            }
        }
    }

    private void ResetState()
    {
        currentSequence?.Kill();
        GetComponent<GridLayoutGroup>().enabled = true;

        boardRect.sizeDelta = new Vector2(boardRect.sizeDelta.x, 0);
        boardRect.anchoredPosition = new Vector2(boardRect.anchoredPosition.x, 0);
        boardRect.localScale = Vector3.one;

        foreach (var item in rewardItems)
        {
            item.DOKill();
            item.localRotation = Quaternion.Euler(0, 90, 0);
            item.localScale = Vector3.one;

            CanvasGroup group = item.GetComponent<CanvasGroup>();
            if (group != null) group.alpha = 1;
        }


        buttonClaim.SetActive(false);
        buttonClaim.transform.localScale = Vector3.one;
        if (buttonTween != null) buttonTween.Kill();
    }

    public void PlaySequence()
    {
        boardRect.sizeDelta = new Vector2(boardRect.sizeDelta.x, 0);
        boardRect.anchoredPosition = new Vector2(boardRect.anchoredPosition.x, 0);

        foreach (var item in rewardItems)
        {
            item.localRotation = Quaternion.Euler(0, 90, 0);
        }

        Sequence mainSeq = DOTween.Sequence();
        currentSequence = mainSeq;

        mainSeq.Append(boardRect.DOSizeDelta(new Vector2(boardRect.sizeDelta.x, boardTargetHeight), boardDuration).SetEase(Ease.OutQuart));
        mainSeq.Join(boardRect.DOAnchorPosY(boardTargetY, boardDuration).SetEase(Ease.OutQuart));

        mainSeq.AppendInterval(0.1f);

        //int num = 0;
        //for (int i = 0; i < rewardItems.Count; i++)
        //{
        //    RectTransform item = rewardItems[i];
        //    mainSeq.Append(item.DORotate(new Vector3(0, 0, 0), flipDuration).SetEase(Ease.OutBack));

        //    if (i < rewardItems.Count - 1)
        //    {
        //        mainSeq.AppendInterval(delayBetweenItems);
        //    }

        //}

        //if (num == 10)
        //{
        //    mainSeq.OnComplete(() =>
        //    {
        //        buttonClaim.SetActive(true);
        //        buttonTween = buttonClaim.transform.DOScale(1.3f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        //    });
        //}
        int num = 0;
        for (int i = 0; i < rewardItems.Count; i++)
        {
            RectTransform item = rewardItems[i];
            Tween t = item.DORotate(new Vector3(0, 0, 0), flipDuration).SetEase(Ease.OutBack);
            // Append tween to the sequence so items flip in order
            mainSeq.Append(t);

            // Khi tween của item này kết thúc thì tăng counter và kiểm tra điều kiện
            t.OnComplete(() =>
            {
                num++;
                if (num >= rewardAmount && rewardAmount > 0)
                {
                    if (!buttonClaim.activeSelf)
                    {
                        buttonClaim.SetActive(true);
                        if (buttonTween != null) buttonTween.Kill();
                        buttonTween = buttonClaim.transform.DOScale(1.3f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                    }
                }
            });

            if (i < rewardItems.Count - 1)
            {
                mainSeq.AppendInterval(delayBetweenItems);
            }
        }
    }


    public void ClaimRewards()
    {
        if (rewardItems == null || rewardItems.Count == 0) return;
        rewardAmount = 0;
        GetComponent<GridLayoutGroup>().enabled = false;
        Sequence claimSeq = DOTween.Sequence();
        currentSequence = claimSeq;
        buttonClaim.SetActive(false);
        for (int i = 0; i < rewardItems.Count; i++)
        {
            RectTransform item = rewardItems[i];

            claimSeq.Insert(i * flyDelayBetween,
                item.DOMove(claimTarget.position, flyDuration).SetEase(flyEase)
            );

            claimSeq.Insert(i * flyDelayBetween,
                item.DOScale(0.3f, flyDuration).SetEase(Ease.InQuad)
            );

            CanvasGroup group = item.GetComponent<CanvasGroup>();
            if (group != null)
            {
                claimSeq.Insert(i * flyDelayBetween, group.DOFade(0, flyDuration));
            }
        }

        claimSeq.OnComplete(() => {
            claimTarget.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f);

            ClosePopup();
        });
    }

    private void ClosePopup()
    {
        boardRect.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
            popUp.SetActive(false);
        });
    }
}
