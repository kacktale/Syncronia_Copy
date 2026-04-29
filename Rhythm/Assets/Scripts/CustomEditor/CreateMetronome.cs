using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class CreateMetronome : MonoBehaviour
{
    public static CreateMetronome Instance;
    public float BPM;
    private float originBPM;
    public int makedMetronome;
    public GameObject metronomePrefab;
    public Transform metronomeSpawn;

    private float elapsedTime;
    public int poolSize = 4;
    private List<GameObject> pooledmetronome;
    public CreateNote createNote;

    private SelectTrackData selectData;
    [SerializeField] private int serchList = 0;
    private int serchStartLongList = 0;
    [SerializeField] private int serchEndLongList = 0;
    public float speed { get; private set; } = 0;

    private bool isCount;
    private bool isEnd;
    [SerializeField] private float pauseTime;

    private float songTime;

    private TrackData trackData;
    public List<EditorLongNoteData> endLongNotes = new List<EditorLongNoteData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        selectData = SelectTrackData.Instance;
        trackData = TrackData.Instance;
        pooledmetronome = new List<GameObject>();

        // TODO : 나중에 곡이 들어오게 되면 사용함
        //EditorAudioManager.Instance.MusicInstance.getDescription(out EventDescription desc);
        //desc.getLength(out int length);
        //float songTime = length / 1000f;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(metronomePrefab, metronomeSpawn.position, Quaternion.identity, metronomeSpawn);
            obj.SetActive(false);
            pooledmetronome.Add(obj);
        }
    }

    private void Start()
    {
        if (selectData == null)
        {
            selectData = SelectTrackData.Instance;
        }
        
        BPM = selectData.trackData.BPM;
        originBPM = BPM;

        speed = BPM / originBPM + UserData.instance.userSpeed;
        createNote.ChangeJudge(speed);
        endLongNotes = new List<EditorLongNoteData>(selectData.trackData.longnoteData);
        RebaseEndNote();
    }

    void RebaseEndNote()
    {
        if (endLongNotes == null) return;
        endLongNotes.Sort((a, b) => a.end.noteMetronomePosY.CompareTo(b.end.noteMetronomePosY));
    }

    void Update()
    {
        if (trackData.PlayStoped) return;
        if (speed == 0)
        {
            SetData();
            return;
        }

        if (createNote.Continue_CO != null) return;
        if (isCount) return;

        elapsedTime += Time.deltaTime;
        MakeMetronome();
        if(serchEndLongList + serchList == selectData.trackData.noteCount && !isEnd)
        {
            isEnd = true;
            trackData.SetResultData(true);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledmetronome.Count; i++)
        {
            if (!pooledmetronome[i].activeInHierarchy)
            {
                return pooledmetronome[i];
            }
        }
        GameObject obj = Instantiate(metronomePrefab, metronomeSpawn.position, Quaternion.identity, metronomeSpawn);
        obj.SetActive(false);
        pooledmetronome.Add(obj);
        return obj;
    }
    public void MakeMetronome()
    {
        if (makedMetronome >= selectData.trackData.metronomeCount * 4) return;
        float waitTime = (60f / BPM / 4);
        if (waitTime <= elapsedTime)
        {
            makedMetronome++;
            if (makedMetronome % 16 == 1)
            {
                GameObject pooledMetronome = GetPooledObject();
                MoveDown metronomeSpeed = pooledMetronome.GetComponent<MoveDown>();

                metronomeSpeed.Speed = speed * metronomeSpeed.Speed;

                pooledMetronome.SetActive(true);
            }
            elapsedTime = 0f;

            if (serchList + serchEndLongList > selectData.trackData.noteCount) return;
            for (int i = 1; i <= 4; i++)
            {
                if(selectData.trackData.noteData.Count > serchList)
                {
                    if (selectData.trackData.noteData[serchList].noteMetronomePosY == makedMetronome)
                    {
                        //Debug.Log($"{serchList} {selectData.trackData.noteData[serchList].noteMetronomePosY} {selectData.trackData.noteData[serchList].noteMetronomePosX + 1}");
                        createNote.CreateNotes((NoteEnum)selectData.trackData.noteData[serchList].type, selectData.trackData.noteData[serchList].noteMetronomePosX + 1, speed);
                        serchList++;
                    }
                }
                if(selectData.trackData.longnoteData.Count > serchStartLongList)
                {
                    // Debug.Log(selectData.trackData.longnoteData.Count);
                    // Debug.Log($"{selectData.trackData.longnoteData[serchStartLongList].start.noteMetronomePosY}{" | "}{makedMetronome}");
                    if (selectData.trackData.longnoteData[serchStartLongList].start.noteMetronomePosY == makedMetronome)
                    {
                        int line = selectData.trackData.longnoteData[serchStartLongList].start.noteMetronomePosX + 1;
                        createNote.CreateLongNoteTail(serchStartLongList, line, speed);
                        createNote.CreateNotes(NoteEnum.LongNote, line, speed, 0, serchStartLongList, true);
                        //Debug.LogWarning($"LongNote Maked X : {selectData.trackData.longnoteData[serchStartLongList].start.noteMetronomePosX + 1}" +
                        //    $"\n start Y : {selectData.trackData.longnoteData[serchStartLongList].start.noteMetronomePosY}" +
                        //    $"\n end Y : {selectData.trackData.longnoteData[serchStartLongList].end.noteMetronomePosY}");
                        serchStartLongList++;
                    }
                }
                if(endLongNotes.Count > serchEndLongList)
                {
                    if (endLongNotes[serchEndLongList].end.noteMetronomePosY == makedMetronome)
                    {
                        int line = endLongNotes[serchEndLongList].end.noteMetronomePosX + 1;
                        createNote.CreateNotes(NoteEnum.LongNote, line, speed, 0, serchEndLongList, false);
                        //Debug.LogWarning($"LongNote Ended X : {line}" +
                        //    $"\n start Y : {endLongNotes[serchEndLongList].start.noteMetronomePosY}" +
                        //    $"\n end Y : {endLongNotes[serchEndLongList].end.noteMetronomePosY}");
                        serchEndLongList++;
                    }
                }
            }
        }
    }

    void SetData()
    {
        BPM = selectData.trackData.BPM;

        speed = BPM/originBPM * UserData.instance.userSpeed;
        createNote.ChangeJudge(speed);
        RebaseEndNote();
    }
    IEnumerator StartPauseCounting()
    {
        if (isCount) yield break;
        isCount = true;

        while (createNote.Continue_CO != null)
        {
            yield return null;
        }

        float pauseValue = pauseTime;
        int countValue = 0;
        while (0 < pauseValue)
        {
            if (!trackData.PlayStoped)
            {
                pauseValue = Mathf.Clamp(pauseValue - Time.deltaTime, 0, pauseTime);
            }
            if(countValue != Mathf.CeilToInt(pauseValue) && pauseValue != 0)
            {
                countValue = Mathf.CeilToInt(pauseValue);
                trackData.UpdateCountDown(countValue);
            }
            yield return null;
        }

        while (0 < createNote.pausePluseTime)
        {
            createNote.pausePluseTime = Mathf.Clamp(createNote.pausePluseTime - Time.deltaTime, 0, 1);
            yield return null;
        }
        isCount = false;
    }

    public void PauseCall()
    {
        if (isCount || createNote.Continue_CO != null) return;
        createNote.NoteRollBack(speed, (int)pauseTime);
        StartCoroutine(StartPauseCounting());
    }
}
