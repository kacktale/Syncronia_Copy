using UnityEngine;

[DefaultExecutionOrder(-150)]
public class SongSelectDataManager : MonoBehaviour
{
    public static SongSelectDataManager Instance;

    public SongData CurrentSelectedSong { get; private set; }

    private void Awake()
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
    }

    public void SetSong(SongData song)
    {
        CurrentSelectedSong = song;
        Debug.Log($"[Manager] 확정된 곡: {song.title} / 난이도: {song.ChoiceSongDifficulty}");
    }
}