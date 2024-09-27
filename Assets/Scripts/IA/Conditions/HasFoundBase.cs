using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasFoundBase", menuName = "Conditions/Has Found Base", order = 0)]
    public class HasFoundBase : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_perception.mainFactoryEnemy != null)
            {
                return true;
            }
            return false;
        }
    }
}
