using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debilitate", menuName = "Debilitate Skill")]
public class Debilitate : Skill
{
    [SerializeField] private int luckBoundry;

    public override void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {
        float luck = Random.Range(1, 100);
        if(luck >= luckBoundry)
        {
            tragetUnit.attackBuff = 0.85f;
            AudioManager.Instance.playSound(sound);
        }
    }
}
