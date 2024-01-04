using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("玩家的生命值")]
    public int health = 5;
    [Tooltip("玩家受到伤害后闪烁的次数")]
    public int numBlinks = 2;
    [Tooltip("玩家受到伤害后闪烁的时间")]
    public float seconds = 0.1f;

    private Renderer playerRenderer;    // 玩家的 Renderer 组件
    private Animator playerAnimator;    // 玩家的 Animator 组件
    
    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        playerAnimator = GetComponent<Animator>();

        // 初始化血量条
        UIHealthBar.HealthMax = health;
        UIHealthBar.HealthCurrent = health;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(int damage) {
        // 更新血量
        health -= damage;
        if (health <= 0) health = 0;
        
        // 更新血量条
        UIHealthBar.HealthCurrent = health;

        // 玩家死亡
        if (health <= 0) {
            playerAnimator.SetTrigger("Death");
            Invoke("KillPlayer", 0.9f);
            return;
        }

        // 受伤后闪烁
        BlinkPlayer(numBlinks, seconds);
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
    }

    /// <summary>
    /// 角色播放完死亡动画后(0.9s), 销毁角色
    /// </summary>
    void KillPlayer() {
        Destroy(gameObject);
    }
}
