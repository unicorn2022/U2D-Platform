using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EastEgg : MonoBehaviour {
    [Tooltip("彩蛋密码")]
    public string easterEggPassword = "2314";
    [Tooltip("目前输入的密码")]
    public static string Password;

    [Tooltip("彩蛋生成的金币的预制体")]
    public GameObject coin;
    [Tooltip("彩蛋生成的金币的数量")]
    public int coinQuantity = 20;
    [Tooltip("彩蛋生成的金币的移动速度")]
    public float coinUpSpeed = 10;
    [Tooltip("彩蛋生成的金币的间隔时间")]
    public float intervalTime = 0.1f;

    void Start() {
        Password = "";
    }

    void Update() {
        if(Password == easterEggPassword) {
            StartCoroutine(GetEnumerator());
            Password = "";
        }
    }

    IEnumerator GetEnumerator() {
        for(int i = 0; i < coinQuantity; i++) {
            GameObject gb = Instantiate(coin, transform.position, transform.rotation);
            Vector2 randomDirection = new Vector2(Random.Range(-0.3f, 0.3f), 1.0f);
            gb.GetComponent<Rigidbody2D>().velocity = randomDirection * coinUpSpeed;
            
            yield return new WaitForSeconds(intervalTime);
        }
    }
}
