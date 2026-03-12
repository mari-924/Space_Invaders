using UnityEngine;
using System;
using System.Collections;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites;
    public Sprite[] shootingSprites;
    private SpriteRenderer spriteRenderer;
    private int animationFrame;
    public int scoreValue = 10;
    public Action<Invader> killed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StepAnimation()
    {
        animationFrame = (animationFrame + 1) % animationSprites.Length;
        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    public void PlayShootAnimation()
    {
        StartCoroutine(ShootRoutine());
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

        spriteRenderer.sprite = originalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            this.killed?.Invoke(this);
            this.gameObject.SetActive(false);
        }
    }
}