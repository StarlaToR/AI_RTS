using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTS
{
    [System.Serializable]
    public class PlanStep
    {
        public GoalType goalType = GoalType.ECO;
        public ConditonsTest[] conditonsTests;
        public BehaviorBlackboard behaviorBlackboard = new BehaviorBlackboard();

        public PlanStep CloneStep()
        {
            PlanStep step = new PlanStep();
            step.goalType = goalType;
            step.conditonsTests = (ConditonsTest[]) conditonsTests.Clone();
            step.behaviorBlackboard = behaviorBlackboard.Clone();

            return step;
        }

    }

    [System.Serializable]
    public class PlanPhase
    {
        public int currentStep;
        public PlanStep[] step = new PlanStep[0];

        public PlanPhase ClonePhase()
        {
            PlanPhase phase = new PlanPhase();

            phase.currentStep = currentStep;
            phase.step = new PlanStep[step.Length];
            for (int i = 0; i < step.Length; i++)
            {
                phase.step[i] = step[i].CloneStep();
            }

            return phase;
        }
    }

    [CreateAssetMenu(fileName = "Plan", menuName ="Plan")]
    public class IA_Plan : ScriptableObject
    {
        public int currentPhase;
        public PlanPhase[] phase = new PlanPhase[0]; 

        public IA_Plan ClonePlan()
        {
            IA_Plan plan = CreateInstance<IA_Plan>();
            plan.currentPhase = currentPhase;
            plan.phase = new PlanPhase[phase.Length];
            for (int i = 0; i < plan.phase.Length; i++)
            {
                plan.phase[i] = phase[i].ClonePhase();
            }

            return plan;
        }
    }
}
