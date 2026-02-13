using UnityEngine;
using System.Collections;

public class ItemPlayer : MonoBehaviour
{
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

    // internal
    private Vector2 groundPos;
    private float height;
    private float heightVelocity;
    private int bounceCount;
    private bool isGrounded;
    private bool isCollected = false;

    public int hpPercentRestore , manaPercentRestore;
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

        _isEnableCollect = false;
    }

    //private void OnEnable()
    //{
    //    Throw(new Vector2(1f, 0f)/*, throwSpeed, initialHeightVelocity*/);
    //}

    void Update()
    {
        if (!isGrounded)
        {
            UpdateHorizontalMovement();
            UpdateRotation();
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
                _isEnableCollect = true;
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

        // Reset visual state
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;

        // Clean up any spawned fire object

        // Reset transform rotation to default so it doesn't carry over rotation
        transform.rotation = Quaternion.identity;
    }


    public void Throw(Vector2 throwDir/*, float throwSpeed, float heightForce*/)
    {
        horizontalVelocity = throwDir.normalized * throwSpeed;
        //initialHeightVelocity = heightForce;
        //heightVelocity = heightForce;
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

            if (playerController != null && !isCollected && _isEnableCollect)
            {
                float mag = Mathf.Abs(playerController.Char.transform.position.y - transform.position.y);
                if(mag > 0.5f) return;

                isCollected = true;
                int _hp = (int)(DataManager.Instance.playerData[playerController.id].Hp * hpPercentRestore);
                int _mana = (int)(100 * manaPercentRestore);
                playerController.SetHp(_hp);
                playerController.SetMana(_mana);
                PlayerChar _playerChar = playerController.transform.parent.GetComponent<PlayerChar>();
                if(hpPercentRestore > manaPercentRestore && _playerChar != null)
                {
                    _playerChar.effectHP.SetActive(true);
                    goEffect = _playerChar.effectHP;
                }
                else if(manaPercentRestore > hpPercentRestore && _playerChar != null)
                {
                    _playerChar.effectMana.SetActive(true);
                    goEffect = _playerChar.effectMana;
                }
                StartCoroutine(DestroyItem());
            }
        }
    }

    private IEnumerator DestroyItem()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(2f);
        goEffect.SetActive(false);
        Destroy(gameObject);
    }

}
