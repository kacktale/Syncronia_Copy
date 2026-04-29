using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EditorData
{
    public string Name;
    public int noteCount = 0;
    public int BPM = 0;
    public int metronomeCount = 0;
    public BossSkillType bossType = BossSkillType.None;

    public List<EditorNoteData> noteData;
    public List<EditorLongNoteData> longnoteData;
}

public class EditorSave : MonoBehaviour
{
    public static EditorSave instance;
    public EditorData data;
    public string folderPath = "Assets/songData"; // 파일 경로

    void Awake()
    {
        instance = this;
        if (data.noteData == null) data.noteData = new List<EditorNoteData>();
        if (data.longnoteData == null) data.longnoteData = new List<EditorLongNoteData>();
    }

    [ContextMenu("To Json Data")]
    public void SaveData(string fileName)
    {
        data.noteData.Sort((a, b) => a.noteMetronomePosY.CompareTo(b.noteMetronomePosY));
        data.longnoteData.Sort((a, b) => a.start.noteMetronomePosY.CompareTo(b.start.noteMetronomePosY));

        data.Name = fileName;
        string save = JsonUtility.ToJson(data, true);

        if (!fileName.EndsWith(".json")) fileName += ".json";

        string savePath = Path.Combine(folderPath, fileName);

        // 폴더가 존재하는지 확인만
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"폴더가 없음! : {folderPath}");
            return;
        }

        File.WriteAllText(savePath, save);
        Debug.Log($"저장 완료: {savePath}");
    }

    public bool CheckFileExist(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        if (!fileName.EndsWith(".json")) fileName += ".json";

        return File.Exists(Path.Combine(folderPath, fileName));
    }

    public void LoadData(string fileName)
    {
        if (!fileName.EndsWith(".json")) fileName += ".json";

        string loadPath = Path.Combine(folderPath, fileName);

        if (File.Exists(loadPath))
        {
            string loadJson = File.ReadAllText(loadPath);
            data = JsonUtility.FromJson<EditorData>(loadJson);

            if (data.noteData == null) data.noteData = new List<EditorNoteData>();
            if (data.longnoteData == null) data.longnoteData = new List<EditorLongNoteData>();

            Debug.Log("File Load Complete: " + fileName);
        }
        else
        {
            Debug.LogError("File Not Found: " + loadPath);
        }
    }

    public void AllClearNoteData()
    {
        data.noteData.Clear();
        data.longnoteData.Clear();
        data.noteCount = 0;
        Debug.LogWarning("노트 데이터를 모두 초기화했는데 님 ㄱㅊ?");
    }
}