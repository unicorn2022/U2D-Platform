using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBomb : MonoBehaviour { 
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();

        controls.GamePlay.PlayerAttackBomb.started += ctx => AttackBomb();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion
    
    [Tooltip("炸弹")]
    public GameObject bomb;

    void AttackBomb() {
        Instantiate(bomb, transform.position, transform.rotation);
    }
}
