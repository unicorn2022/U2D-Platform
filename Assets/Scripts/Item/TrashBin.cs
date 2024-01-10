using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Tooltip("垃圾桶内金币数量")]
    public int coinCurrent = 0;
    [Tooltip("垃圾桶内金币数量上限")]
    public int coinMax = 10;

    private bool isPlayerInTrashBin = false;    // 玩家是否进入了垃圾桶范围

    void Update() {
        // 按下F键投币
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInTrashBin && coinCurrent < coinMax && UICoin.coinNumber > 0) {
            UICoin.coinNumber--;
            coinCurrent++;
            SoundManager.instance.PlayThrowCoin();
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
