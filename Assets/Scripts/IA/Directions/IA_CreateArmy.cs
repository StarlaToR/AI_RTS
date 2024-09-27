using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    [CreateAssetMenu(fileName = "Dir_Create_Army", menuName = "Directions/Create Army", order = 0)]
    public class IA_CreateArmy : IA_Direction
    {
        public override IA_Direction Clone()
        {
            IA_CreateArmy createArmy = CreateInstance<IA_CreateArmy>();
            createArmy.parameterObject = parameterObject;
            createArmy.behaviors = behaviors.ConvertAll<IA_Behavior>(c => { return c.Clone(); });
            createArmy.Init();
            return createArmy;
        }
    }
}