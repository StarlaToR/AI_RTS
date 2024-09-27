using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasEnoughCapturePoints", menuName = "Conditions/Has Enough Capture Points", order = 0)]
    public class HasEnoughCapturePoints : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_perception.captureTargetBuildingList.Count >= blackboard.capturePointCount)
            {
                return true;
            }
            return false;
        }
    }
}
