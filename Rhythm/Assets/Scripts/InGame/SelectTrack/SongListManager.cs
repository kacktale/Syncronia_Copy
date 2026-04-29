using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongListManager : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _contentArea;
    [SerializeField] private List<SongData> originSongDB;

    public List<SongData> ReadOnlySongList => originSongDB;

    private bool _isUpdating = false; // 업데이트 중 OnValidate 예외 방지용

    [ContextMenu("리스트 생성 OR 초기화")]
    public void GenerateListInEditor()
    {
        _isUpdating = true; // 업데이트 시작

#if UNITY_EDITOR
        GUI.FocusControl(null); // 포커스 해제 (변경 사항 적용)
        Selection.activeObject = null;
#endif

        while (_contentArea.childCount > 0)
        {
            // 에디터에서는 Destroy가 아니라 DestroyImmediate를 사용해야 함
            DestroyImmediate(_contentArea.GetChild(0).gameObject);
        }

        // 생성
        foreach (SongData data in originSongDB)
        {
#if UNITY_EDITOR
            GameObject newBtn = (GameObject)PrefabUtility.InstantiatePrefab(_buttonPrefab, _contentArea);
#else
            GameObject newBtn = Instantiate(_buttonPrefab, _contentArea);
#endif
            // 데이터 설정
            if (newBtn.TryGetComponent(out SongSelectBtn btnScript))
            {
                btnScript.Setup(data);
                newBtn.name = $"Btn_{data.title}";

#if UNITY_EDITOR
                EditorUtility.SetDirty(newBtn);
#endif
            }
        }

#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate(); // 사용하지 않는 자원 정리
#endif
        Debug.Log($"{originSongDB.Count}개의 버튼 생성 완료");

        _isUpdating = false; // 업데이트 종료
    }

    // 인스펙터 값 변경 시 자동 실행 함수
    private void OnValidate()
    {
        // 업데이트 중이거나 재생 중, 혹은 참조가 없으면 리턴
        if (_isUpdating || Application.isPlaying || _contentArea == null || originSongDB == null) return;

        // 개수가 다를 때 (추가/삭제 시)는 수동 갱신 필요
        if (_contentArea.childCount != originSongDB.Count) return;

        int count = Mathf.Min(_contentArea.childCount, originSongDB.Count);
        for (int i = 0; i < count; i++)
        {
            var btnObj = _contentArea.GetChild(i).gameObject;
            var btnScript = btnObj.GetComponent<SongSelectBtn>();

            if (btnScript != null)
            {
                btnScript.Setup(originSongDB[i]);
                btnObj.name = $"Btn_{originSongDB[i].title}"; // 이름 변경
            }
        }
    }
}