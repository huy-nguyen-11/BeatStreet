using UnityEngine;
using Spine.Unity;
using Spine;


[RequireComponent(typeof(SpriteRenderer))]
public class StaticDepthObject : MonoBehaviour
{
    [Header("Settings")]
    public float yOffset = 0f;

    public float threshold = 0.05f;

    private SpriteRenderer sr;
    private Transform player;
    //private SpriteRenderer playerSR;
    private SkeletonAnimation playerSkeletonAnim;

    private float lastPlayerY;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        GameObject p = PlayerController.Instance != null ? PlayerController.Instance.gameObject : GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;

        if (p != null)
        {
            player = p.transform;
            //playerSR = p.GetComponent<SpriteRenderer>();
            playerSkeletonAnim = p.GetComponent<SkeletonAnimation>();
            lastPlayerY = player.position.y;
        }
        else
        {
            Debug.LogWarning("not found Player (tag = Player)");
        }
    }

    void LateUpdate()
    {
        if (player == null || playerSkeletonAnim == null) return;

        float playerY = player.position.y;

        if (Mathf.Abs(playerY - lastPlayerY) < 0.0001f) return;

        lastPlayerY = playerY;

        UpdateSorting(playerY);
    }

    void UpdateSorting(float playerY)
    {
        float myY = transform.position.y + yOffset;

        if (myY < playerY - threshold)
        {
            //sr.sortingOrder = playerSR.sortingOrder + 1;
            sr.sortingOrder = /*playerSkeletonAnim.GetComponent<Renderer>().sortingOrder + 10*/ 12;
        }
        else if (myY > playerY + threshold)
        {
            //sr.sortingOrder = playerSR.sortingOrder - 1;
            sr.sortingOrder = /*playerSkeletonAnim.GetComponent<Renderer>().sortingOrder - 10*/ 4;
        }
    }
}
