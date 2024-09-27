using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RTS
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Global Game Parameters",menuName ="Parameters/GlobalParameters")]
    public class GlobalGameStateParameters :  ScriptableObject
    {
        public Dictionary<string, bool> globalParameters = new Dictionary<string, bool>();
        [SerializeField] List<string> parametersNames;

        public void UpdateDictionary()
        {
            for (int i = 0; i < parametersNames.Count; i++)
            {
                if(!globalParameters.ContainsKey(parametersNames[i]))
                {
                    globalParameters.Add(parametersNames[i], false);
                }
            }

             string[] namesArray = new string[globalParameters.Keys.Count];
            globalParameters.Keys.CopyTo(namesArray, 0);
            for (int i = 0; i < namesArray.Length; i++)
            {

                if(!parametersNames.Contains(namesArray[i]))
                {
                    globalParameters.Remove(namesArray[i]);
                }
            }

            
        }
    }
}
