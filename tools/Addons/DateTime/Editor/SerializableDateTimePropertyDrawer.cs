#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer
    {
        private const float LabelWidth = 14f;
        private const float Spacing = 4f;
        private const float FieldOffset = LabelWidth + Spacing;
        private const float MinWidthForSummary = 350f;
        private const float SmallFieldWidth = 26f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (position.width >= MinWidthForSummary)
            {
                SerializedProperty ticksProp = property.FindPropertyRelative("ticks");
                DateTime dt = new DateTime(ticksProp.longValue);
                string summary = $" ({dt:yyyy.MM.dd:HH:mm:ss})";
                label.text = label.text + summary;
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float remainingWidth = position.width - (FieldOffset * 6) - (SmallFieldWidth * 5);
            float yearFieldWidth = Mathf.Max(SmallFieldWidth, remainingWidth);

            SerializedProperty sValueProperty = property.FindPropertyRelative("ticks");
            DateTime dateTime = new DateTime(sValueProperty.longValue);

            EditorGUI.BeginChangeCheck();

            int year = DrawField(ref position, "Y", dateTime.Year, yearFieldWidth);
            int month = DrawField(ref position, "M", dateTime.Month, SmallFieldWidth);
            int day = DrawField(ref position, "D", dateTime.Day, SmallFieldWidth);
            int hour = DrawField(ref position, "H", dateTime.Hour, SmallFieldWidth);
            int minute = DrawField(ref position, "m", dateTime.Minute, SmallFieldWidth);
            int second = DrawField(ref position, "s", dateTime.Second, SmallFieldWidth);

            if (EditorGUI.EndChangeCheck())
            {
                year = Mathf.Clamp(year, 1, 9999);
                month = Mathf.Clamp(month, 1, 12);
                day = Mathf.Clamp(day, 1, DateTime.DaysInMonth(year, month));
                hour = Mathf.Clamp(hour, 0, 23);
                minute = Mathf.Clamp(minute, 0, 59);
                second = Mathf.Clamp(second, 0, 59);

                sValueProperty.longValue = new DateTime(year, month, day, hour, minute, second).Ticks;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private int DrawField(ref Rect position, string label, int value, float fieldWidth)
        {
            Rect labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
            GUI.Label(labelRect, label);
            
            Rect fieldRect = new Rect(position.x + LabelWidth, position.y, fieldWidth, position.height);
            int result = EditorGUI.IntField(fieldRect, value);
            
            position.x += LabelWidth + fieldWidth + Spacing;
            
            return result;
        }
    }
}
#endif