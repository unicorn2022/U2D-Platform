using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("敌人的血量")]
    public int health = 5;
    [Tooltip("敌人的伤害")]
    public int damage = 1;
    [Tooltip("敌人受伤后闪烁的时间")]
    public float flashTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Color originColor;

    protected void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    protected void Update() {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 敌人受到伤害
    /// </summary>
    /// <param name="damage">受到的伤害</param>
    public void TakeDamage(int damage) {
        health -= damage;

        // 受伤后红色闪烁
        spriteRenderer.color = Color.red;
        Invoke("ResetColor", flashTime);
    }
    /// <summary>
    /// 敌人受到伤害后, 闪烁一下, 闪烁结束后, 恢复原来的颜色
    /// </summary>
    void ResetColor() {
        spriteRenderer.color = originColor;
    }
}
