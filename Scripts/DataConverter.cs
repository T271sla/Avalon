using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;

[System.Serializable]
public class BaseUnitData
{
    public float unitMaxHealth;
    public float unitHealth;
    public bool active = true;
    public int unitId = 1;
    public float unitX;
    public float unitY;
    public SkillData skillData;
    public List<StatusData> unitStatusesData = new List<StatusData>();

    public BaseUnitData(BaseUnit unit)
    {
        this.unitMaxHealth = unit.unitMaxHealth;
        this.unitHealth = unit.unitHealth;
        this.active = unit.active;
        this.unitId = unit.unitId;
        this.unitX = unit.transform.position.x;
        this.unitY = unit.transform.position.y;
        if (unit.skill != null)
        {
            this.skillData = new SkillData(unit.skill);
        }
        if (unit.unitStatuses == null || unit.unitStatuses.Count() == 0)
        {
            return;
        }

        foreach (Status status in unit.unitStatuses)
        {
            unitStatusesData.Add(new StatusData(status));
        }
    }
}

[System.Serializable]
public class SkillData
{
    public bool isActive = false;
    public int turnCasted;
    public int turnAvailable;

    public SkillData(Skill skill)
    {
        this.isActive = skill.isActive;
        this.turnCasted = skill.turnCasted;
        this.turnAvailable = skill.turnAvailable;
    }
}

[System.Serializable]
public class StatusData
{
    public string statusName;
    public int turnCasted;
    public int turnLifted;

    public StatusData(Status status)
    {
        this.statusName = status.statusName;
        this.turnCasted = status.turnCasted;
        this.turnLifted = status.turnLifted;
    }
}

[System.Serializable]
public class ProgressData
{
    public List<LevelStatisticsData> levels = new List<LevelStatisticsData>();
    public int completedLevels = 0;

    public bool warriorUpdgrade1 = false;
    public bool warriorUpdgrade2 = false;

    public bool lancerUpdgrade1 = false;
    public bool lancerUpdgrade2 = false;

    public bool ghostUpdgrade1 = false;
    public bool ghostUpdgrade2 = false;

    public ProgressData(ProgressSave progressSave)
    {
        this.completedLevels = progressSave.completedLevels;
        this.warriorUpdgrade1 = progressSave.warriorUpdgrade1;
        this.warriorUpdgrade2 = progressSave.warriorUpdgrade2;
        this.lancerUpdgrade1 = progressSave.lancerUpdgrade1;
        this.lancerUpdgrade2 = progressSave.lancerUpdgrade2;
        this.ghostUpdgrade1 = progressSave.ghostUpdgrade1;
        this.ghostUpdgrade2 = progressSave.ghostUpdgrade2;

        foreach (LevelStatistics level in progressSave.levels)
        {
            this.levels.Add(new LevelStatisticsData(level));
        }
    }
}

[System.Serializable]
public class LevelStatisticsData
{
    public int levelNumber;
    public bool completed = false;

    public LevelStatisticsData(LevelStatistics level)
    {
        this.levelNumber = level.levelNumber;
        this.completed = level.completed;
    }
}
