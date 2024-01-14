using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();
        controls.GamePlay.Pause.started += ctx => CommunicateWithPauseMenu();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("游戏是否暂停")]
    public static bool isPaused = false;
    [Tooltip("暂停菜单UI")]
    public GameObject pauseMenuUI;

    void CommunicateWithPauseMenu() {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ResumeGame() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void MainMenu() {
        Time.timeScale = 1.0f;
        isPaused = false;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame() {
        Application.Quit();
    }
}
