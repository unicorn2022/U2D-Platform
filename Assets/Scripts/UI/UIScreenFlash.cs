using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenFlash : MonoBehaviour
{
    [Tooltip("屏幕红闪对应的Image组件")]  
    public Image image;
    [Tooltip("屏幕红闪的持续时间")]
    public float flashTime = 0.1f;
    [Tooltip("屏幕红闪的颜色")]
    public Color flashColor;   // 红闪的颜色

    private Color defaultColor; // 默认的颜色

    void Start() {
        defaultColor = image.color;
    }

    /// <summary>
    /// 屏幕红闪
    /// </summary>
    public void FlashScreen() {
        StartCoroutine(Flash());
    }
    IEnumerator Flash() {
        image.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        image.color = defaultColor;
    }
}
