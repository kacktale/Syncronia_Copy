using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;

public class MakeNote : MonoBehaviour
{
    public static MakeNote Instance;
    public GameObject[] notes;
    public Transform noteParants;
    public int selectNoteType = 0;

    private List<List<GameObject>> noteDummy;
    private List<EditorNoteData> noteDataTmp;

    private EditorLongNoteData currentLongNoteData;

    public EditorNoteData selectNoteData;
    private GameObject lastLongNoteTrail;

    public int inputX = 0;
    public int inputY = 0;

    public bool[] BeatMode = new bool[3];
    public int beatInput = 0;
    public bool createLongNote = false;
    public int longNoteLine;

    [Header("NoteInstall")]
    public Vector3 mousePosition;
    public Vector3 positionFix;
    public Transform preNote;
    public GameObject[] preNoteObj;
    private GameObject placedNote;
    public bool editorUIOpen => PanelManager.Instance != null && (PanelManager.Instance.IsBlockPanelOpen || PanelManager.Instance.IsEditorPanelOpen);

    [Header("NewInputSystem")]
    InputSystem_Actions inputActions;

    [Header("Undo")]
    private List<NoteHistory> undoList = new List<NoteHistory>();
    private List<NoteHistory> redoList = new List<NoteHistory>();
    private int maxUndoCount = 30;
    private int maxRedoCount = 30;

    private void Awake()
    {
        Instance = this;

        inputActions = new InputSystem_Actions();

        inputActions.UI.Undo.performed += ctx => UndoNote();
        inputActions.UI.Redo.performed += ctx => RedoNote();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Start()
    {
        noteDummy = new List<List<GameObject>>();
        noteDataTmp = new List<EditorNoteData>();
        for (int i = 0; i < 4; i++) noteDummy.Add(new List<GameObject>());
        MakeDummyNote();
    }

    float Snap(float value)
    {
        float pos1 = Mathf.Repeat(value, 1.5f);
        return value = (value - pos1) + (pos1 >= 0.75f ? 1.5f : 0f);
    }

    float SnapY(float value)
    {
        float pos1, pos2;
        if (BeatMode[0]) { pos1 = Mathf.Repeat(value, 1.5f); pos2 = value - pos1; pos1 = pos1 >= 0.75f ? 1.5f : 0f; }
        else if (BeatMode[1]) { pos1 = Mathf.Repeat(value, 0.75f); pos2 = value - pos1; pos1 = pos1 >= 0.375f ? 0.75f : 0f; }
        else { pos1 = Mathf.Repeat(value, 0.375f); pos2 = value - pos1; pos1 = pos1 >= 0.1875f ? 0.375f : 0f; }
        return value = pos2 + pos1;
    }

    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        positionFix = new Vector3(Snap(mousePosition.x), SnapY(mousePosition.y), 0f);

        inputX = (int)(positionFix.x / 1.5f) + 2;
        inputY = (int)((positionFix.y + 4.5f) / 1.5f);

        ShowDummyNote();
        SerchNoteInstall();
        SerchRemoveNote();

        // if (Input.mouseScrollDelta.y != 0 && MakeGrid.Instance.IsCreated)
        // {
        //     noteParants.transform.position -= new Vector3(0, Input.mouseScrollDelta.y * 1.5f, 0);
        // }

        // TODO : MakeGrid 파트로 이동했음.
    }

    void ShowDummyNote()
    {
        if (!editorUIOpen)
        {
            preNote.gameObject.SetActive((positionFix.x <= 1.5f && positionFix.x >= -3f && MakeGrid.Instance.IsCreated));
            preNote.position = positionFix;
        }
    }

