using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private Queue<Collider2D> collisionQueue = new Queue<Collider2D>();
    public Vector2 boxSize = new Vector2(2f, 2f);
    public bool isCheckCharacter;
    float Dame = 0;
    private bool isCheckTrigger = false;
    public LayerMask layerMaskPlayer;
    public LayerMask layerMaskEnemy;
    public bool isSkill;
    private void Update()
    {
        if (collisionQueue.Count > 0)
        {
            if (!isCheckTrigger)
            {
                PlayerController.Instance.PerformAttack();
                isCheckTrigger = true;
            }
            Collider2D currentCollision = collisionQueue.Dequeue();
            if (!isSkill)
                HandleCollision(currentCollision);
            else
                HandleCollisionSkill(currentCollision);
        }
        else
        {
            if (isCheckTrigger)
                isCheckTrigger = false;
        }
    }
    public void SetAttack(float dame, int id)
    {
        Dame = dame;
        if (isCheckCharacter)
        {
            if (id == 3)
                PlayerAttack();
            else
                PlayerAttackDirection();
        }
        else
            EnemyAttack();
    }
    public void SetAttackSkill(float dame, int id)
    {
        Dame = dame;
        PlayerAttackJump();
    }
    public void PlayerAttackDirection()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        if (hits.Length == 0) return;
        bool Direction = CheckDirection(hits);
        GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
        foreach (var hit in hits)
        {
            float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;
            if (distanceX < 0 && Direction)
                collisionQueue.Enqueue(hit);
            else if (distanceX >= 0 && !Direction)
                collisionQueue.Enqueue(hit);
        }
    }
    public void PlayerAttackJump()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        if (hits.Length == 0) return;
        foreach (var hit in hits)
        {
            collisionQueue.Enqueue(hit);
        }
    }
    public void PlayerAttack()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        if (hits.Length == 0) return;
        bool Direction = CheckDirection(hits);
        GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyChar>().enemyController.currentHitIndex = 4;
            float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;
            if (distanceX < 0 && Direction)
                collisionQueue.Enqueue(hit);
            else if (distanceX >= 0 && !Direction)
                collisionQueue.Enqueue(hit);
        }
    }
    public void EnemyAttack()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskPlayer);
        if (hits.Length == 0) return;
        foreach (var hit in hits)
        {
            PlayerChar player = hit.gameObject.GetComponent<PlayerChar>();
            Transform enemy = transform.parent.parent;
            if (!player.playerController.isImmortal
                && Mathf.Abs(player.transform.position.x - enemy.position.x) <= 0.75f
                && Mathf.Abs(player.transform.position.y - enemy.transform.position.y) <= 0.25f)
            {
                AudioBase.Instance.AudioEnemy(1);
                bool direction = transform.parent.rotation.y == 0 ? true : false;
                player.playerController.HitDirection = direction;
                player.playerController.SetHit(Dame);
            }
        }
    }
    public bool CheckDirection(Collider2D[] enemys)
    {
        Collider2D nearestHit = null;
        float nearestDistanceX = float.MaxValue;
        foreach (var enemy in enemys)
        {
            float distanceX = GamePlayManager.Instance._Player.transform.position.x - enemy.transform.position.x;
            if (distanceX < nearestDistanceX)
            {
                nearestDistanceX = distanceX;
                nearestHit = enemy;
            }
        }
        float yRotation = nearestDistanceX < 0f ? -180 : 0;
        return nearestDistanceX < 0f;
    }
    public IEnumerator SetAttackSkill2Player(float dame)
    {
        Dame = dame;
        PlayerAttackJump();
        yield return new WaitForSeconds(0.2f);
        PlayerAttackJump();
        yield return new WaitForSeconds(0.2f);
        PlayerAttackJump();
        yield return new WaitForSeconds(0.2f);
        PlayerAttackJump();
    }
    private void HandleCollision(Collider2D collision)
    {
        bool isCheckMission = false;
        EnemyChar enemy = collision.gameObject.GetComponent<EnemyChar>();
        if (enemy != null
            && Mathf.Abs(PlayerController.Instance.Char.position.x - enemy.transform.position.x) <= 0.75f
            && Mathf.Abs(PlayerController.Instance.Char.position.y - enemy.transform.position.y) <= 0.25f)
        {
            PlayerController.Instance.CountCombo();
            enemy.enemyController.SetHit(Dame);
            if (!isCheckMission)
            {
                isCheckMission = true;
                GamePlayManager.Instance.SetMission(7, 1);
            }
        }
    }
    private void HandleCollisionSkill(Collider2D collision)
    {
        bool isCheckMission = false;
        EnemyChar enemy = collision.gameObject.GetComponent<EnemyChar>();
        if (enemy != null)
        {
            PlayerController.Instance.CountCombo();
            enemy.enemyController.SetHit(Dame);
            if (!isCheckMission)
            {
                isCheckMission = true;
                GamePlayManager.Instance.SetMission(7, 1);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, 0), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
 