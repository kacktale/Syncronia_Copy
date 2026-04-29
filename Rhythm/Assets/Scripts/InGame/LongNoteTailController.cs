using UnityEngine;

public class LongNoteTailController : MonoBehaviour
{
    private CreateNote createNote;
    private MoveDown tailMove;
    private Transform spawnAnchor;
    private Transform judgeAnchor;

    private int longId = -1;
    private int line = -1;
    private float speed;
    private MoveDown startNote;
    private MoveDown endNote;
    private bool isInitialized = false;
    private bool isResolved = false;

    public int LongId => longId;
    public int Line => line;
    public MoveDown EndNote => endNote;
    public bool HasActiveEndNote => IsBoundEndNote(endNote);

    public void Initialize(CreateNote owner, MoveDown tail, int runtimeLongId, int runtimeLine, float runtimeSpeed)
    {
        createNote = owner;
        tailMove = tail;
        longId = runtimeLongId;
        line = runtimeLine;
        speed = runtimeSpeed;
        startNote = null;
        endNote = null;
        isResolved = false;
        isInitialized = true;

        EnsureAnchors();
        UpdateAnchorPositions();
        SyncTailEndpoints();
    }

    public void RegisterStartNote(MoveDown note)
    {
        startNote = note;
        SyncTailEndpoints();
    }

    public void RegisterEndNote(MoveDown note)
    {
        endNote = note;
        SyncTailEndpoints();
    }

    public void MarkResolved()
    {
        isResolved = true;
        gameObject.SetActive(false);
    }

    public void SetSpeed(float runtimeSpeed)
    {
        speed = runtimeSpeed;
    }

    private void LateUpdate()
    {
        if (!isInitialized || isResolved || createNote == null || tailMove == null) return;

        UpdateAnchorPositions();
        SyncTailEndpoints();
    }

    private void OnDisable()
    {
        if (!isResolved && createNote != null)
        {
            createNote.UnregisterLongNoteTail(longId, this);
        }

        startNote = null;
        endNote = null;
        isInitialized = false;
        isResolved = false;
        longId = -1;
        line = -1;
        speed = 0f;
    }

    private void EnsureAnchors()
    {
        if (spawnAnchor == null)
        {
            GameObject anchor = new GameObject("SpawnAnchor");
            anchor.hideFlags = HideFlags.HideInHierarchy;
            anchor.transform.SetParent(transform, false);
            spawnAnchor = anchor.transform;
        }

        if (judgeAnchor == null)
        {
            GameObject anchor = new GameObject("JudgeAnchor");
            anchor.hideFlags = HideFlags.HideInHierarchy;
            anchor.transform.SetParent(transform, false);
            judgeAnchor = anchor.transform;
        }
    }

    private void UpdateAnchorPositions()
    {
        spawnAnchor.position = createNote.GetSpawnLinePositionForRuntime(line, speed);
        judgeAnchor.position = createNote.GetJudgeLinePositionForRuntime(line);
    }

    private void SyncTailEndpoints()
    {
        if (tailMove == null) return;

        tailMove.tailStartPos = IsBoundStartNote(startNote) ? startNote.transform : judgeAnchor;
        tailMove.tailEndPos = IsBoundEndNote(endNote) ? endNote.transform : spawnAnchor;
        tailMove.tempJudgeLinePos = judgeAnchor;
        tailMove.tempEndPos = spawnAnchor;
        tailMove.makeStart = false;
        tailMove.makeEnd = false;
    }

    private bool IsBoundStartNote(MoveDown note)
    {
        return IsBoundNote(note) && note.isLongStart;
    }

    private bool IsBoundEndNote(MoveDown note)
    {
        return IsBoundNote(note) && note.isLongEnd;
    }

    private bool IsBoundNote(MoveDown note)
    {
        return note != null &&
               note.gameObject.activeInHierarchy &&
               note.longId == longId;
    }
}
