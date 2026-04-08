using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StaticDepthObject : MonoBehaviour
{
    [Header("Settings")]
    private string sortingLayerName = "Char";
    [SerializeField] private int sortingBaseOrder = 100;
    [SerializeField] private float sortingScale = 100f;

    [Header("Offsets")]
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private int orderOffset = 0;

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void Start()
    {
        ApplySorting();
    }

    private void LateUpdate()
    {
        ApplySorting();
    }

    private void ApplySorting()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0) return;

        float yForSorting = transform.position.y + yOffset;
        int sortingOrder = sortingBaseOrder - Mathf.RoundToInt(yForSorting * sortingScale) + orderOffset;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer sr = spriteRenderers[i];
            if (sr == null) continue;

            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;
        }
    }
}
