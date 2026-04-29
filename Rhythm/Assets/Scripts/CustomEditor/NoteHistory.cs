using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum NoteHistoryType
{
    Note,
    LongNote
}

[System.Serializable]
public class NoteHistory
{
    public NoteHistoryType type;

    /// <summary>
    /// 롱노트 기본 노트 구분 없이 리스트에 넣고 롱노트에 경우 끝 부분에서 한 리스트에 시작 꼬리 끝을 넣어주는 방식으로 제작
    /// </summary>
    public List<GameObject> NoteHistoryObjs = new List<GameObject>();

    public EditorNoteData noteData;
    public EditorLongNoteData longNoteData;
}