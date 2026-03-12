using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
    public player playerScript;
    public Invaders invadersScript;
    public TextMeshProUGUI scoreText;
    private int score;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (playerScript != null) playerScript.onPlayerDied += GoToCredits;
        if (invadersScript != null) invadersScript.onAllInvadersKilled += GoToCredits;
    }

    public void AddScore(int points)
    {
        score += points;
        if (scoreText != null) scoreText.text = "Score: " + score.ToString("D4");
    }

    private void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}