using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_ScoutEnemyBase", menuName = "Behavior/ScoutEnemyBase", order = 0)]
    public class IA_ScoutEnemyBase : IA_Behavior
    {
        bool init = false;

        public override IA_Behavior Clone()
        {
            base.Clone();
            IA_ScoutEnemyBase newBehavior = CreateInstance<IA_ScoutEnemyBase>();
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
                squad.squadPerception.onlyNonVisibleLocation = true;
                init = true;
            }

            squad.squadPerception.SetTargetType(TargetType.EXPLORATION);

            return new List<IA_Action> { GetAction<IA_MoveToTarget>() };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0)
                return ActionState.FAIL;

            if (blackboard.enemyBase != null)
            {
                squad.squadPerception.onlyNonVisibleLocation = false;
                return ActionState.SUCCESS;
            }
             

            return ActionState.RUNNING;
        }
    }
}
