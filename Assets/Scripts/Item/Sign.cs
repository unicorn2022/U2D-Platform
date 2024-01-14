using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();
        controls.GamePlay.Communicate.started += ctx => CommunicateWithSign();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion
    
    [Tooltip("对话框")]
    public GameObject dialogBox;
    [Tooltip("对话框的Text组件")]
    public Text dialogBoxText;
    [Tooltip("对话框中的显示的文字")]
    public string dialogText;

    private bool isPlayerInSign = false;    // 玩家是否进入了sign范围

    void CommunicateWithSign() {
        if(isPlayerInSign) {
            dialogBoxText.text = dialogText;
            dialogBox.SetActive(true);
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInSign = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            dialogBox.SetActive(false);
            isPlayerInSign = false;
        }
    }
}
