using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float crouchSpeed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D coll;
    public bool isGrounded;
    public bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        // 左右移動
        rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);

        // 跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("isJumping", true);
        }

        // 蹲下
        if (verticalMove < 0 && isGrounded)
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
            coll.enabled = false; // Disable collider when crouching
        }
        else
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
            coll.enabled = true; // Enable collider when standing up
        }

        // 更新動畫
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
        anim.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        // 如果角色向左移動但是面向右邊，或者向右移動但是面向左邊，則翻轉角色
        if ((rb.velocity.x < 0 && transform.localScale.x > 0) || (rb.velocity.x > 0 && transform.localScale.x < 0))
        {
            FlipCharacter();
        }
    }

    void FlipCharacter()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
