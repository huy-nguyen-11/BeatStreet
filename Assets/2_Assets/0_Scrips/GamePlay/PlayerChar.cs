using UnityEngine;

public class PlayerChar : MonoBehaviour
{
    public PlayerController playerController;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WallTurn"))
        {
            if (GamePlayManager.Instance._IconNextTurn.gameObject.activeSelf
                && transform.position.x < collision.transform.position.x)
            {
                GamePlayManager.Instance.SetNextTurn();
                StartCoroutine(playerController.setPlayerNextTurn());
            }
        }
    }
}
