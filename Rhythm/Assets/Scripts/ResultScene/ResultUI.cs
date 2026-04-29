using System.Collections;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    private TrackData.ResultData resultData;

    [SerializeField] private TextMeshProUGUI rateText;
    [SerializeField] private TextMeshProUGUI rateValue;
    [SerializeField] private TextMeshProUGUI breakValue;
    [SerializeField] private TextMeshProUGUI maxMarvlessValue;
    [SerializeField] private TextMeshProUGUI maxHitValue;
    [SerializeField] private TextMeshProUGUI bestComboValue;

    private string[] rating = new string[] { "S", "A", "B", "C", "D" };
    private int[] ratingScore = new int[]{ 95, 90, 80, 70, 60 };

    private void Awake()
    {
        FmodAudioManager.Instance.StopBGM();
    }
    private void Start()
    {
        resultData = ResultManager.Instance.GetResult();
        StartCoroutine(TextLerpAnim_CO());
    }

    private IEnumerator TextLerpAnim_CO()
    {
        float lerpTime = 2;
        float elapsed = 0;

        for (int i = 0; i < rating.Length; i++)
        {
            if(ratingScore[i] <= resultData.rate && resultData.isClear)
            {
                rateText.text = rating[i];
                break;
            }
            if(!resultData.isClear)
            {
                rateText.text = "F";
                break;
            }
        }

        while (lerpTime > elapsed)
        {
            // TODO : 
            elapsed += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(elapsed / lerpTime);

            float tmp = Mathf.FloorToInt(Mathf.Lerp(0, resultData.rate, lerpValue) * 100);
            rateValue.text = (tmp / 100).ToString("F2") + "%";
            breakValue.text = Mathf.FloorToInt(Mathf.Lerp(0, resultData.maxBreak, lerpValue)).ToString();
            maxMarvlessValue.text = Mathf.FloorToInt(Mathf.Lerp(0, resultData.maxMarvless, lerpValue)).ToString();
            maxHitValue.text = Mathf.FloorToInt(Mathf.Lerp(0, resultData.maxHit, lerpValue)).ToString();
            bestComboValue.text = Mathf.FloorToInt(Mathf.Lerp(0, resultData.maxCombo, lerpValue)).ToString();

            yield return null;
        }
    }
}
