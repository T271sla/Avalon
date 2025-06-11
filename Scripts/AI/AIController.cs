using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIController
{
    public Task<StateAI> RunSearchAsync(StateAI state)
    {
        return Task.Run(() =>
        {
            MiniMax miniMax = new MiniMax(state, 5);
            return miniMax.Search();
        });
    }
}
