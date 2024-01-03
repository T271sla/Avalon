using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<Skill> skills = new List<Skill>();
    public static SkillManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void addSkill(Skill skill)
    {
        skills.Add(skill);
    }

    public void checkSkillsAvailable()
    {
        List<Skill> skillsToRemove = new List<Skill>();

        foreach (Skill skill in skills)
        {
            if (GameManager.Instance.gameTurn >= skill.turnAvailable)
            {
                skill.isActive = true;
                skillsToRemove.Add(skill);
            }
        }

        // Remove the skills after the loop
        foreach (Skill skillToRemove in skillsToRemove)
        {
            skills.Remove(skillToRemove);
        }
    }
}
