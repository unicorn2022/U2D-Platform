using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour {
    [Tooltip("宝箱中的物品")]
    public GameObject coin;

    private bool canOpen = false;
    private bool isOpened = false;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        // F键开启宝箱
        if (canOpen && !isOpened && Input.GetKeyDown(KeyCode.F)) {
            animator.SetTrigger("Open");
            isOpened = true;
            Invoke("GenerateCoin", 0.5f);
            Invoke("DestroyBox", 5f);
        }
    }

    /// <summary>
    /// 生成金币
    /// </summary>
    void GenerateCoin() {
         Instantiate(coin, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 销毁宝箱
    /// </summary>
    void DestroyBox() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            canOpen = false;
        }
    }
}
