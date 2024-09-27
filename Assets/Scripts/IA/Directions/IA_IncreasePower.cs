using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "Dir_IncreasePower", menuName = "Directions/IncreasePower", order = 0)]
    public class IA_IncreasePower : IA_Direction
    {
        public override IA_Direction Clone()
        {
            IA_IncreasePower createArmy = new IA_IncreasePower();
            createArmy.parameterObject = parameterObject;
            createArmy.behaviors = behaviors.ConvertAll<IA_Behavior>(c => { return c.Clone(); });
            createArmy.Init();
            return base.Clone();
        }
    }
}
