using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_RepairTarget", menuName = "Actions/RepairTarget", order = 0)]
    public class IA_RepairTarget : IA_Action
    {
        bool started = false;

        public IA_RepairTarget()
        {
            type = ActionType.SQUAD;
        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            if (!started)
            {
                foreach (Unit unit in squad.unitList)
                    unit.StartRepairing(squad.currentBehavior.blackboard.damagedFactories[0]);

                started = true;
            }

            if (squad.currentBehavior.blackboard.damagedFactories.Count == 0)
                return ActionState.SUCCESS;

            if (squad.unitList.Count == 0)
                return ActionState.FAIL;

            return ActionState.RUNNING;
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_RepairTarget>();
            action.parameterObject = this.parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }
    }
}
