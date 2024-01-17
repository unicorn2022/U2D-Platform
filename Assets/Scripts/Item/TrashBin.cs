using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TrashBin : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();
        controls.GamePlay.Communicate.started += ctx => CommunicateWithTrashBin();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("垃圾桶内金币数量")]
    public int coinCurrent = 0;
    [Tooltip("垃圾桶内金币数量上限")]
    public int coinMax = 10;
    [Tooltip("金币满后, 可以激活的物体")]
    public GameObject[] activeObjects;

    private bool isPlayerInTrashBin = false;    // 玩家是否进入了垃圾桶范围

    void CommunicateWithTrashBin() {
        if (isPlayerInTrashBin && coinCurrent < coinMax && UICoin.coinNumber > 0) {
            UICoin.coinNumber--;
            coinCurrent++;
            SoundManager.instance.PlayThrowCoin();
        }

        if(coinCurrent == coinMax) {
            for(int i = 0; i < activeObjects.Length; i++)
                activeObjects[i].SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInTrashBin = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInTrashBin = false;
        }
    }
}
