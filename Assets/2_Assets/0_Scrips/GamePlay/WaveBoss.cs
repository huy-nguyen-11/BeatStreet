using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class WaveBoss : MonoBehaviour, IpooledObject
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public int direction = 1;

    [Header("Trail & Life")]
    public float startTimmer = 0.1f;
    public float lifeTimeMax = 3f;


    [Header("Spawn Scale (DOTween)")]
    public float spawnScale = 0.5f;
    public float spawnScaleDuration = 0.15f;

    private float _timmer;
    private float lifeTime;
    private float posX;
    private HashSet<int> _hitEnemyIds = new HashSet<int>();
    public LayerMask enemyLayer;

    //damagae
    private int baseDamage = 5;
    private int currentDamage;

    public void OnObjectSpawn()
    {
        _timmer = startTimmer;
        lifeTime = lifeTimeMax;
        currentDamage = baseDamage;
        posX = transform.position.x;

        _hitEnemyIds.Clear();

        // Reset scale and animate from 0 -> spawnScale using DOTween
        transform.DOKill(); // ensure no leftover tweens
        transform.localScale = Vector3.zero;
        transform.DOScale(spawnScale, spawnScaleDuration).SetEase(Ease.OutBack);
    }

    void OnDisable()
    {
        // Kill any running tweens when object is disabled/returned to pool
        transform.DOKill();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        lifeTime -= dt;
        UpdateDamage();
        MoveHorizontal(dt);
        CheckEnemyHit();
        HandleTrail(dt);
        HandleLifeTime();
    }

    void UpdateDamage()
    {
        float lifeRatio = lifeTime / lifeTimeMax;
        lifeRatio = Mathf.Clamp01(lifeRatio);

        currentDamage = Mathf.RoundToInt(baseDamage * lifeRatio);
    }

    void MoveHorizontal(float dt)
    {
        posX += direction * moveSpeed * dt;
        transform.position = new Vector3(posX, transform.position.y, transform.position.z);
    }

    void HandleTrail(float dt)
    {
        _timmer -= dt;
        if (_timmer <= 0)
        {
            GameObject trail = ObjectPooler.Instance.SpawnFromPool("WaveBossEffect", transform.position, Quaternion.Euler(60, 0, -90*direction));
            trail.transform.position = new Vector3(trail.transform.position.x, trail.transform.position.y, 0);
            ObjectPooler.Instance.ReturnToPool("WaveBossEffect", trail, 0.3f);
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
                playerController.SetHit(currentDamage);
            }
        }
    }
}