using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("玩家的生命值")]
    public int health = 5;
    [Tooltip("玩家受到伤害后闪烁的次数")]
    public int numBlinks = 5;
    [Tooltip("玩家受到伤害后闪烁的时间")]
    public float seconds = 0.1f;

    private Renderer playerRenderer;        // 玩家的 Renderer 组件
    private Animator playerAnimator;        // 玩家的 Animator 组件
    private UIScreenFlash uiScreenFlash;    // 屏幕红闪组件
    private Rigidbody2D rigidbody;          // 玩家的刚体组件
    private PolygonCollider2D polygonCollider2D;  // 玩家的 PolygonCollider2D 组件
    
    void Start() {
        playerRenderer = GetComponent<Renderer>();
        playerAnimator = GetComponent<Animator>();
        uiScreenFlash = GetComponent<UIScreenFlash>();
        rigidbody = GetComponent<Rigidbody2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();

        // 初始化血量条
        UIHealthBar.HealthMax = health;
        UIHealthBar.HealthCurrent = health;
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(int damage) {
        // 更新血量
        health -= damage;
        // 禁用碰撞体, 实现短暂无敌效果
        polygonCollider2D.enabled = false;
        if (health <= 0) health = 0;
        
        // 更新血量条
        UIHealthBar.HealthCurrent = health;

        // 玩家死亡
        if (health <= 0) {
            playerAnimator.SetTrigger("Death");
            GameController.isPlayerAlive = false;
            // 玩家死亡后, 不能移动, 不能受重力影响
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0;
            return;
        }

        // 受伤后闪烁
        BlinkPlayer(numBlinks, seconds);
        // 屏幕红闪
        uiScreenFlash.FlashScreen();
    }

    /// <summary>
    /// 玩家受到伤害后闪烁
    /// </summary>
    /// <param name="numBlinks">闪烁次数</param>
    /// <param name="seconds">闪烁时间</param>
    void BlinkPlayer(int numBlinks, float seconds) {
        StartCoroutine(DoBlinks(numBlinks, seconds));
    }
    IEnumerator DoBlinks(int numBlinks, float seconds) {
        for(int i = 0; i < numBlinks * 2; i++) {
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(seconds);
        }
        playerRenderer.enabled = true;

        // 闪烁结束后一段时间, 恢复碰撞体
        yield return new WaitForSeconds(0.5f);
        polygonCollider2D.enabled = true;
    }

    /// <summary>
    /// 角色播放完死亡动画后, 销毁角色
    /// </summary>
    void KillPlayer() {
        Destroy(gameObject);
    }
}
