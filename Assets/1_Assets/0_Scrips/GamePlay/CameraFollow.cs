using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2f);

    [Header("Bounds")]
    [SerializeField] public bool useBounds = true;
    [SerializeField] public float minX = -10f;
    [SerializeField] public float maxX = 10f;
    [SerializeField] public float minY = -5f;
    [SerializeField] public float maxY = 5f;

    public Camera cam;
    private float halfHeight;
    private float halfWidth;
    public bool isFollow = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        CalculateCameraBounds();
    }

    private void CalculateCameraBounds()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (isFollow) return;
        // Tính toán vị trí đích cho camera
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // Di chuyển mượt đến vị trí đích
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Giới hạn camera để không hiển thị ra ngoài bounds
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX + halfWidth, maxX - halfWidth);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY + halfHeight, maxY - halfHeight);
        }
        transform.position = smoothedPosition;
    }
    private void OnDrawGizmos()
    {
        if (!useBounds) return;

        /*  Gizmos.color = Color.yellow;
          Gizmos.DrawWireCube(
              new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
              new Vector3(maxX - minX, maxY - minY, 1)
          );*/
        if (Application.isPlaying && cam != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
                new Vector3(maxX - minX - halfWidth * 2, maxY - minY - halfHeight * 2, 1)
            );
        }
    }
    // Cập nhật khi thay đổi kích thước màn hình
    private void OnValidate()
    {
        if (cam != null)
            CalculateCameraBounds();
    }
}