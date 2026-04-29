using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    static ResultManager instance;
    public static ResultManager Instance => instance;

    private TrackData.ResultData resultData;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetResult(TrackData.ResultData resultData)
    {
        this.resultData = resultData;
        StartCoroutine(LoadResultScene_CO());
    }

    public TrackData.ResultData GetResult()
    {
        return resultData;
    }

    private IEnumerator LoadResultScene_CO()
    {
        Debug.Log("결과창 로딩..");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(5, LoadSceneMode.Single);
    }
}
