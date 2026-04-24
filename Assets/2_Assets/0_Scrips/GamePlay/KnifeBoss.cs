using System.Collections.Generic;
using UnityEngine;

public class KnifeBoss : MonoBehaviour, IpooledObject
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotateSpeed = 720f;
    public int direction = 1;
    public Transform centerPos;

    [Header("Wave Settings")]
    public float amplitude = 0.5f;
    public float frequency = 3f;

    [Header("Trail & Life")]
    public float startTimmer = 0.1f;
    public float lifeTimeMax = 3f;

    private float _timmer;
    private float lifeTime;
    private float elapsedTime;
    private float posX;
    private float baseY;
    private float zPos;
    private bool hasBounced = false;
    private HashSet<int> _hitEnemyIds = new HashSet<int>();
    public LayerMask enemyLayer;

    public void OnObjectSpawn()
    {
        _timmer = startTimmer;
        lifeTime = lifeTimeMax;
        elapsedTime = 0f;
        hasBounced = false;

        posX = transform.position.x;
        baseY = transform.position.y;
        zPos = transform.position.z;

        _hitEnemyIds.Clear();
    }


    void Start()
    {
        
    }

    void Update()
    {
        float dt = Time.deltaTime;
        elapsedTime += dt;
        lifeTime -= dt;

        Rotate(dt);
        MoveHorizontal(dt);
        ApplySinVertical();
        CheckWallBounceAndClamp();
        CheckEnemyHit();
        HandleTrail(dt);
        HandleLifeTime();
    }

    void Rotate(float dt)
    {
        transform.Rotate(0, 0, rotateSpeed * dt);
    }

    void MoveHorizontal(float dt)
    {
        posX += direction * moveSpeed * dt;
    }

    void ApplySinVertical()
    {
        float y = baseY + Mathf.Sin(elapsedTime * frequency) * amplitude;
        transform.position = new Vector3(posX, y, zPos);
    }

    void CheckWallBounceAndClamp()
    {
        if (centerPos == null) return;

        float rightLimit =  centerPos.position.x + 6f;
        float leftLimit = centerPos.position.x - 6f;

        if (!hasBounced)
        {
            if (posX >= rightLimit)
            {
                posX = rightLimit;
                FlipDirection();
                hasBounced = true;
            }
            else if (posX <= leftLimit)
            {
                posX = leftLimit;
                FlipDirection();
                hasBounced = true;

            }
        }
        else
        {
            // reset state when shuriken moves away from the wall
            if (posX < rightLimit - 0.3f && posX > leftLimit + 0.3f)
            {
                hasBounced = false;
            }
        }
    }

    void FlipDirection()
    {
        _hitEnemyIds.Clear();
        direction *= -1;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;

        baseY = transform.position.y - Mathf.Sin(elapsedTime * frequency) * amplitude;
    }

    void HandleTrail(float dt)
    {
        _timmer -= dt;
        if (_timmer <= 0)
        {
            GameObject trail = ObjectPooler.Instance.SpawnFromPool("Knife_Effect", transform.position, Quaternion.identity);
            ObjectPooler.Instance.ReturnToPool("Knife_Effect", trail, 0.3f);
            _timmer = startTimmer;
        }
    }

    void HandleLifeTime()
    {
        if (lifeTime <= 0f)
        {
            gameObject.SetActive(false);
        }
    }


    void CheckEnemyHit()
    {
        // OverlapBoxAll center must be Vector2; angle then mask
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(1.28f, 0.5f), enemyLayer);

        if (hits == null || hits.Length == 0) return;

        foreach (var hit in hits)
        {
            if (hit == null) continue;
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                int id = enemy.GetInstanceID();
                if (_hitEnemyIds.Contains(id))
                    continue;

                _hitEnemyIds.Add(id);
                //enemy.TakeDamage(GetDamage());
            }
        }
    }


    //private int GetDamage()
    //{
    //    int damage = PlayerDataManager.Data.intelligence;
    //    float r = Random.Range(0.5f, 0.7f);
    //    damage = Mathf.RoundToInt(damage * r);

    //    return damage;
    //}

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
                if(playerController.isImmortal || playerController.state == PlayerController.State.Dead || ShouldIgnorePlayerHit(playerController))
                {
                    return;
                }
                int damage = 30; // You can set the damage value as needed
                playerController.HitDirection = playerController.Char.transform.position.x >= transform.position.x;
                playerController.SetHit(damage);
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
