using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameBehavior : MonoBehaviour
{
    public player playerScript;
    public Invaders invadersScript;
    public GameObject gameOverScreen;
    public GameObject youWinScreen;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore = 0;
    private int highScore = 0;
    private bool screenActive = false;
    private float screenStartTime;

    private void Awake()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (youWinScreen != null) youWinScreen.SetActive(false);
        
        Time.timeScale = 1f;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
    }

    private void Update()
    {
        if (screenActive)
        {
            bool clicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            bool touched = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

            if (clicked || touched || Time.unscaledTime - screenStartTime >= 3f)
            {
                RestartGame();
            }
        }
    }

    private void OnEnable()
    {
        if (playerScript != null) playerScript.onPlayerDied += ShowGameOver;
        if (invadersScript != null) invadersScript.onAllInvadersKilled += ShowWinScreen;
    }

    private void OnDisable()
    {
        if (playerScript != null) playerScript.onPlayerDied -= ShowGameOver;
        if (invadersScript != null) invadersScript.onAllInvadersKilled -= ShowWinScreen;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore.ToString("D4");
        highScoreText.text = "High Score: " + highScore.ToString("D4");
    }

    private void ShowGameOver()
    {
        TriggerEndScreen(gameOverScreen);
    }

    private void ShowWinScreen()
    {
        TriggerEndScreen(youWinScreen);
    }

    private void TriggerEndScreen(GameObject screen)
    {
        if (screen != null)
        {
            screen.SetActive(true);
            screenActive = true;
            screenStartTime = Time.unscaledTime;
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}