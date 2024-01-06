using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("相机的动画组件")]
    public Animator cameraAnimator;
    
    void Start()
    {
        GameController.cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 相机抖动
    /// </summary>
    public void Shake() {
        cameraAnimator.SetTrigger("Shake");
    }
}
