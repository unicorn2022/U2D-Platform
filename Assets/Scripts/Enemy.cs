using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("敌人的血量")]
    public int health = 5;
    [Tooltip("敌人的伤害")]
    public int damage = 1;

    protected void Start() {

    }

    protected void Update() {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 敌人受到伤害
    /// </summary>
    /// <param name="damage">受到的伤害</param>
    public void TakeDamage(int damage) {
        health -= damage;
    }
}
