using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TextMeshProUGUI winText;
    [SerializeField]
    private TextMeshProUGUI tutorialText;
    [SerializeField]
    private int scoreToWin = 10;
    [SerializeField]
    private float stopThrehold = 0.01f;
    private PlayerController playerController;
    private List<Rigidbody> balls = new List<Rigidbody>();

    private int score = 0;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerController.SetCanHit(true);
        // load balls
        GameObject[] ballObjs = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ballObj in ballObjs)
        {
            Rigidbody ballRb = ballObj.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                balls.Add(ballRb);
            }
        }
    }

    void Update()
    {
        if (tutorialText.gameObject.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            tutorialText.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        // Check if balls are moving
        if (!AreBallsMoving() && !playerController.IsMoving())
        {
            Debug.Log("All balls stopped, enabling player control.");
            playerController.SetCanHit(true);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score: " + score;
        if (score == scoreToWin)
        {
            WinGame();
        }
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
    {
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    private bool AreBallsMoving()
    {
        foreach (Rigidbody ball in balls)
        {
            if (ball != null && (ball.linearVelocity.magnitude > stopThrehold && !ball.IsSleeping()))
            {
                return true;
            }
        }
        return false;
    }
}
