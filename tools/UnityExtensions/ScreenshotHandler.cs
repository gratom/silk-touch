#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tools
{
    public class ScreenshotHandler
    {
        [MenuItem("Tools/Capture Screenshot &%f", false, 100)]
        public static void Capture()
        {
            int width = Screen.width;
            int height = Screen.height;
            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string fileName = "Screenshot_" + width + "x" + height + "_" + timestamp + ".png";
            string fullPath = Path.Combine(Application.temporaryCachePath, fileName);

            ScreenCapture.CaptureScreenshot(fullPath);
            Debug.Log("Screenshot captured at " + width + "x" + height + ". Path: " + fullPath);
        }

        [MenuItem("Tools/Open Screenshots Folder", false, 100)]
        public static void OpenFolder()
        {
            string path = Application.temporaryCachePath;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            EditorUtility.OpenWithDefaultApp(path);
        }
    }
}
#endif