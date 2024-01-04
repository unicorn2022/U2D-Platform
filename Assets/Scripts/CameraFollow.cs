using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("相机跟随的目标")]
    public Transform target;
    [Tooltip("平滑值"), Range(0, 1)]
    public float smoothing = 0.1f;

    [Tooltip("相机的最小位置")]
    public Vector2 minPosition;
    [Tooltip("相机的最大位置")]
    public Vector2 maxPosition;

    void Start()
    {
        
    }

    private void LateUpdate() {
        if (target == null) return;

        if (transform.position != target.position) {
            Vector3 targetPosition = target.position;
            // 限定相机的移动范围
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            // 通过插值的方式, 让相机移动到目标位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }

    /// <summary>
    /// 设置相机的移动范围
    /// </summary>
    /// <param name="minPos">最小位置</param>
    /// <param name="maxPos">最大位置</param>
    public void SetCameraPositionLimit(Vector2 minPos, Vector2 maxPos) {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}
