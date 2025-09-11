using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
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
    private TextMeshProUGUI cameraHint;

    private InputHandler inputHandler;
    private string player2Name = "CPU";
    private string gameMode = "";

    void Start()
    {
        inputHandler = FindAnyObjectByType<InputHandler>();
        inputHandler.OnClick += OnClick;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            cameraHint.gameObject.SetActive(false);
        }
    }

    void OnClick(Vector2 postion)
    {
        if (tutorialText.gameObject.activeInHierarchy)
        {
            tutorialText.gameObject.SetActive(false);
        }
    }

    internal void OnGameStart(string gameMode)
    {
        this.gameMode = gameMode;
        if (gameMode == "PlayerVsCpu")
        {
            player2Name = "CPU";
        }
        else if (gameMode == "PlayerVsPlayer")
        {
            player2Name = "Player 2";
        }
        tutorialText.text = "Player 1 \n Drag the white ball with mouse to shoot";
        scoreText.text = "Player 1: 0";
        scoreText2.text = player2Name + ": 0";
    }

    internal void OnGameEnd(int player)
    {
        if (player == 0)
        {
            winText.text = "Draw!";
        }
        else
        {
            if (gameMode == "PlayerVsCpu")
            {
                winText.text = player == 1 ? "You won!" : "You lose!";
            }
            else if (gameMode == "PlayerVsPlayer")
            {
                winText.text = "Player " + player + " wins!";
            }
        }
        tutorialText.gameObject.SetActive(false);
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    internal void SetScore(int player, int score)
    {
        if (player == 1)
        {
            scoreText.text = "Player 1: " + score;
        }
        else
        {
            scoreText2.text = player2Name + ": " + score;
        }
    }

    internal void OnNextTurn(int player)
    {
        tutorialText.text = "Player " + player + "\n Your turn";
        tutorialText.gameObject.SetActive(true);
    }

    internal void OnExtraShot(int player)
    {
        tutorialText.text = "Player " + player + " \n Take another shot";
        tutorialText.gameObject.SetActive(true);
    }

    internal void OnCpuTurn()
    {
        tutorialText.text = "CPU is taking a shot...";
        tutorialText.gameObject.SetActive(true);
    }
}
