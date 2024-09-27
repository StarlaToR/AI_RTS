using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_IsExistEnemyCapturePoint", menuName = "Conditions/Is Exist Enemy Capture Point", order = 0)]
    public class IsExistEnemyCapturePoint : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            foreach (TargetBuilding item in data.ai_perception.discorverTargetBuildings)
            {
                if(item.GetTeam() != data.ai_manager.aiController.GetTeam())
                {
                    return true;
                }
            }

            return false;
        }
    }
}