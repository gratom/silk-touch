#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class FolderTracker
{
    public static string CurrentEditorFolder => currentFolder;
    private static string currentFolder;

    static FolderTracker()
    {
        EditorApplication.update += GetProjectViewFolder;

        //debug
        //SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(5, 1, currentFolder.Length * 15, 20));
        GUI.Label(new Rect(0, 0, currentFolder.Length * 15, 20), currentFolder);
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void GetProjectViewFolder()
    {
        Type projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        if (projectBrowserType == null)
        {
            Debug.LogError("ProjectBrowser type not found");
            return;
        }

        EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll(projectBrowserType) as EditorWindow[];
        if (allWindows == null || allWindows.Length == 0)
        {
            Debug.LogError("No ProjectBrowser window found");
            return;
        }

        EditorWindow browser = allWindows[0];

        FieldInfo field = projectBrowserType.GetField("m_LastFolders", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            string[] folders = field.GetValue(browser) as string[];
            if (folders != null && folders.Length > 0)
            {
                currentFolder = folders[0];
            }
            else
            {
                currentFolder = "";
            }
        }
        else
        {
            Debug.LogError("Field 'm_LastFolders' not found");
        }
    }
}
#endif