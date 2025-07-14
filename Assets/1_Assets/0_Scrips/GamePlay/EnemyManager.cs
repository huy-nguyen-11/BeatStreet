using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Enemy List (Assign manually)")]
    public List<EnemyChar> enemyList = new List<EnemyChar>();  // Kéo thả các enemy tại đây
    private Vector3? leftReserved;
    private Vector3? rightReserved;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (enemyList == null || enemyList.Count == 0)
        {
            enemyList = new List<EnemyChar>(FindObjectsOfType<EnemyChar>());
        }
    }

    public bool TryReservePosition(Vector3 position)
    {
        if (ApproximatelyEqual(position, leftReserved)) return false;
        if (ApproximatelyEqual(position, rightReserved)) return false;

        if (leftReserved == null)
        {
            leftReserved = position;
            return true;
        }
        else if (rightReserved == null)
        {
            rightReserved = position;
            return true;
        }

        return false;
    }

    public void ReleasePosition(Vector3 position)
    {
        if (ApproximatelyEqual(position, leftReserved)) leftReserved = null;
        if (ApproximatelyEqual(position, rightReserved)) rightReserved = null;
    }

    private bool ApproximatelyEqual(Vector3 a, Vector3? b)
    {
        if (b == null) return false;
        return Vector3.Distance(a, b.Value) < 0.1f;
    }

    public bool IsAnyEnemyTooClose()
    {
        if(enemyList != null || enemyList.Count > 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                for (int j = i + 1; j < enemyList.Count; j++)
                {
                    if (Vector3.Distance(enemyList[i].transform.position, enemyList[j].transform.position) < 1f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        return true;
  
    }
}
