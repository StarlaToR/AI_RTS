using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasDefended", menuName = "Conditions/HasDefended", order = 0)]
    public class HasDefended : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (blackboard.enemyUnits.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
