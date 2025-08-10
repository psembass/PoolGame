using UnityEngine;

public class HoleController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");
            gameManager.GameOver();
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball entered the hole");
            gameManager.AddScore(1);
        }
        Destroy(collision.gameObject);
    }
}
