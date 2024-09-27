using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_DefendLocation", menuName = "Behavior/DefendLocation", order = 0)]
    public class IA_DefendLocation : IA_Behavior
    {
        float destinationRadius = 10f;

        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_DefendLocation newBehavior = CreateInstance<IA_DefendLocation>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            squad.squadPerception.SetTargetType(TargetType.DEFENCE);

            IA_Action action = null;    

            bool canAttack = false;

            if ((squad.transform.position - blackboard.destination).magnitude > destinationRadius)
            {
                action = GetAction<IA_MoveToTarget>();
                return new List<IA_Action> { action };
            }
            if (blackboard.enemyUnits.Count != 0)
            {
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
                }

                if (canAttack)
                    action = GetAction<IA_AttackTarget>();
                else
                    action = GetAction<IA_MoveToTarget>();

                return new List<IA_Action> { action };
            }
            if (blackboard.damagedFactories.Count != 0)
            {
                foreach (Unit unit in unitSquad.unitList)
                {
                    foreach (Factory factory in blackboard.damagedFactories)
                    {
                        if (unit.CanRepair(factory))
                        {
                            canAttack = true;
                            break;
                        }
                    }
                }

                if (canAttack)
                    action = GetAction<IA_RepairTarget>();
                else
                    action = GetAction<IA_MoveToTarget>();

                return new List<IA_Action> { action };
            }

            return new List<IA_Action> { GetAction<IA_MoveAroundLocation>() };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_UnitSquad unitSquad = squad as IA_UnitSquad;
            if (unitSquad.unitList.Count <= 0 || timer.IsFinished())
                return ActionState.FAIL;

            if ((squad.transform.position - blackboard.destination).magnitude < destinationRadius)
            {
                if (blackboard.enemyUnits.Count == 0 && blackboard.damagedFactories.Count == 0)
                    return ActionState.SUCCESS;
            }

            return ActionState.RUNNING;
        }
    }
}
