using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("角色水平方向的移动速度")]
    public float runSpeed = 5.0f;
    [Tooltip("角色跳跃的速度")]
    public float jumpSpeed = 8.0f;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private BoxCollider2D feetCollider;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Run();
        Flip();
        Jump();
    }

    /// <summary>
    /// 水平方向的移动
    /// </summary>
    void Run() {
        // 通过刚体组件, 控制角色移动
        float moveDir = Input.GetAxis("Horizontal"); // 水平方向的移动
        Vector2 playerVelocity = new Vector2(moveDir * runSpeed, rigidbody.velocity.y);
        rigidbody.velocity = playerVelocity;

        // 控制动画的切换
        bool playerHasXAxisSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;
        animator.SetBool("Running", playerHasXAxisSpeed);
    }

    /// <summary>
    /// 根据移动速度, 翻转角色
    /// </summary>
    void Flip() {
        bool playerHasXAxisSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasXAxisSpeed) {
            if(rigidbody.velocity.x > 0.1f) {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if(rigidbody.velocity.x < -0.1f) {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    /// <summary>
    /// 角色跳跃
    /// </summary>
    void Jump() {
        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            Vector2 jumpVelocity = new Vector2(0.0f, jumpSpeed);
            rigidbody.velocity = Vector2.up * jumpVelocity;
        }
    }

    /// <summary>
    /// 判断角色是否在地面上
    /// </summary>
    /// <returns>
    /// true表示在地面上, false表示不在地面上
    /// </returns>
    bool IsGrounded() {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
}
