using System.Collections;
using UnityEngine;


public class FireDamaga : MonoBehaviour
{
    private int fireDamage = 5;
    public float damageInterval = 1f;

    private Coroutine damageCoroutine;
    private PlayerController currentPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //currentPlayer = collision.GetComponent<PlayerController>();
            //if (currentPlayer != null)
            //{
            //    Debug.Log("Player entered fire damage area.");
            //    damageCoroutine = StartCoroutine(DamageOverTime());
            //}

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
                //playerController.SetHit(damage);
                currentPlayer = playerController;
                damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopDamage();
        }
    }

    IEnumerator DamageOverTime()
    {
        while (true)
        {
            //todo effect burn ; no set fall player
            currentPlayer.SetHitFromFire(fireDamage);
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

        currentPlayer = null;
    }

    private void OnDisable()
    {
        StopDamage();
    }
}
