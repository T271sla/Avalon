using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Heal Skill")]
public class Heal : Skill
{

    public override void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {
        float unitX = UnitManager.Instance.selectedHero.transform.position.x;
        float unitY = UnitManager.Instance.selectedHero.transform.position.y;

        for (float x = unitX - 2; x <= unitX + 2; x++)
        {
            for (float y = unitY - 2; y <= unitY + 2; y++)
            {
                bool positionInRange = x >= 0 && x < GridManager.Instance.width && y >= 0 && y < GridManager.Instance.height;
                if (!positionInRange)
                {
                    continue;
                }

                Tile currentTile = GridManager.Instance.GetTileAtPoint(new Vector2(x, y));

                float distanceMoveX2 = Mathf.Abs(x - unitX);
                float distanceMoveY2 = Mathf.Abs(y - unitY);
                if (distanceMoveX2 + distanceMoveY2 <= 2 && currentTile.OccupiedUnit != null && currentTile.OccupiedUnit.Faction == unit.Faction)
                {
                    currentTile.OccupiedUnit.unitHealth += 40f;
                    if(currentTile.OccupiedUnit.unitHealth > currentTile.OccupiedUnit.unitMaxHealth)
                    {
                        currentTile.OccupiedUnit.unitHealth = currentTile.OccupiedUnit.unitMaxHealth;
                    }
                    currentTile.OccupiedUnit.bar.setValue(currentTile.OccupiedUnit.unitHealth);
                }
            }
        }

        AudioManager.Instance.playSound(sound);

        turnCasted = GameManager.Instance.gameTurn;
        turnAvailable = turnCasted + skillCooldown;
        unit.skill.isActive = false;
        SkillManager.Instance.addSkill(this);

        UnitManager.Instance.endUnitTurn();
        MenuManager.instance.buttonsActivity(false);
    }
}
