using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // VARIAVEIS PRIVADAS
    private Rigidbody2D rb;
    private Animator anim;
    private float moveX;
    private bool isGrounded;
    private int jumpsRemaining;
    private bool facingRight = true;
    private bool isDead = false;
    private bool isAttacking = false;

    // VARIAVEIS PUBLICAS
    public float speed;
    public float runSpeed = 10f;
    public float jumpForce = 12f;
    public int maxJumps = 2;
    public int life;
    public TextMeshProUGUI textLife;

    [Header("Gravidade")]
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleJump();
        HandleAttack();
        AplicarGravidade();
        Flip();
        UpdateAnimations();

        textLife.text = life.ToString();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Move();
    }

    void HandleMovement()
    {
        moveX = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f :
                Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;
    }

    void HandleJump()
    {
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsRemaining--;
        }
    }

    void HandleAttack()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
        {
            isAttacking = true;
            anim.ResetTrigger("attack");
            anim.SetTrigger("attack");
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return null;
        yield return null;

        float attackLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackLength);

        anim.ResetTrigger("attack");
        isAttacking = false;
    }

    void Move()
    {
        bool isRunning = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : speed;
        rb.linearVelocity = new Vector2(moveX * currentSpeed, rb.linearVelocity.y);
    }

    void AplicarGravidade()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Keyboard.current.spaceKey.isPressed
                 && !Keyboard.current.upArrowKey.isPressed)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = moveX != 0;
        bool isRunning = isMoving && (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
    }

    void Flip()
    {
        if (moveX > 0 && !facingRight || moveX < 0 && facingRight)
        {
            // ← chave que estava faltando
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("die");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpsRemaining = maxJumps;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}