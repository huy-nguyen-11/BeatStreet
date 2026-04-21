using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour, IpooledObject
{
    Transform player;
    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private bool isCollected = false;
    private bool hasLanded = false; 
    private float groundY; // Vị trí Y giả lập mặt đất

    private GameObject child0;
    private GameObject child1;
    private GameObject effect;

    public void OnObjectSpawn()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        child0 = transform.GetChild(0).gameObject;
        child1 = transform.GetChild(1).gameObject;
        effect = transform.GetChild(2).gameObject;

        isCollected = false;
        hasLanded = false;
        // Thêm độ lệch ngẫu nhiên cho mặt đất để tạo cảm giác có chiều sâu (perspective)
        float randomDepth = Random.Range(-0.15f, 0.15f);
        groundY = transform.position.y - 0.2f + randomDepth;

        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 1.5f;

        child0.SetActive(true);
        child1.SetActive(true);
        effect.SetActive(false);

        // Random nhẹ kích thước và góc xoay ban đầu
        transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
        child0.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

        PopOut();
    }

    void Update()
    {
        if (isCollected || player == null) return;

        // Logic giả lập rơi xuống đất
        if (!hasLanded)
        {
            if (rb.linearVelocity.y < 0 && transform.position.y <= groundY)
            {
                Landing();
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < 1f)
            {
                MoveToPlayer();
            }

            if (distance <= 0.35f)
            {
                CollectCoin();
            }
        }
    }

    void Landing()
    {
        hasLanded = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        // Snap về vị trí mặt đất giả lập để không bị lún sâu hơn
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
    }

    void CollectCoin()
    {
        if (isCollected) return;

        isCollected = true;


        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.simulated = false;

        GamePlayManager.Instance.AddCoin();


        child0.SetActive(false);
        child1.SetActive(false);


        effect.SetActive(true);

        StartCoroutine(DisableAfterEffect());
    }

    IEnumerator DisableAfterEffect()
    {
        yield return new WaitForSeconds(1f);

        effect.SetActive(false);
        gameObject.SetActive(false);
    }

    void PopOut()
    {
        // Tăng độ rộng văng ngang để các đồng xu không bị chụm lại một chỗ
        float forceX = Random.Range(-2.5f, 2.5f);
        float forceY = Random.Range(4f, 7f);
        rb.linearVelocity = new Vector2(forceX, forceY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vẫn giữ lại va chạm với tường để xu không bay xuyên tường khi văng ra
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("WallTurn"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void MoveToPlayer()
    {
        if (rb == null || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direction * 8f, Time.deltaTime * 5f);
        rb.gravityScale = 0;
    }
}
