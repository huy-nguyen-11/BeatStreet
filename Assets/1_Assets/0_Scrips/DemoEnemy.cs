using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoEnemy : MonoBehaviour
{
    public Transform player;
    public float horizontalOffset = 2f;
    public float verticalOffset = 1f;
    public float checkRadius = 0.5f;
    public LayerMask obstacleMask;
    public float moveSpeed = 2f;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;
    private Vector3? currentReservedTarget;

    private bool isAttacking = false;


    void Update()
    {
        if (isMoving)
        {
            FollowPath();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < 1f && !EnemyManager.Instance.IsAnyEnemyTooClose())
            {
                if (!isAttacking)
                {
                    Debug.Log($"{name} tấn công player!");
                    isAttacking = true;
                }
            }
            else
            {
                if (currentReservedTarget != null)
                {
                    EnemyManager.Instance.ReleasePosition(currentReservedTarget.Value);
                    currentReservedTarget = null;
                }
                isAttacking = false;
                DecideMovement();
            }
        }
    }


    void DecideMovement()
    {
        Vector3 leftTarget = new Vector3(player.position.x - horizontalOffset, player.position.y, 0);
        Vector3 rightTarget = new Vector3(player.position.x + horizontalOffset, player.position.y, 0);

        bool leftBlocked = Physics2D.OverlapCircle(leftTarget, checkRadius, obstacleMask);
        bool rightBlocked = Physics2D.OverlapCircle(rightTarget, checkRadius, obstacleMask);

        if (!leftBlocked && EnemyManager.Instance.TryReservePosition(leftTarget))
        {
            //Debug.Log($"{name} đã reserve vị trí bên trái: {leftTarget}");
            if (IsPathBlockedByPlayer(leftTarget))
            {
                CreateDetourPath(leftTarget);
            }
            else
            {
                pathQueue.Enqueue(leftTarget);
            }
            currentReservedTarget = leftTarget;
        }
        else if (!rightBlocked && EnemyManager.Instance.TryReservePosition(rightTarget))
        {
            //Debug.Log($"{name} đã reserve vị trí bên phải: {rightTarget}");
            if (IsPathBlockedByPlayer(rightTarget))
            {
                CreateDetourPath(rightTarget);
            }
            else
            {
                pathQueue.Enqueue(rightTarget);
            }
            currentReservedTarget = rightTarget;
        }
        else if (!leftBlocked || !rightBlocked)
        {
            Debug.Log($"{name} không tìm được vị trí cạnh player! Đang chờ...");
        }

        isMoving = pathQueue.Count > 0;
    }

    bool IsPathBlockedByPlayer(Vector3 target)
    {
        Vector2 direction = (target - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }


    void CreateDetourPath(Vector3 finalTarget)
    {
        Vector3 up = new Vector3(transform.position.x, transform.position.y + verticalOffset, 0);
        Vector3 side = new Vector3(finalTarget.x, up.y, 0);
        pathQueue.Enqueue(up);
        pathQueue.Enqueue(side);
        pathQueue.Enqueue(finalTarget);
    }

    void FollowPath()
    {
        if (pathQueue.Count == 0)
        {
            isMoving = false;

            if (currentReservedTarget != null && Vector3.Distance(transform.position, currentReservedTarget.Value) < 0.1f)
            {
                // Không đánh dấu hasArrived = true nữa
                // Giữ nguyên vị trí để chuyển sang trạng thái kiểm tra player
            }

            return;
        }

        Vector3 nextTarget = pathQueue.Peek();
        transform.position = Vector3.MoveTowards(transform.position, nextTarget, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextTarget) < 0.05f)
        {
            pathQueue.Dequeue();
        }
    }

    private void OnDisable()
    {
        if (currentReservedTarget != null)
        {
            EnemyManager.Instance.ReleasePosition(currentReservedTarget.Value);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (currentReservedTarget != null)
        {
            Gizmos.DrawWireSphere(currentReservedTarget.Value, 0.2f);
        }
    }
}
