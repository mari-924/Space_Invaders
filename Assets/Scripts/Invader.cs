using UnityEngine;
using System;

public class Invader : MonoBehaviour
{
   public Sprite[] animationSprites;
   public float animationTime = 0.3f;
   private SpriteRenderer spriteRenderer;
   private int animationFrame;
   public int scoreValue = 10;
   public Action<Invader> killed;

   private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), this.animationTime, this.animationTime);
    }

    private void AnimateSprite()
    {
        animationFrame++;

        if(animationFrame >= this.animationSprites.Length)
        {
            animationFrame = 0;
        }

        spriteRenderer.sprite = this.animationSprites[animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            this.killed?.Invoke(this);
            this.gameObject.SetActive(false);
        }
    }
}