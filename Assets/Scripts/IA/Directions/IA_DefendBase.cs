using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "Dir_DefendBase", menuName = "Directions/DefendBase", order = 0)]
    public class IA_DefendBase : IA_Direction
    {
        public override IA_Direction Clone()
        {
            IA_DefendBase createArmy = new IA_DefendBase();
            createArmy.parameterObject = parameterObject;
            createArmy.behaviors = behaviors.ConvertAll<IA_Behavior>(c => { return c.Clone(); });
            createArmy.Init();
            return base.Clone();
        }
    }
}