using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Tooltip("平台移动的速度")]
    public float speed = 2f;
    [Tooltip("平台到达目标点后移动的时间")]
    public float waitTime = 0.5f;
    [Tooltip("平台移动的目标点")]
    public Transform[] movePositions;

    private int i;
    private float hasWaited;
    private Transform playerDefaultParent;

    void Start() {
        i = 0;
        hasWaited = 0f;
    }

    void Update() {
        if(Vector2.Distance(transform.position, movePositions[i].position) < 0.1f) {
            hasWaited += Time.deltaTime;
            if(hasWaited >= waitTime) {
                hasWaited = 0f;
                i++;
                if(i >= movePositions.Length)  i = 0;
            }
        } else {
            transform.position = Vector2.MoveTowards(transform.position, movePositions[i].position, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 与玩家碰撞, 玩家跟随平台移动
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            playerDefaultParent = collision.transform.parent;
            collision.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// 玩家离开平台, 玩家不再跟随平台移动
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            collision.transform.SetParent(playerDefaultParent);
        }
    }
}
