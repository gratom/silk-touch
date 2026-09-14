using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SilkTouch.Localization
{
    using Managers;
    using Managers.Data;
    using Tools;

    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Scriptables/Localization data", order = 51)]
    public class LocalizationData : ScriptableObject
    {
        private const string DEFAULT_DIRECTORY = "Assets/scriptables/localization/";

        [SerializeField] private string GOOGLE_DOC_ID = "1YaDRsIEhovENQ4Qn4r-Q7OQaxffOLSkux7_1SKeGu4g";
        [SerializeField] private string GOOGLE_SHEET_ID = "0";

        [SerializeField] private List<ActiveLanguageContainer> allLanguages;
        [SerializeField] private List<LocalizationDataContainer> containers;

#if UNITY_EDITOR
        public
#else
        private
#endif
            Dictionary<string, LocalizationDataContainer> dictionaryContainers;


        public void InitDictionary()
        {
            if (dictionaryContainers == null)
            {
                dictionaryContainers = new Dictionary<string, LocalizationDataContainer>(containers.Count);
                foreach (LocalizationDataContainer container in containers)
                {
                    dictionaryContainers.Add(container.Key, container);
                }
            }
        }

        public string GetTextByID(string key)
        {
            if (dictionaryContainers.ContainsKey(key))
            {
                return dictionaryContainers[key].GetTextByLanguage(Services.GetManager<DataManager>().DynamicData.Settings.CurrentLanguage);
            }
            return key;
        }

#if UNITY_EDITOR

        [MenuItem("Localization/Open Google Sheet", true)]
        public static bool OpenGoogleSheetInBrowserStaticValidation()
        {
            if (!CreateLocalizationDataValidation()) //asset exist
            {
                string filePath = DEFAULT_DIRECTORY + "MainLocalizationData.asset";
                LocalizationData data = AssetDatabase.LoadAssetAtPath<LocalizationData>(filePath);
                return !string.IsNullOrEmpty(data.GOOGLE_DOC_ID);
            }
            return false;
        }

        [MenuItem("Localization/Open Google Sheet")]
        public static void OpenGoogleSheetInBrowserStatic()
        {
            string filePath = DEFAULT_DIRECTORY + "MainLocalizationData.asset";
            LocalizationData data = AssetDatabase.LoadAssetAtPath<LocalizationData>(filePath);
            data.OpenGoogleSheetInBrowser();
        }

        [ContextMenu("Open Google Sheet")]
        public void OpenGoogleSheetInBrowser()
        {
            if (string.IsNullOrEmpty(GOOGLE_DOC_ID))
            {
                Debug.LogError("LocalizationData: GOOGLE_DOC_ID is empty!");
                return;
            }
            string url = $"https://docs.google.com/spreadsheets/d/{GOOGLE_DOC_ID}/edit#gid={GOOGLE_SHEET_ID}";
            Application.OpenURL(url);
            Debug.Log($"Opening Google Sheet: {url}");
        }

        public string GetTextByIDDef(string key)
        {
            InitDictionary();
            return dictionaryContainers[key].GetTextByLanguage(GT.Language.English);
        }

        public bool IsContain(string key)
        {
            InitDictionary();
            return dictionaryContainers.ContainsKey(key);
        }

        [MenuItem("Localization/Create main Asset", true)]
        public static bool CreateLocalizationDataValidation()
        {
            string filePath = DEFAULT_DIRECTORY + "MainLocalizationData.asset";
            LocalizationData data = AssetDatabase.LoadAssetAtPath<LocalizationData>(filePath);
            return data == null;
        }

        [MenuItem("Localization/Create main Asset", false)]
        public static void CreateLocalizationData()
        {
            // Create the directory if it doesn't exist
            if (!Directory.Exists(DEFAULT_DIRECTORY))
            {
                Directory.CreateDirectory(DEFAULT_DIRECTORY);
            }

            // Generate a unique filename
            string fileName = "MainLocalizationData.asset";
            string filePath = AssetDatabase.GenerateUniqueAssetPath(DEFAULT_DIRECTORY + fileName);

            // Create a new instance of the ScriptableObject
            LocalizationData newData = CreateInstance<LocalizationData>();

            // Save the ScriptableObject at the specified path
            AssetDatabase.CreateAsset(newData, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newData;
        }

        [MenuItem("Localization/Show main Asset")]
        public static void ShowLocalizationData()
        {
            string filePath = DEFAULT_DIRECTORY + "MainLocalizationData.asset";
            LocalizationData data = AssetDatabase.LoadAssetAtPath<LocalizationData>(filePath);

            if (data != null)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = data;
                EditorGUIUtility.PingObject(data);
            }
            else
            {
                Debug.LogWarning($"Localization asset not found at path: {filePath}");
            }
        }

        [ContextMenu("From GoogleSheet")]
        private async Task LoadFromGoogleSheetAsync()
        {
            VirtualTable table = new VirtualTable(GOOGLE_DOC_ID, GOOGLE_SHEET_ID);
            if (await table.RefreshAsync())
            {
                SetContainers(table);
            }
        }

        public string GetNewValueKey(string key)
        {
            //create new value, put it in list
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
            }
            key = key.Replace(" ", "");
            if (IsContain(key))
            {
                string newKey = key;
                int iterator = 0;
                while (IsContain(newKey))
                {
                    newKey = key + iterator;
                }
                key = newKey;
            }
            LocalizationDataContainer dataContainer = new LocalizationDataContainer();

            foreach (ActiveLanguageContainer languageContainer in allLanguages.Where(x => x.IsActive))
            {
                dataContainer.LanguageContainers.Add(new LanguageContentContainer(languageContainer.Language, ""));
            }
            dataContainer.Key = key;
            containers.Add(dataContainer);
            dictionaryContainers.Add(dataContainer.Key, dataContainer);
            EditorUtility.SetDirty(this);
            return key;
        }

        public void UpdateValue(string key, string newValue)
        {
            LocalizationDataContainer container = dictionaryContainers[key];
            if (container.LanguageContainers[0].Text != newValue)
            {
                container.LanguageContainers[0].Text = newValue;
                container.TranslateAll();
            }

            EditorUtility.SetDirty(this);
        }

        public List<string> GetAllValues(string key)
        {
            return dictionaryContainers[key].LanguageContainers.Select(x => $"[{x.Language.ToCode()}] {x.Text}").ToList();
        }

        public void DeleteElement(int index)
        {
            if (index >= 0 && index < containers.Count)
            {
                if (dictionaryContainers.ContainsKey(containers[index].Key))
                {
                    dictionaryContainers.Remove(containers[index].Key);
                }
                containers.RemoveAt(index);
            }
            EditorUtility.SetDirty(this);
        }

        private void SetContainers(VirtualTable table)
        {
            if (table == null || table.RowCount < 2)
            {
                Debug.LogWarning("LocalizationData: lines less then 2");
                return;
            }

            Dictionary<int, GT.Language> languageColumns = ParseLanguageColumns(table.GetRow(0));
            if (languageColumns.Count == 0)
            {
                Debug.LogWarning("LocalizationData: no language columns found in header row.");
                return;
            }

            int keyColumn = FindKeyColumn(table.GetRow(0), languageColumns);
            containers = new List<LocalizationDataContainer>();

            for (int row = 1; row < table.RowCount; row++)
            {
                string key = table.GetCell(row, keyColumn).Trim();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                LocalizationDataContainer container = new LocalizationDataContainer { Key = key.Replace(" ", "") };

                foreach (KeyValuePair<int, GT.Language> column in OrderLanguageColumns(languageColumns))
                {
                    container.LanguageContainers.Add(new LanguageContentContainer(column.Value, table.GetCell(row, column.Key)));
                }

                containers.Add(container);
                Debug.Log($"LocalizationData: added container Key[{container.Key}]");
            }

            dictionaryContainers = null;
            EditorUtility.SetDirty(this);
        }

        private static Dictionary<int, GT.Language> ParseLanguageColumns(string[] headerRow)
        {
            Dictionary<int, GT.Language> result = new Dictionary<int, GT.Language>();
            for (int col = 0; col < headerRow.Length; col++)
            {
                string header = headerRow[col]?.Trim();
                if (string.IsNullOrEmpty(header))
                {
                    continue;
                }

                if (Enum.TryParse(header, true, out GT.Language language))
                {
                    result[col] = language;
                }
            }

            return result;
        }

        private static int FindKeyColumn(string[] headerRow, Dictionary<int, GT.Language> languageColumns)
        {
            for (int col = 0; col < headerRow.Length; col++)
            {
                if (!languageColumns.ContainsKey(col))
                {
                    return col;
                }
            }

            return 0;
        }

        private static IEnumerable<KeyValuePair<int, GT.Language>> OrderLanguageColumns(
            Dictionary<int, GT.Language> languageColumns)
        {
            return languageColumns
                .OrderBy(kv => kv.Value == GT.Language.English ? 0 : 1)
                .ThenBy(kv => kv.Key);
        }
#endif
    }
}