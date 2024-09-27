using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_MoveToLocation", menuName = "Actions/MoveToLocation", order = 0)]
    public class IA_MoveAroundLocation : IA_Action
    {
        Vector3 target = Vector3.zero;

        public IA_MoveAroundLocation()
        {
            type = ActionType.SQUAD;
        }

        public override void StartAction(IA_Manager manager = null, IA_UnitSquad squad = null)
        {
            base.StartAction(manager, squad);

            float speed = 20f;

            foreach (Unit unit in squad.unitList)
                if (unit.GetUnitData.Speed < speed)
                    speed = unit.GetUnitData.Speed;

            squad.SetSpeed(speed);
        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            return ActionState.FAIL;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            if (target == Vector3.zero)
            {
                target = squad.currentBehavior.blackboard.destination + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                squad.SetDestination(target);
            }
           
            MoveUnits(squad);

            if ((squad.transform.position - target).magnitude < 1f)
                target = Vector3.zero;

            return ActionState.RUNNING;
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_MoveAroundLocation>();
            action.parameterObject = this.parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }

        void MoveUnits(IA_UnitSquad squad)
        {
            int i = 0;
            int j = 0;

            foreach (Unit unit in squad.unitList)
            {
                unit.SetTargetPos(squad.transform.position + squad.transform.forward * 2f * (squad.unitList.Count / 3f - i) 
                    - squad.transform.forward * squad.unitList.Count / 3f - squad.transform.right + squad.transform.right * j);

                j++;
                if (j > 2)
                {
                    j = 0;
                    i++;
                }
            }
        }
    }
}
