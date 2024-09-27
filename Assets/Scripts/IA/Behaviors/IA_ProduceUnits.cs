using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_ProduceUnits", menuName = "Behavior/ProduceUnits", order = 0)]
    public class IA_ProduceUnits : IA_Behavior
    {
        bool CanEvaluate = false;

        public override IA_Behavior Clone()
        {
            base.Clone();
            IA_ProduceUnits newBehavior = CreateInstance<IA_ProduceUnits>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            CanEvaluate = true;

            List<IA_Action> plan = new List<IA_Action>();

            int power = blackboard.unitCount;

            List<Factory> factories = GameServices.GetControllerByTeam(squad.squadData.ai_manager.aiController.GetTeam()).GetFactoryList;

            int priceUnit = GameServices.GetUnitsData[0].Cost;

            int j = 0;
            for (int i = 0; i < power / priceUnit; i++)
            {
                IA_StartUnitProduction action = actions[0].Clone() as IA_StartUnitProduction;
                action.Init(factories[j],0);
                plan.Add(action);

                j++;
                if (j >= factories.Count)
                    j = 0;
            }

            return plan;
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            if (!CanEvaluate) return ActionState.RUNNING;

            IA_FactorySquad factorySquad = squad as IA_FactorySquad;
            if (timer.IsFinished())
                return ActionState.FAIL;

            if (factorySquad.actionWaitingList.Count == 0 && factorySquad.actionRunningList.Count == 0)
                return ActionState.SUCCESS;

            return ActionState.RUNNING;
        }
    }
}