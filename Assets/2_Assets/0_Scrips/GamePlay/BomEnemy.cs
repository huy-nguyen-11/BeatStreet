using System.Collections;
using UnityEngine;

public class BomEnemy : MonoBehaviour , IpooledObject
{
    [Header("Throw")]
    public Vector2 horizontalVelocity;   // vận tốc X,Y gameplay
    public float initialHeightVelocity = 14f;

    [Header("Gravity")]
    public float gravity = -35f;

    [Header("Bounce")]
    [Range(0f, 1f)]
    public float bounceDamping = 0.4f;    // càng nhỏ → nảy yếu
    public int maxBounceCount = 2;

    [Header("Rotation")]
    public float rotationSpeed = 360f;    // độ / giây

    // internal
    private Vector2 groundPos;
    private float height;
    private float heightVelocity;
    private int bounceCount;
    private bool exploded;

    private bool isGrounded;

    [SerializeField] private GameObject explosion , fire;
    private GameObject fireGo;


    public void OnObjectSpawn()
    {
        if (explosion != null)
            explosion.gameObject.SetActive(false);

        // Lấy vị trí gameplay ban đầu\
        Vector3 spawnPos = transform.position;

        // mặt đất thấp hơn spawn
        groundPos = new Vector2(
            spawnPos.x,
            spawnPos.y - 0.6f
        );
        //groundPos = transform.position;
        height = 0.6f;
        heightVelocity = initialHeightVelocity;

    }

    void Start()
    {
       
        //Debug.Log("[Bomb] Spawned & thrown");
    }

    void Update()
    {
        if (exploded) return;

        if (!isGrounded)
        {
            UpdateHorizontalMovement();
            UpdateRotation();
        }
        UpdateVerticalPhysics();
        UpdateVisualPosition();
    }

    // ===============================
    // MOVEMENT
    // ===============================

    void UpdateHorizontalMovement()
    {
        groundPos += horizontalVelocity * Time.deltaTime;
    }

    void UpdateVerticalPhysics()
    {
        if (isGrounded) return;

        heightVelocity += gravity * Time.deltaTime;
        height += heightVelocity * Time.deltaTime;

        if (height <= 0f)
        {
            height = 0f;

            if (bounceCount < maxBounceCount)
            {
                bounceCount++;
                heightVelocity = -heightVelocity * bounceDamping;
            }
            else
            {
                heightVelocity = 0f;
                isGrounded = true;
                OnBombLanded();
            }
        }
    }

    void UpdateVisualPosition()
    {
        transform.position = new Vector3(
            groundPos.x,
            groundPos.y + height,
            transform.position.z
        );
    }

    void UpdateRotation()
    {
        float dir = Mathf.Sign(horizontalVelocity.x);
        transform.Rotate(0f, 0f, rotationSpeed * dir * Time.deltaTime);
    }

    // ===============================
    // EVENTS
    // ===============================

    void OnBombLanded()
    {
        if(explosion != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            explosion.gameObject.SetActive(true);
            StartCoroutine(DestroyObject());
        }else if(fire)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            fireGo = Instantiate(fire, transform.position, Quaternion.identity);
            StartCoroutine(DisableFireChildrenSequentially(fireGo));
        }
        // 👉 Gắn delay nổ ở đây nếu muốn
        // Invoke(nameof(Explode), 0.5f);
    }

    IEnumerator DisableFireChildrenSequentially(GameObject fireParent)
    {
        yield return new WaitForSeconds(1f);
        int childCount = fireParent.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = fireParent.transform.GetChild(i);
            child.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.2f);
        }

        Destroy(fireParent);
        gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = true;
    }


    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("[Bomb] BOOM 💥");

        // TODO:
        // - Spawn effect
        // - Damage enemy
        // - Camera shake

        Destroy(gameObject);
    }

    // ===============================
    // PUBLIC API
    // ===============================

    public void Throw(Vector2 throwDir, float throwSpeed, float heightForce)
    {
        horizontalVelocity = throwDir.normalized * throwSpeed;
        initialHeightVelocity = heightForce;
        heightVelocity = heightForce;

        Debug.Log("[Bomb] Throw called");
    }
}
