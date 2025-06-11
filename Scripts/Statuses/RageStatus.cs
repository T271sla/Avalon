using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "RageStatus", menuName = "Rage Status")]
public class RageStatus : Status
{
    [SerializeField] private float attackBuff = 0.15f;
    [SerializeField] private float defenseDebuff = 0.15f;

    public override void Activate(BaseUnit unit)
    {
        turnCasted = GameManager.Instance.gameTurn;
        turnLifted = turnCasted + statusActiveTime;

        unit.attackBuff += attackBuff;
        unit.defenseBuff -= defenseDebuff;

        HeroOne heroOne = (HeroOne)unit;
        heroOne.enraged.SetActive(true);
    }

    public override void Lift(BaseUnit unit)
    {
        unit.attackBuff -= attackBuff;
        unit.defenseBuff += defenseDebuff;

        turnCasted = default;
        turnLifted = default;

        HeroOne heroOne = (HeroOne)unit;
        heroOne.enraged.SetActive(false);
    }

    public void changeBuffs(float newAttackBuff, float newDefenseDebuff)
    {
        attackBuff = newAttackBuff;
        defenseDebuff = newDefenseDebuff;
    }
}
