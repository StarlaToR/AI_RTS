using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public enum ActionType
    {
        BUILDING,
        SQUAD,
    }
    public enum ActionState
    {
        RUNNING,
        FAIL,
        SUCCESS,
    }

    public abstract class IA_Action : ScriptableObject
    {
        public ActionParameterObject parameterObject;
        public ActionParameters parameters;

        public ActionType type;

        private bool m_hasStarted = false;

        public virtual void InitAction()
        {
            parameters = new ActionParameters(parameterObject);
        }


        public virtual void StartAction(IA_Manager manager = null, IA_UnitSquad squad = null)
        {
            m_hasStarted = true;
        }

        public virtual void EndAction()
        {
            m_hasStarted = false;
        }


        public ActionState Apply(IA_Manager manager = null, IA_UnitSquad squad = null)
        {
            ActionState actionState = ActionState.RUNNING;
            if (!m_hasStarted)
            {
                StartAction(manager, squad);
            }

            if (type == ActionType.BUILDING)
                actionState = BuildingBehavior(manager);
            else
                actionState = SquadBehavior(squad);

            if (actionState == ActionState.SUCCESS || actionState == ActionState.FAIL)
            {
                EndAction();
            }

            return actionState;
        }

        public virtual IA_Action Clone()
        {
            return this;
        }

        public void CloneGeneralPart(IA_Action newAction)
        {
            newAction.type = type;
        }

        protected abstract ActionState BuildingBehavior(IA_Manager manager);
        protected abstract ActionState SquadBehavior(IA_UnitSquad squad);
    }
}
