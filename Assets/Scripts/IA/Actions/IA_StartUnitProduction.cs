using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "AC_StartUnitProduction", menuName = "Actions/StartUnitProduction", order = 0)]
    public class IA_StartUnitProduction : IA_Action
    {
        int m_unitType = 0;
        Factory m_factory = null;
        private int m_unitIndexQueue = 0;
        private bool m_isUnitProduce = false;

        public IA_StartUnitProduction()
        {
            type = ActionType.BUILDING;
        }

        public override void StartAction(IA_Manager manager, IA_UnitSquad unitSquad)
        {

            if (m_factory.RequestUnitBuild(m_unitType))
            {
                base.StartAction();
                m_unitIndexQueue = m_factory.GetQueueSize();
                m_factory.OnUnitBuilt += ProcessUnitHasBeenBuild;
            }
        }

        public override void EndAction()
        {
            base.EndAction();
            m_isUnitProduce = false;
        }


        protected override ActionState BuildingBehavior(IA_Manager manager)
        {
            if (m_isUnitProduce)
                return ActionState.SUCCESS;

            return ActionState.RUNNING;
        }

        public void ProcessUnitHasBeenBuild(Unit unit)
        {
            if (m_unitIndexQueue == 0)
            {
                m_factory.OnUnitBuilt -= ProcessUnitHasBeenBuild;
                m_isUnitProduce = true;
            }
            else
            {
                m_unitIndexQueue--;
            }
        }

        protected override ActionState SquadBehavior(IA_UnitSquad squad)
        {
            return ActionState.FAIL;
        }


        public override IA_Action Clone()
        {

            IA_Action action = CreateInstance<IA_StartUnitProduction>();
            action.parameterObject = this.parameterObject;
            CloneGeneralPart(action);
            action.InitAction();
            return action;
        }

        public void Init(Factory factory, int unitType)
        {
            m_factory = factory;
            m_unitType = unitType;
        }
    }
}
