using Spine.Unity;
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
}
