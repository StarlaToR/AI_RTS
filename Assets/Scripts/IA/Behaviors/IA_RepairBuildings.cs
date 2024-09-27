using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_RepairBuildings", menuName = "Behavior/RepairBuildings", order = 0)]
    public class IA_RepairBuildings : IA_Behavior
    {
        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_RepairBuildings newBehavior = CreateInstance<IA_RepairBuildings>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            squad.squadPerception.SetTargetType(TargetType.DEFENCE);

            IA_Action action = null;

            if (blackboard.damagedFactories.Count == 0) return new List<IA_Action>();

            if ((blackboard.damagedFactories[0].transform.position - squad.transform.position).magnitude <= 3)
                action = GetAction<IA_RepairTarget>();
            else
                action = GetAction<IA_MoveToTarget>();

            return new List<IA_Action> { action };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0 || timer.IsFinished())
                return ActionState.FAIL;

            if (blackboard.damagedFactories.Count == 0)
                return ActionState.SUCCESS;

            return ActionState.RUNNING;
        }
    }
}
