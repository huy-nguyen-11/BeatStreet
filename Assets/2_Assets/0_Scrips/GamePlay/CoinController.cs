using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour, IpooledObject
{
    //public Transform player;
    //Rigidbody2D rb;
    //Vector2 moveDirection;
    //bool isMove = false;
    //bool isCollected = false;


    //public void OnObjectSpawn()
    //{
    //    player = GameObject.FindGameObjectWithTag("Player")?.transform;
    //    rb = GetComponent<Rigidbody2D>();
    //    MoveToRandomPosition();
    //    StartCoroutine(SetTimeStop());
    //    isCollected = false;
    //    gameObject.transform.GetChild(0).gameObject.SetActive(true);
    //    gameObject.transform.GetChild(1).gameObject.SetActive(true);
    //    gameObject.transform.GetChild(2).gameObject.SetActive(false);//effect
    //}

    //void Start()
    //{

    //}
    //void Update()
    //{
    //    if(isCollected) return;

    //    if (!isMove)
    //        transform.Translate(moveDirection * 10 * Time.deltaTime);
    //    else
    //    {
    //        if (Vector2.Distance(transform.position, player.position) < 2)
    //        {
    //            MoveToPlayer();
    //        }
    //        if (Vector2.Distance(transform.position, player.position) <= 0.3f)
    //        {
    //            CollectCoin();
    //        }
    //    }
    //}

    //void CollectCoin()
    //{
    //    if (isCollected) return; // 🔥 double check

    //    isCollected = true;

    //    GamePlayManager.Instance.AddCoin();


    //    transform.GetChild(0).gameObject.SetActive(false);
    //    transform.GetChild(1).gameObject.SetActive(false);
    //    gameObject.transform.GetChild(2).gameObject.SetActive(true);//effect

    //    StartCoroutine(DisableAfterEffect());
    //}

    //IEnumerator DisableAfterEffect()
    //{
    //    yield return new WaitForSeconds(1f);

    //    gameObject.transform.GetChild(2).gameObject.SetActive(false);//effect
    //    gameObject.SetActive(false);
    //}

    //void MoveToRandomPosition()
    //{
    //    moveDirection = Random.insideUnitCircle.normalized;
    //}
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!isMove)
    //        if (collision.collider.gameObject.CompareTag("Wall")
    //            || collision.collider.gameObject.CompareTag("WallTurn"))
    //        {
    //            isMove = true;
    //        }
    //}
    //void MoveToPlayer()
    //{
    //    Vector2 directionToPlayer = (player.position - transform.position).normalized;
    //    rb.linearVelocity = directionToPlayer * 5;
    //}
    //IEnumerator SetTimeStop()
    //{
    //    float time = Random.Range(0.05f, 0.1f);
    //    yield return new WaitForSeconds(time);
    //    isMove = true;
    //}
    Transform player;
    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private bool isMove = false;
    private bool isCollected = false;

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
        isMove = false;

        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        child0.SetActive(true);
        child1.SetActive(true);
        effect.SetActive(false);

        MoveToRandomPosition();
        StartCoroutine(SetTimeStop());
    }

    void Update()
    {
        if (isCollected || player == null) return;

        if (!isMove)
        {
            transform.Translate(moveDirection * 10 * Time.deltaTime);
        }
        else
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < 2f)
            {
                MoveToPlayer();
            }

            if (distance <= 0.3f)
            {
                CollectCoin();
            }
        }
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

    void MoveToRandomPosition()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMove)
        {
            if (collision.collider.CompareTag("Wall") ||
                collision.collider.CompareTag("WallTurn"))
            {
                isMove = true;
            }
        }
    }

    void MoveToPlayer()
    {
        if (rb == null || player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * 5f;
    }

    IEnumerator SetTimeStop()
    {
        float time = Random.Range(0.05f, 0.1f);
        yield return new WaitForSeconds(time);
        isMove = true;
    }
}
