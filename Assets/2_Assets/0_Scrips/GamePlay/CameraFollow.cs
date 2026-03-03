using DG.Tweening;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2f);
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float threshold = 2f;
    private Vector3 velocity;

    [Header("Bounds")]
    [SerializeField] public bool useBounds = true;
    [SerializeField] public float minX = -10f;
    [SerializeField] public float maxX = 10f;
    [SerializeField] public float minY = -5f;
    [SerializeField] public float maxY = 5f;

    public Camera cam;
    private float halfHeight;
    private float halfWidth;
    public bool isFollow = true;

    private void Start()
    {
        cam = GetComponent<Camera>();
        CalculateCameraBounds();
        isFollow = true;
    }

    private void CalculateCameraBounds()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    private void FixedUpdate()
    {
        //        if (target == null) return;
        //        if (!isFollow) return;
        //        // Tính toán vị trí đích cho camera
        //        Vector3 desiredPosition = new Vector3(
        //            target.position.x + offset.x,
        //            target.position.y + offset.y,
        //            transform.position.z
        //        );
        //        //Vector3 desiredPosition = target.position + new Vector3(offset.x, offset.y, transform.position.z);

        //        float distance = Vector3.Distance(transform.position, desiredPosition);
        //        float smooth = distance < threshold ? smoothTime * 3f : smoothTime;

        //        // Di chuyển mượt đến vị trí đích
        //        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        //        //Vector3 smoothedPosition = Vector3.SmoothDamp(
        //        //    transform.position,
        //        //    desiredPosition,
        //        //    ref velocity,
        //        //    smooth
        //        //);
        //        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position,desiredPosition,ref velocity,smoothTime,Mathf.Infinity,Time.fixedDeltaTime
        //);
        //        // Giới hạn camera để không hiển thị ra ngoài bounds
        //        if (useBounds)
        //        {
        //            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX + halfWidth, maxX - halfWidth);
        //            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY + halfHeight, maxY - halfHeight);
        //        }
        //        transform.position = smoothedPosition;
        if (target == null || !isFollow) return;

        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        Vector3 desiredPosition = transform.position;
        Vector3 delta = targetPos - transform.position;

        if (Mathf.Abs(delta.x) > threshold)
        {
            desiredPosition.x = transform.position.x + Mathf.Sign(delta.x) * (Mathf.Abs(delta.x) - threshold);
        }

        if (Mathf.Abs(delta.y) > threshold)
        {
            desiredPosition.y = transform.position.y + Mathf.Sign(delta.y) * (Mathf.Abs(delta.y) - threshold);
        }

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX + halfWidth, maxX - halfWidth);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY + halfHeight, maxY - halfHeight);
        }

        transform.position = smoothedPosition;
    }
    //private void OnDrawGizmos()
    //{
    //    if (!useBounds) return;

    //    /*  Gizmos.color = Color.yellow;
    //      Gizmos.DrawWireCube(
    //          new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
    //          new Vector3(maxX - minX, maxY - minY, 1)
    //      );*/
    //    if (Application.isPlaying && cam != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(
    //            new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0),
    //            new Vector3(maxX - minX - halfWidth * 2, maxY - minY - halfHeight * 2, 1)
    //        );
    //    }
    //}

    // Cập nhật khi thay đổi kích thước màn hình
    private void OnValidate()
    {
        if (cam != null)
            CalculateCameraBounds();
    }

    /// <summary>
    ///  shkaing camera
    /// </summary>
    [Header("Defaults")]
    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultStrength = 0.35f;
    [SerializeField] private int defaultVibrato = 20;
    [SerializeField] private float defaultRandomness = 90f;

    private Tween shakeTween ,shakeTween2;


    public void Shake()
    {
        Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
    }

    public void Shake2()
    {
        Shake2(0.12f, 0.05f,45,0);
    }

    public void Shake(float duration, float strength, int vibrato = 15, float randomness = 0f)
    {
        KillShake();
        shakeTween = transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness,
            fadeOut: true
        );
    }

    public void Shake2(float duration, float strength, int vibrato = 15, float randomness = 0f)
    {
        KillShake2();
        shakeTween2 = transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness,
            fadeOut: true
        );
    }

    private void KillShake()
    {
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill();
    }

    private void KillShake2()
    {
        if (shakeTween2 != null && shakeTween2.IsActive())
            shakeTween2.Kill();
    }

}