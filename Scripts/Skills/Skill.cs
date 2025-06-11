using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject
{
    public string skillName;
    public int skillCooldown;
    public int skillActiveTime;
    public string skillDescription;
    public bool isActive = true;
    public SkillType SkillType;
    [SerializeField] protected AudioClip sound;
    public int turnCasted;
    public int turnAvailable;
    public int turnLifted;

    public virtual void Preparation(BaseUnit unit, BaseUnit targetUnit)
    {

    }

    public virtual void Activate(BaseUnit unit, BaseUnit targetUnit)
    {

    }
}

public enum SkillType
{
    Passive = 0,
    Active = 1
}
