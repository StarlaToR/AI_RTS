using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_IncreaseGameResources", menuName = "Behavior/IncreaseGameResources", order = 0)]
    public class IA_IncreaseGameResources : IA_Behavior
    {
        public override IA_Behavior Clone()
        {
            base.Clone();
            IA_IncreaseGameResources newBehavior = CreateInstance<IA_IncreaseGameResources>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            if (!timer.isStarted) timer.Start(squad.squadData.parameters.GetFloat("TimeLimit"));

            squad.squadPerception.SetTargetType(TargetType.GET_RESOURCE);

            IA_Action action = null;

            if (blackboard.targetResource != null)
            {
                if ((blackboard.targetResource.transform.position - squad.transform.position).magnitude <= 3)
                    action = GetAction<IA_CaptureResourcePoint>();
                else
                    action = GetAction<IA_MoveToTarget>();
            }
            else
            {
                action = GetAction<IA_MoveToTarget>();
            }

            return new List<IA_Action> { action };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0 && timer.IsFinished())
                return ActionState.FAIL;

            if (blackboard.targetResource != null && blackboard.targetResource.GetTeam() == squad.squadData.ai_manager.aiController.GetTeam())
                return ActionState.SUCCESS;

            return ActionState.RUNNING;
        }
    }
}
