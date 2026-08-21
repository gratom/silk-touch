#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace Tools
{
    public class ScriptablePopupWindow : EditorWindow
    {
        private string[] options;
        private Action<int> onSelect;
        private string search = "";
        private SearchField searchField;
        private Vector2 scrollPosition = Vector2.zero;
        private bool isAutofocus;

        private List<int> filteredIndices = new List<int>();

        public void Init(string[] opts, Action<int> callback, string potentialSearch = "", bool autofocus = true)
        {
            options = opts;
            onSelect = callback;
            searchField = new SearchField();
            search = potentialSearch;

            isAutofocus = autofocus;
            if (autofocus)
            {
                searchField.SetFocus();
            }

            UpdateFilteredIndices();
        }

        private void OnFocus()
        {
            if (searchField != null && isAutofocus)
            {
                searchField.SetFocus();
            }
        }

        private void OnGUI()
        {
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
                string opt = options[originalIdx];

                if (GUILayout.Button(opt))
                {
                    onSelect(originalIdx);
                    Close();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void UpdateFilteredIndices()
        {
            if (options == null)
            {
                filteredIndices = new List<int>();
                return;
            }

            if (string.IsNullOrEmpty(search))
            {
                filteredIndices = new List<int>(options.Length);
                for (int i = 0; i < options.Length; i++)
                {
                    filteredIndices.Add(i);
                }
                return;
            }

            string lowerSearch = search.ToLowerInvariant();
            List<(int index, int score)> scored = new List<(int, int)>();

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == null)
                {
                    continue;
                }

                string lowerOption = options[i].ToLowerInvariant();
                int score;

                if (lowerOption.Contains(lowerSearch))
                {
                    score = lowerOption.IndexOf(lowerSearch, StringComparison.Ordinal);
                }

// НОВАЯ ПРОВЕРКА: Проверяем, идут ли буквы запроса в том же порядке внутри строки (например, M-s-h в Me-s-hFilter)
                else if (IsSubsequence(lowerSearch, lowerOption))
                {
                    // Если это подпоследовательность, даем ей высокий приоритет (чуть хуже Contains)
                    // Чем ближе длина строки к запросу, тем выше элемент
                    score = 500 + (lowerOption.Length - lowerSearch.Length);
                }
                else
                {
                    int distance = StringExtensions.LevenshteinDistance(lowerSearch, lowerOption);
                    score = 1000 + distance;
                }

// Теперь отсекаем по Левенштейну только если это вообще чужое слово
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