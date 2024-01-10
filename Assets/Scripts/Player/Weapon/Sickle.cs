using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sickle : MonoBehaviour {
    [Tooltip("飞行速度")]
    public float flySpeed = 20.0f;
    [Tooltip("旋转速度")]
    public float rotateSpeed = 30.0f;
    [Tooltip("伤害")]
    public int damage = 2;

    private Rigidbody2D rigidbody;
    private Transform playerTransform;
    private CameraShake cameraShake;
    private Vector2 startSpeed;     // 回旋镖飞出去的速度
    private float returnSpeed;      // 回旋镖回到Player身边的速度

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = transform.right * flySpeed;
        startSpeed = rigidbody.velocity;
        returnSpeed = 0;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    void Update() {
        // 旋转
        transform.Rotate(0, 0, -rotateSpeed);

        // 向前飞, 直到到达最远处
        if(rigidbody.velocity.magnitude > 0.1f) {
            rigidbody.velocity -= startSpeed * Time.deltaTime;
        } 
        // 到达最远处后, 开始向Player飞
        else {
            if(returnSpeed < flySpeed) returnSpeed += flySpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, returnSpeed * Time.deltaTime);
        }
        

        // 回旋镖回到Player身边时, 销毁自身
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            cameraShake.Shake();
        }
    }
}
