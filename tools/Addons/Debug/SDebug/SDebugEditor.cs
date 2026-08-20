#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Tools;

public class SDebugEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private Vector2 detailScroll;
    private SDebugObject selectedObject;

    private string selectedTag = "def";
    private string[] availableTags = new string[0];
    private int selectedTagIndex = 0;

    private float itemHeight = 22f;
    private float detailPanelHeight = 150f;
    private bool isResizing;

    [MenuItem("Tools/Editors/SDebug", false, 1000)]
    public static void ShowWindow()
    {
        GetWindow<SDebugEditor>("SDebug");
    }

    private void OnGUI()
    {
        UpdateTags();
        DrawToolbar();

        // Сначала обрабатываем ресайз, чтобыRect-ы ниже получили актуальную высоту
        HandleResizing();

        float listHeight = position.height - detailPanelHeight - 25;
        Rect listRect = new Rect(0, 20, position.width, listHeight);
        DrawDebugList(listRect);

        Rect detailRect = new Rect(0, position.height - detailPanelHeight, position.width, detailPanelHeight);
        DrawDetailPanel(detailRect);
    }

    private GUIStyle listStyle;
    private GUIStyle detailStyle;

    private void InitStyles()
    {
        Color col = new Color(0.8f, 0.8f, 0.8f);

        listStyle = new GUIStyle(EditorStyles.label);
        listStyle.richText = true;
        listStyle.normal.textColor = col;

        detailStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
        detailStyle.richText = true;
        detailStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
    }

    private void HandleResizing()
    {
        // Создаем невидимую область захвата шириной в 5 пикселей на границе панелей
        Rect resizerRect = new Rect(0, position.height - detailPanelHeight - 5, position.width, 5);

        // Меняем курсор при наведении
        EditorGUIUtility.AddCursorRect(resizerRect, MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.MouseDown && resizerRect.Contains(Event.current.mousePosition))
        {
            isResizing = true;
        }

        if (isResizing)
        {
            // Вычисляем новую высоту (инвертируем, так как панель прижата к низу)
            detailPanelHeight = position.height - Event.current.mousePosition.y;

            // Ограничиваем высоту, чтобы окно не ломалось
            detailPanelHeight = Mathf.Clamp(detailPanelHeight, 50f, position.height - 100f);

            // Принудительно перерисовываем окно
            Repaint();
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isResizing = false;
        }
    }

    private void UpdateTags()
    {
        // Обновляем список тегов только если их количество изменилось
        if (availableTags.Length != SDebug.DebugObjects.Count)
        {
            availableTags = SDebug.DebugObjects.Keys.ToArray();

            // Пытаемся сохранить индекс выбранного тега при обновлении списка
            selectedTagIndex = -1;
            for (int i = 0; i < availableTags.Length; i++)
            {
                if (availableTags[i] == selectedTag)
                {
                    selectedTagIndex = i;
                    break;
                }
            }

            // Если старый тег исчез (Clear), сбрасываем на первый доступный
            if (selectedTagIndex == -1 && availableTags.Length > 0)
            {
                selectedTagIndex = 0;
                selectedTag = availableTags[0];
            }
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Clear All", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            SDebug.Clear();
            selectedObject = null;
            availableTags = new string[0];
        }

        GUILayout.Space(10);

        // Выпадающий список тегов
        if (availableTags.Length > 0)
        {
            int newIndex = EditorGUILayout.Popup(selectedTagIndex, availableTags, EditorStyles.toolbarPopup, GUILayout.Width(150));
            if (newIndex != selectedTagIndex)
            {
                selectedTagIndex = newIndex;
                selectedTag = availableTags[selectedTagIndex];
                selectedObject = null; // Сбрасываем выбор при смене тега
                scrollPosition = Vector2.zero;
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Tags", EditorStyles.miniLabel, GUILayout.Width(150));
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDebugList(Rect area)
    {
        if (string.IsNullOrEmpty(selectedTag) || !SDebug.DebugObjects.ContainsKey(selectedTag))
        {
            GUI.Label(area, "No data for selected tag...", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        List<SDebugObject> targetList = SDebug.DebugObjects[selectedTag];
        int totalCount = targetList.Count;
        float totalHeight = totalCount * itemHeight;

        scrollPosition = GUI.BeginScrollView(area, scrollPosition, new Rect(0, 0, area.width - 20, totalHeight));

        int firstVisible = (int)(scrollPosition.y / itemHeight);
        int visibleCount = (int)(area.height / itemHeight) + 2;
        InitStyles();
        for (int i = firstVisible; i < firstVisible + visibleCount; i++)
        {
            if (i < 0 || i >= totalCount)
            {
                continue;
            }
            Rect rect = new Rect(0, i * itemHeight, area.width, itemHeight);
            DrawItem(rect, targetList[i], i);
        }

        GUI.EndScrollView();
    }

    private void DrawItem(Rect rect, SDebugObject obj, int index)
    {
        if (selectedObject == obj)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.4f, 0.6f, 0.5f));
        }
        else if (index % 2 == 0)
        {
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.05f));
        }

        string timeStr = obj.time.ToString("HH:mm:ss");
        int newlineIndex = obj.stringData.IndexOf('\n');
        string firstLine = newlineIndex > 0 ? obj.stringData.Substring(0, newlineIndex) : obj.stringData;

        string label = $"[{timeStr}] {firstLine}";

        if (GUI.Button(rect, label, listStyle))
        {
            selectedObject = obj;
            detailScroll = Vector2.zero;
        }
    }

    private void DrawDetailPanel(Rect area)
    {
        InitStyles();
        EditorGUI.DrawRect(area, new Color(0.12f, 0.12f, 0.12f, 1f));
        EditorGUI.DrawRect(new Rect(area.x, area.y, area.width, 1), new Color(0.3f, 0.3f, 0.3f, 1f));

        GUILayout.BeginArea(new Rect(area.x + 5, area.y + 5, area.width - 10, area.height - 10));

        if (selectedObject != null)
        {
            EditorGUILayout.LabelField($"Full Log [{selectedObject.time:HH:mm:ss.fff}]", EditorStyles.boldLabel);
            detailScroll = EditorGUILayout.BeginScrollView(detailScroll);

            float contentHeight = detailStyle.CalcHeight(new GUIContent(selectedObject.stringData), area.width - 25);
            EditorGUILayout.SelectableLabel(selectedObject.stringData, detailStyle, GUILayout.Height(contentHeight));

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Select an item to see details", EditorStyles.centeredGreyMiniLabel);
        }

        GUILayout.EndArea();
    }

    private void OnInspectorUpdate()
    {
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}

#endif