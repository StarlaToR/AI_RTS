using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{

    public enum GoalType
    {
        INFOS = 0,
        UNIT = 1,
        DAMAGE = 2,
        DEFENSE = 3,
        ECO = 4
    }

    public class IA_Commander : MonoBehaviour
    {
        private IA_Perception m_perception;
        public IA_Manager manager;

        public List<IA_Behavior> behaviors = new List<IA_Behavior>();
        private List<IA_Behavior> m_cloneBehavior = new List<IA_Behavior>();
        public IA_Plan gamePlan;
        private IA_Plan m_cloneGamePlan;

        public List<ConditonsTest> defenseConditions = new List<ConditonsTest>();
        public bool isDefensiveMode = false;
        public float defensiveModeDuration = 1.0f;

        private float m_defensiveTimer = 0.0f;

        [HideInInspector] public List<IA_Behavior> currentPlan = new List<IA_Behavior>();
        public List<string> currentPlanString = new List<string>();


        #region General Methods
        public void Init()
        {
            m_cloneGamePlan = gamePlan.ClonePlan();
            m_perception = GetComponent<IA_Perception>();
            manager = GetComponent<IA_Manager>();

            m_perception.OnEnemiesNearBase += GenerateDefensiveOrder;
            foreach (IA_Behavior item in behaviors)
            {
                m_cloneBehavior.Add(item.Clone());
            }
            CreatePlan();
        }

        private int ComputePlanCost(IA_Behavior[] behaviorsPlan)
        {
            int costResult = 0;
            for (int i = 0; i < behaviorsPlan.Length; i++)
            {
                costResult += behaviorsPlan[i].cost;
            }
            if (costResult == 0) costResult = 100;
            return costResult;
        }

        private SquadData CreateSquadData(IA_Behavior behavior)
        {
            SquadData squadData = new SquadData();
            squadData.ai_manager = manager;
            squadData.ai_perception = m_perception;
            if (behavior != null) squadData.parameters = behavior.parameters;
            return squadData;
        }

        public int CalculSquadPower(Unit[] units)
        {
            int power = 0;
            for (int i = 0; i < units.Length; i++)
            {
                power += units[i].GetUnitData.DpsScore;
            }

            return power;
        }

        public void GetTargetBuildingInfo(TargetBuilding targetBuilding)
        {
            if (targetBuilding.isCaptured)
            {
                GenerateDefensiveOrderTargetBuilding(targetBuilding.transform.position);
            }
            else
            {
                CreatePlan();
            }
        }
        #endregion

        #region MonoBehaviour Methods
        // Update is called once per frame
        void Update()
        {
            if (!isDefensiveMode)
            {
                if (IsCurrentStepValidate())
                {
                    if (m_cloneGamePlan.phase[0].currentStep != m_cloneGamePlan.phase[0].step.Length - 1)
                    {
                        m_cloneGamePlan.phase[0].currentStep++;
                        CreatePlan();
                    }
                }
                else
                {
                    if (!manager.IsBehaviorInQueue())
                    {
                        CreatePlan();
                    }
                }
            }

            UpdateDefensiveMode();
        }

        public void OnApplicationQuit()
        {
            m_cloneBehavior.Clear();
            currentPlanString.Clear();
        }

        #endregion

        #region Main Plan Methods
        public bool IsCurrentStepValidate()
        {
            SquadData squadData = CreateSquadData(null);
            PlanPhase planPhase = m_cloneGamePlan.phase[m_cloneGamePlan.currentPhase];

            foreach (ConditonsTest item in planPhase.step[planPhase.currentStep].conditonsTests)
            {
                if (!item.IsValid(null, planPhase.step[planPhase.currentStep].behaviorBlackboard, squadData))
                {
                    return false;
                }
            }

            return true;
        }

        public void CreatePlan()
        {
            currentPlan.Clear();
            currentPlanString.Clear();

            GenerateNewPlan();
            manager.ApplyNewPlan(currentPlan);
        }

        private void GenerateNewPlan()
        {
            PlanPhase planPhase = m_cloneGamePlan.phase[m_cloneGamePlan.currentPhase];
            PlanStep goalStep = planPhase.step[planPhase.currentStep];

            List<IA_Behavior> inversePlan = new List<IA_Behavior>();
            int lowestCostFound = 100;
            for (int i = 0; i < m_cloneBehavior.Count; i++)
            {
                if (goalStep.goalType != m_cloneBehavior[i].goalTypeBehavior) continue;

                if (!m_cloneBehavior[i].stateParameters.gameParameters.ContainsKey(goalStep.conditonsTests[0].gameStateParameterName))
                    continue;

                m_cloneBehavior[i].blackboard = goalStep.behaviorBlackboard;
                List<IA_Behavior> stepPlan = new List<IA_Behavior>();
                stepPlan.Add(m_cloneBehavior[i]);
                FindPrecendentBehavior(m_cloneBehavior[i], stepPlan);

                int cost = ComputePlanCost(stepPlan.ToArray());

                if (stepPlan[stepPlan.Count - 1].isInvalid)
                {
                    Debug.LogWarning($"Warning : This behavior has been invalidate {stepPlan[stepPlan.Count - 1].GetType().Name}");
                    continue;
                }

                if (cost < lowestCostFound)
                {
                    lowestCostFound = cost;
                    inversePlan = stepPlan;
                }
            }

            inversePlan.Reverse();
            currentPlan = inversePlan.ConvertAll(n =>
            {
                currentPlanString.Add(n.GetType().Name);
                return n.Clone();
            });

            currentPlan.ForEach(n => { n.blackboard = goalStep.behaviorBlackboard; });
        }

        public void FindPrecendentBehavior(IA_Behavior behavior, List<IA_Behavior> inversePlan)
        {
            SquadData squadData = CreateSquadData(behavior);

            // Check if all conditions are validate
            if (behavior.CanStart(squadData))
                return;

            for (int j = 0; j < behavior.conditonsTests.Length; j++)
            {
                if (behavior.conditonsTests[j].lastResult) continue;

                behavior.isInvalid = true;
                int lowerCost = 100;

                ConditonsTest conditon = behavior.conditonsTests[j];
                List<IA_Behavior> lowestPlan = new List<IA_Behavior>();

                for (int i = 0; i < m_cloneBehavior.Count; i++)
                {
                    if (!m_cloneBehavior[i].stateParameters.gameParameters.ContainsKey(conditon.gameStateParameterName))
                        continue;

                    behavior.isInvalid = false;
                    m_cloneBehavior[i].blackboard = behavior.blackboard;

                    List<IA_Behavior> stepPlan = inversePlan;
                    stepPlan.Add(m_cloneBehavior[i]);

                    Debug.Log(m_cloneBehavior[i].GetType().Name.ToString());

                    FindPrecendentBehavior(m_cloneBehavior[i], stepPlan);
                    int cost = ComputePlanCost(stepPlan.ToArray());
                    stepPlan = stepPlan.GetRange(inversePlan.Count, stepPlan.Count - inversePlan.Count);

                    if (cost < lowerCost)
                    {
                        lowerCost = cost;
                        lowestPlan = stepPlan;
                    }
                }

                inversePlan.AddRange(lowestPlan);
            }
        }
        #endregion

        #region Defense Plan Methods
        private void UpdateDefensiveMode()
        {
            if (m_perception.isEnemyNextBase || !isDefensiveMode) return;

            if (m_defensiveTimer > defensiveModeDuration)
            {
                if(isDefensiveMode)  CreatePlan();

                isDefensiveMode = false;
                m_defensiveTimer = 0.0f;
            }
            else
            {
                m_defensiveTimer += Time.deltaTime;
            }
        }

        private List<IA_Behavior> GenerateNewPlan(PlanStep step)
        {
            List<IA_Behavior> resultPlan = new List<IA_Behavior>();
            List<IA_Behavior> inversePlan = new List<IA_Behavior>();
            int lowestCostFound = 100;

            for (int i = 0; i < m_cloneBehavior.Count; i++)
            {
                if (step.goalType != m_cloneBehavior[i].goalTypeBehavior) continue;

                if (!m_cloneBehavior[i].stateParameters.gameParameters.ContainsKey(step.conditonsTests[0].gameStateParameterName))
                    continue;

                m_cloneBehavior[i].blackboard = step.behaviorBlackboard;
                List<IA_Behavior> stepPlan = new List<IA_Behavior>();
                stepPlan.Add(m_cloneBehavior[i]);
                FindPrecendentBehavior(m_cloneBehavior[i], stepPlan);

                int cost = ComputePlanCost(stepPlan.ToArray());

                if (stepPlan[stepPlan.Count - 1].isInvalid)
                {
                    Debug.LogWarning($"Warning : This behavior has been invalidate {stepPlan[stepPlan.Count - 1].GetType().Name}");
                    continue;
                }

                if (cost < lowestCostFound)
                {
                    lowestCostFound = cost;
                    inversePlan = stepPlan;
                }
            }

            inversePlan.Reverse();
            resultPlan = inversePlan.ConvertAll(n =>
            {
                currentPlanString.Add(n.GetType().Name);
                return n.Clone();
            });

            resultPlan.ForEach(n => { n.blackboard = step.behaviorBlackboard; });
            return resultPlan;
        }

        public void GenerateDefensiveOrder(Unit[] units)
        {
            if (isDefensiveMode) return;

            isDefensiveMode = true;

            PlanStep goalStep = new PlanStep();
            goalStep.goalType = GoalType.DEFENSE;

            goalStep.conditonsTests = defenseConditions.ToArray();
            goalStep.behaviorBlackboard.unitCount = CalculSquadPower(units);
            goalStep.behaviorBlackboard.destination = m_perception.mainFactory.transform.position;

            currentPlanString.Clear();

            currentPlan = GenerateNewPlan(goalStep);
            manager.ApplyNewPlan(currentPlan);

        }

        public void GenerateDefensiveOrderTargetBuilding(Vector3 position)
        {
            // Check influencial map and generate order;
            PlanStep goalStep = new PlanStep();
            goalStep.goalType = GoalType.DEFENSE;

            List<Unit> units = m_perception.GetEnemyAroundPoint(position);
            goalStep.behaviorBlackboard.unitCount = CalculSquadPower(units.ToArray());
            goalStep.conditonsTests = defenseConditions.ToArray();
            goalStep.behaviorBlackboard.destination = position;

            List<IA_Behavior> defensePlan = GenerateNewPlan(goalStep);
            manager.AddOtherPlan(defensePlan);
        }
        #endregion
    }
}
