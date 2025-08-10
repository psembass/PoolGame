using UnityEngine;

public class HoleController : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            // todo Add UI and game over logic
            Debug.Log("Game Over");
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            // todo Add point to player
            Debug.Log("Ball entered the hole");
        }
        Destroy(collision.gameObject);
    }
}
