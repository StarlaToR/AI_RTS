using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_IsCapturePointNotFound", menuName = "Conditions/Is Capture Point Not Found", order = 0)]
    public class IsCapturePointNotFound : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_perception.discorverTargetBuildings.Count == data.ai_perception.maxTargetBuilding) return false;

            return true;
        }
    }
}