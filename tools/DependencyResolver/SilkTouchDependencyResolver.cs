#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.PlayerLoop;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace SilkTouch.Tools.DependencyResolver
{
    [InitializeOnLoad]
    public class SilkTouchDependencyResolver : EditorWindow
    {
        private enum DependencyStatus
        {
            Missing,
            LowVersion,
            Ok
        }

        private class Dependency
        {
            public string Name;
            public string PackageName;
            public string InstallSource;
            public string RequiredVersion;
            public bool IsPackage;
            public DependencyStatus Status;
        }

        private static readonly List<Dependency> dependencies = new List<Dependency>
        {
            new Dependency
            {
                Name = "UniTask",
                PackageName = "com.cysharp.unitask",
                InstallSource = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
                IsPackage = true
            },
            new Dependency
            {
                Name = "Unity UI (TMP)",
                PackageName = "com.unity.ugui",
                InstallSource = "com.unity.ugui",
                IsPackage = true
            },
            new Dependency
            {
                Name = "Newtonsoft Json",
                PackageName = "com.unity.nuget.newtonsoft-json",
                InstallSource = "com.unity.nuget.newtonsoft-json@3.2.1",
                RequiredVersion = "3.2.1",
                IsPackage = true
            }
        };

        private static ListRequest listRequest;
        private static AddRequest addRequest;
        private static int currentIndex = -1;
        private static bool isInstalling;

        static SilkTouchDependencyResolver()
        {
            EditorApplication.delayCall += CheckWrapper;
        }

        private static void CheckWrapper()
        {
            CheckPackages(false);
        }

        private static void CheckPackages(bool force)
        {
            if (SilkTouchEditorConfig.instance.IgnoreDependencyCheck && !force)
            {
                return;
            }
            listRequest = Client.List(true, true);
            onceAction = () => CheckListProgress(force);
            EditorApplication.update += PostUpdate;
        }

        private static Action onceAction;

        private static void PostUpdate()
        {
            if (listRequest != null && listRequest.IsCompleted)
            {
                EditorApplication.update -= PostUpdate;
                onceAction?.Invoke();
            }
        }

        private static void CheckListProgress(bool force)
        {
            if (listRequest.Status == StatusCode.Success)
            {
                bool hasIssues = false;
                for (int i = 0; i < dependencies.Count; i++)
                {
                    dependencies[i].Status = DependencyStatus.Missing;
                    foreach (PackageInfo package in listRequest.Result)
                    {
                        if (package.name == dependencies[i].PackageName)
                        {
                            dependencies[i].Status = IsVersionSatisfied(package.version, dependencies[i].RequiredVersion)
                                ? DependencyStatus.Ok
                                : DependencyStatus.LowVersion;
                            break;
                        }
                    }

                    if (dependencies[i].Status != DependencyStatus.Ok)
                    {
                        hasIssues = true;
                    }
                }

                if (hasIssues || force)
                {
                    ShowWindow();
                }
            }
            listRequest = null;
        }

        private static bool IsVersionSatisfied(string installedVersion, string requiredVersion)
        {
            if (string.IsNullOrEmpty(requiredVersion))
            {
                return true;
            }

            return CompareVersions(installedVersion, requiredVersion) >= 0;
        }

        private static int CompareVersions(string a, string b)
        {
            int[] va = ParseVersion(a);
            int[] vb = ParseVersion(b);
            for (int i = 0; i < 3; i++)
            {
                if (va[i] != vb[i])
                {
                    return va[i].CompareTo(vb[i]);
                }
            }
            return 0;
        }

        private static int[] ParseVersion(string version)
        {
            int[] result = { 0, 0, 0 };
            if (string.IsNullOrEmpty(version))
            {
                return result;
            }

            string core = version.Split('-', '+')[0];
            string[] parts = core.Split('.');
            for (int i = 0; i < parts.Length && i < 3; i++)
            {
                int.TryParse(parts[i], out result[i]);
            }

            return result;
        }

        [MenuItem("SilkTouch/Dependencies")]
        public static void CheckAndShowWindow()
        {
            CheckPackages(true);
        }

        public static void ShowWindow()
        {
            SilkTouchDependencyResolver window = GetWindow<SilkTouchDependencyResolver>("Silk Touch Setup");
            window.minSize = new Vector2(400, 200);
            window.maxSize = new Vector2(400, 200);
            window.Show();
        }

        private void OnDestroy()
        {
            if (!IsAllDependenciesInstalled())
            {
                SilkTouchEditorConfig.instance.IgnoreDependencyCheck = true;
            }
        }

        private void OnGUI()
        {
            bool allInstalled = IsAllDependenciesInstalled();

            GUILayout.Space(15);
            if (!allInstalled)
            {
                EditorGUILayout.HelpBox("Silk Touch requires additional dependencies.\nPlease install missing packages to ensure full functionality.",
                    MessageType.Warning
                );
            }

            GUILayout.Space(10);

            for (int i = 0; i < dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(dependencies[i].Name, GUILayout.Width(120));

                switch (dependencies[i].Status)
                {
                    case DependencyStatus.Ok:
                        GUI.color = Color.green;
                        EditorGUILayout.LabelField("[Installed]");
                        break;
                    case DependencyStatus.LowVersion:
                        GUI.color = Color.yellow;
                        EditorGUILayout.LabelField("[Low version]");
                        break;
                    default:
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField("[Missing]");
                        break;
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUI.enabled = !isInstalling;

            if (!isInstalling && !allInstalled)
            {
                if (GUILayout.Button("Install Missing", GUILayout.Height(35)))
                {
                    InstallNextDependency();
                }
            }

            GUI.enabled = true;

            if (isInstalling)
            {
                EditorGUILayout.HelpBox("Installing dependencies... Please, wait.", MessageType.Info);
            }
        }

        private static void InstallNextDependency()
        {
            currentIndex = -1;
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i].Status != DependencyStatus.Ok)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex != -1)
            {
                isInstalling = true;
                Debug.Log($"[Silk Touch] Starting installation: {dependencies[currentIndex].Name}...");
                addRequest = Client.Add(dependencies[currentIndex].InstallSource);
                EditorApplication.update += InstallProgress;
            }
            else
            {
                isInstalling = false;
                if (HasOpenInstances<SilkTouchDependencyResolver>())
                {
                    GetWindow<SilkTouchDependencyResolver>().Close();
                }
                AssetDatabase.Refresh();
            }
        }

        private static void InstallProgress()
        {
            if (addRequest != null && addRequest.IsCompleted)
            {
                EditorApplication.update -= InstallProgress;

                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"[Silk Touch] Successfully installed: {addRequest.Result.packageId}");
                    dependencies[currentIndex].Status = DependencyStatus.Ok;
                    InstallNextDependency();
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.LogError($"[Silk Touch] Failed to install {dependencies[currentIndex].Name}: {addRequest.Error.message}");
                    isInstalling = false;
                }

                addRequest = null;
            }
        }

        private static bool IsAllDependenciesInstalled()
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i].Status != DependencyStatus.Ok)
                {
                    return false;
                }
            }

            return true;
        }
    }

}

#endif
