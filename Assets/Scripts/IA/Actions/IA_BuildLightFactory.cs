using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_BuildLightFactory", menuName = "Actions/BuildLightFactory", order = 0)]
    public class IA_BuildLightFactory : IA_Action
    {
        private bool m_spawned = false;

        public IA_BuildLightFactory()
        {
            type = ActionType.BUILDING;
        }

        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            if (!m_spawned)
            {
                if (manager.SpawnBuilding(0))
                {
                    m_spawned = true;
                    return ActionState.RUNNING;
                }

                return ActionState.FAIL;
            }

            foreach (Factory factory in manager.factories)
            {
                if (factory.CurrentState == Factory.State.UnderConstruction)
                    return ActionState.RUNNING;
            }

            return ActionState.SUCCESS;
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            return ActionState.FAIL;
        }

        public override IA_Action Clone()
        {
            IA_Action action = CreateInstance<IA_BuildLightFactory>();
            action.parameterObject = this.parameterObject;
            action.InitAction();
            CloneGeneralPart(action);
            return action;
        }
    }
}
