#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Tools
{
    using UMP = UniversalMousePosition;

    public class ScriptFinderTool : EditorWindow
    {
        private const int WIDTH = 800;
        private const int HEIGHT = 500;

        private static List<ParsedClassData> cachedClasses = new List<ParsedClassData>();

        private string search = "";
        private SearchField searchField;
        private Vector2 scrollPosition = Vector2.zero;
        private List<int> filteredIndices = new List<int>();

        private GUIStyle buttonStyle;
        private Texture2D windowBgTexture;
        private Texture2D buttonNormalTexture;
        private Texture2D buttonHoverTexture;
        private Texture2D buttonActiveTexture;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            //CustomDoubleClickHotkeysTrigger.Register(KeyCode.LeftShift, Init);

            // Кэшируем классы сразу при компиляции/запуске Unity
            //RefreshCache();
        }

        private static void Init()
        {
            Vector2Int size = new Vector2Int(WIDTH, HEIGHT);
            ScriptFinderTool window = GetWindow<ScriptFinderTool>("Scripts finder");

            Vector2Int pos = UMP.GetScaledCursorPosition().ToInt();
            UMP.RECT screenSize = UMP.GetCurrentMonitorRect();
            pos = new Vector2Int(
                Mathf.Clamp(pos.x - size.x / 2, screenSize.Left, screenSize.Right - size.x),
                Mathf.Clamp(pos.y - size.y / 2, screenSize.Top, screenSize.Bottom - size.y)
            );

            window.maxSize = size;
            window.minSize = size;
            window.position = new Rect(pos.x, pos.y, size.x, size.y);

            // Если кэш почему-то пуст, заполняем
            if (cachedClasses.Count == 0)
            {
                RefreshCache();
            }

            window.UpdateFilteredIndices();
            window.Show();
        }

        private static void RefreshCache()
        {
            cachedClasses.Clear();

            List<Type> types = new List<Type>();

            // 1. Загружаем основную сборку проекта
            try
            {
                Assembly projectAssembly = Assembly.Load("Assembly-CSharp");
                types.AddRange(projectAssembly.GetTypes());
            }
            catch (Exception)
            {
                // Сборка может не загрузиться, если в проекте пока нет скриптов вне asmdef
            }

            // 2. Загружаем доп-сборки
            try
            {
                Assembly toolsAssembly = Assembly.Load("tools");
                types.AddRange(toolsAssembly.GetTypes());
            }
            catch (Exception)
            {
                Debug.LogWarning("Сборка 'tools' не найдена или еще не скомпилирована движком.");
            }

            // 1. Указываем конкретную папку для поиска ассетов
            string targetFolder = "Assets/Scripts";

            // Проверяем существование папки на случай, если нейминг изменился (регистр букв)
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                // Попробуем с маленькой буквы, если папка называется "Assets/scripts"
                targetFolder = "Assets/scripts";
                if (!AssetDatabase.IsValidFolder(targetFolder))
                {
                    Debug.LogWarning("Папка Assets/Scripts не найдена!");
                    return;
                }
            }

            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { targetFolder });
            Dictionary<string, MonoScript> scriptMapping = new Dictionary<string, MonoScript>(guids.Length);

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (asset != null)
                {
                    string className = asset.GetClass()?.Name ?? System.IO.Path.GetFileNameWithoutExtension(path);
                    if (!scriptMapping.ContainsKey(className))
                    {
                        scriptMapping.Add(className, asset);
                    }
                }
            }

            // 2. Связываем типы из сборки только с теми скриптами, которые попали в маппинг из нашей папки
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];

                if (type.IsNested || type.Name.StartsWith("<"))
                {
                    continue;
                }

                if (scriptMapping.TryGetValue(type.Name, out MonoScript boundAsset))
                {
                    string baseName = type.BaseType != null ? type.BaseType.Name : "object";
                    cachedClasses.Add(new ParsedClassData(type.Name, baseName, boundAsset));
                }
            }
        }

        private void OnFocus()
        {
            searchField?.SetFocus();
        }

        private void OnEnable()
        {
            searchField = new SearchField();
            searchField.SetFocus();
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
            if (windowBgTexture != null)
            {
                DestroyImmediate(windowBgTexture);
            }
            if (buttonNormalTexture != null)
            {
                DestroyImmediate(buttonNormalTexture);
            }
            if (buttonHoverTexture != null)
            {
                DestroyImmediate(buttonHoverTexture);
            }
            if (buttonActiveTexture != null)
            {
                DestroyImmediate(buttonActiveTexture);
            }
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;
            if (currentEvent != null && currentEvent.type == EventType.KeyDown)
            {
                // Закрытие окна по нажатию Escape
                if (currentEvent.keyCode == KeyCode.Escape)
                {
                    currentEvent.Use();
                    Close();
                    return;
                }

                // Открытие первого элемента по нажатию Enter
                if ((currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter) && filteredIndices.Count > 0)
                {
                    currentEvent.Use();

                    int firstIdx = filteredIndices[0];
                    ParsedClassData targetData = cachedClasses[firstIdx];

                    AssetDatabase.OpenAsset(targetData.ScriptAsset);
                    Close();
                    return;
                }
            }

            // Инициализируем стиль кнопок один раз при отрисовке
            if (buttonStyle == null)
            {
                InitStyle();
            }

            string newSearch = searchField.OnGUI(search);
            if (newSearch != search)
            {
                search = newSearch;
                UpdateFilteredIndices();
            }

            GUILayout.BeginVertical();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            for (int i = 0; i < filteredIndices.Count; i++)
            {
                int originalIdx = filteredIndices[i];
                ParsedClassData data = cachedClasses[originalIdx];

                // Получаем строку с подсвеченным совпадением
                string labelText = GetHighlightedLabel(data, search);

                if (GUILayout.Button(labelText, buttonStyle, GUILayout.Height(26)))
                {
                    AssetDatabase.OpenAsset(data.ScriptAsset);
                    Close();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void InitStyle()
        {
            // Цвета из запроса
            Color hexWindowBg = "#252525".HexToColor();
            Color hexBtnNormal = "#303030".HexToColor();
            Color hexBtnHover = "#3a3a3a".HexToColor();
            Color hexBtnActive = "#202020".HexToColor();
            Color hexTextDefault = "#d5ffb0".HexToColor();

            // Создаем текстуры
            windowBgTexture = hexWindowBg.ToSolidTexture();
            buttonNormalTexture = hexBtnNormal.ToSolidTexture();
            buttonHoverTexture = hexBtnHover.ToSolidTexture();
            buttonActiveTexture = hexBtnActive.ToSolidTexture();

            // Настройка стиля кнопки
            buttonStyle = new GUIStyle(GUIStyle.none); // Сбрасываем стандартный скин miniButton
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.richText = true;
            buttonStyle.fontSize = 11; // Размер текста уменьшен, как просил
            buttonStyle.padding = new RectOffset(12, 12, 0, 0);

            // Убираем внешние отступы между кнопками, чтобы они шли плотно друг к другу
            buttonStyle.margin = new RectOffset(0, 0, 1, 1);

            // Назначаем текстуры на состояния
            buttonStyle.normal.background = buttonNormalTexture;
            buttonStyle.normal.textColor = hexTextDefault;

            buttonStyle.hover.background = buttonHoverTexture;
            buttonStyle.hover.textColor = hexTextDefault;

            buttonStyle.active.background = buttonActiveTexture;
            buttonStyle.active.textColor = hexTextDefault;
        }

        private void UpdateFilteredIndices()
        {
            if (string.IsNullOrEmpty(search))
            {
                filteredIndices = new List<int>(cachedClasses.Count);
                for (int i = 0; i < cachedClasses.Count; i++)
                {
                    filteredIndices.Add(i);
                }
                return;
            }

            string lowerSearch = search.ToLowerInvariant();
            List<(int index, int score)> scored = new List<(int, int)>();

            for (int i = 0; i < cachedClasses.Count; i++)
            {
                string lowerOption = cachedClasses[i].ClassName.ToLowerInvariant();
                int score;

                if (lowerOption.Contains(lowerSearch))
                {
                    score = lowerOption.IndexOf(lowerSearch, StringComparison.Ordinal);
                }
                else if (IsSubsequence(lowerSearch, lowerOption))
                {
                    score = 500 + (lowerOption.Length - lowerSearch.Length);
                }
                else
                {
                    score = 1000 + StringExtensions.LevenshteinDistance(lowerSearch, lowerOption);
                }

                if (score > 1000 && lowerSearch.Length > 2 && score - 1000 > lowerSearch.Length)
                {
                    continue;
                }

                scored.Add((i, score));
            }

            scored.Sort((a, b) => a.score.CompareTo(b.score));

            filteredIndices = new List<int>(scored.Count);
            for (int i = 0; i < scored.Count; i++)
            {
                filteredIndices.Add(scored[i].index);
            }
        }

        private string GetHighlightedLabel(ParsedClassData data, string searchStr)
        {
            if (string.IsNullOrEmpty(searchStr))
            {
                return data.DisplayLayout;
            }

            string name = data.ClassName;
            int startIdx = name.IndexOf(searchStr, StringComparison.OrdinalIgnoreCase);

            // Если это прямое вхождение (Contains) — подсвечиваем кусок золотым цветом
            if (startIdx >= 0)
            {
                string match = name.Substring(startIdx, searchStr.Length);
                string highlightedName = name.Replace(match, $"<color=#ffcc00ff><b>{match}</b></color>");
                return $"{highlightedName} <color=#808080ff>: {data.BaseClassName}</color>";
            }

            // Для Subsequence или Левенштейна оставляем стандартную раскладку, 
            // так как побуквенный разбор RichText сильно бьет по перформансу в OnGUI.
            return data.DisplayLayout;
        }

        private bool IsSubsequence(string search, string option)
        {
            int searchIdx = 0;
            int optionIdx = 0;

            while (searchIdx < search.Length && optionIdx < option.Length)
            {
                if (search[searchIdx] == option[optionIdx])
                {
                    searchIdx++;
                }
                optionIdx++;
            }

            return searchIdx == search.Length;
        }
    }
}
#endif