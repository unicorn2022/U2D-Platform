using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();

        controls.GamePlay.PlayerAttack.started += ctx => Attack();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion
    
    [Tooltip("角色攻击的伤害")]
    public int damage = 2;

    private Animator animator;
    private PolygonCollider2D collider;

    private bool canAttack = true; // 角色是否可以攻击

    void Start() {
        animator = transform.parent.GetComponent<Animator>();
        collider = GetComponent<PolygonCollider2D>();
    }

    /// <summary>
    /// 角色攻击, 及其动画的切换
    /// </summary>
    void Attack() {
        // 由于控制了最短攻击间隔, 因此可以长按攻击键持续攻击
        if (canAttack) {
            animator.SetTrigger("Attack");
            canAttack = false;
        }
    }

    /// <summary>
    /// 攻击碰撞体碰撞到敌人, 造成伤害
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
    }


    #region 角色攻击事件
    /// <summary>
    /// 角色攻击后, 待刀光出现(0.3s), 启动碰撞体
    /// </summary>
    public void AttackStart() {
        collider.enabled = true;
    }
    /// <summary>
    /// 角色攻击启动碰撞体, 一段时间后(0.1s), 关闭碰撞体
    /// </summary>
    public void AttackEnd() {
        collider.enabled = false;
    }
    /// <summary>
    /// 角色攻击后, 等待动画播放完成(0.5s), 重置攻击状态
    /// </summary>
    public void AttackReset() {
        canAttack = true;
    }
    #endregion
}
