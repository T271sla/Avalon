using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Debilitate", menuName = "Debilitate Skill")]
public class Debilitate : Skill
{
    [SerializeField] private int luckBoundry;
    [SerializeField] private Status debilitateStatus;

    public override void Activate(BaseUnit unit, BaseUnit targetUnit)
    {
        float luck = Random.Range(1, 100);
        if(luck >= luckBoundry)
        {
            targetUnit.addStatus(debilitateStatus);
            AudioManager.Instance.playSound(sound);
        }
    }
}
