using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasFoundCapturePoint", menuName = "Conditions/Has Found Capture Point", order = 0)]
    public class HasFoundCapturePoint : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_perception.discorverTargetBuildings.Count > 0)
            {
                foreach (TargetBuilding targetBuilding in data.ai_perception.discorverTargetBuildings)
                {
                    if (targetBuilding.OwningTeam != data.ai_manager.aiController.GetTeam())
                        return true;
                }
            }
            return false;
        }
    }
}