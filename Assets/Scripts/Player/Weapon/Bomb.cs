using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour {
    [Tooltip("炸弹的初始速度")]
    public Vector2 startSpeed = new Vector2(4, 10);
    [Tooltip("炸弹的伤害")]
    public int damage = 5;
    [Tooltip("扔出炸弹后, 延迟爆炸")]
    public float explodeTime = 1.5f;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private PolygonCollider2D explosionRange;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        explosionRange = transform.GetChild(0).gameObject.GetComponent<PolygonCollider2D>();
        rigidbody.velocity = transform.right * startSpeed.x + transform.up * startSpeed.y;

        Invoke("Explode", explodeTime);
    }

    /// <summary>
    /// 扔出去后1.5s, 爆炸
    /// </summary>
    void Explode() {
        animator.SetTrigger("Explode");
    }

    /// <summary>
    /// 开始爆炸后, 进行伤害判定
    /// </summary>
    void StartAttack() {
        explosionRange.enabled = true;
    }

    /// <summary>
    /// 爆炸动画播放完毕后, 销毁炸弹
    /// </summary>
    void DestroyBomb() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 炸弹爆炸时, 对敌人造成伤害
        if (collision.gameObject.CompareTag("Enemy")) {
            Debug.Log("击中敌人");
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        // 炸弹爆炸时, 对玩家造成伤害
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
