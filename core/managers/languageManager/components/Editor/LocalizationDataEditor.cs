#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Localization
{
    using Tools;
    
    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : Editor
    {
        private SerializedProperty allLanguages;
        private SerializedProperty containers;
        private SerializedProperty path;
        private readonly GUILayoutOption checkBoxStyle = GUILayout.Width(20);

        private Vector2 containersScrollPosition;

        private void OnEnable()
        {
            allLanguages = serializedObject.FindProperty("allLanguages");
            containers = serializedObject.FindProperty("containers");
            path = serializedObject.FindProperty("GOOGLE_DOC_ID");
        }

        private void InitLanguages()
        {
            bool[] checkArray = new bool [Enum.GetNames(typeof(GT.Language)).Length];
            for (int i = 0; i < allLanguages.arraySize; i++)
            {
                SerializedProperty languageContainer = allLanguages.GetArrayElementAtIndex(i);
                SerializedProperty language = languageContainer.FindPropertyRelative("language");
                int index = language.enumValueIndex;
                if (checkArray[index])
                {
                    allLanguages.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }
                checkArray[index] = true;
            }

            for (int i = 0; i < checkArray.Length; i++)
            {
                if (!checkArray[i])
                {
                    allLanguages.InsertArrayElementAtIndex(allLanguages.arraySize);
                    SerializedProperty languageContainer = allLanguages.GetArrayElementAtIndex(allLanguages.arraySize - 1);
                    SerializedProperty language = languageContainer.FindPropertyRelative("language");
                    language.enumValueIndex = i;
                }
            }
            allLanguages.GetArrayElementAtIndex(0).FindPropertyRelative("isActive").boolValue = true;
        }

        private void OpenGoogleSheetInBrowser(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Debug.LogError("LocalizationData: GOOGLE_DOC_ID is empty!");
                return;
            }

            // Формируем прямую ссылку на конкретный лист (gid) таблицы
            string url = $"https://docs.google.com/spreadsheets/d/{s}/edit#gid={0}";

            // Открываем в браузере по умолчанию
            Application.OpenURL(url);
            Debug.Log($"Opening Google Sheet: {url}");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InitLanguages();

            EditorGUILayout.PropertyField(path);
            if (GUILayout.Button("open sheet"))
            {
                OpenGoogleSheetInBrowser(path.stringValue);
            }

            EditorGUILayout.LabelField("All Languages", EditorStyles.boldLabel);
            if (GUILayout.Button(allLanguages.isExpanded ? "hide" : "show"))
            {
                allLanguages.isExpanded = !allLanguages.isExpanded;
                if (allLanguages.isExpanded && containers.isExpanded)
                {
                    containers.isExpanded = false;
                }
            }

            #region languages

            if (allLanguages.isExpanded)
            {

                EditorGUI.indentLevel++;
                for (int i = 0; i < allLanguages.arraySize; i++)
                {
                    SerializedProperty languageContainer = allLanguages.GetArrayElementAtIndex(i);
                    SerializedProperty isActive = languageContainer.FindPropertyRelative("isActive");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(((GT.Language)languageContainer.FindPropertyRelative("language").enumValueIndex).ToString());
                    if (i == 0)
                    {
                        GUI.enabled = false; // Lock the toggle
                        EditorGUILayout.Toggle("", true, checkBoxStyle);
                        GUI.enabled = true;
                        EditorGUILayout.LabelField("default");
                    }
                    else
                    {
                        bool b = EditorGUILayout.Toggle("", isActive.boolValue, checkBoxStyle);
                        if (isActive.boolValue != b)
                        {
                            isActive.boolValue = b;
                            RefreshLanguages();
                        }

                        EditorGUILayout.LabelField(isActive.boolValue ? "on" : "off");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            #endregion

            #region translated containers

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Content containers", EditorStyles.boldLabel);

            if (GUILayout.Button(containers.isExpanded ? "hide" : "show"))
            {
                containers.isExpanded = !containers.isExpanded;
                if (allLanguages.isExpanded && containers.isExpanded)
                {
                    allLanguages.isExpanded = false;
                }
            }

            if (containers.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Переменная для хранения позиции скролла должна быть объявлена на уровне класса:
                // private Vector2 containersScrollPosition;
                containersScrollPosition = EditorGUILayout.BeginScrollView(containersScrollPosition, GUILayout.Height(400)); // Фиксированная высота окна

                float itemHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                int totalItems = containers.arraySize;

                // Резервируем общее пространство под весь список, чтобы ползунок скролла работал корректно
                Rect totalRect = EditorGUILayout.GetControlRect(false, totalItems * itemHeight);

                // Вычисляем, какие элементы видны в данный момент
                int firstVisibleIdx = Mathf.FloorToInt(containersScrollPosition.y / itemHeight);
                int lastVisibleIdx = Mathf.Min(totalItems - 1, Mathf.CeilToInt((containersScrollPosition.y + 400) / itemHeight));

                for (int i = firstVisibleIdx; i <= lastVisibleIdx; i++)
                {
                    // Вычисляем позицию конкретного элемента (с запасом под скроллбар)
                    Rect itemRect = new Rect(totalRect.x, totalRect.y + i * itemHeight, totalRect.width - 16f, itemHeight);

                    SerializedProperty container = containers.GetArrayElementAtIndex(i);

// Вместо BeginArea и Horizontal разбиваем Rect напрямую:
                    Rect buttonRect = new Rect(itemRect.x, itemRect.y, 20f, itemHeight);
                    Rect textRect = new Rect(itemRect.x + 25f, itemRect.y, itemRect.width - 25f, itemHeight);

// 1. Отрисовка кнопки
                    if (GUI.Button(buttonRect, "x"))
                    {
                        LocalizationData localizationData = (LocalizationData)serializedObject.targetObject;
                        localizationData.DeleteElement(i);
                        break;
                    }

// 2. Логика строки
                    string str = container.FindPropertyRelative("languageContainers").GetArrayElementAtIndex(0).FindPropertyRelative("text").stringValue;
                    int nextLineIndex = str.IndexOf('\n');
                    if (nextLineIndex != -1)
                    {
                        str = str.Substring(0, nextLineIndex);
                    }

// 3. Отрисовка текста (GUI.Label займет ВСЮ ширину textRect без обрезания пополам)
                    GUI.Label(textRect, i + " " + str);
                }

                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }

            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshLanguages()
        {
            HashSet<int> enumValues = new HashSet<int>();
            for (int i = 0; i < allLanguages.arraySize; i++)
            {
                SerializedProperty languageContainer = allLanguages.GetArrayElementAtIndex(i);
                if (languageContainer.FindPropertyRelative("isActive").boolValue)
                {
                    enumValues.Add(languageContainer.FindPropertyRelative("language").enumValueIndex);
                }
            }

            for (int i = 0; i < containers.arraySize; i++)
            {
                SerializedProperty container = containers.GetArrayElementAtIndex(i);
                HashSet<int> containerEnumValues = new HashSet<int>();
                SerializedProperty innerContainer = container.FindPropertyRelative("languageContainers");

                for (int j = 0; j < innerContainer.arraySize; j++)
                {
                    if (!enumValues.Contains(innerContainer.GetArrayElementAtIndex(j).FindPropertyRelative("language").enumValueIndex))
                    {
                        //remove
                        innerContainer.DeleteArrayElementAtIndex(j);
                        j--;
                    }
                    else
                    {
                        containerEnumValues.Add(innerContainer.GetArrayElementAtIndex(j).FindPropertyRelative("language").enumValueIndex);
                    }
                }

                foreach (int value in enumValues)
                {
                    if (!containerEnumValues.Contains(value))
                    {
                        //add
                        innerContainer.InsertArrayElementAtIndex(innerContainer.arraySize);
                        innerContainer.GetArrayElementAtIndex(innerContainer.arraySize - 1).FindPropertyRelative("language").enumValueIndex = value;
                        innerContainer.GetArrayElementAtIndex(innerContainer.arraySize - 1).FindPropertyRelative("text").stringValue =
                            GT.Translate(innerContainer.GetArrayElementAtIndex(0).FindPropertyRelative("text").stringValue,
                                (GT.Language)innerContainer.GetArrayElementAtIndex(0).FindPropertyRelative("language").enumValueIndex,
                                (GT.Language)innerContainer.GetArrayElementAtIndex(innerContainer.arraySize - 1).FindPropertyRelative("language").enumValueIndex);
                    }
                }
            }
        }
    }
}

#endif