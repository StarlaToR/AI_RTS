using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_CaptureResource", menuName = "Behavior/CaptureResource", order = 0)]
    public class IA_CaptureResource : IA_Behavior
    {

        bool init = false;
        const float m_captureRange = 5f;

        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_CaptureResource newBehavior = CreateInstance<IA_CaptureResource>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            if (!init)
            {
                blackboard.targetResource = null;
                init = true;
            }

            squad.squadPerception.SetTargetType(TargetType.GET_RESOURCE);

            IA_Action action = null;

            if (blackboard.targetResource == null)
            {
                blackboard.targetResource = squad.squadData.ai_perception.GetClosestCapturePointDiscover(squad.transform.position);
                blackboard.destination = blackboard.targetResource.transform.position;
            }

            if (blackboard.targetResource != null && (blackboard.targetResource.transform.position - squad.transform.position).magnitude <= m_captureRange)
                action = GetAction<IA_CaptureResourcePoint>();
            else
                action = GetAction<IA_MoveToTarget>();

            return new List<IA_Action> { action };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0 || timer.IsFinished())
                return ActionState.FAIL;

            if (blackboard.targetResource != null && blackboard.targetResource.GetTeam() == squad.squadData.ai_manager.aiController.GetTeam())
                return ActionState.SUCCESS;

            return ActionState.RUNNING;
        }
    }
}
