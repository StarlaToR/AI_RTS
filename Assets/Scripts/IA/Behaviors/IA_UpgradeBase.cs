using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "BE_UpgradeBase", menuName = "Behavior/UpgradeBase", order = 0)]
    public class IA_UpgradeBase : IA_Behavior
    {
        public override IA_Behavior Clone()
        {
            base.Clone();

            IA_UpgradeBase newBehavior = CreateInstance<IA_UpgradeBase>();
            newBehavior.parameterObject = parameterObject;
            newBehavior.actions = actions.ConvertAll(c => { return c.Clone(); });
            CloneGeneralPart(newBehavior);
            newBehavior.Init();
            return newBehavior;
        }

        public override List<IA_Action> GetPrioritaryAction(IA_Squad squad)
        {
            IA_FactorySquad factorySquad = squad as IA_FactorySquad;
            IA_Action action;

            int numLight = 0;
            int numHeavy = 0;

            foreach (Factory factory in factorySquad.factories)
            {
                if (factory.Cost == 10)
                    numLight++;
                else
                    numHeavy++;
            }

            if (numLight > 2 * numHeavy)
                action = GetAction<IA_BuildHeavyFactory>();
            else
                action = GetAction<IA_BuildLightFactory>();

            return new List<IA_Action> { action };
        }

        public override ActionState GetCurrentState(IA_Squad squad)
        {
            IA_FactorySquad factorySquad = squad as IA_FactorySquad;
            if (timer.IsFinished())
                return ActionState.FAIL;

            if (squad.currentBehavior.blackboard.factoriesCount != factorySquad.factories.Count)
                return ActionState.RUNNING;

            foreach (Factory factory in factorySquad.factories)
            {
                if (factory.CurrentState == Factory.State.UnderConstruction)
                    return ActionState.RUNNING;
            }

            return ActionState.SUCCESS;
        }
    }
}
