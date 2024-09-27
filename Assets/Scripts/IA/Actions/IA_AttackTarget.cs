using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_AttackTarget", menuName = "Actions/AttackTarget", order = 0)]
    public class IA_AttackTarget : IA_Action
    {
        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            if (squad.unitList.Count == 0)
                return ActionState.FAIL;

            BaseEntity target;

            BehaviorBlackboard blackboard = squad.currentBehavior.blackboard;

            if (blackboard.enemyUnits.Count != 0)
            {
                target = blackboard.enemyUnits[0];
            }
            else if (blackboard.enemyBase != null)
            {
                target = blackboard.enemyBase;
            }
            else if (blackboard.enemyFactory != null)
            {
                target = blackboard.enemyFactory;
            }
            else
            {
                return ActionState.SUCCESS;
            }


            if (target == null) return ActionState.RUNNING;

            foreach (Unit unit in squad.unitList)
            {
                if (unit.CanAttack(target))
                    unit.SetAttackTarget(target);
                else
                    unit.SetTargetPos(target.transform.position);
            }

            return ActionState.RUNNING;
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_AttackTarget>();
            action.parameterObject = this.parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }
    }
}
