using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    
    public abstract class ConditonsTest : ScriptableObject
    {
        public abstract bool IsValid(ActionParameters parameters,BehaviorBlackboard blackboard,SquadData data);
        public string gameStateParameterName;
        [HideInInspector] public bool lastResult;
    }

}
