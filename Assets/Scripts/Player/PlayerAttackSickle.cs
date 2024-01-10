using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSickle : MonoBehaviour
{
    [Tooltip("回旋镖")]
    public GameObject sickle;

    void Update() {
        if(Input.GetKeyDown(KeyCode.E)) {
            Instantiate(sickle, transform.position, transform.rotation);
        }
    }
}