    void SerchNoteInstall()
    {
        if (Input.GetMouseButtonDown(0) && preNote.gameObject.activeInHierarchy && !NotePlaced() && !editorUIOpen)
        {
            GameObject obj = CheckNoteCollect(selectNoteType);
            obj.transform.position = positionFix;

            bool isAlreadyCreating = createLongNote; // 상태 변경 전 저장
            SerchLongNote(obj);

            EditorNoteData newNoteData = SetData();
            noteDataTmp.Add(newNoteData); // 단일 노트 데이터를 상호작용 리스트에 추가
            selectNoteData = newNoteData;

            if (selectNoteType == 1)
            {
                // 롱노트 시작 시 구조체 생성 및 리스트 추가
                if (createLongNote)
                {
                    currentLongNoteData = new EditorLongNoteData();
                    currentLongNoteData.start = newNoteData;
                    EditorSave.instance.data.longnoteData.Add(currentLongNoteData);
                    EditorSave.instance.data.noteCount++;

                    NoteHistory newHistory = new NoteHistory();
                    newHistory.type = NoteHistoryType.LongNote;
                    newHistory.longNoteData = currentLongNoteData;
                    newHistory.NoteHistoryObjs.Add(obj);
                    AddToUndoList(newHistory);
                }

                // 롱노트 종료 시 End 데이터 주입 및 완료 처리 -> 계산 ** 어려워서 AI 돌림
                else if (isAlreadyCreating && !createLongNote)
                {
                    if (currentLongNoteData != null)
                    {
                        currentLongNoteData.end = newNoteData;
                        currentLongNoteData.tailLength =Mathf.Abs(currentLongNoteData.end.noteMetronomePosY - currentLongNoteData.start.noteMetronomePosY);
                        
                        if(undoList.Count > 0)
                        {
                            NoteHistory lastHistory = undoList[undoList.Count - 1];

                            if(lastHistory.type == NoteHistoryType.LongNote && lastHistory.longNoteData == currentLongNoteData)
                            {
                                lastHistory.NoteHistoryObjs.Add(obj);
                                lastHistory.NoteHistoryObjs.Add(lastLongNoteTrail);
                            }
                        }
                    }
                    currentLongNoteData = null;
                }
            }
            else
            {
                EditorSave.instance.data.noteData.Add(newNoteData);
                EditorSave.instance.data.noteCount++;

                NoteHistory newHistory = new NoteHistory();
                newHistory.type = NoteHistoryType.Note;
                newHistory.noteData = newNoteData;
                newHistory.NoteHistoryObjs.Add(obj);
                AddToUndoList(newHistory);
            }
            Debug.Log($"{"저장된 노트 갯수 : "}{EditorSave.instance.data.noteCount}{" | "}{"롱노트의 갯수 : "} {noteDataTmp.Count - EditorSave.instance.data.noteCount}");
        }
    }

    void SerchLongNote(GameObject obj)
    {
        float originalPosX = 0, originalPosY = 0;
        if (currentLongNoteData != null)
        {
            originalPosX = (currentLongNoteData.start.noteMetronomePosX - 2f) * 1.5f;
            originalPosY = ((currentLongNoteData.start.noteMetronomePosY + 3) * 0.375f) - 4.5f;
        }

        if (selectNoteType == 1 && createLongNote && longNoteLine == inputX)
        {
            createLongNote = false; // 작성 완료
            if (lastLongNoteTrail != null)
            {
                lastLongNoteTrail.transform.localScale = new Vector2(1.45f, Vector3.Distance(obj.transform.localPosition, new Vector3(originalPosX, originalPosY, 0)));
                lastLongNoteTrail.transform.localPosition = new Vector2(lastLongNoteTrail.transform.position.x, (originalPosY + obj.transform.localPosition.y) / 2);
            }
        }
        else if (selectNoteType == 1 && !createLongNote)
        {
            createLongNote = true; // 작성 시작
            GameObject trailObj = CheckNoteCollect(2);
            trailObj.transform.position = positionFix;
            lastLongNoteTrail = trailObj;
            longNoteLine = inputX;
        }
        else if (selectNoteType == 1 && createLongNote && longNoteLine != inputX)
        {
            // 다른 라인 클릭 시 기존 데이터 롤백
            if (lastLongNoteTrail != null) lastLongNoteTrail.SetActive(false);
            EditorSave.instance.data.noteCount--;

            if (currentLongNoteData != null)
            {
                noteDataTmp.Remove(currentLongNoteData.start); // 상호작용 리스트 제거
                EditorSave.instance.data.longnoteData.Remove(currentLongNoteData); // 데이터 리스트 제거

                GameObject prevHead = LastLongNote(originalPosX, originalPosY);
                if (prevHead != null) prevHead.SetActive(false);
            }

            createLongNote = true; // 새로 시작
            obj = CheckNoteCollect(2);
            obj.transform.position = positionFix;
            lastLongNoteTrail = obj;
            longNoteLine = inputX;
        }
    }

