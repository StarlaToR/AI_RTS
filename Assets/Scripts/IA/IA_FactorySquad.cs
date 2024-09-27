using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{

    public struct ActionData
    {
        public IA_Action action;
        public int priority;

        public ActionData(IA_Action act, int prio) { action = act; priority = prio; }
    }

    public class IA_FactorySquad : IA_Squad
    {
        public List<Factory> factories;
        public ActionParameters parameters;

        public IA_FactorySquad(SquadData squadData) : base(squadData)
        {
            factories = squadData.ai_manager.factories;
        }

        private bool m_hasGoal = false;
        public List<ActionData> actionWaitingList;
        public List<ActionData> actionRunningList;

        public override void ComputeBestAction()
        {
            if (currentBehavior.actions != null && currentBehavior.actions.Count != 0)
            {
                List<IA_Action> actionsPossible = currentBehavior.GetPrioritaryAction(this);

                if (actionsPossible != null && actionsPossible.Count != 0)
                    foreach (IA_Action action in actionsPossible)
                        actionWaitingList.Add(new ActionData(action, 1));
            }
        }

        public override void SendDirectionData(SquadData data)
        {
            actionWaitingList = new List<ActionData>();
            actionRunningList = new List<ActionData>();

            this.parameters = data.parameters;
            squadData = data;
            factories = squadData.ai_manager.factories;
            m_hasGoal = true;
        }

        public void LaunchBehaviorCompute()
        {
            ComputeBestAction();
        }

        #region MonoBehavior Functions
        public void Start()
        {

        }

        public void Update()
        {
            UpdateWaitingAction();
            UpdateRunningAction();
        }
        #endregion 

        public bool IsFinished()
        {
            foreach (Factory factory in factories)
                if (factory.GetQueueSize() != 0)
                    return false;

            if (actionWaitingList.Count != 0 || actionRunningList.Count != 0)
                return false;

            return true;
        }

        public void UpdateWaitingAction()
        {
            int[] indexArray = new int[actionWaitingList.Count];
            for (int i = 0; i < actionWaitingList.Count; i++)
            {
                // Test if action is possible
                if (true)
                {
                    actionRunningList.Add(actionWaitingList[i]);

                }
            }

            for (int i = 0; i < indexArray.Length; i++)
            {
                actionWaitingList.RemoveAt(indexArray[i]);
            }
        }

        public void UpdateRunningAction()
        {
            List<ActionData> succesList = new List<ActionData>();

            foreach (ActionData item in actionRunningList)
            {
                ActionState succes = item.action.Apply(squadData.ai_manager);
                if (succes == ActionState.SUCCESS || succes == ActionState.FAIL)
                {
                    succesList.Add(item);
                }
            }


            foreach (ActionData item in succesList)
            {
                actionRunningList.Remove(item);
            }


            if (actionRunningList.Count == 0 && actionWaitingList.Count == 0)
            {
                if (m_hasGoal)
                {
                    m_hasGoal = false;
                    CallBehaviorFinish(ActionState.SUCCESS);
                }
            }
        }

        public void StopBehavior()
        {
            actionWaitingList.Clear();
            actionRunningList.Clear();
            m_hasGoal = false;
            CallBehaviorFinish(ActionState.FAIL);
        }
    }
}
