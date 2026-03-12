using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class player : MonoBehaviour
{
    public Projectile laserPrefab;
    public float speed = 5.0f;
    public bool laserActive;
    public int health = 3;
    public System.Action onPlayerDied;

    public Sprite[] idleSprites;
    public Sprite[] shootingSprites;
    public Sprite[] explosionSprites;
    public float animationTime = 0.5f;

    public AudioClip shootSound;
    public AudioClip explosionSound;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private int animationFrame;
    
    private bool _isDead = false;
    private bool _isShooting = false;
    private bool _isExploding = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        InvokeRepeating(nameof(AnimateIdle), animationTime, animationTime);
    }

    private void AnimateIdle()
    {
        if (_isDead || _isShooting || _isExploding || idleSprites.Length == 0) return;
        animationFrame = (animationFrame + 1) % idleSprites.Length;
        spriteRenderer.sprite = idleSprites[animationFrame];
    }

    void Update()
    {
        if (_isDead) return;

        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) {
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;
        }
        else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) {
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!laserActive) {
            Projectile projectile = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += LaserDestroyed;
            laserActive = true;
            
            if (shootSound != null) audioSource.PlayOneShot(shootSound);
            
            StartCoroutine(PlayShootAnimation());
        }
    }

    private IEnumerator PlayShootAnimation()
    {
        if (shootingSprites.Length < 3) yield break;
        _isShooting = true;
        foreach (Sprite s in shootingSprites)
        {
            spriteRenderer.sprite = s;
            yield return new WaitForSeconds(0.05f);
        }
        _isShooting = false;
    }

    private void LaserDestroyed() { laserActive = false; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isExploding) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Invader") || other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Invader")) health = 0;
            else { health--; Destroy(other.gameObject); }

            if (health <= 0) {
                StartCoroutine(ExplodePlayer());
            } else {
                this.transform.position = new Vector3(0, this.transform.position.y, 0);
            }
        }
    }

    private IEnumerator ExplodePlayer()
    {
        _isDead = true;
        _isExploding = true;
        
        if (explosionSound != null) audioSource.PlayOneShot(explosionSound);

        if (explosionSprites.Length >= 3)
        {
            foreach (Sprite s in explosionSprites)
            {
                spriteRenderer.sprite = s;
                yield return new WaitForSeconds(0.1f);
            }
        }

        onPlayerDied?.Invoke();
        this.gameObject.SetActive(false);
    }
}