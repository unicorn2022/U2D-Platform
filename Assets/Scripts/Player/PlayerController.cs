﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("角色水平方向的移动速度")]
    public float runSpeed = 5.0f;
    [Tooltip("角色一段跳跃的速度")]
    public float jumpSpeed = 8.0f;
    [Tooltip("角色二段跳跃的速度")]
    public float doubleJumpSpeed = 5.0f;


    private Rigidbody2D rigidbody;
    private Animator animator;
    private BoxCollider2D feetCollider;

    private bool canDoubleJump = false; // 角色是否可以二段跳

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        if (!GameController.isPlayerAlive) return;
        Run();
        Jump();
        OneWayPlatformCheck();
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
        if (Input.GetButtonDown("Jump")) {
            // 在地面上, 进行一段跳
            if (IsGrounded()) {
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
        }

        // 纵向速度小于0, 表示角色正在下落
        if(rigidbody.velocity.y < 0.0f) {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", true);
        }

        // 角色落地后, 重置动画
        if(IsGrounded()) {
            animator.SetBool("Fall", false);
        }
    }

    /// <summary>
    /// 判断角色是否在地面上
    /// </summary>
    /// <returns>
    /// true表示在地面上, false表示不在地面上
    /// </returns>
    bool IsGrounded() {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("ForeGround"))
            || feetCollider.IsTouchingLayers(LayerMask.GetMask("MovingPlatform"))
            || feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    }

    
    /// <summary>
    /// 角色在单向移动平台上, 且按下向下键, 角色掉落
    /// </summary>
    void OneWayPlatformCheck() {
        if (IsGrounded()) gameObject.layer = LayerMask.NameToLayer("Player");

        if (IsOneWayPlatform() && Input.GetAxis("Vertical") < -0.1f) {
            // 将角色的碰撞层短暂改为OneWayPlatform, 使角色可以穿过单向移动平台
            gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
            // 0.5秒后, 将角色的碰撞层改回Player
            Invoke("ResetPlayerLayer", 0.5f);
        }
    }
    /// <summary>
    /// 判断角色是否在单向移动平台上
    /// </summary>
    /// <returns>
    /// true表示在, false表示不在
    /// </returns>
    bool IsOneWayPlatform() {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    }
    
    void ResetPlayerLayer() {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
