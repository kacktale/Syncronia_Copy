using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNote : MonoBehaviour
{
    public static CreateNote Instance { get; private set; }

    public GameObject[] notes;
    public Vector3[] lineXpos;
    public HitNote[] hits;
    public GameObject[] noteCollectObj;

    public Transform judgeLineTransform;

    private List<GameObject> shortNoteCollecter;
    private List<GameObject> longNoteCollecter;
    private List<GameObject> tailCollecter;

    public Dictionary<int, MoveDown> LongTempCollecter;
    public Dictionary<int, MoveDown> longTailCollecter;

    public Coroutine Continue_CO;

    public float pausePluseTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        LongTempCollecter = new Dictionary<int, MoveDown>();
        longTailCollecter = new Dictionary<int, MoveDown>();
        CreateDummyNotes();
        if (SelectTrackData.Instance.trackData.bossType != BossSkillType.None)
        {
            notes[3] = BossController.Instance.GetBossSkill(SelectTrackData.Instance.trackData.bossType);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDummyNotes()
    {
        shortNoteCollecter = new List<GameObject>();
        longNoteCollecter = new List<GameObject>();
        tailCollecter = new List<GameObject>();
        for (int i = 0; i < 30; i++)
        {
            GameObject obj = Instantiate(notes[0],transform.position,Quaternion.identity, noteCollectObj[0].transform);
            obj.SetActive(false);
            shortNoteCollecter.Add(obj);
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(notes[1], transform.position, Quaternion.identity, noteCollectObj[1].transform);
            GameObject objtail = Instantiate(notes[2], transform.position, Quaternion.identity, noteCollectObj[2].transform);
            obj.SetActive(false);
            objtail.SetActive(false);
            MoveDown tailSetPos = objtail.GetComponent<MoveDown>();
            tailSetPos.tempEndPos = gameObject.transform;
            tailSetPos.tempJudgeLinePos = judgeLineTransform;
            longNoteCollecter.Add(obj);
            tailCollecter.Add(objtail);

            obj.GetComponent<MoveDown>().isLong = true;
        }
    }

    public GameObject CheckNoteCollet(int type)
    {
        if(type == 0)
        {
            for (int i = 0; i < shortNoteCollecter.Count; i++)
            {
                if (!shortNoteCollecter[i].activeInHierarchy)
                {
                    shortNoteCollecter[i].SetActive(true);
                    return shortNoteCollecter[i];
                }
            }
        }
        else if (type == 1)
        {
            for (int i = 0; i < longNoteCollecter.Count; i++)
            {
                if (!longNoteCollecter[i].activeInHierarchy)
                {
                    longNoteCollecter[i].SetActive(true);
                    return longNoteCollecter[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < tailCollecter.Count; i++)
            {
                if (!tailCollecter[i].activeInHierarchy)
                {
                    tailCollecter[i].SetActive(true);
                    return tailCollecter[i];
                }
            }
        }
        GameObject obj = Instantiate(notes[type], transform.position, Quaternion.identity, noteCollectObj[type].transform);
        obj.SetActive(false);

        if (type == 0) shortNoteCollecter.Add(obj);
        else if (type == 1) longNoteCollecter.Add(obj);
        else tailCollecter.Add(obj);

        return obj;
    }

    public void CreateNotes(NoteEnum noteType, int line, float speed, float tailLenght = 0, int longId = -1, bool isLongStart = false)
    {
        int noteTypeNum = Convert.ToInt32(noteType);

        if(noteTypeNum == 4) return;
        GameObject note = CheckNoteCollet(noteTypeNum);

        MoveDown noteSpeed = note.GetComponent<MoveDown>();
        noteSpeed.Speed = speed * noteSpeed.Speed;

        hits[line - 1].notes.Add(note);

        noteSpeed.hitNote = hits[line - 1];
        note.transform.position = lineXpos[line - 1];
        if(noteTypeNum == 1)
        {
            if (longId < 0) return;
            if (LongTempCollecter.TryGetValue(line, out MoveDown tailInfo))
            {
                if (isLongStart)
                {
                    tailInfo.makeStart = true;
                    tailInfo.tailStartPos = note.transform;
                }
                else
                {
                    tailInfo.makeEnd = true;
                    tailInfo.tailEndPos = note.transform;
                    LongTempCollecter.Remove(line);
                }
            }
        }
    }

    public void CreateLongNoteTail(int longId, int line, float speed)
    {
        GameObject note = CheckNoteCollet(2);
        note.transform.position = lineXpos[line - 1];

        MoveDown noteSpeed = note.GetComponent<MoveDown>();

        noteSpeed.makeStart = false;
        noteSpeed.makeEnd = false;

        noteSpeed.Speed = speed * noteSpeed.Speed;
        noteSpeed.hitNote = hits[line - 1];
        noteSpeed.line = line;

        LongTempCollecter[line] = noteSpeed;
    }


    public void ChangeJudge(float speed)
    {
        foreach(HitNote hit in hits)
        {
            for(int i = 0; i < hit.judgeDistance.Length; i++)
            {
                float newDis = hit.judgeDistance[i] * ((speed * 4) /20);
                hit.judgeDistance[i] = newDis;
            }
        }
    }

    public void NoteRollBack(float speed, int pauseTime = 3)
    {
        if (Continue_CO != null) return;
        Continue_CO = StartCoroutine(NoteRollBack_CO(speed, pauseTime));
    }

    IEnumerator NoteRollBack_CO(float speed, int pauseTime)
    {
        // 노트 오브젝트들 위치 구하기
        List<GameObject> activeObjects = new List<GameObject>();

        Dictionary<GameObject, float> originPosition = new Dictionary<GameObject, float>();

        float mostBottomNotePos = 0;

        // 위치 구하기
        float Height = pauseTime * (speed * 4); // 4는 노트에 기본으로 존재하는 이동속도

        // 위치까지 올리기
        float lerpPosY = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var notes = hits[i].notes;
            foreach (GameObject note in notes)
            {
                if (note.activeSelf)
                {
                    var moveDown = note.GetComponent<MoveDown>();

                    activeObjects.Add(note);
                    originPosition.Add(note, note.transform.position.y);

                    mostBottomNotePos = mostBottomNotePos < note.transform.position.y ? mostBottomNotePos : note.transform.position.y;
                }
            }
            if (hits[i].findLongNote_CO != null && Vector3.zero != hits[i].rollBackPos.GetValueOrDefault())
            {
                // 생성 및 초기 설정
                GameObject startLong = CheckNoteCollet(1);
                var noteSpeed = startLong.GetComponent<MoveDown>();
                noteSpeed.Speed *= speed;
                noteSpeed.isRollBack = true;

                // 롱노트 테일에 startLong 위치 등록
                if(longTailCollecter.TryGetValue(i + 1, out MoveDown longTail) && longTail.gameObject.activeInHierarchy)
                {
                    longTail.tailStartPos = startLong.transform;
                }

                // 있어야 되었을 위치를 넣어줌
                startLong.transform.position = hits[i].rollBackPos.GetValueOrDefault();
                Debug.Log(startLong.transform.position);

                // 다시 판정을 진행시킬 수 있도록 리스트 추가
                hits[i].notes.Add(startLong);
                noteSpeed.hitNote = hits[i];

                // 올릴 수 있도록 추가
                activeObjects.Add(startLong);
                originPosition.Add(startLong, startLong.transform.position.y);

                // 기존에 실행되던 롱노트 위치를 잡아주는 코루틴 함수 종료
                hits[i].StopCoroutine(hits[i].findLongNote_CO);
                hits[i].findLongNote_CO = null;

                hits[i].rollBackPos = null;
            }
            if(hits[i].serchLongNote_CO != null)
            {
                hits[i].StopCoroutine(hits[i].serchLongNote_CO);
                hits[i].serchLongNote_CO = null;
                hits[i].isCurrentLongNote = false;
            }
        }

        if (activeObjects.Count <= 0) yield break;

        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float LerpValue = Mathf.Clamp01(elapsed / duration);

            foreach (GameObject note in activeObjects)
            {
                float startY = originPosition[note];

                lerpPosY = Mathf.Lerp(startY, Height + (transform.position.y + startY), LerpValue);
                note.transform.position = new Vector3(note.transform.position.x,
                                                      lerpPosY,
                                                      note.transform.position.z);
            }
            yield return null;
        }

        foreach (GameObject note in activeObjects)
        {
            MoveDown moveDown = note.GetComponent<MoveDown>();
            if(moveDown.isLong)
            {
                moveDown.isRollBack = false;
            }
        }

        pausePluseTime = (transform.position.y - mostBottomNotePos) / (speed * 4f); // 노트 생성이 추가로 밀리게 될 시간

        Continue_CO = null;
    }
}
