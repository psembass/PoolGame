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

    private int score = 0;

    void Update()
    {
        if (tutorialText.gameObject.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            tutorialText.gameObject.SetActive(false);
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
}
