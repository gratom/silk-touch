using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

[InitializeOnLoad]
public class SilkTouchDependencyResolver : EditorWindow
{
    private const string UniTaskPackageName = "com.cysharp.unitask";
    private const string UniTaskGitUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";

    private static ListRequest listRequest;
    private static AddRequest addRequest;
    private static bool isInstalling;

    static SilkTouchDependencyResolver()
    {
        EditorApplication.delayCall += CheckPackage;
    }

    private static void CheckPackage()
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
                bool hasUniTask = false;
                foreach (PackageInfo package in listRequest.Result)
                {
                    if (package.name == UniTaskPackageName)
                    {
                        hasUniTask = true;
                        break;
                    }
                }
                if (!hasUniTask)
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
        window.minSize = new Vector2(400, 150);
        window.maxSize = new Vector2(400, 150);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(15);
        EditorGUILayout.HelpBox("Silk Touch need UniTask.\nPlease install this dependency to ensure full functionality.",
            MessageType.Warning
        );

        GUILayout.Space(10);

        GUI.enabled = !isInstalling;
        
        if (!isInstalling)
        {
            if (GUILayout.Button("Install UniTask", GUILayout.Height(35)))
            {
                InstallUniTask();
            }
        }

        GUI.enabled = true;

        if (isInstalling)
        {
            EditorGUILayout.HelpBox("Installing UniTask... Please, wait.", MessageType.Info);
        }
    }

    private void InstallUniTask()
    {
        isInstalling = true;
        Debug.Log("[Silk Touch] Starting UniTask installation...");
        addRequest = Client.Add(UniTaskGitUrl);
        EditorApplication.update += InstallProgress;
    }

    private static void InstallProgress()
    {
        if (addRequest != null && addRequest.IsCompleted)
        {
            isInstalling = false;
            EditorApplication.update -= InstallProgress;

            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"[Silk Touch] Successfully installed: {addRequest.Result.packageId}");

                if (HasOpenInstances<SilkTouchDependencyResolver>())
                {
                    GetWindow<SilkTouchDependencyResolver>().Close();
                }

                AssetDatabase.Refresh();
            }
            else if (addRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError($"[Silk Touch] Failed to install UniTask: {addRequest.Error.message}");
            }

            addRequest = null;
        }
    }
}