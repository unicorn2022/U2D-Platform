﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Tooltip("角色攻击的伤害")]
    public int damage = 1;

    private Animator animator;
    private PolygonCollider2D collider;

    private bool canAttack = true; // 角色是否可以攻击

    void Start()
    {
        animator = transform.parent.GetComponent<Animator>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        Attack();
    }

    /// <summary>
    /// 角色攻击, 及其动画的切换
    /// </summary>
    void Attack() {
        // 由于控制了最短攻击间隔为0.5s, 因此可以长按攻击键持续攻击
        if (Input.GetButton("Attack") && canAttack) {
            animator.SetTrigger("Attack");
            canAttack = false;
            Invoke("AttackStart", 0.35f);
            Invoke("AttackReset", 0.5f);
        }
    }
    /// <summary>
    /// 角色攻击后, 待刀光出现(0.35s), 启动碰撞体
    /// </summary>
    void AttackStart() {
        collider.enabled = true;
        Invoke("AttackEnd", 0.05f);
    }
    /// <summary>
    /// 角色攻击启动碰撞体, 一段时间后(0.05s), 关闭碰撞体
    /// </summary>
    void AttackEnd() {
        collider.enabled = false;
    }
    /// <summary>
    /// 角色攻击后, 等待动画播放完成(0.5s), 重置攻击状态
    /// </summary>
    void AttackReset() {
        canAttack = true;
    }
}
