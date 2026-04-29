using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class HitNote : MonoBehaviour
{
    public TrackData trackData;
    public GameObject pressedObjct;
    public GameObject lineObjct;
    private SpriteRenderer hitRenderer;
    public Color startColor;
    public Color pressedColor;

    public float[] judgeDistance;
    private bool[] judgeInDistance = new bool[6];
    public bool isCurrentLongNote = false;
    public List<GameObject> notes;
    public Coroutine serchLongNote_CO = null;
    public Coroutine findLongNote_CO = null;
    public Vector3? rollBackPos;

    public int line;

    public void Awake()
    {
        hitRenderer = GetComponent<SpriteRenderer>();
    }
    public void OnHit(InputAction.CallbackContext context)
    {
        if (pressedObjct == null || hitRenderer == null)
        {
            Debug.LogError("KeyHandler: pressedObjct 또는 hitRenderer가 할당되지 않았습니다.");
            return;
        }

        if (context.performed)
        {
            if (isCurrentLongNote) return;

            pressedObjct.SetActive(true);
            hitRenderer.color = pressedColor;
            ScanNoteNearby();
        }

        if (context.canceled)
        {
            pressedObjct.SetActive(false);
            hitRenderer.color = startColor;

            if (isCurrentLongNote && serchLongNote_CO == null)
            {
                ScanNoteNearby();
            }
        }

    }

    void ScanNoteNearby()
    {
        GameObject lowestObj = null;
        float lowestY = float.MaxValue;

        for (int i = notes.Count - 1; i >= 0; i--)
        {
            GameObject obj = notes[i];
            if (obj != null && obj.activeInHierarchy)
            {
                float y = obj.transform.position.y;
                if (y < lowestY)
                {
                    lowestY = y;
                    lowestObj = obj;
                }
            }
        }

        ScanJudge(lowestObj);
    }

    void ScanJudge(GameObject note)
    {
        if (note == null)
        {
            // 롱노트 타일이 없는데, 키를 땟을때
            if (isCurrentLongNote)
            {
                int jugedLevel = (int)TrackData.judgeValue.Miss;
                trackData.judge = (TrackData.judgeValue)jugedLevel;
                trackData.judgeHit[jugedLevel]++;
                trackData.combo = 0;

                trackData.UpdateJudge(jugedLevel, line);
                Debug.Log($"{line} 번의 로드");

                serchLongNote_CO = StartCoroutine(AsyncSerchLongNote_CO());
            }
            return;
        }

        MoveDown noteData = note.GetComponent<MoveDown>();

        float distance = Mathf.Abs(note.transform.position.y - lineObjct.transform.position.y);

        int judgeLevel = -1;
        for (int i = 0; i < judgeDistance.Length; i++)
        {
            if (distance <= judgeDistance[i])
            {
                judgeLevel = i;
                break;
            }
        }

        if (judgeLevel >= 5) // Miss 판정일 때
        {
            int jugedLevel = (int)TrackData.judgeValue.Miss;
            trackData.judge = (TrackData.judgeValue)jugedLevel;
            trackData.judgeHit[jugedLevel]++;
            trackData.combo = 0;

            trackData.UpdateJudge(jugedLevel, line);
            // Debug.Log($"{line} 번의 로드");

            // 키를 땟지만(시작 롱노트 타일을 누른 후) 롱노트 탐색중이 아닐때(끝 롱노트가 있을때)
            if (serchLongNote_CO == null && isCurrentLongNote)
            {
                serchLongNote_CO = StartCoroutine(AsyncSerchLongNote_CO());
                return;
            }

            trackData.finishJudgeCount++;
        }
        else if (judgeLevel >= 0) // Miss 이외에 판정일 때
        {
            trackData.judge = (TrackData.judgeValue)judgeLevel;
            trackData.judgeHit[judgeLevel]++;
            trackData.combo++;

            if (noteData != null)
            {
                noteData.Speed = 4;
            }

            note.SetActive(false);
            notes.Remove(note);
            trackData.UpdateJudge(judgeLevel, line);

            // Debug.Log($"{line} 번의 로드");

            if (noteData.isLong)
            {
                if(!isCurrentLongNote)
                {
                    FindStartLongPos(note.transform.position);
                }
                else
                {
                    trackData.finishJudgeCount++;
                }

                isCurrentLongNote = isCurrentLongNote == true ? false : true;
                return;
            }

            trackData.finishJudgeCount++;
        }
    }

    public void MissStartLong(Vector3 startLongPos)
    {
        // TODO : 시작인지 끝인지 알 수 없는데 시작이 입력된 상태에서 다시하기를 하면 이게 반대가 되어버림
        isCurrentLongNote = isCurrentLongNote == true ? false : true;

        serchLongNote_CO = StartCoroutine(AsyncSerchLongNote_CO());
        FindStartLongPos(startLongPos);
    }

    /// <summary>
    /// 아직 생성되지 않은 롱노트를 찾아 판정을 제어해주는 함수
    /// </summary>
    private IEnumerator AsyncSerchLongNote_CO()
    {
        if (serchLongNote_CO != null) yield break;

        GameObject longNoteEnd = null;
        MoveDown longNoteMove = null;

        while (isCurrentLongNote && longNoteEnd == null)
        {
            if (notes.Count > 0)
            {
                foreach (var note in notes)
                {
                    var move = note.GetComponent<MoveDown>();
                    if (move.isLong)
                    {
                        longNoteEnd = note;
                        longNoteMove = move;

                        longNoteMove.isNote = false;
                        break;
                    }
                }
            }
            yield return null;
        }

        while (isCurrentLongNote && longNoteEnd != null)
        {
            if (!longNoteEnd.activeSelf)
            {
                longNoteMove.isNote = true;
                notes.Remove(longNoteEnd);

                isCurrentLongNote = false;

                serchLongNote_CO = null;

                trackData.finishJudgeCount++;
                yield break;
            }
            yield return null;
        }
    }

    public void FindStartLongPos(Vector3 startLongPos)
    {
        findLongNote_CO = StartCoroutine(FindStartLongPos_CO(startLongPos));
    }

    /// <summary>
    /// 지워진 startLong노트를 롤백시키기 위해 startLong노트의 위치를 구하는 함수
    /// </summary>
    /// <param name="startLongPos">롱노트가 지워졌을때 위치</param>
    private IEnumerator FindStartLongPos_CO(Vector3 startLongPos)
    {
        float time = 3f;

        while (time > 0)
        {
            if(!trackData.PlayStoped)
            {
                time -= Time.deltaTime;
                rollBackPos = new Vector3(startLongPos.x,
                startLongPos.y - (3 - time) * (CreateMetronome.Instance.speed * 4),
                startLongPos.z);
            }
            if(!isCurrentLongNote)
            {
                findLongNote_CO = null;
                yield break;
            }

            yield return null;
        }

        findLongNote_CO = null;
    }
}
