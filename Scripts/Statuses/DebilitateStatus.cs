using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebilitateStatus", menuName = "Debilitate Status")]
public class DebilitateStatus : Status
{
    [SerializeField] private float attackDebuff = 0.15f;

    public override void Activate(BaseUnit unit)
    {
        turnCasted = GameManager.Instance.gameTurn;
        turnLifted = turnCasted + statusActiveTime;
        unit.attackBuff -= attackDebuff;
    }

    public override void Lift(BaseUnit unit)
    {
        unit.attackBuff += attackDebuff;
        turnCasted = default;
        turnLifted = default;
    }
}
