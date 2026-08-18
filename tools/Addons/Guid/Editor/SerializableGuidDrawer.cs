#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        private float ySep = 20;
        private float buttonSize;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty guidStringValue = property.FindPropertyRelative("stringGuid");

            position = EditorGUI.PrefixLabel(new Rect(position.x, position.y + ySep / 2, position.width, position.height), GUIUtility.GetControlID(FocusType.Passive), label);
            position.y -= ySep / 2;
            buttonSize = position.width / 3;

            if (GUI.Button(new Rect(position.xMin, position.yMin, buttonSize, ySep - 2), "New"))
            {
                SerializableGuid sGuid = new SerializableGuid(Guid.NewGuid());
                guidStringValue.stringValue = sGuid.ToString();
            }

            if (GUI.Button(new Rect(position.xMin + buttonSize, position.yMin, buttonSize, ySep - 2), "Copy"))
            {
                EditorGUIUtility.systemCopyBuffer = guidStringValue.stringValue;
            }

            if (GUI.Button(new Rect(position.xMin + buttonSize * 2, position.yMin, buttonSize, ySep - 2), "Empty"))
            {
                guidStringValue.stringValue = SerializableGuid.Empty.ToString();
            }

            Rect pos = new Rect(position.xMin, position.yMin + ySep, position.width, ySep - 2);

            guidStringValue.stringValue = EditorGUI.TextField(pos, guidStringValue.stringValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ySep * 2;
        }
    }
}

#endif