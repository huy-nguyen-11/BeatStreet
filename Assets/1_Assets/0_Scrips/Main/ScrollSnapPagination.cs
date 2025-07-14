using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSnapPagination : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public int totalPages = 3;
    public Image[] pageIndicators;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;
    public bool autoScroll = false;

    private float[] pagePositions;
    private int currentPage = 0;
    private bool isDragging = false;

    public void Start()
    {
        CalculatePagePositions();
        scrollRect.onValueChanged.AddListener(OnScroll);
        UpdatePageIndicators();

        if (autoScroll && totalPages > 1)
            StartCoroutine(AutoScrollPages());
    }

    void CalculatePagePositions()
    {
        pagePositions = new float[totalPages];
        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = (float)i / (totalPages - 1);
        }
    }

    void OnScroll(Vector2 position)
    {
        if (!isDragging)
            return;

        float closestPos = float.MaxValue;
        int closestIndex = currentPage;

        for (int i = 0; i < pagePositions.Length; i++)
        {
            float distance = Mathf.Abs(position.x - pagePositions[i]);
            if (distance < closestPos)
            {
                closestPos = distance;
                closestIndex = i;
            }
        }
        currentPage = closestIndex;
        UpdatePageIndicators();
    }

    public void OnEndDrag()
    {
        isDragging = false;
        StartCoroutine(SnapToPage(currentPage));
    }

    public void OnBeginDrag()
    {
        isDragging = true;
    }

    IEnumerator SnapToPage(int pageIndex)
    {
        float target = pagePositions[pageIndex];
        float duration = 0.3f;
        float elapsed = 0;
        float start = scrollRect.horizontalNormalizedPosition;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = target;
        UpdatePageIndicators();
    }

    void UpdatePageIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].color = (i == currentPage) ? activeColor : inactiveColor;
        }
    }

    IEnumerator AutoScrollPages()
    {
        while (autoScroll)
        {
            yield return new WaitForSeconds(4f);
            currentPage = (currentPage + 1) % totalPages;
            StartCoroutine(SnapToPage(currentPage));
        }
    }
    public void PagesSellect(int id)
    {
        currentPage = id;
        StartCoroutine(SnapToPage(id));
    }
}
