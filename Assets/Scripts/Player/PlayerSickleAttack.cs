using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSickleAttack : MonoBehaviour
{
    [Tooltip("回旋镖")]
    public GameObject sickle;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) {
            Instantiate(sickle, transform.position, transform.rotation);
        }
    }
}
