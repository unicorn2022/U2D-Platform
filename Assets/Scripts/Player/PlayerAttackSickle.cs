using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSickle : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();

        controls.GamePlay.Sickle.started += ctx => AttackSickle();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion
    
    [Tooltip("回旋镖")]
    public GameObject sickle;
    
    void AttackSickle() {
        Instantiate(sickle, transform.position, transform.rotation);
    }
}
