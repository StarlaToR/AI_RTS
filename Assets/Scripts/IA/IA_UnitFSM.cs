using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public enum UnitState
    {
        IDLE,
        MOVE,
        ATTACK,
        FLEE,
        REPAIR,
        CAPTURE,
    }

    public class IA_UnitFSM : MonoBehaviour
    {
        [SerializeField] UnitState currState = UnitState.IDLE;

        IA_UnitPerception perception;
        Unit unit;


        // Start is called before the first frame update
        void Start()
        {
            unit = GetComponent<Unit>();
            perception = GetComponentInChildren<IA_UnitPerception>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (currState)
            {
                case UnitState.IDLE:
                    IdleBehavior();
                    break;
                case UnitState.MOVE:
                    MoveBehavior();
                    break;
                case UnitState.ATTACK:
                    AttackBehavior();
                    break;
                case UnitState.FLEE:
                    FleeBehavior();
                    break;
                case UnitState.REPAIR:
                    RepairBehavior();
                    break;
                case UnitState.CAPTURE:
                    CaptureBehavior();
                    break;
                default:
                    break;
            }
        }

        void IdleBehavior()
        {
            if (unit.IsLowHP())
            {
                currState = UnitState.FLEE;
                return;
            }
            else if (perception.ennemiesInSight.Count > 0 || perception.ennemyBuildingsInSight.Count > 0)
            {
                currState = UnitState.ATTACK;
                return;
            }
            else if (perception.neutralBuildingsInSight.Count > 0)
            {
                currState = UnitState.CAPTURE;
                return;
            }
            else if (perception.allyBuildingsInSight.Count > 0)
            {
                foreach (Factory building in perception.allyBuildingsInSight)
                {
                    if (building.NeedsRepairing())
                    {
                        currState = UnitState.REPAIR;
                        return;
                    }
                }
            }
            
        }

        void MoveBehavior()
        {

        }

        void AttackBehavior()
        {
            if (perception.ennemiesInSight.Count <= 0 && perception.ennemyBuildingsInSight.Count <= 0)
            {
                currState = UnitState.IDLE;
                return;
            }
            else if (unit.IsLowHP())
            {
                currState = UnitState.FLEE;
                return;
            }

            BaseEntity target = perception.ennemyBuildingsInSight[0];
            if (perception.ennemiesInSight.Count > 0)
                target = perception.ennemiesInSight[0];

            if (unit.CanAttack(target))
                unit.SetAttackTarget(target);
            else
                unit.SetTargetPos(target.transform.position);
        }

        void FleeBehavior()
        {

        }

        void RepairBehavior()
        {

        }

        void CaptureBehavior()
        {

        }
    }
}
