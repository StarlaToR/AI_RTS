using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_AttackEnemy", menuName = "Behavior/AttackEnemy", order = 0)]
    public class IA_AttackEnemy : IA_Behavior
    {
        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_AttackEnemy newBehavior = CreateInstance< IA_AttackEnemy>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            squad.squadPerception.SetTargetType(TargetType.ATTACK_BASE);

            IA_Action action = null;

            if (blackboard.enemyBase == null) return new List<IA_Action>();

            bool canAttack = false;

            foreach (Unit unit in unitSquad.unitList)
            {
                foreach (Unit enemy in blackboard.enemyUnits)
                {
                    if (unit.CanAttack(enemy))
                    {
                        canAttack = true;
                        break;
                    }
                }

                if (unit.CanAttack(blackboard.enemyBase))
                {
                    canAttack = true;
                    break;
                }
            }

            if (canAttack)
                action = GetAction<IA_AttackTarget>();
            else
                action = GetAction<IA_MoveToTarget>();

            return new List<IA_Action> { action };
        }

        public override ActionState GetCurrentState(IA_Squad squad) 
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0 || timer.IsFinished())
                return ActionState.FAIL;

            return ActionState.RUNNING;
        }

    }
}
