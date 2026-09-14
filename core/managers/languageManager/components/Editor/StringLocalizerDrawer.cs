#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;

    [CustomPropertyDrawer(typeof(StringLocalizer))]
    public class StringLocalizerDrawer : PropertyDrawer
    {
        private static GUIStyle errorStyle;
        private static GUIStyle ErrorStyle
        {
            get
            {
                if (errorStyle == null && EditorStyles.label != null)
                {
                    errorStyle = new GUIStyle(EditorStyles.label);
                    errorStyle.normal.textColor = Color.red;
                }
                return errorStyle;
            }
        }

        private static LocalizationData data;
        private string[] keys;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TryFindData();

            SerializedProperty keyProp = property.FindPropertyRelative("key");

            if (data == null || !data.IsContain(keyProp.stringValue))
            {
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            }

            IEnumerable<string> listAll = data.GetAllValues(keyProp.stringValue).Skip(1);
            int extraLines = listAll.Count();

            return EditorGUIUtility.singleLineHeight * (1 + extraLines) + EditorGUIUtility.standardVerticalSpacing * extraLines;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keyProp = property.FindPropertyRelative("key");

            EditorGUI.BeginProperty(position, label, property);

            if (keys == null || keys.Length < 1)
            {
                if (data != null)
                {
                    keys = data.dictionaryContainers.Keys.ToArray();
                }
            }

            Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (data != null)
            {
                string buttonText = string.IsNullOrEmpty(keyProp.stringValue) ? "Select Localization Key..." : keyProp.stringValue;

                Rect labelRect = new Rect(currentRect.x, currentRect.y, EditorGUIUtility.labelWidth, currentRect.height);
                Rect buttonRect = new Rect(currentRect.x + EditorGUIUtility.labelWidth, currentRect.y, currentRect.width - EditorGUIUtility.labelWidth, currentRect.height);

                EditorGUI.LabelField(labelRect, label);

                if (GUI.Button(buttonRect, buttonText, EditorStyles.popup))
                {
                    if (keys != null && keys.Length > 0)
                    {
                        ScriptablePopupWindow popup = ScriptableObject.CreateInstance<ScriptablePopupWindow>();
                        popup.Init(keys, (index) =>
                        {
                            keyProp.stringValue = keys[index];
                            property.serializedObject.ApplyModifiedProperties();
                        });

                        popup.ShowAsDropDown(new Rect(UniversalMousePosition.GetCursorPosition().x, UniversalMousePosition.GetCursorPosition().y, 1, 1), new Vector2(300, 400));
                    }
                }

                currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (!data.IsContain(keyProp.stringValue) && !string.IsNullOrEmpty(keyProp.stringValue))
                {
                    EditorGUI.LabelField(currentRect, "error : missing key", ErrorStyle);
                }
                else if (data.IsContain(keyProp.stringValue))
                {
                    IEnumerable<string> listAll = data.GetAllValues(keyProp.stringValue).Skip(1);
                    foreach (string s in listAll)
                    {
                        EditorGUI.LabelField(currentRect, s.FirstRowTruncated(), EditorStyles.miniLabel);
                        currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            else
            {
                Rect labelRect = new Rect(currentRect.x, currentRect.y, currentRect.width - 90, currentRect.height);
                Rect btnRect = new Rect(currentRect.x + currentRect.width - 85, currentRect.y, 85, currentRect.height);

                EditorGUI.LabelField(labelRect, "error : localization data not found", ErrorStyle);
                if (GUI.Button(btnRect, "generate"))
                {
                    LocalizationData.CreateLocalizationData();
                }
            }

            EditorGUI.EndProperty();
        }

        private static void TryFindData()
        {
            const string filter = "t:LocalizationData";

            if (data == null)
            {
                string[] guid = AssetDatabase.FindAssets(filter);
                if (guid != null && guid.Length > 0)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid[0]);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        data = AssetDatabase.LoadAssetAtPath<LocalizationData>(assetPath);
                        data?.InitDictionary();
                    }
                }
            }
        }
    }
}
#endif