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

    /// <summary>
    ///  shkaing camera
    /// </summary>
    [Header("Defaults")]
    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultStrength = 0.35f;
    [SerializeField] private int defaultVibrato = 20;
    [SerializeField] private float defaultRandomness = 90f;

    [Header("Directional")]
    [SerializeField] private float directionalPushRatio = 0.6f;

    private Vector3 originalLocalPos;
    private Tween shakeTween;

    private void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    #region BASIC SHAKE
    public void Shake()
    {
        Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
    }

    public void Shake(float duration, float strength, int vibrato = 20, float randomness = 90f)
    {
        KillShake();

        transform.localPosition = originalLocalPos;

        shakeTween = transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness,
            fadeOut: true
        ).OnComplete(ResetPosition);
    }
    #endregion

    #region DIRECTIONAL SHAKE (CORE)
    /// <summary>
    /// Rung theo hướng (direction = hướng tác động vào player)
    /// </summary>
    public void ShakeDirectional(
        Vector2 direction,
        float duration = -1f,
        float strength = -1f,
        int vibrato = -1,
        float randomness = -1f
    )
    {
        if (direction == Vector2.zero)
            direction = Random.insideUnitCircle.normalized;

        duration = duration > 0 ? duration : defaultDuration;
        strength = strength > 0 ? strength : defaultStrength;
        vibrato = vibrato > 0 ? vibrato : defaultVibrato;
        randomness = randomness > 0 ? randomness : defaultRandomness;

        KillShake();
        transform.localPosition = originalLocalPos;

        Vector3 dir = direction.normalized;

        // 1️⃣ Push nhanh theo hướng bị đánh
        Sequence seq = DOTween.Sequence();

        seq.Append(
            transform.DOLocalMove(
                originalLocalPos + dir * strength * directionalPushRatio,
                duration * 0.2f
            ).SetEase(Ease.OutQuad)
        );

        // 2️⃣ Rung ngược lại + ngẫu nhiên
        seq.Append(
            transform.DOShakePosition(
                duration * 0.8f,
                strength,
                vibrato,
                randomness,
                fadeOut: true
            )
        );

        seq.OnComplete(ResetPosition);
        shakeTween = seq;
    }
    #endregion

    #region HIT SHAKE (THEO LỰC)
    /// <summary>
    /// Rung theo lực đánh (damage / knockback)
    /// </summary>
    public void ShakeByForce(Vector2 direction, float force)
    {
        float strength = Mathf.Clamp(force * 0.02f, 0.15f, 1.2f);
        float duration = Mathf.Clamp(force * 0.01f, 0.1f, 0.4f);

        ShakeDirectional(direction, duration, strength);
    }
    #endregion

    #region UTIL
    private void KillShake()
    {
        if (shakeTween != null && shakeTween.IsActive())
            shakeTween.Kill();
    }

    private void ResetPosition()
    {
        transform.localPosition = originalLocalPos;
    }
    #endregion
}