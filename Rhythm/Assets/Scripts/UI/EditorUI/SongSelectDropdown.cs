using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SongSelectDropdown : MonoBehaviour
{
    public TMP_Dropdown selectDropdown;

    private List<string> songPaths = new List<string>();

    private float songTime;

    private void Start()
    {
        GratifyDropDown();

        selectDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        if (songPaths.Count > 0)
        {
            OnDropdownValueChanged(0);
        }
    }

    public void GratifyDropDown()
    {
        selectDropdown.ClearOptions();
        songPaths.Clear();

        RuntimeManager.StudioSystem.getBank("bank:/Master", out Bank masterBank);

        if (!masterBank.isValid())
        {
            Debug.LogError("Master Bank 없음");
            return;
        }

        // Bank 안에 있는 모든 Event 가져온거
        masterBank.getEventList(out EventDescription[] events);
        List<string> dropdownOptions = new List<string>();

        foreach(var desc in events)
        {
            desc.getPath(out string path);

            if (!path.StartsWith("event:/TestSong/")) continue; // 가져오는 경로 TestSong 안에서만 가져오게 처리했음

            string songName = path.Substring(path.LastIndexOf("/") + 1);

            dropdownOptions.Add(songName);
            songPaths.Add(path); // FMOD 경로 보관
        }

        selectDropdown.AddOptions(dropdownOptions);
        Debug.Log($"{dropdownOptions.Count} 개의 곡 불러옴");
    }

    public string GetSelectSongPath()
    {
        if(songPaths.Count == 0) return string.Empty;
        return songPaths[selectDropdown.value];
    }


    // TODO: Editor FmodAudioManager 지금 선택된 곡을 넘기고 오딛오 변경을 요청하는 코드
    private void OnDropdownValueChanged(int index)
    {
        if (songPaths.Count == 0) return;

        string selectedPath = songPaths[index];
        Debug.Log($"선택된 곡 경로: {selectedPath}");

        
        EditorAudioManager.Instance.ChangeAndPlayMusic(selectedPath); 
    }
}
