using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_MoveToTarget", menuName = "Actions/MoveToTarget", order = 0)]
    public class IA_MoveToTarget : IA_Action
    {
        public enum Formation
        {
            COLUMN,
            INLINE,
            ARROW,
            SQUARE,
        }

        Vector3 m_destination = Vector3.zero;

        public IA_MoveToTarget()
        {
            type = ActionType.SQUAD;
        }

        public override void StartAction(IA_Manager manager = null, IA_UnitSquad squad = null)
        {
            base.StartAction(manager, squad);

            float lowestSpeed = 20f;

            foreach (Unit unit in squad.unitList)
                if (unit.GetUnitData.Speed < lowestSpeed)
                    lowestSpeed = unit.GetUnitData.Speed;

            squad.SetSpeed(lowestSpeed);

        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            if (squad.currentBehavior.blackboard.destination != m_destination)
            {
                m_destination = squad.currentBehavior.blackboard.destination;

                if (!squad.SetDestination(m_destination))
                    return ActionState.FAIL;
            }

            return MoveUnits(squad, squad.currentBehavior.blackboard.squadFormation);
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_MoveToTarget>();
            action.parameterObject = this.parameterObject;
            CloneGeneralPart(action);
            action.InitAction();
            return action;
        }

        ActionState MoveUnits(IA_UnitSquad squad, int formationType)
        {
            switch (formationType)
            {
                case (int)Formation.COLUMN:
                    return ColumnMovement(squad);
                case (int)Formation.INLINE:
                    return InlineMovement(squad);
                case (int)Formation.ARROW:
                    return ArrowMovement(squad);
                default:
                    return ActionState.FAIL;
            }
        }

        ActionState ColumnMovement(IA_UnitSquad squad)
        {
            ActionState state = ActionState.SUCCESS;
            int i = 0;
            foreach (Unit unit in squad.unitList)
            {
                Vector3 targetPos = squad.transform.position + squad.transform.forward * 2f * (squad.unitList.Count - i) - squad.transform.forward * squad.unitList.Count;
                unit.SetTargetPos(targetPos);
                i++;

                if (state != ActionState.RUNNING && (unit.transform.position - m_destination).magnitude < 1f)
                    state = ActionState.RUNNING;
            }

            return state;
        }

        ActionState InlineMovement(IA_UnitSquad squad)
        {
            ActionState state = ActionState.SUCCESS;
            int i = 0;
            foreach (Unit unit in squad.unitList)
            {
                Vector3 targetPos = squad.transform.position + squad.transform.right * 2f * (squad.unitList.Count - i) - squad.transform.right * squad.unitList.Count;
                unit.SetTargetPos(targetPos);
                i++;

                if (state != ActionState.RUNNING && (unit.transform.position - m_destination).magnitude < 1f)
                    state = ActionState.RUNNING;
            }

            return state;
        }

        ActionState ArrowMovement(IA_UnitSquad squad)
        {
            ActionState state = ActionState.SUCCESS;

            int i = 0;
            int j = 0;
            bool isLeft = true;

            foreach (Unit unit in squad.unitList)
            {
                Vector3 targetPos = squad.transform.position + squad.transform.right * squad.unitList.Count * i * (isLeft ? 1 : -1) - squad.transform.forward * squad.unitList.Count * j;
                unit.SetTargetPos(targetPos);
                isLeft = !isLeft;

                if (!isLeft)
                {
                    i++;
                    j++;
                }

                if (state != ActionState.RUNNING && (unit.transform.position - m_destination).magnitude < 1f)
                    state = ActionState.RUNNING;
            }

            return state;
        }
    }
}
