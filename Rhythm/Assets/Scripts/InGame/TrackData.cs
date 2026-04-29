using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DefaultExecutionOrder(-150)]

public class TrackData : MonoBehaviour
{
    public int combo = 0;
    public int[] judgeHit = new int[6]; // Mav, Per, Good, Near, Bad, Miss
    private float[] judgeDamage = new float[6] { 0.5f, 0.4f, 0.3f, 0.2f, 0.1f, -5}; // Mav, Per, Good, Near, Bad, Miss
    public int point = 0;
    public enum judgeValue { Marvless = 0, Perfect = 1, Good = 2, Near = 3, Bad = 4, Miss = 5 };
    public judgeValue judge;
    public Color[] judgeColors;

    public TextMeshProUGUI comboText;
    public TextMeshProUGUI judgeText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI countText;
    public Animator comboAnimation;
    public Animator JudgeAnimation;
    public Animator[] HitAnimation;
    public Animator countAnimation;
    public SpriteRenderer[] hitSprite;
    public Slider hpSlider;
    public static TrackData Instance;

    public bool PlayStoped;
    public int finishJudgeCount;

    private int maxCombo = 0;

    private float maxHp = 100;

    public struct ResultData
    {
        public int maxMarvless;
        public int maxHit;
        public int maxBreak;
        public int maxCombo;
        public float rate;
        public bool isClear;
    }
    void Awake()
    {
        //UpdateUI();
        Instance = this;
        PlayStoped = true;
    }
    private void Start()
    {

        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;

        StartCoroutine(StartDelay());
    }

    float CheckRate()
    {
        float rate = 0;
        int pressNote = 0;

        for(int i = 0; i < judgeHit.Length; i++)
        {
            rate += judgeHit[i] * (10 - i * 2) * 10;
            pressNote += judgeHit[i];
        }
        
        if (pressNote > 0)
        {
            rate /= pressNote;
        }

        return rate;
    }

    void UpdateUI()
    {
        comboText.text = combo.ToString();
        comboAnimation.SetTrigger("TextAnim");
        rateText.text = "Rate : " + CheckRate().ToString("F2") + "%";
        maxCombo = combo > maxCombo ? combo : maxCombo;
    }

    public void UpdateJudge(int num, int line = 5)
    {
        judgeText.text = judge.ToString();
        judgeText.color = judgeColors[num];
        JudgeAnimation.SetTrigger("TextAnim");
        if(num != 5)
        {
            hitSprite[line].color = judgeColors[num];
            HitAnimation[line].SetTrigger("Hit");
        }
        UpdateUI();

        hpSlider.value += judgeDamage[num];
        if(hpSlider.value <= 0)
        {
            hpSlider.value = 0;
            PlayStoped = true;
            SetResultData(false);
        }
    }

    public void UpdateCountDown(int countValue)
    {
        countText.text = countValue.ToString();
        countAnimation.SetTrigger("TextAnim");
    }

    public void SetResultData(bool isClear)
    {
        ResultData resultData = new ResultData();

        resultData.maxMarvless = judgeHit[0];
        for (int i = 0; i < judgeHit.Length; i++)
        {
            //Debug.Log($"{"판정 :"}{(judgeValue)i}{"갯수 :"}{judgeHit[i]}");
            resultData.maxHit += judgeHit[i];
            resultData.maxBreak = i == 5 ? judgeHit[i] : resultData.maxBreak;
        }
        Debug.Log(judgeHit[0] + judgeHit[5]);
        resultData.maxHit -= (judgeHit[0] + judgeHit[5]);
        resultData.maxCombo = maxCombo;
        resultData.rate = CheckRate();
        resultData.isClear = isClear;

        ResultManager.Instance.SetResult(resultData);
    }

    //public ResultData GetResultData()
    //{
    //    return resultData;
    //}

    public void StopTrack(InputAction.CallbackContext context)
    {
        PlayStoped = true;
        PanelManager.Instance.ShowPanel<GamePausePanel>();
    }

    public void StartTrack()
    {
        PlayStoped = false;
        PanelManager.Instance.HidePanel<GamePausePanel>();
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2f);
        StartTrack();
    }
}
