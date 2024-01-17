using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour {
    [Tooltip("子弹预制体")]
    public GameObject bullet;
    [Tooltip("子弹发射位置")]
    public Transform muzzlePosition;

    private Vector3 mousePosition;
    private Vector2 gunDirection;

    void Update() {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        gunDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        if(Mouse.current.leftButton.isPressed) {
            Instantiate(bullet, muzzlePosition.position, Quaternion.Euler(transform.eulerAngles));
        }
    }
}
