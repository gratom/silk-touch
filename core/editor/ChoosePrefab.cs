#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SilkTouch.EditorScripts
{
    using Managers;
    using Boot;
    using Tools;
    using UMP = Tools.UniversalMousePosition;

    internal class PrefabPicker : EditorWindow
    {
        private struct ManagerData
        {
            public Type Type;
            public string PrefabPath;
            public bool IsInBoot;
            public bool HasPrefab => !string.IsNullOrEmpty(PrefabPath);
        }

        private static List<ManagerData> managers = new List<ManagerData>();
        private const string PREFAB_FOLDER = "Assets/prefabs/managers";
        private const string BOOT_SETTINGS_PATH = "Assets/scriptables/BootSettings.asset";

        private const int WIDTH = 420;
        private const int LINE_HEIGHT = 21;
        private const int TOP_OUTFIT = 5;
        private const int CENTRAL_OUTFIT = 8;
        private const int BOTTOM_OUTFIT = 3;

        [MenuItem("Scripts/Choose Manager #m")]
        private static void Init()
        {
            RefreshManagersList();
            Vector2Int size = GetWindowSize();
            PrefabPicker window = GetWindow<PrefabPicker>();

            Vector2Int pos = UMP.GetScaledCursorPosition().ToInt();
            UMP.RECT screenSize = UMP.GetCurrentMonitorRect();
            pos = new Vector2Int(
                Mathf.Clamp(pos.x - size.x / 2, screenSize.Left, screenSize.Right - size.x),
                Mathf.Clamp(pos.y - size.y / 2, screenSize.Top, screenSize.Bottom - size.y)
            );

            window.maxSize = size;
            window.minSize = size;
            window.position = new Rect(pos.x, pos.y, size.x, size.y);
            window.Show();
        }

        private static void RefreshManagersList()
        {
            managers.Clear();

            //Get list of all managers from BootSettings
            BootSettings boot = AssetDatabase.LoadAssetAtPath<BootSettings>(BOOT_SETTINGS_PATH);
            HashSet<BaseManager> bootManagers = new HashSet<BaseManager>();
            if (boot != null && boot.ManagerContainers != null)
            {
                foreach (ManagerContainer container in boot.ManagerContainers)
                {
                    if (container.functionalManager != null)
                    {
                        bootManagers.Add(container.functionalManager);
                    }
                    if (container.fakeManager != null)
                    {
                        bootManagers.Add(container.fakeManager);
                    }
                }
            }

            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(BaseManager).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && p != typeof(BaseManager));

            //Prefab mapping
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { PREFAB_FOLDER });
            Dictionary<Type, string> prefabMap = new Dictionary<Type, string>();
            Dictionary<Type, bool> bootStatusMap = new Dictionary<Type, bool>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    continue;
                }

                BaseManager component = prefab.GetComponentInChildren<BaseManager>(true);
                if (component != null)
                {
                    Type type = component.GetType();
                    if (!prefabMap.ContainsKey(type))
                    {
                        prefabMap.Add(type, path);
                        bootStatusMap[type] = bootManagers.Contains(component);
                    }
                }
            }

            foreach (Type type in allTypes)
            {
                managers.Add(new ManagerData
                {
                    Type = type,
                    PrefabPath = prefabMap.ContainsKey(type) ? prefabMap[type] : null,
                    IsInBoot = bootStatusMap.ContainsKey(type) && bootStatusMap[type]
                });
            }

            managers = managers.OrderBy(m => m.Type.Name).ToList();
        }

        private static Vector2Int GetWindowSize()
        {
            int height = (managers.Count + 1) * LINE_HEIGHT + TOP_OUTFIT + CENTRAL_OUTFIT + BOTTOM_OUTFIT + 10;
            return new Vector2Int(WIDTH, height);
        }

        private void OnGUI()
        {
            Event currentEvent = Event.current;
            if (currentEvent != null && currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape)
            {
                currentEvent.Use();
                Close();
                return;
            }

            GUILayout.Space(TOP_OUTFIT);
            DrawHeaderButtons();
            GUILayout.Space(CENTRAL_OUTFIT);

            if (managers.Count == 0)
            {
                GUILayout.Label("No Manager classes found.");
                return;
            }

            foreach (ManagerData data in managers)
            {
                DrawManagerRow(data);
            }
        }

        private void DrawHeaderButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(8);
                if (GUILayout.Button("Open Folder", GUILayout.Height(LINE_HEIGHT), GUILayout.Width(100)))
                {
                    OpenManagersFolder();
                }
                if (GUILayout.Button("Select Boot", GUILayout.Height(LINE_HEIGHT), GUILayout.Width(100)))
                {
                    SelectBootSettings();
                }
                if (GUILayout.Button("Create Script", GUILayout.Height(LINE_HEIGHT), GUILayout.Width(100)))
                {
                    ScriptCreator.CreateManager();
                }
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawManagerRow(ManagerData data)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Rect iconStatusRect = GUILayoutUtility.GetRect(20, LINE_HEIGHT, GUILayout.Width(20));
                if (data.HasPrefab)
                {
                    string color = data.IsInBoot ? "#32CD32" : "#FF6E11"; // LimeGreen : Gold
                    string label = $"<color={color}>\u25CF</color>";

                    GUIStyle statusStyle = new GUIStyle(EditorStyles.label) { richText = true, alignment = TextAnchor.MiddleCenter };
                    GUI.Label(iconStatusRect, label, statusStyle);
                }

                //select button
                GUI.enabled = data.HasPrefab;
                if (GUILayout.Button(data.Type.Name, GUILayout.Width(WIDTH * 0.48f)))
                {
                    SelectPrefab(data.PrefabPath);
                    Close();
                }
                if (GUILayout.Button("[ ]", GUILayout.Width(WIDTH * 0.07f)))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(data.PrefabPath);
                    EditorGUIUtility.PingObject(obj);
                }
                GUI.enabled = true;

                // create button
                GUI.enabled = !data.HasPrefab;
                if (GUILayout.Button("Create Prefab", GUILayout.Width(WIDTH * 0.3f)))
                {
                    ManagerInstanceCreator.CreateFromType(data.Type);
                    RefreshManagersList();
                }
                GUI.enabled = true;
            }
        }

        private void SelectBootSettings()
        {
            Object boot = AssetDatabase.LoadAssetAtPath<Object>(BOOT_SETTINGS_PATH);
            if (boot != null)
            {
                Selection.activeObject = boot;
                EditorGUIUtility.PingObject(boot);
            }
            else
            {
                Debug.LogWarning($"BootSettings not found at {BOOT_SETTINGS_PATH}");
            }
        }

        private void OpenManagersFolder()
        {
            if (!Directory.Exists(PREFAB_FOLDER))
            {
                Directory.CreateDirectory(PREFAB_FOLDER);
                AssetDatabase.Refresh();
            }

            ManagerData prefabWithFolder = managers.FirstOrDefault(m => m.HasPrefab);
            if (!string.IsNullOrEmpty(prefabWithFolder.PrefabPath))
            {
                prefabWithFolder.PrefabPath.OpenFolder();
            }
            else
            {
                Object folderObj = AssetDatabase.LoadAssetAtPath<Object>(PREFAB_FOLDER);
                Selection.activeObject = folderObj;
                EditorGUIUtility.PingObject(folderObj);
            }
        }

        private void SelectPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                Selection.activeObject = prefab;
                AssetDatabase.OpenAsset(prefab);
            }
        }
    }
}
#endif