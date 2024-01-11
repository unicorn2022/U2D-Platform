using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Text loadingText;

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneIndex">场景序号</param>
    public void LoadLevel(int sceneIndex) {
        StartCoroutine(AsyncLoadLevel(sceneIndex));
    }
    IEnumerator AsyncLoadLevel(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        // 等待操作完成
        while (!operation.isDone) {
            float progress = operation.progress / 0.9f; // progress的范围是[0, 0.9]
            loadingSlider.value = progress;
            loadingText.text = string.Format("{0:0}%", progress * 100);
            yield return null;
        }
    }


    /// <summary>
    /// 开始游戏
    /// </summary>
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame() {
        Application.Quit();
    }
}
