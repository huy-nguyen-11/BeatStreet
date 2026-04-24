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
    public LayerMask damageLayer;
    [SerializeField] private GameObject explosion , fire;
    private GameObject fireGo;
    private bool isEnableHit = true;
    private bool moveRight = true;

    public EnemyController enemyController;
    public float indexOfDamage;

    public void OnObjectSpawn()
    {
        if (explosion != null)
            explosion.gameObject.SetActive(false);

        // Lấy vị trí gameplay ban đầu\
        Vector3 spawnPos = transform.position;

        // mặt đất thấp hơn spawn
        groundPos = new Vector2(
            spawnPos.x,
            spawnPos.y - 0.4f
        );
        //groundPos = transform.position;
        height = 0.4f;
        heightVelocity = initialHeightVelocity;

        isEnableHit = true;

        //if(fireGo != null)
        //{
        //    transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // deactive broken bomb sprite
        //}
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
                isEnableHit = false;
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
            StartCoroutine(HandleBombExplosion());
        }
        else if(fire)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            //transform.GetChild(0).GetChild(0).gameObject.SetActive(true); // active broken bomb sprite
            fireGo = Instantiate(fire, transform.position, Quaternion.identity);
            fire.GetComponent<FireDamaga>().fireDamage = (int)(enemyController.dame*indexOfDamage);
            StartCoroutine(DisableFireChildrenSequentially(fireGo));
        }
        else
        {
            StartCoroutine(DestroyObject());
        }
        // 👉 Gắn delay nổ ở đây nếu muốn
        // Invoke(nameof(Explode), 0.5f);
    }

    IEnumerator HandleBombExplosion()
    {
        yield return new WaitForSeconds(0.75f);
        GamePlayManager.Instance._CameraFollow.Shake();
        DealExplosionDamage();
        GetComponent<SpriteRenderer>().enabled = false;
        explosion.gameObject.SetActive(true);
        StartCoroutine(DestroyObject());
    }

    IEnumerator DisableFireChildrenSequentially(GameObject fireParent)
    {
        yield return new WaitForSeconds(2f);
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

    void OnDisable()
    {
        // Stop any running coroutines started by this instance
        StopAllCoroutines();

        // Reset runtime state so object can be safely reused by pooling
        exploded = false;
        isGrounded = false;
        bounceCount = 0;
        height = 0f;
        heightVelocity = 0f;
        horizontalVelocity = Vector2.zero;

        // Reset visual state
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;

        // Turn off explosion effect if assigned
        if (explosion != null)
            explosion.SetActive(false);

        // Clean up any spawned fire object
        if (fireGo != null)
        {
            Destroy(fireGo);
            fireGo = null;
            //transform.GetChild(0).GetChild(0).gameObject.SetActive(false); // deactive broken bomb sprite
        }

        // Reset transform rotation to default so it doesn't carry over rotation
        transform.rotation = Quaternion.identity;
    }

    // ===============================
    // PUBLIC API
    // ===============================


    public void Throw(Vector2 throwDir, float throwSpeed, float heightForce)
    {
        moveRight = throwDir.x >= 0;
        horizontalVelocity = throwDir.normalized * throwSpeed;
        initialHeightVelocity = heightForce;
        heightVelocity = heightForce;
    }

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 0.6f;

    void DealExplosionDamage()
    {
        // Include both Player and Enemy layers
        LayerMask combinedMask = damageLayer;
        int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
        if (enemyLayerIndex >= 0)
            combinedMask |= (1 << enemyLayerIndex);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, combinedMask);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            PlayerChar player = hit.GetComponent<PlayerChar>();
            if (player != null)
            {
                if (ShouldIgnorePlayerHit(player.playerController))
                    continue;

                player.playerController.HitDirection = player.transform.position.x >= transform.position.x;
                player.playerController.HitCount = 2;
                player.playerController.SetHit(enemyController.dame*indexOfDamage);
                ObjectPooler.Instance.SpawnFromPool("Hit", transform.position, Quaternion.Euler(0, 0, 0));
                continue;
            }

            EnemyChar enemyChar = hit.GetComponent<EnemyChar>();
            if (enemyChar == null)
                enemyChar = hit.GetComponentInParent<EnemyChar>();

            if (enemyChar != null && enemyChar.enemyController != null
                && enemyChar.enemyController.state != EnemyCharacter.State.Dead)
            {
                enemyChar.enemyController.SetHit((enemyController.dame * indexOfDamage)/1.5f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerController == null)
            {
                playerController = collision.GetComponentInParent<PlayerController>();
            }
            if (playerController == null)
            {
                playerController = PlayerController.Instance;
            }

            if (playerController != null && isEnableHit)
            {
                if (ShouldIgnorePlayerHit(playerController))
                    return;

                playerController.HitDirection = moveRight;
                playerController.SetHit(enemyController.dame*indexOfDamage);
                ObjectPooler.Instance.SpawnFromPool("Hit", transform.position, Quaternion.Euler(0, 0, 0));
                isEnableHit = false;
            }
        }
    }

    private bool ShouldIgnorePlayerHit(PlayerController playerController)
    {
        if (playerController == null) return true;

        if (playerController.state == PlayerCharacter.State.Ulti)
            return true;

        if (GamePlayManager.Instance != null && GamePlayManager.Instance.isCheckUlti)
            return true;

        return false;
    }


}
