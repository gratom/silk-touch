#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SilkTouch.Sounds
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : Editor
    {
        private SerializedProperty soundSettingsProp;

        private void OnEnable()
        {
            soundSettingsProp = serializedObject.FindProperty("soundSettings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Sound Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Sound Setting"))
            {
                soundSettingsProp.arraySize++;
            }

            for (int i = 0; i < soundSettingsProp.arraySize; i++)
            {
                SerializedProperty settingProp = soundSettingsProp.GetArrayElementAtIndex(i);
                SerializedProperty clipProp = settingProp.FindPropertyRelative("clip");
                SerializedProperty typeProp = settingProp.FindPropertyRelative("type");
                SerializedProperty volumeProp = settingProp.FindPropertyRelative("volume");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(clipProp, GUIContent.none, GUILayout.MinWidth(150));
                EditorGUILayout.PropertyField(typeProp, GUIContent.none, GUILayout.Width(150));
                volumeProp.floatValue = EditorGUILayout.Slider(volumeProp.floatValue, 0f, 1f);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    soundSettingsProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif