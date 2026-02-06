using DG.Tweening;
using Spine.Unity;
using System.Collections;
using UnityEngine;
public class DemoSpine : MonoBehaviour
{
    [Header("Sorting Layers")]
    public string backLayer = "Player_Back";
    public string frontLayer = "Player_Front";
    public string enemyLayer = "Enemy";

    [Header("Sorting Orders")]
    public int backOrder = 10;
    public int enemyOrder = 15;
    public int frontOrder = 20;

    [Header("References")]
    public SkeletonAnimation playerSkeleton;
    public SkeletonAnimation enemySkeleton;

    /// <summary>
    /// Gọi khi bắt đầu grab enemy
    /// </summary>
    /// 

    void Start()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log($"=== Spine Separator HARD Debug ({renderers.Length} MeshRenderer) ===");

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            Debug.Log(
                $"[{i}] GO={r.gameObject.name}, " +
                $"Layer={r.sortingLayerName}, Order={r.sortingOrder}, " +
                $"Material={r.sharedMaterial.name}"
            );
        }

        var skeleton = GetComponent<SkeletonAnimation>().Skeleton;

        Debug.Log("=== SLOT DRAW ORDER ===");

        for (int i = 0; i < skeleton.DrawOrder.Count; i++)
        {
            Debug.Log($"{i}: {skeleton.DrawOrder.Items[i].Data.Name}");
        }
    }

    public void ApplyGrabSorting()
    {
        if (playerSkeleton == null || enemySkeleton == null)
        {
            Debug.LogError("SpineSortingBinder: Missing skeleton reference");
            return;
        }

        // ===== PLAYER =====
        var playerRenderers = playerSkeleton.GetComponentsInChildren<MeshRenderer>();

        if (playerRenderers.Length < 2)
        {
            Debug.LogError("Player must have at least 2 submeshes (Separator Slot)");
            return;
        }

        // Submesh 0: Back (body + left arm)
        playerRenderers[0].sortingLayerName = backLayer;
        playerRenderers[0].sortingOrder = backOrder;

        // Submesh 1: Front (right arm)
        playerRenderers[1].sortingLayerName = frontLayer;
        playerRenderers[1].sortingOrder = frontOrder;

        // ===== ENEMY =====
        var enemyRenderer = enemySkeleton.GetComponent<MeshRenderer>();
        enemyRenderer.sortingLayerName = enemyLayer;
        enemyRenderer.sortingOrder = enemyOrder;
    }

    /// <summary>
    /// Gọi khi thả enemy (trở về bình thường)
    /// </summary>
    public void ResetSorting()
    {
        // Player về layer mặc định
        var playerRenderers = playerSkeleton.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in playerRenderers)
        {
            r.sortingLayerName = "Player";
            r.sortingOrder = 10;
        }

        // Enemy về layer mặc định
        var enemyRenderer = enemySkeleton.GetComponent<MeshRenderer>();
        enemyRenderer.sortingLayerName = "Enemy";
        enemyRenderer.sortingOrder = 10;
    }

    [ContextMenu("DEBUG: Print MeshRenderers")]
    public void DebugMeshRenderers()
    {

        var renderers = playerSkeleton.GetComponentsInChildren<MeshRenderer>();

        Debug.Log($"=== Spine Sorting Debug ({renderers.Length} MeshRenderer) ===");

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            Debug.Log(
                $"[{i}] " +
                $"Layer={r.sortingLayerName}, " +
                $"Order={r.sortingOrder}, " +
                $"Material={r.sharedMaterial.name}"
            );
        }
    }
    public SkeletonAnimation skeletonAnimation;

    private Coroutine flashCoroutine;
    Color hitColor = HexToColor("#FFCD45");

    public void Demo()
    {
        FlashRed(0.1f, 5);
    }

    public void FlashRed(float flashTime = 0.1f, int flashCount = 3)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine(flashTime, flashCount));
    }

    private IEnumerator FlashCoroutine(float flashTime, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            skeletonAnimation.skeleton.SetColor(hitColor);
            yield return new WaitForSeconds(flashTime);

            skeletonAnimation.skeleton.SetColor(Color.white);
            yield return new WaitForSeconds(flashTime);
        }
    }

    public static Color HexToColor(string hex)
    {
        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.white;
    }

    //[SerializeField] private SkeletonAnimation skeletonAnimation;
    //public GameObject bom;
    //public Transform posBom;

    //private Tween flashTween;

    //public void Demo()
    //{
    //    FlashRed(0.1f, 2);
    //}

    //public void FlashRed(
    //    float flashDuration = 0.08f,
    //    int flashCount = 3,
    //    Color? hitColor = null
    //)
    //{
    //    if (skeletonAnimation == null) return;

    //    // Màu đỏ hit (cho phép custom)
    //    Color flashColor = hitColor ?? new Color(1f, 0.3f, 0.3f, 1f);

    //    // Kill tween cũ để tránh chồng hiệu ứng
    //    flashTween?.Kill();

    //    flashTween = DOTween.Sequence()
    //        .SetUpdate(true) // vẫn chạy dù Time.timeScale = 0 (nếu pause game)
    //        .AppendCallback(() =>
    //        {
    //            skeletonAnimation.skeleton.SetColor(flashColor);
    //        })
    //        .AppendInterval(flashDuration)
    //        .AppendCallback(() =>
    //        {
    //            skeletonAnimation.skeleton.SetColor(Color.white);
    //        })
    //        .AppendInterval(flashDuration)
    //        .SetLoops(flashCount);
    //}

    //private void OnDisable()
    //{
    //    flashTween?.Kill();
    //}

    //public void Explode()
    //{
    //    if (bom != null)
    //    {
    //        GameObject bomb = Instantiate(bom, posBom.transform.position , Quaternion.identity);
    //        BomEnemy bomEnemy = bom.GetComponent<BomEnemy>();
    //        bomEnemy.Throw(
    //            throwDir: new Vector2(1f, 0f),  // ném sang phải
    //            throwSpeed: 3f,
    //            heightForce: 7f
    //        );
    //    }
    //}

    //public void Bone()
    //{
    //    if (bom != null)
    //    {
    //        GameObject bomb = Instantiate(bom, posBom.transform.position, Quaternion.identity);
    //        BomEnemy bomEnemy = bom.GetComponent<BomEnemy>();
    //        bomEnemy.Throw(
    //            throwDir: new Vector2(1f, 0f),  // ném sang phải
    //            throwSpeed: 6f,
    //            heightForce: 3.5f
    //        );
    //    }
    //}

    //public void Molotov()
    //{
    //    if (bom != null)
    //    {
    //        GameObject bomb = Instantiate(bom, posBom.transform.position, Quaternion.identity);
    //        BomEnemy bomEnemy = bom.GetComponent<BomEnemy>();
    //        bomEnemy.Throw(
    //            throwDir: new Vector2(1f, 0f),  // ném sang phải
    //            throwSpeed: 3f,
    //            heightForce: 4f
    //        );
    //    }
    //}

}
