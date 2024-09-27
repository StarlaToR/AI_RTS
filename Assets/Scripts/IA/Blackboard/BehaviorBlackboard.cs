using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{

    public enum ProductionDirectionType
    {
        Power,
        Speed,
        Fast,
    }


    [System.Serializable]
    public class BehaviorBlackboard : System.Object
    {
        public int unitCount;
        public int factoriesCount;
        public int capturePointCount;

        public Vector3 destination = Vector3.zero;
        public TargetBuilding targetResource = null;

        public Factory enemyBase = null;
        public Factory enemyFactory = null;
        public List<Unit> enemyUnits = new List<Unit>();

        public ProductionDirectionType productionDirectionType;

        public List<Factory> damagedFactories = new List<Factory>();

        public int squadFormation = 0;


        public BehaviorBlackboard Clone()
        {
            return (BehaviorBlackboard)this.MemberwiseClone();

        }
    }
}
