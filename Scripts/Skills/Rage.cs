using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rage", menuName = "Rage Skill")]
public class Rage : Skill
{
    public float attackBonus = 0.15f;
    public float defenseBonus = 0.15f;
    [SerializeField] private RageStatus rageStatus;

    public override void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {
        unit.addStatus(rageStatus);

        turnCasted = GameManager.Instance.gameTurn;
        turnAvailable = turnCasted + skillCooldown;
        unit.skill.isActive = false;
        SkillManager.Instance.addSkill(this);
        AudioManager.Instance.playSound(sound);
        MenuManager.Instance.skillButtonActivity(unit.skill.isActive);
    }
}
