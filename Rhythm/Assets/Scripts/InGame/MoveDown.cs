using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public int line;
    public float Speed = 4;
    public bool isNote = false;
    public bool isLong = false;
    public bool isTail = false;
    public bool isRollBack = false;
    public Transform tailStartPos;
    public Transform tailEndPos;
    public Transform tempEndPos;
    public Transform tempJudgeLinePos;

    public bool makeStart = false;
    public bool makeEnd = false;

    public HitNote hitNote;

    private float baseSpeed;
    private TrackData trackData;

    private void Awake()
    {
        trackData = TrackData.Instance;
        baseSpeed = Speed;
    }

    void Update()
    {
        if (trackData.PlayStoped) return;
        if (isTail)
        {
            if (tailStartPos == null) tailStartPos = tempJudgeLinePos;
            if (tailEndPos == null)
            {
                if(!makeEnd) tailEndPos = tempEndPos;
                else gameObject.SetActive(false);
            }

            if (tailStartPos != null && !tailStartPos.gameObject.activeInHierarchy)
            {
                tailStartPos = tempJudgeLinePos;
                if(!CreateNote.Instance.longTailCollecter.ContainsKey(line))
                {
                    CreateNote.Instance.longTailCollecter.Add(line, this);
                }
            }
            if (tailEndPos != null && !tailEndPos.gameObject.activeInHierarchy) tailEndPos = tempJudgeLinePos;

            Vector3 startPos = tailStartPos != null ? tailStartPos.position : transform.position;
            Vector3 endPos = tailEndPos != null ? tailEndPos.position : transform.position;
            float length = Mathf.Abs(endPos.y - startPos.y);

            transform.localScale = new Vector3(transform.localScale.x, length, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, (startPos.y + endPos.y) * 0.5f, transform.position.z);
            if (length <= 0.0001f && makeStart && makeEnd) gameObject.SetActive(false);
            return;
        }
        transform.position += Vector3.down * Time.deltaTime * Speed;

        if (transform.position.y <= -3.8f -(transform.localScale.y / 2))
        {
            if (isRollBack) return;
            Speed = 4;

            if (isNote)
            {
                hitNote.trackData.judgeHit[5]++;
                hitNote.trackData.combo = 0;
                hitNote.trackData.judge = TrackData.judgeValue.Miss;
                hitNote.trackData.UpdateJudge(5);
                hitNote.notes.Remove(this.gameObject);

                if (isLong && hitNote.serchLongNote_CO == null)
                {
                    hitNote.MissStartLong(transform.position);
                }
            }
            transform.position = Vector3.up * 6f + Vector3.left * 5;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (isTail)
        {
            makeStart = false;
            makeEnd = false;
            tailStartPos = null;
            tailEndPos = null;

            if(CreateNote.Instance.longTailCollecter.ContainsKey(line))
            {
                CreateNote.Instance.longTailCollecter.Remove(line);
            }
        }
        Speed = baseSpeed;
    }
}
