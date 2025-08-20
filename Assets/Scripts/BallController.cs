using UnityEngine;

public class BallController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        gameManager.RegisterBall(this);
    }
}
