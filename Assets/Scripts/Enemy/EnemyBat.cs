using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("敌人移动的等待时间")]
    public float startWaitTime = 1f;

    [Tooltip("敌人的移动范围: 左下角")]
    public Transform leftDownPosition;
    [Tooltip("敌人的移动范围: 右上角")]
    public Transform rightUpPosition;

    private float waitTime;             // 敌人移动的等待时间
    private Vector2 targetPosition;     // 敌人移动的目标位置
    protected void Start()
    {
        base.Start();
        waitTime = startWaitTime;
        targetPosition = GetRandomPosition();
    }

    protected void Update()
    {
        base.Update();

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, targetPosition) < 0.1f) {
            if (waitTime <= 0) {
                targetPosition = GetRandomPosition();
                waitTime = startWaitTime;
            } else {
                waitTime -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 在规定的范围内, 随机生成一个位置
    /// </summary>
    /// <returns>随机位置</returns>
    Vector2 GetRandomPosition() {
        return new Vector2(
            Random.Range(leftDownPosition.position.x, rightUpPosition.position.x),
            Random.Range(leftDownPosition.position.y, rightUpPosition.position.y)
        );
    }
}
