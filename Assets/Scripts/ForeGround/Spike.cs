using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
    [Tooltip("尖刺的伤害")]
    public int damage = 1;

    private PlayerHealth playerHealth;  // 玩家的生命值组件

    void Start() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            if (playerHealth != null) {
                // 玩家受到伤害
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
