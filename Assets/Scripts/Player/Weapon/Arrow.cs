using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Arrow : MonoBehaviour {
    [Tooltip("箭的移动速度")]
    public float speed = 15f;
    [Tooltip("箭的伤害值")]
    public int damage = 2;
    [Tooltip("箭的最大飞行距离")]
    public float maxDistance = 15f;

    private Rigidbody2D rigidbody;
    private Vector2 startPosition;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = transform.right * speed;
        startPosition = transform.position;
    }

    void Update() {
        if (Vector2.Distance(transform.position, startPosition) > maxDistance) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
