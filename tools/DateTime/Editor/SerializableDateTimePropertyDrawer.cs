#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            Rect yearRect = new Rect(position.x, position.y, 40, position.height);
            Rect monthRect = new Rect(position.x + 45, position.y, 30, position.height);
            Rect dayRect = new Rect(position.x + 80, position.y, 30, position.height);
            Rect hourRect = new Rect(position.x + 115, position.y, 30, position.height);
            Rect minuteRect = new Rect(position.x + 150, position.y, 30, position.height);
            Rect secondRect = new Rect(position.x + 185, position.y, 30, position.height);

            // Get the SDateTime object
            SerializedProperty sValueProperty = property.FindPropertyRelative("ticks");
            long sValue = sValueProperty.longValue;
            DateTime dateTime = new DateTime(sValue);
            EditorGUI.BeginChangeCheck();

            // Draw fields - pass GUIContent.none to each so they don't have labels
            int year = EditorGUI.IntField(yearRect, dateTime.Year);
            int month = EditorGUI.IntField(monthRect, dateTime.Month);
            int day = EditorGUI.IntField(dayRect, dateTime.Day);
            int hour = EditorGUI.IntField(hourRect, dateTime.Hour);
            int minute = EditorGUI.IntField(minuteRect, dateTime.Minute);
            int second = EditorGUI.IntField(secondRect, dateTime.Second);

            if (EditorGUI.EndChangeCheck())
            {
                // Мягкая валидация диапазонов перед созданием DateTime
                year = Mathf.Clamp(year, 1, 9999);
                month = Mathf.Clamp(month, 1, 12);
                day = Mathf.Clamp(day, 1, DateTime.DaysInMonth(year, month));
                hour = Mathf.Clamp(hour, 0, 23);
                minute = Mathf.Clamp(minute, 0, 59);
                second = Mathf.Clamp(second, 0, 59);

                sValueProperty.longValue = new DateTime(year, month, day, hour, minute, second).Ticks;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif