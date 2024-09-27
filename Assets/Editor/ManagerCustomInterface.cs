using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RTS
{
    [CustomEditor(typeof(IA_Manager))]
    [CanEditMultipleObjects]
    public class ManagerCustomInterface : Editor
    {
        public void OnEnable()
        {
            if (Application.isPlaying)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}


