using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmartBat : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("当玩家与敌人距离一定范围以内, 敌人开始追击玩家")]
    public float radius = 15f;

    private Transform playerTransform;

    void Start() {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        base.Update();

        // 如果玩家在一定范围内, 敌人开始追击玩家
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < radius) {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        }
    }
}
