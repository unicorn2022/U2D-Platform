using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour {
    [Tooltip("捡起的武器对应玩家的第几种攻击方式")]
    public int weaponID;

    private GameObject weapon;

    void Start() {
        weapon = GameObject.Find("Player").transform.GetChild(weaponID).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            Destroy(gameObject);
            weapon.SetActive(true);
        }
    }
}
