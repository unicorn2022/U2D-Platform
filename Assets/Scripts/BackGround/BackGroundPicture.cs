using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundPicture : MonoBehaviour {

    [Header("背景图片数组")]
    public Sprite[] backGroundPictures;

    private Animator animator;              // 动画控制器
    private SpriteRenderer spriteRenderer;  // 背景图片渲染器

    public int currentBackGround;  // 当前背景图片下标
    private bool needChange;        // 是否需要切换背景图片

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentBackGround = 0;
        needChange = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            animator.SetTrigger("ChangeBackGround");
            needChange = true;
        }

        if(needChange && spriteRenderer.color.a == 0f) {
            currentBackGround = (currentBackGround + 1) % backGroundPictures.Length;
            spriteRenderer.sprite = backGroundPictures[currentBackGround];
            needChange = false;
        }
    }
}
