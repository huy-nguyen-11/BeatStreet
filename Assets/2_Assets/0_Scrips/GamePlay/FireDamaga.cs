using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireDamaga : MonoBehaviour
{
    private int fireDamage = 5;
    public float damageInterval = 1f;

    private Coroutine damageCoroutine;
    private readonly HashSet<PlayerController> playersInFire = new HashSet<PlayerController>();
    private readonly HashSet<EnemyController> enemiesInFire = new HashSet<EnemyController>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController == null)
                playerController = collision.GetComponentInParent<PlayerController>();
            if (playerController == null)
                playerController = PlayerController.Instance;

            if (playerController != null)
            {
                playersInFire.Add(playerController);
                if (damageCoroutine == null)
                    damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyChar enemyChar = collision.GetComponent<EnemyChar>();
            if (enemyChar == null)
                enemyChar = collision.GetComponentInParent<EnemyChar>();

            if (enemyChar != null && enemyChar.enemyController != null)
            {
                enemiesInFire.Add(enemyChar.enemyController);
                if (damageCoroutine == null)
                    damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController == null)
                playerController = collision.GetComponentInParent<PlayerController>();
            if (playerController == null)
                playerController = PlayerController.Instance;

            if (playerController != null)
                playersInFire.Remove(playerController);
        }
        else if (collision.CompareTag("Enemy"))
        {
            EnemyChar enemyChar = collision.GetComponent<EnemyChar>();
            if (enemyChar == null)
                enemyChar = collision.GetComponentInParent<EnemyChar>();

            if (enemyChar != null && enemyChar.enemyController != null)
                enemiesInFire.Remove(enemyChar.enemyController);
        }

        if (playersInFire.Count == 0 && enemiesInFire.Count == 0)
            StopDamage();
    }

    IEnumerator DamageOverTime()
    {
        while (true)
        {
            foreach (var player in playersInFire)
            {
                if (player.state == PlayerCharacter.State.Ulti)
                    continue;
                if (GamePlayManager.Instance != null && GamePlayManager.Instance.isCheckUlti)
                    continue;
                if (player != null && !player.IsDead)
                    player.SetHitFromFire(fireDamage);
            }

            foreach (var enemy in enemiesInFire)
            {
                if (enemy != null && enemy.state != EnemyCharacter.State.Dead)
                    enemy.SetHit(fireDamage);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    void StopDamage()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        playersInFire.Clear();
        enemiesInFire.Clear();
    }

    private void OnDisable()
    {
        StopDamage();
    }
}
