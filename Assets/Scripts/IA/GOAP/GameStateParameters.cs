using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RTS
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GS_GameState", menuName = "Parameters/GameParameters")]
    public class GameStateParameters : ScriptableObject ,ISerializationCallbackReceiver
    {
        public List<string> _keys = new List<string>();
        public List<bool> _values = new List<bool>();
        [SerializeField] public Dictionary<string, bool> gameParameters = new Dictionary<string, bool>();
        private bool isPlaying = false;

     

        public void OnAfterDeserialize()
        {
            if (isPlaying) return;
            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            {
                if (!gameParameters.ContainsKey(_keys[i])) gameParameters.Add(_keys[i], _values[i]);
            }

            string[] namesArray = new string[gameParameters.Keys.Count];
            gameParameters.Keys.CopyTo(namesArray, 0);
            for (int i = 0; i < namesArray.Length; i++)
            {
                if (!_keys.Contains(namesArray[i]))
                {
                    gameParameters.Remove(namesArray[i]);
                }
            }
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in gameParameters)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnBeforeSerialize()
        {
            if (isPlaying) return;
            gameParameters = new Dictionary<string, bool>();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
                gameParameters.Add(_keys[i], _values[i]);

        }

        void OnGUI()
        {
            if (Application.isPlaying) return; ;
            foreach (var kvp in gameParameters)
                GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
        }


    }
}
