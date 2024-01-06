using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [Tooltip("销毁时间")]
    public float timeToDestroy = 1f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        
    }
}
