using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTS
{
    public enum TargetType
    {
        EXPLORATION,
        RAID,
        ATTACK_BASE,
        GET_RESOURCE,
        DEFENCE,
    }

    public class IA_SquadPerception : MonoBehaviour
    {
        public IA_UnitSquad m_squad;
        public TargetType m_targetType;
        List<Unit> currentEnemyList = new List<Unit>();
        private bool isInit = false;

        public bool activeLockPerception = false;
        public bool onlyNonVisibleLocation = false;

        public void Init(IA_Squad squad)
        {
            m_squad = squad as IA_UnitSquad;
            m_targetType = TargetType.EXPLORATION;

            isInit = true;
        }

        public void SetTargetType(TargetType type) { m_targetType = type; }

        // Update is called once per frame
        void Update()
        {
            if (!isInit || m_squad.currentBehavior == null) return;

            CheckEnemyUnitsAround();

            if (activeLockPerception) return;
            switch (m_targetType)
            {
                case TargetType.EXPLORATION:
                    GetUnexploredLocation();
                    GetEnemyBase();
                    break;
                case TargetType.RAID:
                    GetClosestEnemyBuilding();
                    GetClosestEnemyPos();
                    break;
                case TargetType.GET_RESOURCE:

                    if (!CheckResourceAround())
                    {
                        GetUnexploredLocation();
                    }
                    else
                    {
                        m_squad.ComputeBestAction();
                    }


                    break;
                case TargetType.ATTACK_BASE:
                    GetEnemyBase();
                    GetClosestEnemyPos();
                    break;
                case TargetType.DEFENCE:
                    GetSurroundingDamagedBuildings();
                    GetClosestEnemyPos();
                    break;
                default:
                    break;
            }
        }

        private bool CheckResourceAround()
        {
            BehaviorBlackboard blackboard = m_squad.currentBehavior.blackboard;

            if (blackboard.targetResource != null && blackboard.targetResource.GetTeam() != m_squad.squadData.ai_manager.aiController.GetTeam())
                return true;

            blackboard.targetResource = m_squad.squadData.ai_perception.GetCapturePointInArea(m_squad.transform.position);
            if (blackboard.targetResource != null && m_squad.squadData.ai_perception.discorverTargetBuildings.Contains(blackboard.targetResource))
            {
                blackboard.destination = blackboard.targetResource.transform.position;
                return true;
            }

            blackboard.targetResource = null;

            return false;
        }

        void CheckEnemyUnitsAround()
        {
            if (!isInit) return;

            float radius = 20f;

            foreach (Unit unit in m_squad.squadData.ai_perception.adversaryVisibleEntities.unitsList)
            {
                if (unit == null)
                {
                    currentEnemyList.Remove(unit);
                }
                else if ((unit.transform.position - m_squad.transform.position).magnitude <= radius)
                {
                    if (currentEnemyList.Contains(unit))
                        continue;

                    unit.OnDeadEvent2 += RemoveDeadUnit;
                    currentEnemyList.Add(unit);
                }
                else
                {
                    if (currentEnemyList.Contains(unit))
                    {
                        currentEnemyList.Remove(unit);
                        unit.OnDeadEvent2 -= RemoveDeadUnit;
                    }
                }
            }

            if (m_squad.currentBehavior != null && currentEnemyList.Count != 0)
            {
                m_squad.currentBehavior.blackboard.enemyUnits = currentEnemyList;
            }
        }

        void GetClosestEnemyPos()
        {
            if (m_squad.currentBehavior == null) return;

            if (currentEnemyList.Count != 0)
            {
                m_squad.currentBehavior.blackboard.enemyUnits = currentEnemyList;
                m_squad.currentBehavior.blackboard.destination = m_squad.currentBehavior.blackboard.enemyUnits[0].transform.position;
            }
        }

        void RemoveDeadUnit(BaseEntity entity)
        {
            Unit unit = entity as Unit;

            if (m_squad != null && m_squad.currentBehavior != null)
            {
                m_squad.currentBehavior.blackboard.enemyUnits.Remove(unit);
            }
        }


        void GetClosestEnemyBuilding()
        {
            if (m_squad.currentBehavior.blackboard.enemyFactory != null) return;

            Factory factoryTarget = new Factory();
            float length = 500f;

            foreach (Factory factory in m_squad.squadData.ai_perception.adversaryVisibleEntities.factoriesList)
            {
                if ((factory.transform.position - m_squad.transform.position).magnitude <= length)
                {
                    factoryTarget = factory;
                    length = (factory.transform.position - m_squad.transform.position).magnitude;
                }
            }

            m_squad.currentBehavior.blackboard.enemyFactory = factoryTarget;
            m_squad.currentBehavior.blackboard.destination = factoryTarget.transform.position;
        }

        void GetUnexploredLocation()
        {
            if (m_squad.currentBehavior.blackboard.destination != Vector3.zero &&
                !m_squad.squadData.ai_perception.IsLocationVisible(m_squad.currentBehavior.blackboard.destination))
                return;

            bool isValid = false;
            Vector3 location = new Vector3(Random.Range(50, 450), 0, Random.Range(450, 50));
            NavMeshHit hit = new NavMeshHit();
            bool isSampleValid = NavMesh.SamplePosition(location, out hit, 1000f, NavMesh.AllAreas);
            location = hit.position;

            if(onlyNonVisibleLocation)
            {
                isValid = m_squad.squadData.ai_perception.IsLocationVisible(location) && m_squad.squadData.ai_perception.IsLocationWaVisible(location);
            }
            else
            {
                 isValid = m_squad.squadData.ai_perception.IsLocationVisible(location);
            }

            while ( isValid || !isSampleValid)
            {
                location = new Vector3(Random.Range(50, 450), 0, Random.Range(450, 50));

                isSampleValid =  NavMesh.SamplePosition(location, out hit, 1000f, NavMesh.AllAreas);
                location = hit.position;

                if (onlyNonVisibleLocation)
                {
                    isValid = m_squad.squadData.ai_perception.IsLocationVisible(location) || m_squad.squadData.ai_perception.IsLocationWaVisible(location);
                }else
                {
                    isValid = m_squad.squadData.ai_perception.IsLocationVisible(location);
                }
            }

            m_squad.currentBehavior.blackboard.destination = location;
        }

        void GetSurroundingDamagedBuildings()
        {
            float radius = 20f;
            List<Factory> damagedBuildings = new List<Factory>();

            foreach (Factory factory in m_squad.squadData.ai_perception.allyVisibleEntities.factoriesList)
                if ((factory.transform.position - m_squad.transform.position).magnitude <= radius && factory.NeedsRepairing())
                    damagedBuildings.Add(factory);

            m_squad.currentBehavior.blackboard.damagedFactories = damagedBuildings;
            //m_squad.currBehavior.blackboard.destination = damagedBuildings[0].transform.position;
        }

        void GetEnemyBase()
        {
            if (m_squad.squadData.ai_perception.mainFactoryEnemy == null) return;

            m_squad.currentBehavior.blackboard.enemyBase = m_squad.squadData.ai_perception.mainFactoryEnemy;
            m_squad.currentBehavior.blackboard.destination = m_squad.squadData.ai_perception.mainFactoryEnemy.transform.position;
        }
    }
}
