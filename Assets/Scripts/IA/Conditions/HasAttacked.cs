using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasAttacked", menuName = "Conditions/Has Attacked", order = 0)]
    public class HasAttacked : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (blackboard.enemyBase == null)
            {
                return true;
            }
            return false;
        }
    }
}
