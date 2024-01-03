using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject
{
    public string skillName;
    public int skillCooldown;
    public int skillActiveTime;
    public string skillDescription;
    public bool isActive = false;
    public SkillType SkillType;
    [SerializeField] protected AudioClip sound;
    public int turnCasted;
    public int turnAvailable;

    public virtual void Preparation(BaseUnit unit, BaseUnit tragetUnit)
    {

    }

    public virtual void Activate(BaseUnit unit, BaseUnit tragetUnit)
    {

    }
}

public enum SkillType
{
    Passive = 0,
    Active = 1
}
