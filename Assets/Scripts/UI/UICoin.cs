using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour
{
    [Tooltip("角色当前收集的金币数量")]
    public static int coinNumber;
    [Tooltip("金币的UI文本组件")]
    public Text coinText;

    void Start() {
        coinNumber = 0;
    }

    void Update() {
        coinText.text = coinNumber.ToString();
    }
}
