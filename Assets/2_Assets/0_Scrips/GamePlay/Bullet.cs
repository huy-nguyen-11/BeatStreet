using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IpooledObject
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 3f; 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool moveRight = true; 

    private Coroutine lifetimeCoroutine;
    private bool isActive = false;

    public void OnObjectSpawn()
    {
        isActive = true;
        moveRight = transform.right.x >= 0f;
        if (rb != null)
        {
            float direction = moveRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
        
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
        }
        lifetimeCoroutine = StartCoroutine(LifetimeCountdown());
    }

    private void FixedUpdate()
    {
        if (isActive && rb != null)
        {
            float direction = moveRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * speed, 0f);
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
            
            if (playerController != null)
            {
                playerController.HitDirection = moveRight;
                ObjectPooler.Instance.SpawnFromPool("Hit", transform.position, Quaternion.Euler(0, 0, 0));
                playerController.SetHit(damage);
            }
            
            DestroyBullet();
        }
    }


    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        if (isActive)
        {
            DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        if (!isActive) return;
        
        isActive = false;
        
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }
       
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        gameObject.SetActive(false);
    }

    public void SetDirection(bool right)
    {
        moveRight = right;
    }
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetSpeed(float spd)
    {
        speed = spd;
    }
}
