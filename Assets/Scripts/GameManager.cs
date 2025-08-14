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
    private TextMeshProUGUI scoreText2;
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

    private int currentPlayer = 1;

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    private bool scoreAdded = false;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        playerController.CanHit = true;
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
        // init UI
        tutorialText.text = "Player " + currentPlayer + " \n Drag the white ball with mouse to shoot";
        scoreText.text = "Player 1: " + scorePlayer1;
        scoreText2.text = "Player 2: " + scorePlayer2;
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
        if (!playerController.CanHit && !AreBallsMoving() && !playerController.IsMoving())
        {
            Debug.Log("All balls stopped, enabling player control.");
            if (!scoreAdded)
            {
                // Switch to next player
                SwitchPlayer();
            } else
            {
                // Next hit for the current player
                scoreAdded = false;
                tutorialText.text = "Player " + currentPlayer + " \n Take another shot";
                tutorialText.gameObject.SetActive(true);
                playerController.CanHit = true;
            }
        }
    }

    public void AddScore(int points)
    {
        if (currentPlayer == 1)
        {
            scorePlayer1 += points;
            scoreText.text = "Player 1: " + scorePlayer1;
        } else if (currentPlayer == 2)
        {
            scorePlayer2 += points;
            scoreText2.text = "Player 2: " + scorePlayer2;
        }
        scoreAdded = true;

        // Check endgame condition
        int ballsLeft = scoreToWin - scorePlayer1 - scorePlayer2;
        if (scorePlayer1 > scorePlayer2 + ballsLeft || scorePlayer2 > scorePlayer1 + ballsLeft)
        {
            WinGame();
        }
        else if (ballsLeft == 0)
        {
            NobodyWins();
        }
    }
    public void ResetWhiteBall()
    {
        playerController.ResetPosition();
        // Switch to next player
        SwitchPlayer();
    }

    private void SwitchPlayer()
    {
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        tutorialText.text = "Player " + currentPlayer + "\n Your turn";
        tutorialText.gameObject.SetActive(true);
        playerController.CanHit = true;
        scoreAdded = false;
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
        winText.text = "Player " + currentPlayer + " wins!";
        winText.gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        restartButton.gameObject.SetActive(true);
    }

    public void NobodyWins()
    {
        winText.text = "Draw!";
        winText.gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
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
