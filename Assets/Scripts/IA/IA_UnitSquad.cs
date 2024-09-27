using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace RTS
{
    public class IA_UnitSquad : IA_Squad
    {
        public float refeshCooldown = 1f;
        public List<Unit> unitList;
        [HideInInspector] public NavMeshAgent agent;

        public IA_Action m_currAction;
        private float m_refreshTimer = 1f;

        public bool assembled = false;
        private Vector3 m_assemblingPoint = Vector3.zero;
        private float m_assembleBaseRadius = 4f;
        private float m_assembleAddedRadius = 0.3f;

        private bool m_isWaiting = false;

        public IA_UnitSquad(SquadData data, List<Unit> units) : base(data)
        {
            unitList = units;
        }

        public void Init(SquadData data, List<Unit> units)
        {
            m_assembleBaseRadius = 4f;
            m_assembleAddedRadius = 0.3f;

            unitList = new List<Unit>();
            for (int i = 0; i < currentBehavior.blackboard.unitCount || i < units.Count; i++)
            {
                unitList.Add(units[i]);
            }
            squadData = data;
            agent = GetComponent<NavMeshAgent>();

            Vector3 pos = Vector3.zero;

            foreach (Unit unit in unitList)
                pos += unit.transform.position;

            transform.position = pos / unitList.Count;

            if (squadPerception == null)
                squadPerception = GetComponent<IA_SquadPerception>();
            squadPerception.Init(this);

            ComputeBestAction();

            StartAssembling();
        }

        public override void ComputeBestAction()
        {
            if (currentBehavior.actions != null && currentBehavior.actions.Count != 0)
            {
                List<IA_Action> actionsPossible = currentBehavior.GetPrioritaryAction(this);

                if (actionsPossible != null && actionsPossible.Count != 0)
                    m_currAction = actionsPossible[0];
            }

            m_refreshTimer = refeshCooldown;
        }

        public void SetBehavior(IA_Behavior behavior)
        {
            currentBehavior = behavior;
            m_isWaiting = false;
        }

        public bool SetDestination(Vector3 destination)
        {
            return agent.SetDestination(destination);
        }

        public void SetSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void StopBehavior()
        {
            squadData.ai_manager.DeleteUnitSquad(this);
            squadData.ai_manager.FinishBehavior(currentBehavior, currentBehavior.GetCurrentState(this));
            currentBehavior = null;
            StartRetreat();
            m_isWaiting = true;
            Destroy(gameObject);
        }

        void StartAssembling()
        {
            assembled = false;

            foreach (Unit unit in unitList)
                unit.SetTargetPos(transform.position);
        }

        bool IsAssembled()
        {
            m_assemblingPoint = transform.position;

            for (int i = 0; i < unitList.Count; i++)
            {
                Unit unit = unitList[i];
                unit.SetTargetPos(transform.position);
            }

            for (int i = 0; i < unitList.Count; i++)
            {
                Unit unit = unitList[i];
                if (unit == null)
                {
                    unitList.RemoveAt(i);
                    i--;
                    continue;
                }

                if ((unit.transform.position - m_assemblingPoint).magnitude > m_assembleBaseRadius + m_assembleAddedRadius * unitList.Count)
                    return false;
            }

            assembled = true;
            return true;
        }

        void StartRetreat()
        {
            SetDestination(squadData.ai_perception.mainFactory.transform.position);
            foreach (Unit unit in unitList)
                unit.SetTargetPos(squadData.ai_perception.mainFactory.transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            if(unitList.Count == 0)
            {
                StopBehavior();
            }

            if ((!assembled && !IsAssembled()) || m_isWaiting)
                return;

            if (Input.GetKeyDown(KeyCode.J))
            {
                currentBehavior.blackboard.destination = new Vector3(409, 0, 91);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                currentBehavior.blackboard.destination = new Vector3(377, 0, 370);
            }

            if (currentBehavior.GetCurrentState(this) != ActionState.RUNNING && !m_isWaiting)
            {
                StopBehavior();
                return;
            }

            if (m_currAction == null)
                m_refreshTimer = 0f;

            if (m_refreshTimer > 0f)
                m_refreshTimer -= Time.deltaTime;
            else
                ComputeBestAction();

            if (m_currAction != null && m_currAction.Apply(null, this) != ActionState.RUNNING)
                m_currAction = null;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (currentBehavior && currentBehavior.blackboard != null) Gizmos.DrawLine(transform.position, currentBehavior.blackboard.destination);
        }
    }
}
