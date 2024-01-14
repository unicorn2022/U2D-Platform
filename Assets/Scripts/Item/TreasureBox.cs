using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();
        controls.GamePlay.Communicate.started += ctx => CommunicateWithTreasureBox();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("宝箱中的物品")]
    public GameObject coin;

    private bool canOpen = false;
    private bool isOpened = false;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void CommunicateWithTreasureBox() {
        if (canOpen && !isOpened) {
            animator.SetTrigger("Open");
            isOpened = true;
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
