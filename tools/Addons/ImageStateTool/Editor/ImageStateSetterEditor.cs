#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tools.Img.Editor
{
    using EDGL = EditorGUILayout;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ImageStateSetter))]
    public class ImageStateSetterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EDGL.PropertyField(serializedObject.FindProperty("mainUserTarget"));
            MonoBehaviour mono = (MonoBehaviour)serializedObject.FindProperty("mainUserTarget").objectReferenceValue;
            if (mono != null)
            {
                EDGL.LabelField(mono.GetType().ToString());

                Type monoType = mono.GetType();
                List<FieldInfo> fields = monoType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                fields.RemoveAll(x => Attribute.GetCustomAttribute(x, typeof(ImageStateTag)) is null);

                string[] options = fields.Select(x => x.GetValue(mono).ToString()).ToArray();
                List<string> listOptions = options.ToList();

                SerializedProperty serializedList = serializedObject.FindProperty("states");
                serializedList.arraySize = EDGL.IntField("Elements count", serializedList.arraySize);

                for (int i = 0; i < serializedList.arraySize; i++)
                {
                    EDGL.BeginVertical();
                    EDGL.BeginHorizontal();
                    serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("ignoreSprite").boolValue =
                        EDGL.Toggle(serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("ignoreSprite").boolValue, GUILayout.Width(25));
                    EDGL.PropertyField(serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("sprite"));
                    EDGL.EndHorizontal();

                    EDGL.BeginHorizontal();
                    serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("ignoreColor").boolValue =
                        EDGL.Toggle(serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("ignoreColor").boolValue, GUILayout.Width(25));
                    EDGL.PropertyField(serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("color"));
                    EDGL.EndHorizontal();
                    int selected = listOptions.IndexOf(serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("stateTag").stringValue);
                    if (selected < 0)
                    {
                        selected = 0;
                    }

                    int newSelected = EDGL.Popup("State:", selected, options);
                    serializedList.GetArrayElementAtIndex(i).FindPropertyRelative("stateTag").stringValue = options[newSelected];

                    EDGL.EndVertical();
                    EDGL.Space(10);
                }
            }

            EDGL.PropertyField(serializedObject.FindProperty("image"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif