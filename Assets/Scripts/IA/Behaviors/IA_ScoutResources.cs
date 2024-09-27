using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_ScoutResources", menuName = "Behavior/ScoutResources", order = 0)]
    public class IA_ScoutResources : IA_Behavior
    {
        public float rangeToCapturePoint = 5;

        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_ScoutResources newBehavior = CreateInstance<IA_ScoutResources>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            newBehavior.rangeToCapturePoint = rangeToCapturePoint;
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            //squad.squadData.parameters.SetFloat("TimeLimit", 60);
            //if (!timer.isStarted) timer.Start(squad.squadData.parameters.GetFloat("TimeLimit"));

            squad.squadPerception.SetTargetType(TargetType.GET_RESOURCE);

            return new List<IA_Action> { GetAction<IA_MoveToTarget>() };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0)
            {
                return ActionState.FAIL;
            }

            if (blackboard.targetResource != null && blackboard.targetResource.GetTeam() != squad.squadData.ai_manager.aiController.GetTeam() && squad.squadData.ai_perception.discorverTargetBuildings.Contains(blackboard.targetResource))
            {
                return ActionState.SUCCESS;
            }

            return ActionState.RUNNING;
        }
    }
}
