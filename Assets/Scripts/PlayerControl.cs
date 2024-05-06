using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    //����,����,���D �Y�ƽվ�
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

        // ���k����
        rb.velocity = new Vector2(horizontalMove * moveSpeed, rb.velocity.y);

        // ���D
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // �ۤU(���ҥ� �z�ѨS�ۤU�ʵe)
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

        // ��s�ʵe
        anim.SetFloat("running", Mathf.Abs(horizontalMove));//�����]�B

        if (isGrounded)//�������D �� �Y��
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

        //��������
        if (Input.GetButtonDown("Attack") && Time.time >= nextAttackTime)
        {
            Attack();
            // �������j
            nextAttackTime = Time.time + 0.5f / attackRate;
        }
    }

    void FixedUpdate()
    {
        // �p�G����V�����ʦ��O���V�k��A�Ϊ̦V�k���ʦ��O���V����A�h½�ਤ��
        if ((rb.velocity.x < 0 && transform.localScale.x > 0) || (rb.velocity.x > 0 && transform.localScale.x < 0))
        {
            FlipCharacter();
        }
    }

    //�����P�w
    void Attack()
    {
        // ��������ʵe
        anim.SetBool("Attack2", true);

        if(isGrounded)
        {
            moveSpeed = 0f;
        }
        else { moveSpeed = 7f; }
        

        // �Ұʧ���Ĳ�o��
        OnAttackStart();

        // �ھڨ��⪺���V�ӨM�w������V
        Vector2 attackDirection = transform.right; // ���]������V�O���⭱�V���k��
        if (transform.localScale.x < 0)
        {
            attackDirection = -transform.right; // �p�G���⭱�V�����A������V�N�O�ۤϤ�V
        }

        // �b������V�I�[�O�q
        GetComponent<Rigidbody2D>().AddForce(attackDirection * attackForce, ForceMode2D.Impulse);

        StartCoroutine(DelayedOnAttackEnd());
    }

    IEnumerator DelayedOnAttackEnd()
    {
        yield return new WaitForSeconds(0.3f); // ����0.3��

        // �b���ݤ������ OnAttackEnd ��k
        OnAttackEnd();
    }

    // �����ʵe�ƥ�
    void OnAttackStart()
    {
        // �ҥΧ����P�w�d��
        attackTrigger.enabled = true;
    }

    void OnAttackEnd()
    {
        // �b���������ɸT�Χ����P�w
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
