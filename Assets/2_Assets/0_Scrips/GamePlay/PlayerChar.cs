using UnityEngine;

public class PlayerChar : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject effectMana, effectHP;

    private void Start()
    {
        effectMana.SetActive(false);
        effectHP.SetActive(false);
    }

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
