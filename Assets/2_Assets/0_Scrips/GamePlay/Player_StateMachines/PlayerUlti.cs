using System.Collections;
using UnityEngine;

public class PlayerUlti : PlayerStateManager
{
    private const float IdealDistanceX = 2.5f;
    private const float MaxDeltaY = 0.1f;
    private const float NearEnemyDistanceX = 1f;
    private const float DashSpeed = 7f;
    private const float MinWalkDuration = 0.2f;
    private const float MaxWalkDuration = 0.5f;
    private const float MinDashDuration = 0.1f;
    int count_hit = 0;

    public PlayerUlti(PlayerController player) : base(player) { }
    public override void Enter()
    {
        count_hit = 0;
        playerController.state = PlayerController.State.Ulti;
        playerController.StartCoroutine(WaitForResetSkin());
        if (playerController.skeletonAnimation != null && playerController.skeletonAnimation.AnimationState != null)
        {
            playerController.skeletonAnimation.AnimationState.Event += HandleEvent;
        }
    }

    IEnumerator WaitForResetSkin()
    {
        EnemyController enemy = GamePlayManager.Instance != null ? GamePlayManager.Instance._Enemy : null;
        Transform enemyTransform = enemy != null ? enemy.transform : null;
        Transform mover = playerController.Char != null ? playerController.Char : playerController.transform;

        if (enemyTransform != null)
        {
            Vector3 idealPosition = ResolveWalkIdealPosition(enemyTransform, mover.position);
            playerController.PlayAnim("Walk", true);
            float walkSpeed = 3.6f;
            float walkElapsed = 0f;
            while ((Mathf.Abs(mover.position.x - idealPosition.x) > 0.02f || walkElapsed < MinWalkDuration) && walkElapsed < MaxWalkDuration)
            {
                Vector3 beforeMove = mover.position;
                if (Mathf.Abs(beforeMove.x - idealPosition.x) > 0.02f)
                {
                    mover.position = Vector3.MoveTowards(beforeMove, idealPosition, walkSpeed * Time.deltaTime);
                }
                Vector3 clampedPos = mover.position;
                clampedPos.y = Mathf.Clamp(clampedPos.y, enemyTransform.position.y - MaxDeltaY, enemyTransform.position.y + MaxDeltaY);
                mover.position = clampedPos;
                playerController.SetFacingDirection(enemyTransform.position.x > mover.position.x);
                walkElapsed += Time.deltaTime;
                yield return null;
            }

            playerController.PlayAnim("Strength", false);
            yield return WaitForChangeAnimEnd(0.45f);
        }

        if (enemyTransform != null)
        {
            float side = mover.position.x < enemyTransform.position.x ? -1f : 1f;
            float dashElapsed = 0f;
            while (true)
            {
                float targetX = enemyTransform.position.x + (side * NearEnemyDistanceX);
                Vector3 currentPos = mover.position;

                if (GamePlayManager.Instance != null)
                {
                    targetX = Mathf.Clamp(targetX, GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX);
                }

                float nextX = Mathf.MoveTowards(currentPos.x, targetX, DashSpeed * Time.deltaTime);
                mover.position = new Vector3(nextX, currentPos.y, currentPos.z);
                playerController.SetFacingDirection(enemyTransform.position.x > mover.position.x);
                dashElapsed += Time.deltaTime;

                if (Mathf.Abs(nextX - targetX) <= 0.02f && dashElapsed >= MinDashDuration)
                {
                    break;
                }
                yield return null;
            }
        }

        playerController.PlayAnim(playerController.animUlti, false);
        playerController.idAttackArea = 1;
        yield return new WaitForSeconds(3.5f);
        AudioBase.Instance.AudioPlayer(17);
        yield return new WaitForSeconds(0.8f);
        GamePlayManager.Instance.SetStopFollowCamera();
        GamePlayManager.Instance.isCheckUlti = false;
        GamePlayManager.Instance.ResetAfterUlti();
        playerController.SwitchToRunState(playerController.playerIdle);
        GamePlayManager.Instance.SetFollowCamera();
    }

