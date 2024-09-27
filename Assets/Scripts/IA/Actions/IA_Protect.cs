using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_Protect", menuName = "Actions/Protect", order = 0)]
    public class IA_Protect : IA_Action
    {
        public IA_Protect()
        {
            type = ActionType.SQUAD;
        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            return ActionState.FAIL;
        }

        public override IA_Action Clone()
        {
            IA_Action action = new IA_MoveToTarget();
            action.parameterObject = this.parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }
    }
}
