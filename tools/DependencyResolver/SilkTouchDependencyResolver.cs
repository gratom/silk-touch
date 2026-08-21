using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

[InitializeOnLoad]
public class SilkTouchDependencyResolver : EditorWindow
{
    private class Dependency
    {
        public string Name;
        public string PackageName;
        public string InstallSource;
        public bool IsInstalled;
        public bool IsPackage;
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
        }
    };

    private static ListRequest listRequest;
    private static AddRequest addRequest;
    private static int currentIndex = -1;
    private static bool isInstalling;

    static SilkTouchDependencyResolver()
    {
        EditorApplication.delayCall += CheckPackages;
    }

    private static void CheckPackages()
    {
        listRequest = Client.List(true);
        EditorApplication.update += CheckListProgress;
    }

    private static void CheckListProgress()
    {
        if (listRequest != null && listRequest.IsCompleted)
        {
            EditorApplication.update -= CheckListProgress;

            if (listRequest.Status == StatusCode.Success)
            {
                bool missingDependencies = false;

                for (int i = 0; i < dependencies.Count; i++)
                {
                    dependencies[i].IsInstalled = false;
                    foreach (PackageInfo package in listRequest.Result)
                    {
                        if (package.name == dependencies[i].PackageName)
                        {
                            dependencies[i].IsInstalled = true;
                            break;
                        }
                    }

                    if (!dependencies[i].IsInstalled)
                    {
                        missingDependencies = true;
                    }
                }

                if (missingDependencies)
                {
                    ShowWindow();
                }
            }

            listRequest = null;
        }
    }

    public static void ShowWindow()
    {
        SilkTouchDependencyResolver window = GetWindow<SilkTouchDependencyResolver>("Silk Touch Setup");
        window.minSize = new Vector2(400, 200);
        window.maxSize = new Vector2(400, 200);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(15);
        EditorGUILayout.HelpBox("Silk Touch requires additional dependencies.\nPlease install missing packages to ensure full functionality.",
            MessageType.Warning
        );

        GUILayout.Space(10);

        for (int i = 0; i < dependencies.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(dependencies[i].Name, GUILayout.Width(120));

            if (dependencies[i].IsInstalled)
            {
                GUI.color = Color.green;
                EditorGUILayout.LabelField("[Installed]");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("[Missing]");
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        GUI.enabled = !isInstalling;

        if (!isInstalling)
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
            if (!dependencies[i].IsInstalled)
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
                dependencies[currentIndex].IsInstalled = true;
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
}