using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InGameLoad : MonoBehaviour
{
    public static InGameLoad Instance;

    public RectTransform loadingBar;
    public Image loadingImage;
    public Vector2[] posVec;
    [SerializeField] private FMODUnity.EventReference[] bgmAddress;

    private bool loadingOn = false;
    private float alpha = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        EndLoading();
    }

    void Update()
    {
        if (loadingOn)
        {
            alpha = Mathf.Lerp(alpha, 1, Time.smoothDeltaTime / 0.2f);
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, alpha);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[0], Time.smoothDeltaTime / 0.2f);
        }
        else
        {
            alpha = Mathf.Lerp(alpha, 0, Time.smoothDeltaTime / 0.2f);
            loadingImage.color = new Color(loadingImage.color.r, loadingImage.color.g, loadingImage.color.b, alpha);
            loadingBar.anchoredPosition = Vector2.Lerp(loadingBar.anchoredPosition, posVec[1], Time.smoothDeltaTime / 0.2f);
        }
    }

    public void StartLoading(int sceneIndex)
    {
        Debug.Log($"[InGameLoad] Load Index: {sceneIndex}");
        StartCoroutine(LoadSceneRoutine(sceneIndex, string.Empty));
    }

    public void StartLoading(string sceneName)
    {
        Debug.Log($"[InGameLoad] Load Name: {sceneName}");
        StartCoroutine(LoadSceneRoutine(-1, sceneName));
    }

    private IEnumerator LoadSceneRoutine(int index, string name)
    {
        loadingOn = true;
        FMODUnity.EventReference currentBGM = bgmAddress[index -1];

        FmodAudioManager.Instance.FadeOutBGM(0.8f);

        yield return new WaitForSecondsRealtime(2f);

        if (!string.IsNullOrEmpty(name)) SceneManager.LoadScene(name);
        else if (index >= 0) SceneManager.LoadScene(index);
        else Debug.LogError("¾ÀÀÌ ¾ø½¹");

        EndLoading();
        if(index == 3) FmodAudioManager.Instance.FadeInBGM(currentBGM, 0.8f);
    }

    public void EndLoading() => loadingOn = false;
}