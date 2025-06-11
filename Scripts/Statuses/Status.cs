using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : ScriptableObject
{
    public string statusName;
    public int statusCooldown;
    public int statusActiveTime;
    public string statusDescription;
    public int turnCasted;
    public int turnLifted;
    public bool stackable = false;
    public bool indefinite;

    public virtual void Activate(BaseUnit unit)
    {

    }

    public virtual void Lift(BaseUnit unit)
    {

    }
}

public enum StatusType
{
    OnCast = 0,
    OnTurn = 1
}
