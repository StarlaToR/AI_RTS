using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public class IA_Manager : MonoBehaviour
    {
        // AI components
        public AIController aiController;

        public List<Factory> factories;

        public IA_Perception perception;
        private IA_Commander m_commander;
        private IA_FactorySquad m_factorySquad;
        private List<IA_UnitSquad> m_unitSquadList;

        public const int MAX_QUEUE_BEHAVIOR = 3;
        public const int MAX_RUNNING_BEHAVIOR = 5;


        [HideInInspector] public List<IA_Behavior> waitingQueueBehavior = new List<IA_Behavior>();
        [SerializeField] private List<string> m_queueBehaviorString;
        [HideInInspector] public List<IA_Behavior> activeBehavior = new List<IA_Behavior>();
        [SerializeField] private List<string> m_currentBehaviorString;


        [HideInInspector] public List<Unit> waitingUnitList;

        private Vector3 m_factorySpawnPosition = Vector3.zero;

        [SerializeField] private GameObject m_factorySquadPrefab;
        [SerializeField] private GameObject m_unitSquadPrefab;

        private List<IA_Behavior> m_previousPlan = new List<IA_Behavior>();

        // Launch in AI Controller
        public void Init()
        {
            aiController = GetComponent<AIController>();
            perception = GetComponent<IA_Perception>();
            m_commander = GetComponent<IA_Commander>();

            m_unitSquadList = new List<IA_UnitSquad>();
            factories = GameServices.GetControllerByTeam(aiController.GetTeam()).GetFactoryList;
            SetupFactoriesEvent();

        }

        #region Monobehavior functions
        // Update is called once per frame
        void Update()
        {
            RunBehavior();
        }

        public void OnApplicationQuit()
        {
            for (int i = 0; i < waitingQueueBehavior.Count; i++)
            {
                i = 0;
                IA_Behavior behavior = waitingQueueBehavior[i];
                waitingQueueBehavior.RemoveAt(i);
                DestroyImmediate(behavior);
            };
        }
        #endregion

        #region Behavior Functions
        public bool IsBehaviorInQueue()
        {
            return waitingQueueBehavior.Count != 0 || activeBehavior.Count != 0;
        }

        public bool StartBehavior(IA_Behavior behavior)
        {
            SquadData squadData = CreateSquadData(behavior);

            if (!behavior.CanStart(squadData))
                return false;

            if (behavior.actions.Count == 0) return false;

            if (behavior.actions[0].type == ActionType.BUILDING)
            {
                foreach (IA_Behavior a_Behavior in activeBehavior)
                {
                    if (a_Behavior.actions[0].type == ActionType.BUILDING)
                    {
                        return false;
                    }
                }

                if (m_factorySquad == null) CreateFactorySquad();

                m_factorySquad.currentBehavior = behavior;
                m_factorySquad.SendDirectionData(squadData);
                m_factorySquad.LaunchBehaviorCompute();
                activeBehavior.Add(behavior);
                m_currentBehaviorString.Add(behavior.GetType().Name);
                return true;

            }

            if (behavior.actions[0].type == ActionType.SQUAD)
            {

                IA_UnitSquad unitSquad = CreateUnitSquad();
                unitSquad.currentBehavior = behavior;
                unitSquad.Init(CreateSquadData(behavior), waitingUnitList);
                activeBehavior.Add(behavior);
                m_currentBehaviorString.Add(behavior.GetType().Name);
                return true;
            }
            return true;

        }

        public void AddQueueBehavior(IA_Behavior behavior)
        {

            if (behavior.actions.Count > 0 && behavior.actions[0].type == ActionType.BUILDING)
            {
                behavior.SetupBehaviorParameter();
            }
            m_queueBehaviorString.Add(behavior.GetType().Name);
            waitingQueueBehavior.Add(behavior);
        }

        public void RunBehavior()
        {
            for (int i = 0; i < waitingQueueBehavior.Count; i++)
            {
                if (StartBehavior(waitingQueueBehavior[i]))
                {
                    m_queueBehaviorString.RemoveAt(i);
                    waitingQueueBehavior.RemoveAt(i);
                    i--;
                }
            }
        }

        public void FinishBehavior(IA_Behavior behavior, RTS.ActionState actionState)
        {
            if (behavior == null) return;

            m_previousPlan.Remove(behavior);
            int index = activeBehavior.IndexOf(behavior);
            if (index != -1)
            {
                m_currentBehaviorString.RemoveAt(activeBehavior.IndexOf(behavior));
                activeBehavior.Remove(behavior);
            }
        }

        public void StopBehaviors()
        {
            for (int i = 0; i < m_unitSquadList.Count; i++)
            {
                m_unitSquadList[i].StopBehavior();
            }

            if (m_factorySquad) m_factorySquad.StopBehavior();
        }
        #endregion

        #region Plan Functions
        public void ApplyNewPlan(List<IA_Behavior> newDirection)
        {
            m_previousPlan = newDirection;

            StopBehaviors();

            activeBehavior.Clear();
            waitingQueueBehavior.Clear();
            m_currentBehaviorString.Clear();
            m_queueBehaviorString.Clear();

            for (int i = 0; i < m_previousPlan.Count; i++)
            {
                if (!waitingQueueBehavior.Contains(m_previousPlan[i]))
                {
                    AddQueueBehavior(m_previousPlan[i]);
                }
            }
        }

        public void AddOtherPlan(List<IA_Behavior> otherPlan)
        {
            m_previousPlan.AddRange(otherPlan);
            for (int i = 0; i < otherPlan.Count; i++)
            {
                AddQueueBehavior(otherPlan[i]);

            }
        }
        #endregion

        #region Squad Functions
        public IA_UnitSquad GetUnitSquad()
        {
            return CreateUnitSquad();
        }

        private IA_UnitSquad CreateUnitSquad()
        {
            IA_UnitSquad unitSquad = Instantiate(m_unitSquadPrefab, new Vector3(300, 0, 300), Quaternion.identity).GetComponent<IA_UnitSquad>();
            m_unitSquadList.Add(unitSquad);
            return unitSquad;
        }

        public void DeleteUnitSquad(IA_UnitSquad unitSquad)
        {
            //TODO : Apply all logics at destruction


            for (int i = 0; i < unitSquad.unitList.Count; i++)
            {
                if (waitingUnitList.Contains(unitSquad.unitList[i])) continue;
                waitingUnitList.Add(unitSquad.unitList[i]);
                unitSquad.unitList[i].state = Unit.UnitSquadState.FREE;
            };


            m_unitSquadList.Remove(unitSquad);

        }

        private void CreateFactorySquad()
        {
            if (m_factorySquad != null) return;
            m_factorySquad = Instantiate(m_factorySquadPrefab).GetComponent<IA_FactorySquad>();
        }

        private SquadData CreateSquadData(IA_Behavior behavior)
        {
            SquadData squadData = new SquadData();
            squadData.ai_manager = this;
            squadData.ai_perception = perception;
            squadData.parameters = behavior.parameters;
            return squadData;

        }

        #endregion

        #region Entities Functions

        public void SetupFactoriesEvent()
        {
            foreach (Factory item in factories)
            {
                AddFactoriesList(item);
            }
        }

        public void AddFactoriesList(Factory factory)
        {
            factory.OnUnitBuilt += AddUnitToList;
            factory.OnFactoryBuilt += AddFactoriesList;
            factory.OnDestroFactoryEvent += RemoveFactory;
            if (!perception.IsBaseArea(factory.transform.position))
            {
                factory.IsUnderAtack += m_commander.GenerateDefensiveOrderTargetBuilding;
            }

        }

        public void RemoveFactory(Factory factory)
        {
            factories.Remove(factory);
        }

        public void AddUnitToList(Unit item)
        {
            waitingUnitList.Add(item);
            item.state = Unit.UnitSquadState.FREE;

        }

        public void AddTargetBuilding(TargetBuilding targetBuilding)
        {
            targetBuilding.onStartCapture += m_commander.GetTargetBuildingInfo;
            targetBuilding.OnStopCapture += m_commander.GetTargetBuildingInfo;

            if (!perception.captureTargetBuildingList.Contains(targetBuilding))
                perception.captureTargetBuildingList.Add(targetBuilding);

        }

        public void RemoveTargetBuilding(TargetBuilding targetBuilding)
        {
            targetBuilding.onStartCapture -= m_commander.GetTargetBuildingInfo;
            targetBuilding.OnStopCapture -= m_commander.GetTargetBuildingInfo;
            perception.captureTargetBuildingList.Remove(targetBuilding);
        }

        public bool SpawnBuilding(int factoryType)
        {
            for (float i = 0; i < 2f; i+= 0.01f)
            {
                m_factorySpawnPosition = factories[0].transform.position + new Vector3(20f * Mathf.Cos(i * Mathf.PI), 0, 20f * Mathf.Sin(i * Mathf.PI));

                if (aiController.TryBuildFactory(factoryType, m_factorySpawnPosition))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
