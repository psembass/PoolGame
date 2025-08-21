using UnityEngine;

public class HoleController : MonoBehaviour
{

    void Start()
    {
        GameManager.Instance.RegisterHole(this);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Reset white ball");
            GameManager.Instance.ResetWhiteBall();
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball entered the hole");
            GameManager.Instance.AddScore(1);
            Destroy(collision.gameObject);
        }
    }
}
