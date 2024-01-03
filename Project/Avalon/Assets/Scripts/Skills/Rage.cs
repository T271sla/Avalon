using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rage", menuName = "Rage Skill")]
public class Rage : Skill
{
    public float attackBonus = 1.15f;
    public float defenseBonus = 0.85f;

    public override void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {
        if(unit.skill.skillName == "Deactivate") 
        {
            unit.attackBuff = 1f;
            unit.defenseBuff = 1f;
            unit.skill.isActive = false;
            unit.skill.skillName = "Rage";
            HeroOne heroOne = (HeroOne)unit;
            heroOne.enraged.SetActive(false);
        }
        else
        {
            unit.attackBuff = attackBonus;
            unit.defenseBuff = defenseBonus;
            unit.skill.isActive = true;
            unit.skill.skillName = "Deactivate";
            HeroOne heroOne = (HeroOne)unit;
            heroOne.enraged.SetActive(true);
            AudioManager.Instance.playSound(sound);
        }
    }
}
