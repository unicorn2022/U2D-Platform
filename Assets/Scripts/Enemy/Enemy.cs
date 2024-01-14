using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Tooltip("敌人的血量")]
    public int health = 5;
    [Tooltip("敌人的伤害")]
    public int damage = 1;
    [Tooltip("敌人受伤后闪烁的时间")]
    public float flashTime = 0.2f;

    [Tooltip("敌人受伤后的粒子效果")]
    public GameObject bloodEffect;
    [Tooltip("敌人受伤后浮动显示伤害值")]
    public GameObject floatPoint;
    [Tooltip("敌人死亡后掉落的金币")]
    public GameObject dropCoin;

    private SpriteRenderer spriteRenderer;  // 敌人的 SpriteRenderer 组件
    private Color originColor;              // 敌人原来的颜色
    private PlayerHealth playerHealth;      // 玩家的生命值组件

    protected void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    protected void Update() {
        if (health <= 0) {
            Instantiate(dropCoin, transform.position, Quaternion.identity);
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

        // 受伤后, 生成粒子效果
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        
        // 受伤后, 生成浮动显示伤害值
        GameObject floatpoint = Instantiate(floatPoint, transform.position, Quaternion.identity);
        floatpoint.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();

        // 受伤后, 相机抖动
        GameController.cameraShake.Shake();
    }
    /// <summary>
    /// 敌人受到伤害后, 闪烁一下, 闪烁结束后, 恢复原来的颜色
    /// </summary>
    void ResetColor() {
        spriteRenderer.color = originColor;
    }

    /// <summary>
    /// 敌人与玩家碰撞
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        // 如果敌人与玩家碰撞, 并且玩家的碰撞器是胶囊体碰撞器
        if(collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            if(playerHealth != null) {
                // 玩家受到伤害
                playerHealth.TakeDamage(damage);
                // 敌人受到伤害
                TakeDamage(damage);
            }
        }
    }
}
