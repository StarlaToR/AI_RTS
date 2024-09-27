using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasCapturedResource", menuName = "Conditions/Has Captured Resource", order = 0)]
    public class HasCapturedResource : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (blackboard.targetResource != null && blackboard.targetResource.GetTeam() == data.ai_manager.aiController.GetTeam())
            {
                return true;
            }
            return false;
        }
    }
}
