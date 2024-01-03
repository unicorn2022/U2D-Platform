using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("水平方向的移动速度")]
    public float runSpeed = 5.0f;
    private Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Run();      
    }

    /// <summary>
    /// 水平方向的移动
    /// </summary>
    void Run() {
        // 通过刚体组件, 控制角色移动
        if (rigidbody != null) {
            float moveDir = Input.GetAxis("Horizontal"); // 水平方向的移动
            Vector2 playerVelocity = new Vector2(moveDir * runSpeed, rigidbody.velocity.y);
            rigidbody.velocity = playerVelocity;
        }
    }
}
