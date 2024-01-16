using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpike : MonoBehaviour {
    [Tooltip("造成伤害的碰撞体")]
    public GameObject hideSpikeBox;
    [Tooltip("延迟时间")]
    public float delayTime = 0.5f;

    private Animator animator;
    private bool isAttacking = false; 

    void Start() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            if (!isAttacking) {
                isAttacking = true;
                Invoke("AttackPlayer", delayTime);
            }
        }
    }

    void AttackPlayer() {
        animator.SetTrigger("Attack");
    }

    void ActiveHideSpikeBox() {
        hideSpikeBox.SetActive(true);
    }

    void DeactiveHideSpikeBox() {
        hideSpikeBox.SetActive(false);
        isAttacking = false;
    }
}
