using UnityEngine;

public abstract class BossSkill : MonoBehaviour
{
    [SerializeField] private BossSkillType type;
    public BossSkillType Type => type;
    public abstract void BossSkillStart();
}
