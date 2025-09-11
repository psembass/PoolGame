using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float stopThrehold = 0.01f;

    public static GameManager Instance;

    private PlayerController playerController;
    private UIManager uiManager;
    private List<BallController> balls = new();
    private List<HoleController> holes = new();

    private MainManager mainManager;
    private string gameMode = "PlayerVsCpu";
    private int currentPlayer = 1;
    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    private bool scoreAdded = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // in case there will be more scenes
        }
    }

    void Start()
    {
        // load main manager
        mainManager = MainManager.Instance;
        // load player
        playerController = FindFirstObjectByType<PlayerController>();
        playerController.CanHit = true;
        // init UI
        gameMode = mainManager.gameMode;
        uiManager = GetComponent<UIManager>();
        uiManager.OnGameStart(gameMode);
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
                    uiManager.OnExtraShot(currentPlayer);
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
            uiManager.SetScore(currentPlayer, scorePlayer1);

        } else if (currentPlayer == 2)
        {
            scorePlayer2 += points;
            uiManager.SetScore(currentPlayer, scorePlayer2);
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
            uiManager.OnNextTurn(currentPlayer);
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
            uiManager.OnGameEnd(0);
        }
        else
        {
            uiManager.OnGameEnd(currentPlayer);
        }
        Time.timeScale = 0f; // Pause the game
    }

    public void RegisterBall(BallController ball)
    {
        balls.Add(ball);
    }

    public void RegisterHole(HoleController hole)
    {
        holes.Add(hole);
    }

    private bool AreBallsMoving()
    {
        foreach (BallController ball in balls)
        {
            if (ball == null) continue; // Skip if ball is destroyed
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb.linearVelocity.magnitude > stopThrehold && !ballRb.IsSleeping())
            {
                return true;
            }
        }
        return false;
    }

    private void CpuTurn()
    {
        uiManager.OnCpuTurn();
        // Find optimal direction to hit
        foreach (HoleController hole in holes)
        {
            // Raycast from each hole to the white ball
            Vector3 holeToBall = playerController.transform.position - hole.transform.position;
            float searchRadius = 0.5f;

            if (Physics.SphereCast(hole.transform.position,
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
