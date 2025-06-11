using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public Class Class;
    public BaseUnit unitPrefab;
}

public enum Faction
{
    Hero = 0,
    Enemy = 1
}

public enum Class
{
    Strength = 0,
    Agility = 1,
    Magic = 2
}