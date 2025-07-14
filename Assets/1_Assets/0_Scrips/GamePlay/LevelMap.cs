using UnityEngine;

public class LevelMap : MonoBehaviour
{
    public int level;
    public int TurnEnemy = 0;
    public int CountX = 0;
    public Transform listTurnEnemy;
    public Transform PointPlayer;
    // Bounds
    [SerializeField] public float minX = -10f;
    [SerializeField] public float maxX = 10f;
    [SerializeField] public float minY = -5f;
    [SerializeField] public float maxY = 5f;
    public float[] _pointMinXs;
    public float[] _pointMaxXs;
    private float halfHeight;
    private float halfWidth;
    void Start()
    {
        minX = _pointMinXs[TurnEnemy];
        maxX = _pointMaxXs[TurnEnemy];
        GamePlayManager.Instance._CameraFollow.minX = minX;
        GamePlayManager.Instance._CameraFollow.maxX = maxX;
        GamePlayManager.Instance._CameraFollow.minY = minY;
        GamePlayManager.Instance._CameraFollow.maxY = maxY;
        listTurnEnemy = transform.GetChild(0);
        GamePlayManager.Instance._Player.transform.parent.position = PointPlayer.position;
    }
    private void OnDrawGizmos()
    {
        if (GamePlayManager.Instance! != null)
            if (!GamePlayManager.Instance._CameraFollow.useBounds) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
            new Vector3(maxX - minX, maxY - minY, 1)
        );
        if (Application.isPlaying && Camera.main != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
                new Vector3(maxX - minX - halfWidth * 2, maxY - minY - halfHeight * 2, 1)
            );
        }
    }
    public void NextTurn()
    {

    }
    public void SetCamera()
    {
        minX = _pointMinXs[TurnEnemy];
        maxX = _pointMaxXs[TurnEnemy];
        CountX = TurnEnemy;
        GamePlayManager.Instance._CameraFollow.minX = minX;
        GamePlayManager.Instance._CameraFollow.maxX = maxX;
    }
    private void CalculateCameraBounds()
    {
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }
    private void OnValidate()
    {
        if (Camera.main != null)
            CalculateCameraBounds();
    }

}
