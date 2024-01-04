using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("相机跟随的目标")]
    public Transform target;
    [Tooltip("平滑值"), Range(0, 1)]
    public float smoothing = 0.1f;

    void Start()
    {
        
    }

    private void LateUpdate() {
        if (target == null) return;

        // 通过插值的方式, 让相机移动到目标位置
        if (transform.position != target.position) {
            transform.position = Vector3.Lerp(transform.position, target.position, smoothing);
        }
    }

    void Update()
    {
        
    }
}
