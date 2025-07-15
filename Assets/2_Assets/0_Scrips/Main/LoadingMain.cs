using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMain : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    public bool isCheck;
    public bool isLoadMap;

    private void OnEnable()
    {
        /* if (DataManager.Instance.LevelCurren == 1)
             StartCoroutine(LoadSceneAsyncCoroutine("2_GamePlay"));
         else*/
        StartCoroutine(LoadSceneAsyncCoroutine("1_Main"));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        float timeElapsed = 0f;
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressBar.fillAmount = progress;
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= 1f && asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
