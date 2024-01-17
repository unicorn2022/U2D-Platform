using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToNextLevel : MonoBehaviour {
    [Tooltip("通过MainMenu脚本实现场景加载")]
    public MainMenu mainMenu;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            int index = SceneManager.GetActiveScene().buildIndex;
            mainMenu.LoadLevel(index + 1);
        }
    }
}
