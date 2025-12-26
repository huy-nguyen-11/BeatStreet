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
    private bool isMaxHit = false; 
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
            //if (!isSkill)
            //    HandleCollision(currentCollision);
            //else
            //    HandleCollisionSkill(currentCollision);
            HandleCollision(currentCollision);
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
            //if (id == 3)
            //    PlayerAttack();
            //else
            //    PlayerAttackDirection();
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
        //Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        //if (hits.Length == 0) return;
        //bool Direction = CheckDirection(hits);
        ////GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
        //// Use PlayerController API to set facing so internal flag and visual rotation stay in sync
        //if (PlayerController.Instance != null)
        //{
        //    if (!PlayerController.Instance.isSpeedUpAttack)
        //    {
        //        PlayerController.Instance.SetFacingDirection(Direction);
        //    }
        //}
        //else
        //{
        //    // fallback: keep previous behavior if instance missing
        //    GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
        //}
        //foreach (var hit in hits)
        //{
        //    float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;
        //    bool willEnqueue = (distanceX < 0 && Direction) || (distanceX >= 0 && !Direction);
        //    if (willEnqueue)
        //        collisionQueue.Enqueue(hit);
        //}
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        if (hits.Length == 0) return;

        // Dùng hướng hiện tại của Player (đã được aim từ TriggerAttack)
        bool direction = true; // true: đánh sang phải, false: đánh sang trái
        if (PlayerController.Instance != null)
        {
            direction = PlayerController.Instance.isFacingRight;
        }

        foreach (var hit in hits)
        {
            float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;

            // Enemy chỉ bị trúng nếu nằm phía trước mặt Player
            bool willEnqueue =
                (distanceX < 0 && direction) ||    
                (distanceX >= 0 && !direction);   

            if (willEnqueue)
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
    //public void PlayerAttack()
    //{
    //    Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
    //    if (hits.Length == 0) return;
    //    bool Direction = CheckDirection(hits);
    //    //GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
    //    // Use PlayerController API to set facing so internal flag and visual rotation stay in sync
    //    if (PlayerController.Instance != null)
    //    {
    //        if (!PlayerController.Instance.isSpeedUpAttack)
    //        {
    //            PlayerController.Instance.SetFacingDirection(Direction);
    //        }
    //    }
    //    else
    //    {
    //        // fallback
    //        GamePlayManager.Instance._Player.transform.rotation = Quaternion.Euler(new Vector3(0, !Direction ? -180 : 0, 0));
    //    }
    //    foreach (var hit in hits)
    //    {
    //        hit.GetComponent<EnemyChar>().enemyController.currentHitIndex = 4;
    //        float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;
    //        bool willEnqueue = (distanceX < 0 && Direction) || (distanceX >= 0 && !Direction);
          
    //        if (willEnqueue)
    //            collisionQueue.Enqueue(hit);
    //    }
    //}
    public void EnemyAttack()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskPlayer);
        if (hits.Length == 0) return;
        foreach (var hit in hits)
        {
            PlayerChar player = hit.gameObject.GetComponent<PlayerChar>();
            Transform enemy = transform.parent.parent;
            if (!player.playerController.isImmortal
                && Mathf.Abs(player.transform.position.x - enemy.position.x) <= 1f
                && Mathf.Abs(player.transform.position.y - enemy.transform.position.y) <= 0.2f)
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
        if (enemy != null)
        {
            bool playerOnRight = PlayerController.Instance.Char.position.x > enemy.transform.position.x;
            PlayerController.Instance.CountCombo();
            enemy.enemyController.SetHit(Dame, isMaxHit);
            if (isMaxHit)
            {
                isMaxHit = false;
            }
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
            // Truyền thông tin max_hit vào SetHit
            enemy.enemyController.SetHit(Dame, isMaxHit);
            // Reset flag sau khi sử dụng
            if (isMaxHit)
            {
                isMaxHit = false;
            }
            if (!isCheckMission)
            {
                isCheckMission = true;
                GamePlayManager.Instance.SetMission(7, 1);
            }
        }
    }

    public void SetMaxHit(bool value)
    {
        isMaxHit = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, 0), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}