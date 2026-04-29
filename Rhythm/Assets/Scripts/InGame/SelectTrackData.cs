using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
// �ҷ����� ���� �켱

public class SelectTrackData : MonoBehaviour
{
    public static SelectTrackData Instance;

    [Header("Path Settings")]
    public string folderName = "songData";

    public EditorData trackData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadData();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadData()
    {
        if (SongSelectDataManager.Instance == null || SongSelectDataManager.Instance.CurrentSelectedSong == null)
        {
            Debug.LogWarning("SongSelectDataManager �������� ���� Ȥ�� ���õ� �� ����");
            return;
        }

        var currentSong = SongSelectDataManager.Instance.CurrentSelectedSong;
        
        // ���� �̸� + _ + ���̵� 
        string fileName = $"{currentSong.songFileName}_{currentSong.ChoiceSongDifficulty}";

        if (!fileName.EndsWith(".json")) fileName += ".json";

        string loadPath = Path.Combine(Application.dataPath, folderName, fileName);

        if (!File.Exists(loadPath))
        {
            Debug.LogWarning($"[SelectTrackData] �� ���� ������ ã�� �� �����ϴ�! ���: {loadPath}");
            return;
        }

        string loadData = File.ReadAllText(loadPath);
        trackData = JsonUtility.FromJson<EditorData>(loadData);

        Debug.Log($"[SelectTrackData] ������ �ε� ����: {fileName}");
    }
}