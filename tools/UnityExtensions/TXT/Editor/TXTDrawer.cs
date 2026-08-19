#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    [CustomPropertyDrawer(typeof(TXT))]
    public class TXTDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty textProp = property.FindPropertyRelative("_text");
            SerializedProperty tmpTextProp = property.FindPropertyRelative("_tmpText");

            Rect fieldRect = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            Object draggedObj = EditorGUI.ObjectField(fieldRect, textProp.objectReferenceValue ?? tmpTextProp.objectReferenceValue, typeof(Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                textProp.objectReferenceValue = null;
                tmpTextProp.objectReferenceValue = null;

                if (draggedObj is GameObject go)
                {
                    Text text = go.GetComponent<Text>();
                    TMP_Text tmpText = go.GetComponent<TMP_Text>();

                    if (text != null)
                    {
                        textProp.objectReferenceValue = text;
                    }
                    else if (tmpText != null)
                    {
                        tmpTextProp.objectReferenceValue = tmpText;
                    }
                }
                else if (draggedObj is Text text)
                {
                    textProp.objectReferenceValue = text;
                }
                else if (draggedObj is TMP_Text tmpText)
                {
                    tmpTextProp.objectReferenceValue = tmpText;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif