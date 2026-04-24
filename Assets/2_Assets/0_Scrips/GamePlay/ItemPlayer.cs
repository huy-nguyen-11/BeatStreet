using UnityEngine;
using System.Collections;

public class ItemPlayer : MonoBehaviour
{
    public bool isItemBooster = true; // true = booster, false = key

    [Header("Throw")]
    public Vector2 horizontalVelocity;
    public float initialHeightVelocity = 14f;
    public float throwSpeed = 5f;

    [Header("Gravity")]
    public float gravity = -35f;

    [Header("Bounce")]
    [Range(0f, 1f)]
    public float bounceDamping = 0.4f;
    public int maxBounceCount = 2;

    [Header("Rotation")]
    public float rotationSpeed = 360f;

    [Header("Collection")]
    public float collectionHeightTolerance = 1.0f; 
    public float collectionDelay = 0.1f; 

    // internal
    private Vector2 groundPos;
    private float height;
    private float heightVelocity;
    private int bounceCount;
    private bool isGrounded;
    private bool isCollected = false;
    private float groundedTimer = 0f; // Timer để delay collection

    public int hpPercentRestore, manaPercentRestore;
    private GameObject goEffect;

    private bool _isEnableCollect = false;

    void Start()
    {
        Vector3 spawnPos = transform.position;

        groundPos = new Vector2(
            spawnPos.x,
            spawnPos.y - 0.2f
        );
        height = 0.2f;
        heightVelocity = initialHeightVelocity;
        isCollected = false;
        goEffect = null;
        groundedTimer = 0f;

        _isEnableCollect = false;
    }

    void Update()
    {
        if (!isGrounded)
        {
            UpdateHorizontalMovement();
            UpdateRotation();
        }
        else
        {
            // Tăng timer khi item đã chạm đất
            groundedTimer += Time.deltaTime;
            if (groundedTimer >= collectionDelay)
            {
                _isEnableCollect = true;
            }
        }

        UpdateVerticalPhysics();
        UpdateVisualPosition();
    }

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
                groundedTimer = 0f; // Reset timer khi vừa chạm đất
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

    void OnDisable()
    {
        isGrounded = false;
        bounceCount = 0;
        height = 0f;
        heightVelocity = 0f;
        horizontalVelocity = Vector2.zero;
        groundedTimer = 0f;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;

        transform.rotation = Quaternion.identity;
    }

    public void Throw(Vector2 throwDir)
    {
        horizontalVelocity = throwDir.normalized * throwSpeed;
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

            Debug.Log("player detected: " + playerController.name);

            if (playerController != null && !isCollected && _isEnableCollect)
            {
                // Kiểm tra chiều cao với tolerance lớn hơn
                float mag = Mathf.Abs(playerController.Char.transform.position.y - transform.position.y);
                if (mag > collectionHeightTolerance)
                {
                    Debug.Log($"Item quá cao/quá thấp: {mag:F2} > {collectionHeightTolerance:F2}");
                    return;
                }

                isCollected = true;

                if (isItemBooster)
                {
                    int _hp = (int)(DataManager.Instance.playerData[playerController.id].Hp * hpPercentRestore);
                    int _mana = (int)(100 * manaPercentRestore);
                    playerController.SetHp(_hp);
                    playerController.SetMana(_mana);

                    Debug.Log($"Item collected! HP: +{_hp}, Mana: +{_mana}");

                    PlayerChar _playerChar = playerController.transform.parent.GetComponent<PlayerChar>();
                    if (hpPercentRestore > manaPercentRestore && _playerChar != null)
                    {
                        _playerChar.effectHP.SetActive(true);
                        goEffect = _playerChar.effectHP;
                    }
                    else if (manaPercentRestore > hpPercentRestore && _playerChar != null)
                    {
                        _playerChar.effectMana.SetActive(true);
                        goEffect = _playerChar.effectMana;
                    }

                    GamePlayManager.Instance.SetMission(14, 1); //set mission booster collected
                }
                else
                {
                    GamePlayManager.Instance.keyRewardEffect.PlayKeyRewardEffect();
                    GamePlayManager.Instance.SetMission(14, 1); //set mission booster collected
                }
               
                StartCoroutine(DestroyItem());
            }
            else if (!_isEnableCollect)
            {
                Debug.Log($"Item chưa sẵn sàng: isGrounded={isGrounded}, groundedTimer={groundedTimer:F2}");
            }
        }
    }

    private IEnumerator DestroyItem()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(2f);
        if (goEffect != null)
            goEffect.SetActive(false);
        Destroy(gameObject);
    }
}
