#if UNITY_EDITOR && UI_TMP
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace Tools
{
    public class FontReplacer : EditorWindow
    {
        [SerializeField]
        private TMP_FontAsset oldTMPFont;
        [SerializeField]
        private TMP_FontAsset newTMPFont;
        [SerializeField]
        private Font oldLegacyFont;
        [SerializeField]
        private Font newLegacyFont;

        [MenuItem("Tools/Editors/Replace Fonts Project-Wide", false, 1000)]
        public static void ShowWindow()
        {
            GetWindow<FontReplacer>("Font Replacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("TextMesh Pro Fonts", EditorStyles.boldLabel);
            oldTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Old TMP Font", oldTMPFont, typeof(TMP_FontAsset), false);
            newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newTMPFont, typeof(TMP_FontAsset), false);

            EditorGUILayout.Space(10);

            GUILayout.Label("Legacy UI Fonts", EditorStyles.boldLabel);
            oldLegacyFont = (Font)EditorGUILayout.ObjectField("Old Legacy Font", oldLegacyFont, typeof(Font), false);
            newLegacyFont = (Font)EditorGUILayout.ObjectField("New Legacy Font", newLegacyFont, typeof(Font), false);

            EditorGUILayout.Space(15);

            if (GUILayout.Button("Replace in Active Scene & Prefabs", GUILayout.Height(30)))
            {
                ExecuteReplacement();
            }
        }

        private void ExecuteReplacement()
        {
            bool hasTMP = oldTMPFont != null && newTMPFont != null;
            bool hasLegacy = oldLegacyFont != null && newLegacyFont != null;

            if (!hasTMP && !hasLegacy)
            {
                EditorUtility.DisplayDialog("Error", "Please setup at least one pair of fonts (TMP or Legacy) to replace.", "OK");
                return;
            }

            int totalChanged = 0;

            totalChanged += ReplaceInPrefabs(hasTMP, hasLegacy);
            totalChanged += ReplaceInCurrentScene(hasTMP, hasLegacy);

            EditorUtility.DisplayDialog("Success", $"Font replacement completed!\nTotal components changed: {totalChanged}", "OK");
        }

        private int ReplaceInPrefabs(bool checkTMP, bool checkLegacy)
        {
            int changedCount = 0;
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null)
                {
                    continue;
                }

                bool isPrefabDirty = false;

                if (checkTMP)
                {
                    TMP_Text[] tmpComponents = prefab.GetComponentsInChildren<TMP_Text>(true);
                    for (int j = 0; j < tmpComponents.Length; j++)
                    {
                        if (tmpComponents[j].font == oldTMPFont)
                        {
                            tmpComponents[j].font = newTMPFont;
                            changedCount++;
                            isPrefabDirty = true;
                        }
                    }
                }

                if (checkLegacy)
                {
                    Text[] legacyComponents = prefab.GetComponentsInChildren<Text>(true);
                    for (int j = 0; j < legacyComponents.Length; j++)
                    {
                        if (legacyComponents[j].font == oldLegacyFont)
                        {
                            legacyComponents[j].font = newLegacyFont;
                            changedCount++;
                            isPrefabDirty = true;
                        }
                    }
                }

                if (isPrefabDirty)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                }
            }

            if (changedCount > 0)
            {
                AssetDatabase.SaveAssets();
            }

            return changedCount;
        }

        private int ReplaceInCurrentScene(bool checkTMP, bool checkLegacy)
        {
            int changedCount = 0;
            bool isSceneDirty = false;

            if (checkTMP)
            {
                TMP_Text[] sceneTMPComponents = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                for (int i = 0; i < sceneTMPComponents.Length; i++)
                {
                    if (sceneTMPComponents[i].font == oldTMPFont)
                    {
                        sceneTMPComponents[i].font = newTMPFont;
                        EditorUtility.SetDirty(sceneTMPComponents[i]);
                        changedCount++;
                        isSceneDirty = true;
                    }
                }
            }

            if (checkLegacy)
            {
                Text[] sceneLegacyComponents = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                for (int i = 0; i < sceneLegacyComponents.Length; i++)
                {
                    if (sceneLegacyComponents[i].font == oldLegacyFont)
                    {
                        sceneLegacyComponents[i].font = newLegacyFont;
                        EditorUtility.SetDirty(sceneLegacyComponents[i]);
                        changedCount++;
                        isSceneDirty = true;
                    }
                }
            }

            if (isSceneDirty)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }

            return changedCount;
        }
    }
}
#endif