using UnityEngine;

public class EnemyChar : MonoBehaviour
{
    public EnemyController enemyController;
    public int idEnemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("WallTurn"))
        {
            if (enemyController.state == EnemyController.State.Run)
            {
                enemyController.AvoidWall();
            }
        }
    }
}
