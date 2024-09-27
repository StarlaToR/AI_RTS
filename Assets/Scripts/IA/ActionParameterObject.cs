using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS
{
    public class ActionParameters

    {
        private Dictionary<string, int> intParameters;
        private Dictionary<string, float> floatParameters;

        public ActionParameters(ActionParameterObject parameterObject)
        {
            CreateIntParameterArray(parameterObject.GetSpecificTypeParameters(TypeParameters.INT));
            CreateFloatParameterArray(parameterObject.GetSpecificTypeParameters(TypeParameters.FLOAT));
        }

        // Functions to setup parameters array
        //------------------------------
        public void CreateIntParameterArray(string[] names)
        {
            intParameters = new Dictionary<string, int>();
            for (int i = 0; i < names.Length; i++)
            {
                intParameters.Add(names[i], 0);
            }
        }

        public void CreateFloatParameterArray(string[] names)
        {
            floatParameters = new Dictionary<string, float>();
            for (int i = 0; i < names.Length; i++)
            {
                floatParameters.Add(names[i], 0);
            }
        }
        //---------------------------

        public void SetInt(string name, int value)
        {
            intParameters[name] = value;
        }

        public void SetFloat(string name, float value)
        {
            floatParameters[name] = value;
        }

        public int GetInt(string name)
        {
            if (intParameters.ContainsKey(name))
            {
                return intParameters[name];
            }else
            {
                Debug.LogError($"Didn't find this parameter : {name}");
#if UNITY_EDITOR
                Debug.Break();
#endif
                return -1;
            }
           
        }

        public float GetFloat(string name)
        {
            if (floatParameters.ContainsKey(name))
            {
                return floatParameters[name];
            }
            else
            {
                Debug.LogError($"Didn't find this parameter : {name}");
#if UNITY_EDITOR
                Debug.Break();
#endif
                return -1;
            }

        }
    }

    public enum TypeParameters
    {
        INT,
        FLOAT
    }

    /// Asset pour setup les parametres d'une directons 
    [CreateAssetMenu(fileName= "ActionsParametersMeta",menuName = "Actions/ActionsParametersMeta", order = 0)]
    public class ActionParameterObject : ScriptableObject
    {
        public string[] nameParameter;
        public TypeParameters[] parametersTypes;

        public string[] GetSpecificTypeParameters(TypeParameters type)
        {
            int[] indexArray = new int[nameParameter.Length];
            int currentCount = 0;
            for (int i = 0; i < nameParameter.Length; i++)
            {
                if (parametersTypes[i] == type)
                {
                    indexArray[currentCount] = i;
                    currentCount++;
                }
            }
             Array.Resize<int>(ref indexArray, currentCount);

            string[] sortArray = new string[currentCount];
            for (int i = 0; i < currentCount; i++)
            {
                sortArray[i] = nameParameter[indexArray[i]];
            }
            return sortArray;

        }

    }

}