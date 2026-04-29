using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongDetailInfo : MonoBehaviour
{
    public static SongDetailInfo Instance;

    //public Image songImage;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI BPMText;
    public TextMeshProUGUI composerText;
    public TextMeshProUGUI patternNameText;
    public TextMeshProUGUI lastRateText;
    public TextMeshProUGUI difficultyText;

    private int selectInfo = 0;
    public SongData currentSong;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateSongInfo(SongData data)
    {
        if (data == null) return;
        currentSong = data;

        //songImage.sprite = data.title_img;
        songNameText.SetText($"<{data.title}>");
        BPMText.SetText($"BPM : {data.BPM}");
        composerText.SetText(data.composer);

        // Rate가 0보다 클 때만 표시, 아니면 공란 처리
        if (data.lastRate > 0) lastRateText.SetText($"Rate : {data.lastRate:F2}%");
        else lastRateText.SetText("");

        UpdateDifficulty(0);
    }

    public void UpdateDifficulty(int diffIndex)
    {
        if (currentSong == null) return; // 여기서 써먹습니다.

        if (currentSong.difficulties == null || currentSong.difficulties.Count == 0)
        {
            difficultyText.SetText("LV.-");
            return;
        }

        if (diffIndex < 0 || diffIndex >= currentSong.difficulties.Count) return;

        difficultyText.SetText($"LV.{currentSong.difficulties[diffIndex].songlevel}");
    }
}
