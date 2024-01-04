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
    
    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            Destroy(gameObject);
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
}