    void SerchRemoveNote()
    {
        if (Input.GetMouseButtonDown(1) && preNote.gameObject.activeInHierarchy && NotePlaced())
        {
            SerchData();
            EditorSave.instance.data.noteCount--;
            noteDataTmp.Remove(selectNoteData);

            if (selectNoteData.type == 1)
            {
                // 클릭된 노트가 포함된 롱노트 전체 데이터를 찾아 삭제
                EditorLongNoteData targetToRemove = null;
                foreach (var longNote in EditorSave.instance.data.longnoteData)
                {
                    if (longNote.start == selectNoteData || longNote.end == selectNoteData)
                    {
                        targetToRemove = longNote;
                        break;
                    }
                }

                if (targetToRemove != null)
                {
                    Vector2 startPos = new Vector2(targetToRemove.start.noteMetronomePosX, targetToRemove.start.noteMetronomePosY);
                    Vector2 endPos = new Vector2(targetToRemove.end.noteMetronomePosX, targetToRemove.end.noteMetronomePosY);

                    Vector3 originStartPos = Vector3.right;
                    Vector3 originEndPos = Vector3.right;

                    // 롱노트
                    for (int i = 0; i < noteDummy[1].Count; i++)
                    {
                        if(originStartPos != originEndPos) break;
                        if (!noteDummy[1][i].activeSelf) continue;

                        // 인게임에 저장되는 방식으로 위치값으로 변경 후 비교
                        var longNotePos = new Vector2((noteDummy[1][i].transform.position.x / 1.5f) + 2,
                                                      (noteDummy[1][i].transform.position.y + 4.5f - noteParants.transform.position.y) / 0.375f - 3);

                        if (longNotePos == startPos || longNotePos == endPos)
                        {
                            noteDummy[1][i].SetActive(false);

                            originStartPos = originStartPos == Vector3.right ? noteDummy[1][i].transform.position : originStartPos;
                            originEndPos = noteDummy[1][i].transform.position;
                        }
                    }
                    // 롱노트 테일
                    foreach (var longNoteTrail in noteDummy[2])
                    {
                        if (!longNoteTrail.activeSelf) continue;

                        // 롱노트 타일에서 하나만 y값이 음수가 되어버리면, 포지션 값으로는 정확히 계산이 어렵기때문에 음수인 y값을 절대값으로 바꿔 더해줌
                        float taillLenght = originStartPos.y < 0 == originEndPos.y < 0 ? Mathf.Abs(originStartPos.y) - Mathf.Abs(originEndPos.y) : Mathf.Abs(originStartPos.y) + Mathf.Abs(originEndPos.y);

                        Vector3 tailPos = Vector3.zero;
                        if (originStartPos.y < 0 && originEndPos.y < 0)
                        {
                            tailPos = originStartPos.y > originEndPos.y ?
                                            new Vector3(originStartPos.x, originStartPos.y + taillLenght / 2, originStartPos.z) :
                                            new Vector3(originEndPos.x, originEndPos.y - taillLenght / 2, originEndPos.z);
                        }
                        else
                        {
                            tailPos = originStartPos.y > originEndPos.y ?
                                            new Vector3(originStartPos.x, originStartPos.y - taillLenght / 2, originStartPos.z) :
                                            new Vector3(originEndPos.x, originEndPos.y - taillLenght / 2, originEndPos.z);
                        }

                        if (longNoteTrail.transform.position == tailPos)
                        {
                            longNoteTrail.SetActive(false);
                            longNoteTrail.transform.localScale = new Vector3(longNoteTrail.transform.localScale.x, 0.4f, longNoteTrail.transform.localScale.z); // 재사용 할 수 있게 크기를 줄여줌
                            break;
                        }
                    }
                    EditorSave.instance.data.longnoteData.Remove(targetToRemove);
                    // 같이 있는 노트도 상호작용 리스트에서 제거하는데 왜 안ㅇ없어지는데거언램ㄻ너ㅣㅇㄹㄴㄹ머ㅏ리ㅓㅣ
                    if (targetToRemove.start != selectNoteData) noteDataTmp.Remove(targetToRemove.start);
                    if (targetToRemove.end != selectNoteData && targetToRemove.end != null) noteDataTmp.Remove(targetToRemove.end);
                }
            }
            else
            {
                EditorSave.instance.data.noteData.Remove(selectNoteData);
                if (placedNote != null) placedNote.SetActive(false);
            }
        }
    }

