using System.Collections;
using UnityEngine;

public class Keeper : MonoBehaviour
{
    [Header("Patrulha")]
    public float walkSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Perseguição")]
    public float chaseSpeed = 4f;
    public float visionRange = 6f;

    [Header("Combate")]
    public int maxHealth = 3;
    public int damage = 1;
    public float damageCooldown = 1f;

    // VARIAVEIS PRIVADAS
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private int currentHealth;
    private bool isDead = false;
    private bool facingRight = true;
    private bool isChasing = false;
    private bool canDamage = true;

    private Vector2 startPosition;
    private int patrolDirection = 1;
    private float leftLimit;
    private float rightLimit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        startPosition = transform.position;
        leftLimit = startPosition.x - patrolDistance;
        rightLimit = startPosition.x + patrolDistance;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        isChasing = distanceToPlayer <= visionRange;

        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        ClampPosition();
    }

    void Patrol()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("isChasing", false);

        rb.linearVelocity = new Vector2(patrolDirection * walkSpeed, rb.linearVelocity.y);

        // Vira ao atingir os limites
        if (transform.position.x >= rightLimit && patrolDirection == 1)
        {
            patrolDirection = -1;
            SetFacing(false); // olha para esquerda
        }
        else if (transform.position.x <= leftLimit && patrolDirection == -1)
        {
            patrolDirection = 1;
            SetFacing(true); // olha para direita
        }
    }

    void ChasePlayer()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("isChasing", true);

        float playerX = player.position.x;

        // Para na borda se o player estiver fora dos limites
        if (transform.position.x >= rightLimit && playerX > rightLimit)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            SetFacing(playerX > transform.position.x);
            return;
        }
        if (transform.position.x <= leftLimit && playerX < leftLimit)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            SetFacing(playerX > transform.position.x);
            return;
        }

        float targetX = Mathf.Clamp(playerX, leftLimit, rightLimit);
        float direction = targetX > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

        // Vira na direção correta do movimento
        SetFacing(direction > 0);
    }

    // Controla a direção que o personagem olha
    void SetFacing(bool shouldFaceRight)
    {
        if (shouldFaceRight == facingRight) return; // já está na direção certa

        facingRight = shouldFaceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
        transform.position = new Vector2(clampedX, transform.position.y);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("death");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDamage)
        {
            collision.gameObject.GetComponent<PlayerController>().life -= damage;
            StartCoroutine(DamageCooldown());
            anim.SetTrigger("attack");
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.yellow;
        Vector3 origin = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Gizmos.DrawLine(
            new Vector2(origin.x - patrolDistance, transform.position.y),
            new Vector2(origin.x + patrolDistance, transform.position.y)
        );
    }
}