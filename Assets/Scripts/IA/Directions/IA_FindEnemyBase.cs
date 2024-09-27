using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "Dir_FindEnemyBase", menuName = "Directions/FindEnemyBase", order = 0)]
    public class IA_FindEnemyBase : IA_Direction
    {
        public override IA_Direction Clone()
        {
            IA_FindEnemyBase createArmy = new IA_FindEnemyBase();
            createArmy.parameterObject = parameterObject;
            createArmy.behaviors = behaviors.ConvertAll<IA_Behavior>(c => { return c.Clone(); });
            createArmy.Init();
            return base.Clone();
        }
    }
}