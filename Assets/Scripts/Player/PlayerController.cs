using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("角色水平方向的移动速度")]
    public float runSpeed = 5.0f;
    [Tooltip("角色一段跳跃的速度")]
    public float jumpSpeed = 8.0f;
    [Tooltip("角色二段跳跃的速度")]
    public float doubleJumpSpeed = 5.0f;
    [Tooltip("角色爬梯子的速度")]
    public float climbSpeed = 4.0f;
    [Tooltip("角色在单向移动平台上, 按下向下键后, 掉落的时间")]
    public float changeLayerTime = 0.3f;


    private Rigidbody2D rigidbody;
    private Animator animator;
    private BoxCollider2D feetCollider;
    private PlayerAttack playerAttack;

    private bool canDoubleJump = false; // 角色是否可以二段跳
    private float playerGravity;        // 角色初始的重力系数

    public bool isGrounded = false;        // 角色是否在地面上
    public bool isOneWayPlatform = false;  // 角色是否在单向移动平台上
    public bool isLadder = false;          // 角色是否在梯子上

    public bool isClimbing = false;        // 角色是否在爬梯子
    public bool isJumping = false;         // 角色是否在跳跃
    public bool isFalling = false;         // 角色是否在下落


    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerGravity = rigidbody.gravityScale;
    }

    void Update() {
        if (!GameController.isPlayerAlive) return;
        SetPlayerStateParam();
        Run();
        Jump();
        OneWayPlatform();
        Climb();
    }

    /// <summary>
    /// 设置角色的状态参数
    /// </summary>
    void SetPlayerStateParam() {
        // 碰撞检测参数
        isGrounded = feetCollider.IsTouchingLayers(LayerMask.GetMask("ForeGround"))
            || feetCollider.IsTouchingLayers(LayerMask.GetMask("MovingPlatform"))
            || feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
        isOneWayPlatform = feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
        isLadder = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));

        // 动画参数
        isClimbing = animator.GetBool("Climb");
        isJumping = animator.GetBool("Jump");
        isFalling = animator.GetBool("Fall");
    }

    /// <summary>
    /// 角色移动, 及其动画的切换
    /// </summary>
    void Run() {
        // 通过刚体组件, 控制角色移动
        float moveDir = Input.GetAxis("Horizontal"); // 水平方向的移动
        Vector2 playerVelocity = new Vector2(moveDir * runSpeed, rigidbody.velocity.y);
        rigidbody.velocity = playerVelocity;

        // 控制动画的切换
        bool playerHasXAxisSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;
        animator.SetBool("Run", playerHasXAxisSpeed);
        animator.SetBool("Idle", !playerHasXAxisSpeed);

        // 控制是否需要翻转角色
        if (playerHasXAxisSpeed) {
            if (rigidbody.velocity.x > 0.1f) {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (rigidbody.velocity.x < -0.1f) {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    /// <summary>
    /// 角色跳跃, 及其动画的切换
    /// </summary>
    void Jump() {
        // TODO: 不能在梯子上起跳
        if (isLadder && !isGrounded) {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            return;
        }

        if (Input.GetButtonDown("Jump")) {
            // 在地面上, 进行一段跳
            if (isGrounded) {
                // 通过刚体组件, 控制角色跳跃
                Vector2 jumpVelocity = new Vector2(0.0f, jumpSpeed);
                rigidbody.velocity = Vector2.up * jumpVelocity;

                // 可以进行二段跳
                canDoubleJump = true;
            
                // 控制动画的切换
                animator.SetBool("Jump", true);
                animator.SetBool("Fall", false);
            } 
            // 在空中, 但可以进行二段跳
            else if (canDoubleJump) {
                Vector2 doubleJumpVelocity = new Vector2(0.0f, doubleJumpSpeed);
                rigidbody.velocity = Vector2.up * doubleJumpVelocity;

                // 不可以进行二段跳
                canDoubleJump = false;

                // 控制动画的切换
                animator.SetBool("Jump", true);
                animator.SetBool("Fall", false);
            }
            return;
        }

        // 纵向速度小于0, 表示角色正在下落
        if (rigidbody.velocity.y < 0.0f) {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", true);
        }

        // 角色落地后, 重置动画
        if(isGrounded) {
            animator.SetBool("Fall", false);
        }
    }

    /// <summary>
    /// 角色在单向移动平台上, 且按下向下键, 角色掉落
    /// </summary>
    void OneWayPlatform() {
        if (isGrounded) gameObject.layer = LayerMask.NameToLayer("Player");

        if (isOneWayPlatform && Input.GetAxis("Vertical") < -0.1f) {
            // 将角色的碰撞层短暂改为OneWayPlatform, 使角色可以穿过单向移动平台
            gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
            // 0.3秒后, 将角色的碰撞层改回Player
            Invoke("ResetPlayerLayer", changeLayerTime);
        }
    }
    void ResetPlayerLayer() {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    /// <summary>
    /// 角色爬梯子, 及其动画的切换
    /// </summary>
    void Climb() {
        // 角色在单向移动平台上, 不能爬梯子
        if (isOneWayPlatform) return;

        // 角色在梯子上
        if (isLadder) {
            // 角色不受重力影响
            rigidbody.gravityScale = 0f;

            float moveY = Input.GetAxis("Vertical");
            // 角色在爬梯子
            if(moveY > 0.5f || moveY < -0.5f) {
                animator.SetBool("Climb", true);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, moveY * climbSpeed);
            } 
            // 角色从梯子上跳跃
            else if (isJumping || isFalling){
                animator.SetBool("Climb", false);
            } 
            // 角色停在梯子上
            else {
                animator.SetBool("Climb", false);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
            }
        }
        // 角色不在梯子上
        else {
            animator.SetBool("Climb", false);
            rigidbody.gravityScale = playerGravity;
        }

    }


    #region 角色攻击事件
    void AttackStart() {
        playerAttack.AttackStart();
    }
    void AttackEnd() {
        playerAttack.AttackEnd();
    }
    void AttackReset() {
        playerAttack.AttackReset();
    }
    #endregion

}
