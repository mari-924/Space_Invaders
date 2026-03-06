using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public Projectile laserPrefab;
    public float speed = 5.0f;
    public bool laserActive;
    public int health = 3;
    public System.Action onPlayerDied;
    
    private bool _isDead = false;

    void Update()
    {
        if (_isDead) return;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;
        }
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!laserActive)
        {
            Projectile projectile = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += LaserDestroyed;
            laserActive = true;
        }
    }

    private void LaserDestroyed()
    {
        laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool hitByInvader = other.gameObject.layer == LayerMask.NameToLayer("Invader");
        bool hitByMissile = other.gameObject.layer == LayerMask.NameToLayer("Missile");

        if (hitByInvader || hitByMissile)
        {
            if (hitByInvader) {
                health = 0;
            } else {
                health--;
                Destroy(other.gameObject);
            }

            if (health <= 0)
            {
                _isDead = true;
                onPlayerDied?.Invoke();
                this.gameObject.SetActive(false);
            }
            else
            {
                this.transform.position = new Vector3(0, this.transform.position.y, 0);
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}