    private IEnumerator WaitForChangeAnimEnd(float timeout)
    {
        float elapsed = 0f;
        if (playerController.skeletonAnimation == null || playerController.skeletonAnimation.AnimationState == null)
        {
            yield return new WaitForSeconds(0.25f);
            yield break;
        }

        while (elapsed < timeout)
        {
            var current = playerController.skeletonAnimation.AnimationState.GetCurrent(0);
            if (current != null && current.Animation != null && current.Animation.Name == "Strength")
            {
                float duration = current.Animation.Duration;
                if (duration <= 0f)
                    yield break;

                while (elapsed < timeout)
                {
                    current = playerController.skeletonAnimation.AnimationState.GetCurrent(0);
                    if (current == null || current.Animation == null || current.Animation.Name != "Strength")
                        yield break;
                    if (current.TrackTime >= duration)
                        yield break;

                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private Vector3 ResolveWalkIdealPosition(Transform enemyTransform, Vector3 moverPosition)
    {
        if (playerController.TryGetIdealUltiPosition(enemyTransform, out Vector3 idealPosition, IdealDistanceX, MaxDeltaY))
        {
            return idealPosition;
        }

        float targetY = Mathf.Clamp(moverPosition.y, enemyTransform.position.y - MaxDeltaY, enemyTransform.position.y + MaxDeltaY);
        Vector3 leftCandidate = new Vector3(enemyTransform.position.x - IdealDistanceX, targetY, moverPosition.z);
        Vector3 rightCandidate = new Vector3(enemyTransform.position.x + IdealDistanceX, targetY, moverPosition.z);

        if (GamePlayManager.Instance != null)
        {
            leftCandidate.x = Mathf.Clamp(leftCandidate.x, GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX);
            rightCandidate.x = Mathf.Clamp(rightCandidate.x, GamePlayManager.Instance.minPosX, GamePlayManager.Instance.maxPosX);
            if (GamePlayManager.Instance.maxPosY > GamePlayManager.Instance.minPosY)
            {
                float clampedY = Mathf.Clamp(targetY, GamePlayManager.Instance.minPosY, GamePlayManager.Instance.maxPosY);
                leftCandidate.y = clampedY;
                rightCandidate.y = clampedY;
            }
        }

        bool preferLeft = moverPosition.x < enemyTransform.position.x;
        Vector3 preferred = preferLeft ? leftCandidate : rightCandidate;
        Vector3 opposite = preferLeft ? rightCandidate : leftCandidate;

        bool preferredBlocked = IsPathBlocked(moverPosition, preferred);
        bool oppositeBlocked = IsPathBlocked(moverPosition, opposite);

        if (!preferredBlocked)
            return preferred;
        if (!oppositeBlocked)
            return opposite;

        float preferredDx = Mathf.Abs(preferred.x - enemyTransform.position.x);
        float oppositeDx = Mathf.Abs(opposite.x - enemyTransform.position.x);
        return preferredDx >= oppositeDx ? preferred : opposite;
    }

    private bool IsPathBlocked(Vector3 from, Vector3 to)
    {
        if (playerController.obstacleLayer.value == 0)
            return false;

        RaycastHit2D hit = Physics2D.Linecast(from, to, playerController.obstacleLayer);
        return hit.collider != null;
    }

    void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Hit")
        {
            //if (GamePlayManager.Instance._Enemy.isBoss)
            //{
            //    GamePlayManager.Instance._Enemy.PlayAnim("Damaged", true);
            //}
            //else
            //{
            //    GamePlayManager.Instance._Enemy.PlayAnim("Hit", true);
            //}
            var enemy = GamePlayManager.Instance._Enemy;
            if (enemy == null) return;

            // Choose animation name based on boss or normal enemy
            string anim = enemy.isBoss ? "Damaged" : "Hit";
            ObjectPooler.Instance.SpawnFromPool("Hit", new Vector3(enemy.transform.position.x, enemy.transform.position.y + 0.3f, 0), Quaternion.Euler(0, 0, 0));
            count_hit += 6;
            if(count_hit > 11)
            {
                count_hit = 6;
            }
            AudioBase.Instance.AudioPlayer(count_hit);
            // Ensure enemy state is set to Hit (but don't force if dead or falling)
            if (enemy.state != EnemyController.State.Dead && enemy.state != EnemyController.State.Fall)
            {
                // allow enemy state machine to handle hit logic (knockback, timers, etc.)
                enemy.SwitchToRunState(enemy.enemyHit);
            }

            // Play overlay (track 1) non-looping so repeated hits will layer and feel intense.
            // EnemyCharacter.PlayAnimOnTrack will queue an empty animation after the duration to clear the overlay.
            enemy.PlayAnimOnTrack(anim, trackIndex: 1, loop: false);

            float dirSign = Mathf.Sign(playerController.transform.right.x);
            float num = playerController.id == 2 ? 0.135f : 0.02f; // can change with id player
            enemy.transform.position += new Vector3(dirSign * num, 0f, 0f); // can change with id player

        }
        else if(e.Data.Name == "Hit_Max")
        {
            AudioBase.Instance.AudioPlayer(10);
            GamePlayManager.Instance._Enemy.SetDead();
        }
    }

    public override void Update()
    {

    }
    public override void Exit()
    {
        if (playerController.skeletonAnimation != null && playerController.skeletonAnimation.AnimationState != null)
        {
            playerController.skeletonAnimation.AnimationState.Event -= HandleEvent;
        }
        //GamePlayManager.Instance.backUlti0.SetActive(false);
        //GamePlayManager.Instance.backUlti1.SetActive(false);
        GamePlayManager.Instance.ReseBackUltiShow();
        GamePlayManager.Instance.SetPlayerToDefaultSortingLayer();
    }

    public override void FixedUpdate()
    {
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public override void OnTriggerEnter(Collider2D collision)
    {
    }

    public override void OnTriggerExit(Collider2D collision)
    {
    }

    public override void OnTriggerStay(Collider2D collision)
    {
    }
}
