using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField]private List<Status> statusList;
    public static StatusManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Status getStatus(string name)
    {
        foreach(Status status in statusList) 
        {
            if(status.statusName == name)
            {
                return status;
            }
        }

        return null;
    }
}
