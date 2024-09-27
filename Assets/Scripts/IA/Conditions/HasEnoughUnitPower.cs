using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasEnoughUnitPower", menuName = "Conditions/Has Enough Unit Power", order = 0)]
    class HasEnoughUnitPower :ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_manager.waitingUnitList.Count >= blackboard.unitCount)
            {
                return true;
            }
            return false;
        }
    }
}
