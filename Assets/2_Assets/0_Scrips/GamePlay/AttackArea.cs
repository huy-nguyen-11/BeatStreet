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
    private bool isSkillStrength = false;
    private bool isMaxHit = false;

    public bool isNormalAttack = false;

    //for skill jump 
    private bool isJumpHitActive = false;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    //for enemey toggle
    private bool isEnemyToggleAttack = false;

    private bool isEnemyTongueHitActive = false;
    private bool hasHitPlayerThisTongue = false;

    private const float CHASE_HIT_COOLDOWN = 0.4f;
    private float lastChaseHitTime = -999f;

    /// <summary>On enemy attack tongue. </summary>
    public void StartEnemyTongueHit(float dame)
    {
        Dame = dame;
        isEnemyTongueHitActive = true;
        hasHitPlayerThisTongue = false;
    }

    public void EndEnemyTongueHit()
    {
        isEnemyTongueHitActive = false;
    }

    private void CheckEnemyTongueHitPlayer()
    {
        if (hasHitPlayerThisTongue) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskPlayer);
        if (hits.Length == 0) return;
        foreach (var hit in hits)
        {
            PlayerChar player = hit.gameObject.GetComponent<PlayerChar>();
            if (player == null) continue;

            Transform enemy = transform.parent.parent;
            EnemyController enemyController = enemy.transform.GetChild(0).GetComponent<EnemyController>();

            if (player.playerController.isImmortal) continue;
            if (Mathf.Abs(player.transform.position.y - enemy.transform.position.y) > 0.35f) continue;

            hasHitPlayerThisTongue = true;
            AudioBase.Instance.AudioEnemy(1);
            bool direction = transform.parent.rotation.y == 0 ? true : false;
            player.playerController.HitDirection = direction;
            player.playerController.SetHit(Dame);
            ObjectPooler.Instance.SpawnFromPool("Hit", player.transform.position, Quaternion.Euler(0, 0, 0));
            break;
        }
    }

    private void CheckEnemyChaseHitPlayer()
    {
        EnemyController enemyController = GetComponentInParent<EnemyController>();
        if (enemyController == null) return;
        if (enemyController.typeOfEnemy != TypeOfEnemy.Boss || enemyController.idEnemy != 2 || !enemyController.shouldDirectChase) return;
        if (enemyController.isGrabbed || enemyController.isBeingThrown) return;
        if (enemyController.state != EnemyController.State.Run) return;

        if (Time.time - lastChaseHitTime < CHASE_HIT_COOLDOWN) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskPlayer);
        if (hits.Length == 0) return;

        Vector3 enemyPos = enemyController.Char != null ? enemyController.Char.position : enemyController.transform.position;
        foreach (var hit in hits)
        {
            PlayerChar player = hit.gameObject.GetComponent<PlayerChar>();
            if (player == null) continue;
            if (player.playerController.isImmortal) continue;
            if (Mathf.Abs(player.transform.position.y - enemyPos.y) > 0.35f) continue;

            lastChaseHitTime = Time.time;
            AudioBase.Instance.AudioEnemy(1);
            bool direction = enemyController.Char != null && Mathf.Abs(Mathf.DeltaAngle(enemyController.Char.localEulerAngles.y, 0f)) < 90f;
            player.playerController.HitDirection = direction;
            player.playerController.HitCount = 2;
            player.playerController.SetHit(enemyController.dame);
            ObjectPooler.Instance.SpawnFromPool("Hit", player.transform.position, Quaternion.Euler(0, 0, 0));
            enemyController.shouldDirectChase = false;
            break;
        }
    }

    private void Update()
    {

        //if (collisionQueue.Count > 0)
        //{
        //    if (!isCheckTrigger)
        //    {
        //        PlayerController.Instance.PerformAttack();
        //        isCheckTrigger = true;
        //    }
        //    Collider2D currentCollision = collisionQueue.Dequeue();
        //    //if (!isSkill)
        //    //    HandleCollision(currentCollision);
        //    //else
        //    //    HandleCollisionSkill(currentCollision);
        //    HandleCollision(currentCollision);
        //}
        //else
        //{
        //    if (isCheckTrigger)
        //        isCheckTrigger = false;
        //}
        // ===== Jump skill continuous hit =====
        if (isJumpHitActive)
        {
            CheckJumpHitEnemies();
        }

        if (isEnemyTongueHitActive)
        {
            CheckEnemyTongueHitPlayer();
        }

        CheckEnemyChaseHitPlayer();

        if (collisionQueue.Count > 0)
        {
            if (!isCheckTrigger)
            {
                PlayerController.Instance.PerformAttack();
                isCheckTrigger = true;
            }

            Collider2D currentCollision = collisionQueue.Dequeue();
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
            PlayerAttackDirection();
        }
        else
            EnemyAttack();
    }

    public void SetAttackSkill(float dame, int id)
    {
        isSkillStrength = true;
        Dame = dame;
        PlayerAttackSkillStrengthMax();
    }

    public void SetAttackSkillSpeedUp(float dame, int id)
    {
        Dame = dame;
        PlayerAttackBothSides();
    }

    public void SetAttackSkillJump(float dame, int id)
    {
        Dame = dame;
        PlayerAttackJump();
    }

    public void StartHitJump(float dame)
    {
        Dame = dame;
        isJumpHitActive = true;
        hitEnemies.Clear();
    }

    public void EndHitJump()
    {
        isJumpHitActive = false;
    }

    private void CheckJumpHitEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            boxSize,
            0,
            layerMaskEnemy
        );

        foreach (var hit in hits)
        {
            if (hitEnemies.Contains(hit))
                continue; // enemy này đã bị xử lý rồi

            hitEnemies.Add(hit);
            collisionQueue.Enqueue(hit);
        }
    }



    public void PlayerAttackDirection()
    {
        //Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        //if (hits.Length == 0) return;
        //Debug.Log("name attack arena: " + gameObject.name);
        //Debug.Log("hits.Length: " + hits.Length);
        //bool direction = true;
        //if (PlayerController.Instance != null)
        //{
        //    direction = PlayerController.Instance.isFacingRight;
        //}

        //foreach (var hit in hits)
        //{
        //    float distanceX = GamePlayManager.Instance._Player.transform.position.x - hit.transform.position.x;

        //    // attack enemy only in facing direction    
        //    bool willEnqueue =
        //        (distanceX < 0 && direction) ||    
        //        (distanceX >= 0 && !direction);   

        //    if (willEnqueue)
        //        collisionQueue.Enqueue(hit);
        //}
        Collider2D[] hits = Physics2D.OverlapBoxAll(
       transform.position,
       boxSize,
       0,
       layerMaskEnemy
   );

        if (hits.Length == 0) return;

        bool isFacingRight = true;
        if (PlayerController.Instance != null)
            isFacingRight = PlayerController.Instance.isFacingRight;

        Vector2 playerPos = GamePlayManager.Instance._Player.transform.position;

        float laneTolerance = 0.3f;     // cho phép lệch trục Y (tùy chỉnh)
        float minAttackX = 0.05f;       // tránh enemy đứng trùng tâm player

        foreach (var hit in hits)
        {
            Vector2 enemyPos = hit.transform.position;

            float deltaX = enemyPos.x - playerPos.x;
            float deltaY = Mathf.Abs(enemyPos.y - playerPos.y);

            // 1. cùng lane (chiều sâu)
            bool sameLane = deltaY <= laneTolerance;

            // 2. phía trước mặt
            bool inFront = 
                (deltaX > minAttackX && isFacingRight) ||
                (deltaX < -minAttackX && !isFacingRight);

            if (sameLane && inFront)
            {
                collisionQueue.Enqueue(hit);
            }
        }
    }

    public void PlayerAttackSkillStrengthMax()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, layerMaskEnemy);
        if (hits.Length == 0) return;
        foreach (var hit in hits)
        {
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

    public void PlayerAttackBothSides()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            boxSize,
            0,
            layerMaskEnemy
        );

        if (hits.Length == 0) return;

        Vector2 playerPos = GamePlayManager.Instance._Player.transform.position;

        float laneTolerance = 0.5f;   // giữ nguyên điều kiện lane
        float minAttackX = 0.05f;     // tránh trùng tâm player

        foreach (var hit in hits)
        {
            Vector2 enemyPos = hit.transform.position;

            float deltaX = Mathf.Abs(enemyPos.x - playerPos.x);
            float deltaY = Mathf.Abs(enemyPos.y - playerPos.y);

            // cùng lane theo trục Y
            bool sameLane = deltaY <= laneTolerance;

            // có khoảng cách theo trục X (trước hoặc sau đều được)
            bool validX = deltaX > minAttackX;

            if (sameLane && validX)
            {
                collisionQueue.Enqueue(hit);
            }
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
            EnemyController enemyController = enemy.transform.GetChild(0).GetComponent<EnemyController>();
            if (!player.playerController.isImmortal
                //&& Mathf.Abs(player.transform.position.x - enemy.position.x) <= 1.2f
                && Mathf.Abs(player.transform.position.y - enemy.transform.position.y) <= 0.35f)
            {
                AudioBase.Instance.AudioEnemy(1);
                bool direction = transform.parent.rotation.y == 0 ? true : false;
                player.playerController.HitDirection = direction;
                if(enemyController.typeOfEnemy == TypeOfEnemy.Boss && enemyController.idEnemy == 2)
                {
                    player.playerController.HitCount = 2;
                }
                player.playerController.SetHit(Dame);
                ObjectPooler.Instance.SpawnFromPool("Hit", transform.position, Quaternion.Euler(0, 0, 0));
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

    //public IEnumerator SetAttackSkill2Player(float dame)
    //{
    //    Dame = dame;
    //    PlayerAttackJump();
    //    yield return new WaitForSeconds(0.2f);
    //    PlayerAttackJump();
    //    yield return new WaitForSeconds(0.2f);
    //    PlayerAttackJump();
    //    yield return new WaitForSeconds(0.2f);
    //    PlayerAttackJump();
    //}
    private void HandleCollision(Collider2D collision)
    {
        bool isCheckMission = false;
        EnemyChar enemy = collision.gameObject.GetComponent<EnemyChar>();
        if (enemy != null)
        {
            if (enemy.enemyController.state == EnemyController.State.Fall)
                return;

            bool playerOnRight = PlayerController.Instance.Char.position.x > enemy.transform.position.x;
            PlayerController.Instance.CountCombo();
            // IMPORTANT: set this flag BEFORE calling SetHit().
            // EnemyController.SetHit(...) may immediately switch to EnemyFall (max hit),
            // and EnemyFall.Enter() reads isGetHitStrengthMax to decide knockback direction.
            enemy.enemyController.isGetHitStrengthMax = isSkillStrength;
            enemy.enemyController.SetHit(Dame, isMaxHit);
            if(isNormalAttack)
            {
                ObjectPooler.Instance.SpawnFromPool("Hit", new Vector3(collision.transform.position.x , collision.transform.position.y + 0.5f , 0f), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                ObjectPooler.Instance.SpawnFromPool("Hit", transform.position, Quaternion.Euler(0, 0, 0));
            }
            if (isMaxHit)
            {
                StartCoroutine(ResetMaxHitFlag());
            }
            if (!isCheckMission)
            {
                isCheckMission = true;
                GamePlayManager.Instance.SetMission(7, 1);
            }
        }
    }

    private IEnumerator ResetMaxHitFlag()
    {
        yield return new WaitForSeconds(1);
        isMaxHit = false; 
        isSkillStrength = false;
    }

    //private void HandleCollisionSkill(Collider2D collision)
    //{
    //    bool isCheckMission = false;
    //    EnemyChar enemy = collision.gameObject.GetComponent<EnemyChar>();
    //    if (enemy != null)
    //    {
    //        PlayerController.Instance.CountCombo();
    //        // Truyền thông tin max_hit vào SetHit
    //        enemy.enemyController.SetHit(Dame, isMaxHit);
    //        // Reset flag sau khi sử dụng
    //        if (isMaxHit)
    //        {
    //            isMaxHit = false;
    //        }
    //        if (!isCheckMission)
    //        {
    //            isCheckMission = true;
    //            GamePlayManager.Instance.SetMission(7, 1);
    //        }
    //    }
    //}

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