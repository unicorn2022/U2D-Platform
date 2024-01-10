using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashBinUI : MonoBehaviour
{
    [Tooltip("全局UI画布")]
    public RectTransform CanvasRect;

    [Tooltip("垃圾桶UI预制件")]
    public GameObject uiPrefab;
    [Tooltip("垃圾桶UI偏移")]
    public Vector2 offset = new Vector2(0, 80);


    private TrashBin trashBin;          // 垃圾桶脚本
    private RectTransform trashBinUI;   // 垃圾桶UI元素
    private Image coinNumberImage;      // 垃圾桶显示金币数量的Image组件
    private Text coinNumberText;        // 垃圾桶显示金币数量的Text组件

    void Start() {
        GameObject gameObject = Instantiate(uiPrefab, GameObject.Find("Canvas").transform);
        trashBinUI = gameObject.GetComponent<RectTransform>();
        coinNumberImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        coinNumberText = gameObject.transform.GetChild(1).GetComponent<Text>();
        trashBin = GetComponent<TrashBin>();
    }

    void Update() {
        // 将垃圾桶的世界坐标转换为视口坐标
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        // 将视口坐标转换为画布坐标
        Vector2 screenPosition = new Vector2(
            ((viewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + offset.x,
            ((viewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + offset.y
        );
        // 更新垃圾桶UI元素的位置
        trashBinUI.anchoredPosition = screenPosition;

        // 更新垃圾桶UI中金币数量的显示
        coinNumberImage.fillAmount = (float)trashBin.coinCurrent / trashBin.coinMax;
        coinNumberText.text = trashBin.coinCurrent + " / " + trashBin.coinMax;
    }
}
