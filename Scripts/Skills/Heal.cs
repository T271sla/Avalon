using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Heal Skill")]
public class Heal : Skill
{
    [SerializeField]public int healDistance = 2;
    [SerializeField]public float healAmmount = 40;

    public override void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {
        float unitX = UnitManager.Instance.selectedHero.transform.position.x;
        float unitY = UnitManager.Instance.selectedHero.transform.position.y;

        for (float x = unitX - healDistance; x <= unitX + healDistance; x++)
        {
            for (float y = unitY - healDistance; y <= unitY + healDistance; y++)
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
                    currentTile.OccupiedUnit.modifyHealth(-healAmmount);
                }
            }
        }

        AudioManager.Instance.playSound(sound);

        turnCasted = GameManager.Instance.gameTurn;
        turnAvailable = turnCasted + skillCooldown;
        unit.skill.isActive = false;
        SkillManager.Instance.addSkill(this);

        UnitManager.Instance.endUnitTurn();
        MenuManager.Instance.buttonsActivity(false);
    }
}
