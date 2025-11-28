using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public Transform player;
    Rigidbody2D rb;
    Vector2 moveDirection;
    bool isMove = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        MoveToRandomPosition();
        StartCoroutine(SetTimeStop());
    }
    void Update()
    {
        if (!isMove)
            transform.Translate(moveDirection * 10 * Time.deltaTime);
        else
        {
            if (Vector2.Distance(transform.position, player.position) < 2)
            {
                MoveToPlayer();
            }
            if (Vector2.Distance(transform.position, player.position) <= 0.3f)
            {
                GamePlayManager.Instance.AddCoin();
                Destroy(gameObject);
            }
        }
    }
    void MoveToRandomPosition()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMove)
            if (collision.collider.gameObject.CompareTag("Wall")
                || collision.collider.gameObject.CompareTag("WallTurn"))
            {
                isMove = true;
            }
    }
    void MoveToPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.linearVelocity = directionToPlayer * 5;
    }
    IEnumerator SetTimeStop()
    {
        float time = Random.Range(0.05f, 0.15f);
        yield return new WaitForSeconds(time);
        isMove = true;
    }
}
