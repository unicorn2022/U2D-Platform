using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySnake : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("敌人移动的等待时间")]
    public float startWaitTime = 1f;
    [Tooltip("爬行的目标位置-左")]
    public Transform moveLeftPositions;
    [Tooltip("爬行的目标位置-右")]
    public Transform moveRightPositions;

    public bool moveRight = true;
    private float waitTime;             // 敌人移动的等待时间

    protected void Start() {
        // 初始化随机运动
        moveLeftPositions.localPosition = new Vector3(Random.Range(-5f, 0f), moveLeftPositions.localPosition.y, 0f);
        moveRightPositions.localPosition = new Vector3(Random.Range(0f, 5f), moveRightPositions.localPosition.y, 0f);
        startWaitTime = Random.Range(0.5f, 1.5f);
        moveRight = Random.Range(0, 2) == 0;
        if(moveRight) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }

        base.Start();
        waitTime = startWaitTime;
    }

    protected void Update() {
        base.Update();
        Vector3 targetPosition = moveRight ? moveRightPositions.position : moveLeftPositions.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    
        if(Vector2.Distance(transform.position, targetPosition) < 0.1f) {
            if (waitTime > 0) waitTime -= Time.deltaTime;
            else {
                // 转向
                if (moveRight) {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    moveRight = false;
                } else {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    moveRight = true;
                }
                // 重置等待时间
                waitTime = startWaitTime;
            }
        }
    }

}
