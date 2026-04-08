using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
    public int level;
    public int TurnEnemy = 0;
    public int CountX = 0;
    [SerializeField] private List<GameObject> turnOrderedObjects = new List<GameObject>();
    public Transform listTurnEnemy;
    public Transform PointPlayer;
    public Transform listWallTurn;
    public Transform pointBoss;
    // Bounds
    [SerializeField] public float minX = -10f;
    [SerializeField] public float maxX = 10f;
    [SerializeField] public float minY = -5f;
    [SerializeField] public float maxY = 5f;
    public float[] _pointMinXs;
    public float[] _pointMaxXs;
    private float halfHeight;
    private float halfWidth;

    [ContextMenu("Auto Calculate Bounds")]
    public void AutoCalculateCameraPoints()
    {
        if (listWallTurn == null)
        {
            Debug.LogWarning("listWallTurn is null!");
            return;
        }

        int wallCount = listWallTurn.childCount;

        if (wallCount < 2)
        {
            Debug.LogWarning("Need at least 2 wall turns!");
            return;
        }

        // SỐ SEGMENT = SỐ WALL - 1
        int segmentCount = wallCount - 1;

        _pointMinXs = new float[segmentCount];
        _pointMaxXs = new float[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            Transform leftWall = listWallTurn.GetChild(i);
            Transform rightWall = listWallTurn.GetChild(i + 1);

            BoxCollider2D leftCol = leftWall.GetComponent<BoxCollider2D>();
            BoxCollider2D rightCol = rightWall.GetComponent<BoxCollider2D>();

            if (leftCol == null || rightCol == null)
            {
                Debug.LogWarning($"Missing BoxCollider2D at wall index {i}");
                continue;
            }

            //_pointMinXs[i] = leftCol.bounds.max.x;

            //_pointMaxXs[i] = rightCol.bounds.min.x;
            _pointMinXs[i] = leftWall.position.x;
            _pointMaxXs[i] = rightWall.position.x;
        }
        minX = _pointMinXs[0];
        maxX = _pointMaxXs[0];
    }

    void Start()
    {
        listTurnEnemy = transform.GetChild(1);
        listWallTurn = transform.GetChild(2);

        AutoCalculateCameraPoints();

        // CountX is the turn currently loaded in scene (not the upcoming turn).
        CountX = TurnEnemy;
        ApplyTurnVisibility(CountX);
        ApplyTurnOrderedObjects(TurnEnemy);

        minX = _pointMinXs[TurnEnemy];
        maxX = _pointMaxXs[TurnEnemy];
        GamePlayManager.Instance._CameraFollow.minX = minX;
        GamePlayManager.Instance._CameraFollow.maxX = maxX;
        GamePlayManager.Instance._CameraFollow.minY = minY;
        GamePlayManager.Instance._CameraFollow.maxY = maxY;

        if (GamePlayManager.Instance._Player != null)
            GamePlayManager.Instance._Player.transform.parent.position = PointPlayer.position;
    }
    //private void OnDrawGizmos()
    //{
    //    if (GamePlayManager.Instance! != null)
    //        if (!GamePlayManager.Instance._CameraFollow.useBounds) return;
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(
    //        new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
    //        new Vector3(maxX - minX, maxY - minY, 1)
    //    );
    //    if (Application.isPlaying && Camera.main != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(
    //            new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
    //            new Vector3(maxX - minX - halfWidth * 2, maxY - minY - halfHeight * 2, 1)
    //        );
    //    }
    //}

    public void SetCamera()
    {
        minX = _pointMinXs[TurnEnemy];
        maxX = _pointMaxXs[TurnEnemy];
        CountX = TurnEnemy;
        ApplyTurnVisibility(CountX);
        ApplyTurnOrderedObjects(TurnEnemy);
        GamePlayManager.Instance._CameraFollow.minX = minX;
        GamePlayManager.Instance._CameraFollow.maxX = maxX;
    }

    private void ApplyTurnVisibility(int loadedTurn)
    {
        if (listTurnEnemy == null) return;
        if (listTurnEnemy.childCount == 0) return;

        int loaded = Mathf.Clamp(loadedTurn, 0, listTurnEnemy.childCount - 1);
        for (int i = 0; i < listTurnEnemy.childCount; i++)
        {
            Transform turnRoot = listTurnEnemy.GetChild(i);
            if (turnRoot == null) continue;

            // Keep loaded/past turns active. Hide all future turns to save CPU.
            bool shouldActive = i <= loaded;
            if (turnRoot.gameObject.activeSelf != shouldActive)
            {
                turnRoot.gameObject.SetActive(shouldActive);
            }
        }
    }
    private void CalculateCameraBounds()
    {
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }

    private void ApplyTurnOrderedObjects(int currentTurn)
    {
        if (turnOrderedObjects == null || turnOrderedObjects.Count == 0) return;

        for (int i = 0; i < turnOrderedObjects.Count; i++)
        {
            GameObject turnObject = turnOrderedObjects[i];
            if (turnObject == null) continue;

            bool shouldActive = i == currentTurn;
            if (turnObject.activeSelf != shouldActive)
            {
                turnObject.SetActive(shouldActive);
            }
        }
    }
    private void OnValidate()
    {
        if (Camera.main != null)
            CalculateCameraBounds();
    }

    public void WakeUpEnemyInTurn()
    {
        if (listTurnEnemy == null || TurnEnemy < 0 || TurnEnemy >= listTurnEnemy.childCount)
            return;

        Transform currentTurn = listTurnEnemy.GetChild(TurnEnemy);
        if (currentTurn == null)
            return;

        for (int i = 0; i < currentTurn.childCount; i++)
        {
            Transform enemyTransform = currentTurn.GetChild(i);
            if (enemyTransform == null)
                continue;

            EnemyChar enemyChar = enemyTransform.GetComponent<EnemyChar>();
            if (enemyChar == null || enemyChar.enemyController == null)
                continue;

            bool isDead = enemyChar.enemyController.state == EnemyController.State.Dead;
            if (isDead)
                continue; 

            bool isHidden = !enemyTransform.gameObject.activeSelf;
            bool isCharHidden = enemyChar.enemyController.Char != null && 
                                !enemyChar.enemyController.Char.gameObject.activeSelf;
            if (isHidden || isCharHidden)
            {
                if (isHidden)
                {
                    enemyTransform.gameObject.SetActive(true);
                }
                
                if (enemyChar.enemyController.Char != null)
                {
                    enemyChar.enemyController.Char.gameObject.SetActive(true);
                }

                enemyChar.enemyController.InitializeEnemy();
                
                // Mark enemy as spawned and switch to Spawn state instead of Run
                enemyChar.enemyController.isSpawned = true;
                enemyChar.enemyController.SwitchToRunState(enemyChar.enemyController.enemySpawn);
                
                return;
            }
        }
    }
}
