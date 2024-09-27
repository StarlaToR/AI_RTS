using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "Dir_Attack", menuName = "Directions/Attack", order = 0)]
    public class IA_Attack : IA_Direction
    {
        public override IA_Direction Clone()
        {
            IA_Attack createArmy = new IA_Attack();
            createArmy.parameterObject = parameterObject;
            createArmy.behaviors = behaviors.ConvertAll<IA_Behavior>(c => { return c.Clone(); });
            createArmy.Init();
            return base.Clone();
        }
    }
}
