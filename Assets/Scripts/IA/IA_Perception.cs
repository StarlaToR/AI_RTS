using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public struct VisibleEntities
    {
        public List<Unit> unitsList;
        public List<Factory> factoriesList;

        public void Init()
        {
            unitsList = new List<Unit>();
            factoriesList = new List<Factory>();
        }

        public void Reset()
        {
            unitsList.Clear();
            factoriesList.Clear();
        }

    }

    public class IA_Perception : MonoBehaviour
    {
        public AIController aIController;
        public InfluenceMap influenceMap;
        public FogOfWarSystem fogOfWarSystem;

        public float updateTime = 1;
        private float m_updateTimer = 0.0f;

        [Header("Defense Parameters")]
        public Factory mainFactory;
        [SerializeField] private float m_defenseRange = 30;
        [SerializeField] private float m_targetBuildingDefenseRange = 30;
        [SerializeField] private bool m_showDebugRange = false;
        public bool isEnemyNextBase = false;
        public System.Action<Unit[]> OnEnemiesNearBase;
       

        public System.Action<Vector3> OnBuildingTarget;

        [HideInInspector] public Vector3 positionBaseEnemy;
        [HideInInspector] public Factory mainFactoryEnemy= null;
        private bool m_isBaseFound = false;

        public VisibleEntities allyVisibleEntities;
        public VisibleEntities adversaryVisibleEntities;

        [HideInInspector] public int maxTargetBuilding;
        [HideInInspector] public VisibleEntities visiblesAdversaryEntities;
        [HideInInspector] public List<TargetBuilding> discorverTargetBuildings;
        [HideInInspector] public List<TargetBuilding> captureTargetBuildingList = new List<TargetBuilding>();

        #region Init Functions
        private void InitCompoment()
        {
            aIController = GetComponent<AIController>();
           
        }

        private void InitVisibityObject()
        {
            allyVisibleEntities.Init();
            adversaryVisibleEntities.Init();
            visiblesAdversaryEntities.Init();
        }
        #endregion

        #region MonoBehavior Functions
        public void Init()
        {
            InitCompoment();
            InitVisibityObject();

            discorverTargetBuildings = new List<TargetBuilding>();
            maxTargetBuilding = GameServices.GetTargetBuildings().Length;
        }

        public void Update()
        {
            UpdatePerception();
        }

        #endregion

        public void UpdatePerception()
        {
            if (m_updateTimer > updateTime)
            {
                m_updateTimer = 0.0f;
                UpdateVisibleEnity();
                UpdateDiscoverTargetBuilding();
                isEnemyNextBase = HasEnemyAroundBase();

                if (!m_isBaseFound) CheckIfEnemyBaseView();
            }
            else
            {
                m_updateTimer += Time.deltaTime;

            }
        }

        public void UpdateDiscoverTargetBuilding()
        {
            TargetBuilding[] targetBuildings = GameServices.GetTargetBuildings();

            for (int i = 0; i < targetBuildings.Length; i++)
            {
                if (discorverTargetBuildings.Contains(targetBuildings[i]))
                    continue;

                if (fogOfWarSystem.IsVisible(1 << (int)(aIController.GetTeam()), targetBuildings[i].Visibility.Position))
                    discorverTargetBuildings.Add(targetBuildings[i]);
            }
        }

        public void UpdateVisibleEnity()
        {
            List<BaseEntity> units = new List<BaseEntity>(GameServices.GetControllerByTeam(aIController.GetTeam()).GetFactoryList);
            units.AddRange(GameServices.GetControllerByTeam(aIController.GetTeam()).UnitList);
            units.AddRange(GameServices.GetControllerByTeam(aIController.GetAdversaryTeam()).UnitList);
            units.AddRange(GameServices.GetControllerByTeam(aIController.GetAdversaryTeam()).GetFactoryList);
            allyVisibleEntities.Reset();
            adversaryVisibleEntities.Reset();

            foreach (BaseEntity unit in units)
            {
                if (!fogOfWarSystem.IsVisible(1 << (int)(aIController.GetTeam()), unit.Visibility.Position)) continue;

                if (unit.GetTeam() == aIController.GetTeam())
                {
                    SetBaseEntityInList(allyVisibleEntities, unit);
                }
                else
                {
                    SetBaseEntityInList(adversaryVisibleEntities, unit);
                    SetBaseEntityInList(visiblesAdversaryEntities, unit);
                }
            }

        }

        private void SetBaseEntityInList(VisibleEntities visibleEntitites, BaseEntity entity)
        {
            Unit unit = entity as Unit;
            if (unit && !visibleEntitites.unitsList.Contains(unit))
            {
                visibleEntitites.unitsList.Add(unit);
            }

            Factory factory = entity as Factory;
            if (factory && !visibleEntitites.factoriesList.Contains(factory))
            {
                visibleEntitites.factoriesList.Add(factory);
            }

        }

        public bool HasEnemyAroundBase()
        {
            Unit[] unitVisiblesArray = adversaryVisibleEntities.unitsList.ToArray();
            Vector3 factoryPosition = mainFactory.transform.position;
            List<Unit> m_unitAroundBase = new List<Unit>();

            for (int i = 0; i < unitVisiblesArray.Length; i++)
            {
                if (Vector3.Distance(factoryPosition, unitVisiblesArray[i].transform.position) < m_defenseRange)
                {
                    m_unitAroundBase.Add(unitVisiblesArray[i]);

                }
            }
            if (m_unitAroundBase.Count != 0)
            {
             
                OnEnemiesNearBase.Invoke(m_unitAroundBase.ToArray());
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Unit> GetEnemyAroundPoint(Vector3 position)
        {
            Unit[] unitVisiblesArray = adversaryVisibleEntities.unitsList.ToArray();
            List<Unit> unitAroundPoint = new List<Unit>();

            for (int i = 0; i < unitVisiblesArray.Length; i++)
            {
                if (Vector3.Distance(position, unitVisiblesArray[i].transform.position) < m_targetBuildingDefenseRange)
                {
                    unitAroundPoint.Add(unitVisiblesArray[i]);

                }
            }

            return unitAroundPoint;
        }

        public void OnBaseDestroy()
        {
            m_isBaseFound = false;
        }

        public bool IsBaseArea(Vector3 p1)
        {
            return Vector3.Distance(mainFactory.transform.position, p1) < m_defenseRange;
        }

        public void CheckIfEnemyBaseView()
        {
            List<Factory> factories = adversaryVisibleEntities.factoriesList;
            for (int i = 0; i < factories.Count; i++)
            {
                if (factories[i].factoryPriority == Factory.FactoryPriority.PRIMARY)
                {
                    positionBaseEnemy = factories[i].transform.position;
                    mainFactoryEnemy = factories[i];
                    mainFactoryEnemy.OnDeadEvent += OnBaseDestroy;
                    m_isBaseFound = true;
                    return;
                }
            }
        }

        public TargetBuilding GetCapturePointInArea(Vector3 location, float radius = 40f)
        {
            TargetBuilding[] targetBuildings = GameServices.GetTargetBuildings();

            for (int i = 0; i < targetBuildings.Length; i++)
            {
                if (!fogOfWarSystem.IsVisible(1 << (int)(aIController.GetTeam()), targetBuildings[i].Visibility.Position)) continue;

                if (!discorverTargetBuildings.Contains(targetBuildings[i]))
                    discorverTargetBuildings.Add(targetBuildings[i]);

                if ((targetBuildings[i].Visibility.Position - new Vector2(location.x, location.z)).magnitude <= radius
                    && targetBuildings[i].GetTeam() != aIController.GetTeam())
                    return targetBuildings[i];
            }

            return null;
        }

        public bool IsLocationVisible(Vector3 location)
        {
            bool value = fogOfWarSystem.IsVisible(1 << (int)(aIController.GetTeam()), new Vector2(location.x, location.z));

            return value;
        }
        public bool IsLocationWaVisible(Vector3 location)
        {
            bool value = fogOfWarSystem.WasVisible(1 << (int)(aIController.GetTeam()), new Vector2(location.x, location.z));

            return value;
        }


        public TargetBuilding GetClosestCapturePointDiscover(Vector3 location)
        {
            float greatestDistance = 1000f;
            TargetBuilding target = null;

            foreach (TargetBuilding building in discorverTargetBuildings)
            {
                if (building.GetTeam() != aIController.GetTeam() && (building.transform.position - location).magnitude < greatestDistance)
                {
                    greatestDistance = (building.transform.position - location).magnitude;
                    target = building;
                }
            }

            return target;
        }

        public void OnDrawGizmos()
        {
            if (m_showDebugRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(mainFactory.transform.position, m_defenseRange);
            }

        }
    }
}