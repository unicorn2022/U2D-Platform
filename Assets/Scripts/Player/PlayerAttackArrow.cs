using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArrow : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();

        controls.GamePlay.PlayerAttackArrow.started += ctx => AttackArrow();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("弓箭")]
    public GameObject arrow;

    void AttackArrow() {
        Instantiate(arrow, transform.position, transform.rotation);
    }
}
