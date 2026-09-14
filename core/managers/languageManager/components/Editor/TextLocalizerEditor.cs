#if UNITY_EDITOR && UI_TMP

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SilkTouch.Localization
{
    using Tools;

    [CustomEditor(typeof(TextLocalizer))]
    public class TextLocalizerEditor : Editor
    {
        private static GUIStyle errorStyle;
        private static GUIStyle ErrorStyle
        {
            get
            {
                if (errorStyle == null && EditorStyles.label != null)
                {
                    errorStyle = new GUIStyle(EditorStyles.label);
                }
                return errorStyle;
            }
        }
        private static LocalizationData data;

        private SerializedProperty key;
        private SerializedProperty textProperty;

        // Безопасная проверка: привязаны ли компоненты внутри TXT
        private bool IsTextObjectAssigned
        {
            get
            {
                if (textProperty == null)
                {
                    return false;
                }

                SerializedProperty textCompProp = textProperty.FindPropertyRelative("_text");
                SerializedProperty tmpCompProp = textProperty.FindPropertyRelative("_tmpText");

                bool hasText = textCompProp != null && textCompProp.objectReferenceValue != null;
                bool hasTmp = tmpCompProp != null && tmpCompProp.objectReferenceValue != null;

                return hasText || hasTmp;
            }
        }

        private string textValue
        {
            get
            {
                if (textProperty != null)
                {
                    SerializedProperty textCompProp = textProperty.FindPropertyRelative("_text");
                    SerializedProperty tmpCompProp = textProperty.FindPropertyRelative("_tmpText");

                    if (textCompProp != null && textCompProp.objectReferenceValue is Text textComponent)
                    {
                        return textComponent.text;
                    }

                    if (tmpCompProp != null && tmpCompProp.objectReferenceValue is TMP_Text tmpComponent)
                    {
                        return tmpComponent.text;
                    }
                }
                return string.Empty;
            }
        }

        private void OnEnable()
        {
            ErrorStyle.normal.textColor = Color.red;
            textProperty = serializedObject.FindProperty("text");
            key = serializedObject.FindProperty("key");
        }

        private string[] keys;

        public override void OnInspectorGUI()
        {
            TryFindData();

            if (keys == null || keys.Length < 1)
            {
                if (data != null)
                {
                    keys = data.dictionaryContainers.Keys.ToArray();
                }
            }

            if (keys?.Length < 1)
            {
                return;
            }

            serializedObject.Update();

            if (GUILayout.Button(key.stringValue))
            {
                ScriptablePopupWindow popup = CreateInstance<ScriptablePopupWindow>();
                popup.Init(keys, (x) =>
                {
                    key.stringValue = keys[x];
                    serializedObject.ApplyModifiedProperties();
                });
                popup.ShowAsDropDown(
                    new Rect(UniversalMousePosition.GetCursorPosition().x, UniversalMousePosition.GetCursorPosition().y,
                        1, 1), new Vector2(300, 400));
            }

            EditorGUILayout.PropertyField(textProperty, new GUIContent("TXT Wrapper"));

            if (!IsTextObjectAssigned)
            {
                if (target is Component component)
                {
                    Text textComp = component.GetComponent<Text>();
                    TMP_Text tmpComp = component.GetComponent<TMP_Text>();

                    if (textComp != null || tmpComp != null)
                    {
                        SerializedProperty textCompProp = textProperty.FindPropertyRelative("_text");
                        SerializedProperty tmpCompProp = textProperty.FindPropertyRelative("_tmpText");

                        if (textCompProp != null)
                        {
                            textCompProp.objectReferenceValue = textComp;
                        }
                        if (tmpCompProp != null)
                        {
                            tmpCompProp.objectReferenceValue = tmpComp;
                        }

                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            // Повторная проверка на случай, если автоматический поиск ничего не дал
            if (!IsTextObjectAssigned)
            {
                EditorGUILayout.HelpBox("TXT component references are missing. Please assign a Text or TMP_Text component, or ensure OnValidate initialized it.", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (data != null)
            {
                if (string.IsNullOrEmpty(key.stringValue))
                {
                    //key.stringValue = data.GetNewValueKey(textValue);
                }
                if (!string.IsNullOrEmpty(key.stringValue) && !string.IsNullOrEmpty(textValue) && data.IsContain(key.stringValue) && textValue != data.GetTextByIDDef(key.stringValue))
                {
                    // if (GUILayout.Button("update"))
                    // {
                    //     data.UpdateValue(key.stringValue, textValue);
                    // }
                }
                if (!string.IsNullOrEmpty(key.stringValue) && data.IsContain(key.stringValue))
                {
                    IEnumerable<string> listAll = data.GetAllValues(key.stringValue);
                    foreach (string s in listAll)
                    {
                        if (GUILayout.Button(s.FirstRowTruncated()))
                        {
                            SerializedProperty textCompProp = textProperty.FindPropertyRelative("_text");
                            SerializedProperty tmpCompProp = textProperty.FindPropertyRelative("_tmpText");

                            if (textCompProp != null && textCompProp.objectReferenceValue is Text textComponent)
                            {
                                Undo.RecordObject(textComponent, "Test Localization Value");
                                textComponent.text = s.Substring(5);
                                EditorUtility.SetDirty(textComponent);
                            }
                            else if (tmpCompProp != null && tmpCompProp.objectReferenceValue is TMP_Text tmpComponent)
                            {
                                Undo.RecordObject(tmpComponent, "Test Localization Value");
                                tmpComponent.text = s.Substring(5);
                                EditorUtility.SetDirty(tmpComponent);
                            }
                        }
                    }
                }
                if (!data.IsContain(key.stringValue))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("error : missing key", ErrorStyle);
                    if (GUILayout.Button("regenerate"))
                    {
                        key.stringValue = data.GetNewValueKey(textValue);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("error : localization data not found", ErrorStyle);
                if (GUILayout.Button("generate"))
                {
                    LocalizationData.CreateLocalizationData();
                }
            }

            serializedObject.ApplyModifiedProperties();
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
            if (data?.dictionaryContainers == null)
            {
                data?.InitDictionary();
            }
        }
    }
}
#endif