using UnityEngine;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine.Video;

[System.Serializable]
public struct DifficultyInfo
{
    public SonglvEnum difficultyType;
    public int songlevel;
}

[System.Serializable]
public class SongData
{
    [Header("Basic data")]
    public string title;
    public float BPM;
    public Sprite title_img;
    public string composer;
    public float lastRate;

    [Header("Difficulty Info")]
    public List<DifficultyInfo> difficulties = new List<DifficultyInfo>();

    [Header("Send song Info")]
    public string songFileName;
    public SonglvEnum ChoiceSongDifficulty;
    public FMODUnity.EventReference FmodEvent;
    public VideoClip SongVideoClip;
}
