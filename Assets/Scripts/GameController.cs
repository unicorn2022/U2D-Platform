using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Tooltip("相机抖动组件")]
    public static CameraShake cameraShake;
    [Tooltip("玩家是否存活")]
    public static bool isPlayerAlive = true;
}
