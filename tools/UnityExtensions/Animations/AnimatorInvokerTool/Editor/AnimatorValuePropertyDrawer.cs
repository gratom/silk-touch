#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.Anim.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorValue))]
    public class AnimatorValuePropertyDrawer : PropertyDrawer
    {
        private static Dictionary<int, Action<Rect, SerializedProperty>> dictionary = new Dictionary<int, Action<Rect, SerializedProperty>>()
        {
            { -1, (rect, property) => { property.FindPropertyRelative("parameter").FindPropertyRelative("parameterType").enumValueIndex = 0; } },
            { 0, FloatAction },
            { 1, IntAction },
            { 2, BoolAction },
            { 3, TriggerAction }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect origin = position;

            Rect animParameterRect = origin;
            SerializedProperty animParameter = property.FindPropertyRelative("parameter");
            EditorGUI.PropertyField(animParameterRect, animParameter);

            Rect tempRect = origin;
            tempRect.height = 18;
            tempRect.yMin += animParameter.FindPropertyRelative("animator").objectReferenceValue == null ? 40 : 20;
            tempRect.yMax += animParameter.FindPropertyRelative("animator").objectReferenceValue == null ? 40 : 20;
            dictionary[animParameter.FindPropertyRelative("parameterType").enumValueIndex](tempRect, property);

            EditorGUI.EndProperty();
        }

        private static void BoolAction(Rect origin, SerializedProperty serializedProperty)
        {
            SerializedProperty property = serializedProperty.FindPropertyRelative("boolValue");
            property.boolValue = EditorGUI.Toggle(origin, "setted value", property.boolValue);
        }

        private static void FloatAction(Rect origin, SerializedProperty serializedProperty)
        {
            SerializedProperty property = serializedProperty.FindPropertyRelative("floatValue");
            property.floatValue = EditorGUI.FloatField(origin, "setted value", property.floatValue);
        }

        private static void IntAction(Rect origin, SerializedProperty serializedProperty)
        {
            SerializedProperty property = serializedProperty.FindPropertyRelative("intValue");
            property.intValue = EditorGUI.IntField(origin, "setted value", property.intValue);
        }

        private static void TriggerAction(Rect origin, SerializedProperty serializedProperty)
        {
        }
    }
}
#endif