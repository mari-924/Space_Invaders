using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public Projectile laserPrefab;
    public float speed = 5.0f;
    public bool _laserActive;

    void Update()
    {
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
        if(other.gameObject.layer == LayerMask.NameToLayer("Invader") 
            || other.gameObject.layer == LayerMask.NameToLayer("Missile")){
                SceneManager.LoadScene(SceneManager.GetActiveScene.name);
            
        }
    }
}