    public void NoteInstallLoad()
    {
        if (EditorSave.instance.data == null) return;

        float parentY = noteParants.transform.position.y;

        // 기본 노트 로드
        foreach (EditorNoteData note in EditorSave.instance.data.noteData)
        {
            if (note == null) continue;

            GameObject obj = CheckNoteCollect(0);
            obj.transform.position = new Vector3((note.noteMetronomePosX - 2f) * 1.5f, ((note.noteMetronomePosY + 3f) * 0.375f) - 4.5f + parentY, 0f);
            noteDataTmp.Add(note);
        }

        // 구조체 리스트 순회 -> 롱노트 로드
        foreach (EditorLongNoteData longNote in EditorSave.instance.data.longnoteData)
        {
            if (longNote == null || longNote.start == null || longNote.start.type == 0)
            {
                continue; // 없는 노트 컨티뉴로 돌려버렸음
            }

            EditorNoteData startNode = longNote.start;
            GameObject headObj = CheckNoteCollect(1);
            float startY = ((startNode.noteMetronomePosY + 3f) * 0.375f) - 4.5f + parentY;
            headObj.transform.position = new Vector3((startNode.noteMetronomePosX - 2f) * 1.5f, startY, 0f);
            noteDataTmp.Add(startNode); // 시작점 등록
            //Debug.Log("시작" + headObj.transform.position);

            if (longNote.end != null)
            {
                if (longNote.end.type == 0 && longNote.end.noteMetronomePosY == 0) continue;

                noteDataTmp.Add(longNote.end);

                float endY = ((longNote.end.noteMetronomePosY + 3f) * 0.375f) - 4.5f + parentY;

                GameObject endObj = CheckNoteCollect(1); // 롱노트 End 노트 생성
                endObj.transform.position = new Vector3(headObj.transform.position.x, endY, 0f);
                //Debug.Log("꼬리" + endObj.transform.position);

                GameObject trailObj = CheckNoteCollect(2); // 롱노트 tail 부분 생성
                trailObj.transform.position = new Vector3(headObj.transform.position.x, (startY + endY) / 2f, 0f);
                trailObj.transform.localScale = new Vector2(1.45f, Mathf.Abs(startY - endY));
                //Debug.Log("끝" + trailObj.transform.position);
            }
        }
    }

    void SerchData()
    {
        foreach (EditorNoteData note in noteDataTmp)
        {
            if (note.noteMetronomePosX == inputX && note.noteMetronomePosY == (int)((positionFix.y + 4.5f - noteParants.transform.position.y) / 0.375f - 3))
            {
                selectNoteData = note;
                return;
            }
        }
    }

    EditorNoteData SetData()
    {
        EditorNoteData notedata = new EditorNoteData();
        notedata.noteMetronomePosX = (int)(positionFix.x / 1.5f) + 2;
        notedata.noteMetronomePosY = (int)((positionFix.y + 4.5f - noteParants.transform.position.y) / 0.375f - 3);
        notedata.type = selectNoteType;
        return notedata;
    }

    public GameObject CheckNoteCollect(int type)
    {
        for (int i = 0; i < noteDummy[type].Count; i++)
        {
            if (!noteDummy[type][i].activeInHierarchy)
            {
                noteDummy[type][i].SetActive(true);
                return noteDummy[type][i];
            }
        }
        GameObject obj = Instantiate(notes[type], transform.position, Quaternion.identity, noteParants.transform);
        obj.SetActive(true);
        noteDummy[type].Add(obj);
        return obj;
    }

    bool NotePlaced()
    {
        for (int i = 0; i < noteDummy.Count; i++)
        {
            for (int j = 0; j < noteDummy[i].Count; j++)
            {
                if (noteDummy[i][j].activeInHierarchy && noteDummy[i][j].transform.position == positionFix)
                {
                    placedNote = noteDummy[i][j];
                    return true;
                }
            }
        }
        return false;
    }

