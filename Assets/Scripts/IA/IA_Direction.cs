using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public class IA_Direction : ScriptableObject
    {
        public List<IA_Behavior> behaviors = new List<IA_Behavior>();

        public ActionParameterObject parameterObject;
        public ActionParameters parameters;

        public void Init()
        {
            parameters = new ActionParameters(parameterObject);
        }

        public virtual IA_Direction Clone()
        {
            return this;
        }

    }
}
