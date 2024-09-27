using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public class IA_Behavior : ScriptableObject
    {
        public List<IA_Action> actions = new List<IA_Action>();

        public ActionParameterObject parameterObject;
        public ActionParameters parameters;

        public bool isClone = false;

        public int cost = 1;

        [HideInInspector]public bool isInvalid;
        public BehaviorBlackboard blackboard;
        public BehaviorTimer timer;
        public ConditonsTest[] conditonsTests;
        public GameStateParameters stateParameters;
        public GoalType goalTypeBehavior;

        public void Init()
        {
            parameters = new ActionParameters(parameterObject);
            timer = new BehaviorTimer();
        }

        public virtual IA_Behavior Clone()  
        {
            isClone = true;
            return this;
        }

        public string GetName()
        {
          return  GetType().Name;
        }


        public void CloneGeneralPart(IA_Behavior cloneItem)
        {
            cloneItem.blackboard = new BehaviorBlackboard();
            cloneItem.conditonsTests = conditonsTests;
            cloneItem.stateParameters = stateParameters;
            cloneItem.goalTypeBehavior = goalTypeBehavior;
            cloneItem.cost = cost;
        }

        public bool CanStart(SquadData data)
        {
            bool result = true;
            if (conditonsTests == null) return result;

            for (int i = 0; i < conditonsTests.Length; i++)
            {   
                if(!conditonsTests[i].IsValid(parameters, blackboard, data))
                {
                    conditonsTests[i].lastResult = false;
                    result = false;
                    continue;
                }
                conditonsTests[i].lastResult = true;
            }

            return result;
        }

        public T GetAction<T>() where T : IA_Action
        {
            foreach (IA_Action action in actions)
            {
                T checkedAction = action as T;

                if (checkedAction != null)
                    return checkedAction;
            }
            return null;
        }

        public virtual void SetupBehaviorParameter()
        {
           // throw new NotImplementedException();
        }

        public virtual List<IA_Action> GetPrioritaryAction(IA_Squad squad) { return new List<IA_Action>(); }

        public virtual ActionState GetCurrentState(IA_Squad squad) { return ActionState.FAIL; }
    }
}
