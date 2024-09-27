using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_CaptureResourcePoint", menuName = "Actions/CaptureResourcePoint", order = 0)]
    public class IA_CaptureResourcePoint : IA_Action
    {

        public override void StartAction(IA_Manager manager = null, IA_UnitSquad squad = null)
        {
            base.StartAction(manager, squad);

            squad.currentBehavior.timer.Pause();
            foreach (Unit unit in squad.unitList)
                unit.StartCapture(squad.currentBehavior.blackboard.targetResource);
        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            if (squad.currentBehavior.blackboard.targetResource.GetTeam() == squad.squadData.ai_manager.aiController.GetTeam())
                return ActionState.SUCCESS;

            if (squad.unitList.Count == 0)
                return ActionState.FAIL;

            foreach (Unit unit in squad.unitList)
            {
                if (!unit.IsCapturing())
                    unit.SetCaptureTarget(squad.currentBehavior.blackboard.targetResource);
            }

            return ActionState.RUNNING;
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_CaptureResourcePoint>();
            action.type = ActionType.SQUAD;
            action.parameterObject = parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }
    }
}
