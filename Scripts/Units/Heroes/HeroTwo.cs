using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTwo : BaseHero
{
    public override void Attack(BaseUnit enemy)
    {
        float attack = attackValue(enemy, OccupiedTile);
        enemy.modifyHealth(attack);
        skill.Activate(this, enemy);
        enemy.animator.SetTrigger("Damage");
        AudioManager.Instance.playSound(attackSound);
    }
}