    GameObject LastLongNote(float localX, float localY)
    {
        Vector3 targetPos = new Vector3(localX, localY, 0);
        for (int i = 0; i < noteDummy.Count; i++)
        {
            for (int j = 0; j < noteDummy[i].Count; j++)
            {
                if (noteDummy[i][j].activeInHierarchy && Vector3.Distance(noteDummy[i][j].transform.localPosition, targetPos) < 0.1f)
                    return noteDummy[i][j];
            }
        }
        return null;
    }

    void MakeDummyNote()
    {
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject note = Instantiate(notes[j], transform.position, Quaternion.identity, noteParants);
                note.SetActive(false);
                noteDummy[j].Add(note);
            }
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject note = Instantiate(notes[3], transform.position, Quaternion.identity, noteParants);
            note.SetActive(false);
            noteDummy[3].Add(note);
        }
    }

    public void ChangePreNote()
    {
        for (int i = 0; i < preNoteObj.Length; i++)
        {
            preNoteObj[i].SetActive(i == selectNoteType);
        }
    }

    private void UndoNote()
    {
        if (Keyboard.current != null && Keyboard.current.shiftKey.isPressed)
        {
            return;
        }

        if (!MakeGrid.Instance.IsCreated || undoList.Count == 0)
        {
            Debug.LogWarning("Grid 생성 X 혹은 UndoList 가 비었음");
            return;
        }

        int lastUndoIndex = undoList.Count - 1;
        NoteHistory history = undoList[lastUndoIndex];

        foreach(GameObject obj in history.NoteHistoryObjs)
        {
            if (obj != null) obj.SetActive(false);
        }

        if(history.type == NoteHistoryType.LongNote)
        {
            if(history.longNoteData != null)
            {
                if (history.longNoteData.start != null) noteDataTmp.Remove(history.longNoteData.start);
                if (history.longNoteData.end != null) noteDataTmp.Remove(history.longNoteData.end);

                EditorSave.instance.data.longnoteData.Remove(history.longNoteData);
                EditorSave.instance.data.noteCount--;
            }

            if (createLongNote)
            {
                createLongNote = false;
                currentLongNoteData = null;
                if(lastLongNoteTrail != null)
                {
                    lastLongNoteTrail.transform.localScale = new Vector3(lastLongNoteTrail.transform.localScale.x, 0.4f, lastLongNoteTrail.transform.localScale.z);
                    lastLongNoteTrail.SetActive(false);
                }
            }
        }
        else
        {
            noteDataTmp.Remove(history.noteData);
            EditorSave.instance.data.noteData.Remove(history.noteData);
            EditorSave.instance.data.noteCount--;
        }

        undoList.RemoveAt(lastUndoIndex);
        redoList.Add(history);

        if (redoList.Count > maxRedoCount)
        {
            redoList.RemoveAt(0);
        }

        EditCommendHUD.Instance.TriggerUndo();
        Debug.Log($"Undo 처리 및 남은 기록 : {undoList.Count}");
    }
    
    private void RedoNote()
    {
        if (!MakeGrid.Instance.IsCreated || redoList.Count == 0)
        {
            Debug.LogWarning("Grid 생성 X 혹은 RedoList 가 비었음");
            return;
        }

        int lastRedoIndex = redoList.Count - 1;
        NoteHistory history = redoList[lastRedoIndex];

        foreach(GameObject obj in history.NoteHistoryObjs)
        {
            if (obj != null) obj.SetActive(true);
        }
        
        if(history.type == NoteHistoryType.LongNote)
        {
            if (history.longNoteData != null)
            {
                if(history.longNoteData.start != null) noteDataTmp.Add(history.longNoteData.start);
                if(history.longNoteData.end != null) noteDataTmp.Add(history.longNoteData.end);

                EditorSave.instance.data.longnoteData.Add(history.longNoteData);
                EditorSave.instance.data.noteCount++;


            }
        }
        else
        {
            noteDataTmp.Add(history.noteData);
            EditorSave.instance.data.noteData.Add(history.noteData);
            EditorSave.instance.data.noteCount++;
        }

        redoList.RemoveAt(lastRedoIndex);
        undoList.Add(history);
        EditCommendHUD.Instance.TriggerRedo();
        Debug.Log($"Redo 처리 및 남은 기록 : {redoList.Count}");
    }

    private void AddToUndoList(NoteHistory history)
    {
        redoList.Clear();
        undoList.Add(history);

        if(undoList.Count > maxUndoCount)
        {
            undoList.RemoveAt(0);
        }
    }
}