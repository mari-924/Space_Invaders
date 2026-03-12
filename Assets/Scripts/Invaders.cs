using UnityEngine;
using System;

public class Invaders : MonoBehaviour
{
    public Invader[] prefabs;
    public int rows = 5;
    public int cols = 11;
    public int numKilled { get; private set; }
    public int numAlive => this.totalInvaders - this.numKilled;
    public int totalInvaders => this.rows * this.cols;
    public float percentKilled => (float)this.numKilled / (float)this.totalInvaders;
    public float movementFrequency = 0.5f;
    private float nextMoveTime;
    private Vector3 direction = Vector3.right;
    public AnimationCurve speed;
    public Projectile missilePrefab;
    public float missileAttackRate;
    public Action onAllInvadersKilled;

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float width = 0.699f * (this.cols - 1);
            float height = -0.5f * (this.rows - 1);
            Vector2 centering = new Vector2(-width / 2, -height / 2);
            Vector3 rowPos = new Vector3(centering.x, centering.y + (row * 0.5f), 0.0f);

            for (int col = 0; col < this.cols; col++)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform);
                invader.killed += InvaderKilled;
                Vector3 pos = rowPos;
                pos.x += col * 0.7f;
                invader.transform.position = pos;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextMoveTime)
        {
            MoveInvaders();
            nextMoveTime = Time.unscaledTime + movementFrequency;
        }
    }

    private void MoveInvaders()
    {
        float speedMultiplier = this.speed != null ? this.speed.Evaluate(this.percentKilled) : 1f;
        this.transform.position += direction * speedMultiplier;

        foreach (Transform child in this.transform)
        {
            if (child.gameObject.activeInHierarchy) {
                Invader invaderScript = child.GetComponent<Invader>();
                if (invaderScript != null) invaderScript.StepAnimation();
            }
        }

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy) continue;
            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f)) {
                AdvanceRow();
                break;
            } else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f)) {
                AdvanceRow();
                break;
            }
        }
    }

   private void MissileAttack()
    {
        if (numAlive <= 0) return;
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy) continue;

            if (UnityEngine.Random.value < (1.0f / (float)this.numAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                
                Invader invaderScript = invader.GetComponent<Invader>();
                if (invaderScript != null)
                {
                    invaderScript.PlayShootAnimation();
                }
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        direction.x *= -1.0f;
        Vector3 pos = this.transform.position;
        pos.y -= 0.25f;
        this.transform.position = pos;
    }

    private void InvaderKilled(Invader invader)
    {
        this.numKilled++;
        if (GameBehavior.Instance != null) GameBehavior.Instance.AddScore(invader.scoreValue);
        if (this.numKilled >= this.totalInvaders) {
            onAllInvadersKilled?.Invoke();
        }
    }
}