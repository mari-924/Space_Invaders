using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public Projectile laserPrefab;
    public float speed = 5.0f;
    public bool _laserActive;
    private int health = 3;
    public System.Action onPlayerDied;
    public GameObject gameOverScreen; 
    private bool _isDead = false;

    void Update()
    {
        if (_isDead) return;

        if(Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;
        }else if(Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;
        }
        
        if(Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!_laserActive)
        {
            Projectile projectile = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += LaserDestroyed;
            
            _laserActive = true;
        }
    }

    private void LaserDestroyed()
    {
        _laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Invader") ){
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }else if(other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            health--;
        }

        if(health <= 0)
        {
            onPlayerDied?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }else {
            this.transform.position = new Vector3(0, this.transform.position.y, 0);
        }
    }

    private void TriggerGameOver()
    {
        _isDead = true;

        if(gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        GetComponent<SpriteRenderer>().enabled = false;
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
