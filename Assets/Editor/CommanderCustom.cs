using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RTS
{
    [CustomEditor(typeof(IA_Commander))]
    [CanEditMultipleObjects]
    public class CommanderCustom : Editor
    {
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
