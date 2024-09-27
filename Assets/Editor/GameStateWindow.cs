using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RTS
{
    public class GameStateWindow : EditorWindow
    {
        public GlobalGameStateParameters globalGameStateParameters;
        private GameStateParameters gameStateEdit;
        private int m_indexPopupGameParameters;
        SerializedObject serializedObject;
        [MenuItem("GameState/Game State Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GameStateWindow), false, "Game State Editor");
        }

        public void OnSelectionChange()
        {
          
            GameStateParameters tree = Selection.activeObject as GameStateParameters;
            if (tree)
            {
                gameStateEdit = tree;
                if (gameStateEdit)  serializedObject = new SerializedObject(gameStateEdit);
            }
            else
            {
                gameStateEdit = null;
                serializedObject = null;
            }


            globalGameStateParameters.UpdateDictionary();
        }
        public void OnFocus()
        {
            if (globalGameStateParameters)
            {
                globalGameStateParameters.UpdateDictionary();
                if(gameStateEdit != null && serializedObject == null) serializedObject = new SerializedObject(gameStateEdit);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                   // OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        public void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;

            EditorGUILayout.LabelField("Edit Game Parameters", style);
            EditorGUILayout.Space(20);
            globalGameStateParameters = (GlobalGameStateParameters)EditorGUILayout.ObjectField("Game Parameter", globalGameStateParameters, typeof(GlobalGameStateParameters), true);

            gameStateEdit = EditorGUILayout.ObjectField("Game state", gameStateEdit, typeof(GameStateParameters), true) as GameStateParameters;

            if (gameStateEdit != null && serializedObject != null && !Application.isPlaying)
            {

                bool hasChange = false;

                EditorUtility.SetDirty(gameStateEdit);

                EditorGUILayout.BeginHorizontal();
                string[] keys = new string[globalGameStateParameters.globalParameters.Keys.Count];
                globalGameStateParameters.globalParameters.Keys.CopyTo(keys, 0);
                m_indexPopupGameParameters = EditorGUILayout.Popup(m_indexPopupGameParameters, keys);

                SerializedProperty keysProperties = serializedObject.FindProperty("_keys");
                 SerializedProperty valuesProperties = serializedObject.FindProperty("_values");
                if (GUILayout.Button("Add"))
                {
                    if (!gameStateEdit.gameParameters.ContainsKey(keys[m_indexPopupGameParameters]))
                    {
                        hasChange = true;
                        keysProperties.InsertArrayElementAtIndex(0);
                        SerializedProperty arrayElementIndex = keysProperties.GetArrayElementAtIndex(keysProperties.arraySize - 1);//(keys[m_indexPopupGameParameters], false);
                        arrayElementIndex.stringValue = keys[m_indexPopupGameParameters];


                        valuesProperties.InsertArrayElementAtIndex(0);
                        arrayElementIndex = valuesProperties.GetArrayElementAtIndex(valuesProperties.arraySize - 1);//(keys[m_indexPopupGameParameters], false);
                        arrayElementIndex.boolValue = false;
                    }
                }
                EditorGUILayout.EndHorizontal();




                for (int i = 0; i < keysProperties.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel(keysProperties.GetArrayElementAtIndex(i).stringValue);
                    valuesProperties.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Toggle(valuesProperties.GetArrayElementAtIndex(i).boolValue);
                    if (GUILayout.Button("-"))
                    {
                        hasChange = true;
                        keysProperties.DeleteArrayElementAtIndex(i);
                        valuesProperties.DeleteArrayElementAtIndex(i);

                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (hasChange)
                {
                    SaveChanges();
                    serializedObject.ApplyModifiedProperties();
                    Debug.Log("Apply Modif");
                    AssetDatabase.SaveAssets();

                }

            }

        }


    }
}