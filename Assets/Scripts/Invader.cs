using UnityEngine;
using System;
using System.Collections;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites;
    public Sprite[] shootingSprites;
    public Sprite[] explosionSprites;
    
    public AudioClip shootSound;
    public AudioClip explosionSound;
    private AudioSource audioSource;
    
    private SpriteRenderer spriteRenderer;
    private int animationFrame;
    public int scoreValue = 10;
    public Action<Invader> killed;
    private bool _isExploding = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StepAnimation()
    {
        if (_isExploding) return;
        animationFrame = (animationFrame + 1) % animationSprites.Length;
        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    public void PlayShootAnimation()
    {
        if (!_isExploding) {
            if (shootSound != null) audioSource.PlayOneShot(shootSound);
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        if (shootingSprites.Length == 0) yield break;
        Sprite originalSprite = spriteRenderer.sprite;
        foreach (Sprite s in shootingSprites)
        {
            spriteRenderer.sprite = s;
            yield return new WaitForSeconds(0.1f);
        }
        if (!_isExploding) spriteRenderer.sprite = originalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isExploding) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            StartCoroutine(ExplodeInvader());
        }
    }

    private IEnumerator ExplodeInvader()
    {
        _isExploding = true;
        StopCoroutine(nameof(ShootRoutine));
        
        if (explosionSound != null) audioSource.PlayOneShot(explosionSound);

        if (explosionSprites.Length >= 3)
        {
            foreach (Sprite s in explosionSprites)
            {
                spriteRenderer.sprite = s;
                yield return new WaitForSeconds(0.05f);
            }
        }

        this.killed?.Invoke(this);
        this.gameObject.SetActive(false);
    }
}