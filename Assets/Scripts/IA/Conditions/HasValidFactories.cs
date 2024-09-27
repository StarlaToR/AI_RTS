using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "C_HasValidFactories", menuName = "Conditions/Has Valid Factories", order = 0)]
    public class HasValidFactories : ConditonsTest
    {
        public override bool IsValid(ActionParameters parameters, BehaviorBlackboard blackboard, SquadData data)
        {
            if (data.ai_manager.factories.Count >= blackboard.factoriesCount)
            {
                return true;
            }
            return false;
        }


    }
}