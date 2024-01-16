using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlatform : MonoBehaviour {
    private Animator animator;
    private BoxCollider2D boxCollider;

    void Start() {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            animator.SetTrigger("Collapse");
        }
    }

    void DisableBoxCollider2D() {
        boxCollider.enabled = false;
    }

    void DestroyTrapPlatform() {
        Destroy(gameObject);
    }
}
