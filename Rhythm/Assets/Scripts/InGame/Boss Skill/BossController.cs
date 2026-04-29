using UnityEngine;

public class BossController : MonoBehaviour
{
    private static BossController instance;
    public static BossController Instance => instance;

    [SerializeField] private GameObject[] bossSkills;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 맵 데이터에 저장된 보스에 맞는 보스 오브젝트를 반환하는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetBossSkill(BossSkillType type)
    {
        foreach (GameObject skill in bossSkills)
        {
            if (skill.GetComponent<BossSkill>().Type == type)
            {
                return skill;
            }
        }
        return null;
    }
}
