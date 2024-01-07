using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatPoint : MonoBehaviour {
    [Tooltip("销毁时间")]
    public float timeToDestroy = 0.6f;
    void Start() {
        Destroy(gameObject, timeToDestroy);
    }
}
