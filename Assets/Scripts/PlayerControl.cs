using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    //攻擊,移動,跳躍 係數調整
    public float attackForce = 10f;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    //public float crouchSpeed = 2f;

    public Transform groundCheck;
    public LayerMask ground;
    public Collider2D attackTrigger;
    public Rigidbody2D rb;
    public Animator anim;
    public Collider2D coll;


    public bool isGrounded;
    public bool isCrouching;

    public float attackRate = 1f;
    private float nextAttackTime = 0f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        // 左右移動
        rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);

        // 跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 蹲下(未啟用 理由沒蹲下動畫)
        /*if (Input.GetButton("Crouch") && isGrounded)
        {
            if(Input.GetButton("Crouch") && isGrounded) {
                anim.SetBool("isCrouching", true);
               //coll.enabled = false; // Disable collider when crouching
            }
        }
        else
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
            coll.enabled = true; // Enable collider when standing up
        }*/

        // 更新動畫
        anim.SetFloat("running", Mathf.Abs(horizontalMove));//偵測跑步

        if (isGrounded)//偵測跳躍 或 墜落
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", false);
        }
        else if (!isGrounded && rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
        }
        else if(rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false) ;
            anim.SetBool("falling", true) ;
        }
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            anim.SetBool("jumping", false);
        }

        //攻擊偵測
        if (Input.GetButtonDown("Attack") && Time.time >= nextAttackTime)
        {
            Attack();
            // 攻擊間隔
            nextAttackTime = Time.time + 0.5f / attackRate;
        }
    }

    void FixedUpdate()
    {
        // 如果角色向左移動但是面向右邊，或者向右移動但是面向左邊，則翻轉角色
        if ((rb.velocity.x < 0 && transform.localScale.x > 0) || (rb.velocity.x > 0 && transform.localScale.x < 0))
        {
            FlipCharacter();
        }
    }

    //攻擊判定
    void Attack()
    {
        // 播放攻擊動畫
        anim.SetBool("Attack2", true);

        if(isGrounded)
        {
            moveSpeed = 0f;
        }
        else { moveSpeed = 7f; }
        

        // 啟動攻擊觸發器
        OnAttackStart();

        // 根據角色的面向來決定攻擊方向
        Vector2 attackDirection = transform.right; // 假設攻擊方向是角色面向的右側
        if (transform.localScale.x < 0)
        {
            attackDirection = -transform.right; // 如果角色面向左側，攻擊方向就是相反方向
        }

        // 在攻擊方向施加力量
        GetComponent<Rigidbody2D>().AddForce(attackDirection * attackForce, ForceMode2D.Impulse);

        StartCoroutine(DelayedOnAttackEnd());
    }

    IEnumerator DelayedOnAttackEnd()
    {
        yield return new WaitForSeconds(0.3f); // 等待0.3秒

        // 在等待之後執行 OnAttackEnd 方法
        OnAttackEnd();
    }

    // 攻擊動畫事件
    void OnAttackStart()
    {
        // 啟用攻擊判定範圍
        attackTrigger.enabled = true;
    }

    void OnAttackEnd()
    {
        // 在攻擊結束時禁用攻擊判定
        attackTrigger.enabled = false;
        anim.SetBool("Attack", false);
        moveSpeed = 10f;
    }


    void FlipCharacter()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

}
