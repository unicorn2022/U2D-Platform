using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBomb : MonoBehaviour { 
    [Tooltip("炸弹")]
    public GameObject bomb;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            Instantiate(bomb, transform.position, transform.rotation);
        }
    }
}
