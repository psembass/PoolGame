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
    private float stopThrehold = 0.01f;

    private PlayerController playerController;
    private List<Rigidbody> balls = new List<Rigidbody>();
    private GameObject[] holes;

    private MainManager mainManager;
    private string gameMode = "PlayerVsCpu";
    private string player2Name = "CPU";

    private int currentPlayer = 1;
    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    private bool scoreAdded = false;

    void Start()
    {
        // load main manager
        mainManager = FindFirstObjectByType<MainManager>();
        // load player
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
        // load holes
        holes = GameObject.FindGameObjectsWithTag("Hole");
        // init UI
        gameMode = mainManager.gameMode;
        if (gameMode == "PlayerVsCpu")
        {
            player2Name = "CPU";
        }
        else if (gameMode == "PlayerVsPlayer")
        {
            player2Name = "Player 2";
        }
        tutorialText.text = "Player 1 \n Drag the white ball with mouse to shoot";
        scoreText.text = "Player 1: " + scorePlayer1;
        scoreText2.text = player2Name + ": " + scorePlayer2;
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
                if (gameMode == "PlayerVsCpu" && currentPlayer == 2)
                {
                    CpuTurn();
                }
                else
                {
                    tutorialText.text = "Player " + currentPlayer + " \n Take another shot";
                    tutorialText.gameObject.SetActive(true);
                    playerController.CanHit = true;
                }
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
            scoreText2.text = player2Name + ": " + scorePlayer2;
        }
        scoreAdded = true;

        // Check endgame condition
        int ballsLeft = 10 - scorePlayer1 - scorePlayer2;
        if (scorePlayer1 > scorePlayer2 + ballsLeft || scorePlayer2 > scorePlayer1 + ballsLeft || ballsLeft == 0)
        {
            EndGame();
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
        scoreAdded = false;
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        if (gameMode == "PlayerVsCpu" && currentPlayer == 2)
        {
            CpuTurn();
        }
        else
        {
            tutorialText.text = "Player " + currentPlayer + "\n Your turn";
            tutorialText.gameObject.SetActive(true);
            playerController.CanHit = true;
        }
    }

    public void RestartGame()
    {
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Continue game
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        if (scorePlayer1 == scorePlayer2)
        {
            winText.text = "Draw!";
        } 
        else
        {
            if (gameMode == "PlayerVsCpu")
            {
                winText.text = currentPlayer == 1 ? "You won!" : "You lose!";
            }
            else if (gameMode == "PlayerVsPlayer")
            {
                winText.text = "Player " + currentPlayer + " wins!";
            }
        }
        tutorialText.gameObject.SetActive(false);
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

    private void CpuTurn()
    {
        tutorialText.text = "CPU is taking a shot...";
        tutorialText.gameObject.SetActive(true);
        // Find optimal direction to hit
        for (int i = 0; i < holes.Length; i++)
        {
            // Raycast from each hole to the white ball
            Vector3 holeToBall = playerController.transform.position - holes[i].transform.position;
            float searchRadius = 0.5f;

            if (Physics.SphereCast(holes[i].transform.position,
                                searchRadius,
                                holeToBall.normalized,
                                out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ball"))
                {
                    Debug.Log("CPU smart hit to: " + hit.collider.name);
                    CpuHitPostion(hit.collider.transform.position);
                    return;
                }
            }
        }
        // Hit random ball
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i] != null)
            {
                Debug.Log("CPU dumb hit: " + balls[i].name);
                CpuHitPostion(balls[i].transform.position);
                return;
            }
        }
    }

    private void CpuHitPostion(Vector3 position)
    {
        Vector3 dir = position - playerController.transform.position;
        float hitForce = Random.Range(20, 25);
        playerController.hitForce = dir.normalized * hitForce;
        return;
    }
}
