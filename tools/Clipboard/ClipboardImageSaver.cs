#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using UnityEngine.Experimental.Rendering;

[InitializeOnLoad]
public class ClipboardImageSaver
{
    public const string STANDARD_NAME = "ClipboardImage.png";

    [MenuItem("Assets/Save image from clipboard #v")]
    private static void SaveImage()
    {
        string path = FolderTracker.CurrentEditorFolder;
        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            SaveImageFromClipboard(path);
        }
    }

    private static void SaveImageFromClipboard(string path)
    {
        Texture2D texture = GetFromClipboard();

        if (texture != null)
        {
            byte[] pngBytes = texture.EncodeToPNG();

            string filePath = $"{path}/{GetNextName(STANDARD_NAME, path)}";
            File.WriteAllBytes(filePath, pngBytes);
            AssetDatabase.Refresh();
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            Selection.activeObject = obj;

            EditorGUIUtility.PingObject(obj);
            Debug.Log("Image saved to " + filePath);
        }
        else
        {
            Debug.Log("No image in clipboard");
        }
    }

    public static string GetNextName(string fileName, string directoryPath)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);

        string[] files = Directory.GetFiles(directoryPath);

        string pattern = @"^" + Regex.Escape(fileNameWithoutExtension) + @"(\d*)\." + Regex.Escape(extension.TrimStart('.')) + "$";
        int maxIndex = -1;

        foreach (string file in files)
        {
            string fileNameInDir = Path.GetFileName(file);
            Match match = Regex.Match(fileNameInDir, pattern);

            if (match.Success)
            {
                string indexStr = match.Groups[1].Value;
                int index = string.IsNullOrEmpty(indexStr) ? 0 : int.Parse(indexStr);

                if (index > maxIndex)
                {
                    maxIndex = index;
                }
            }
        }

        int nextIndex = maxIndex + 1;
        string nextFileName = nextIndex == 0 ? fileName : fileNameWithoutExtension + nextIndex + extension;
        return nextFileName;
    }

    [DllImport("UnityClipboard")] private static extern bool Open();
    [DllImport("UnityClipboard")] private static extern int Width();
    [DllImport("UnityClipboard")] private static extern int Height();
    [DllImport("UnityClipboard")] private static extern int BitsPerPixel();
    [DllImport("UnityClipboard")] private static extern IntPtr Read();

    public static Texture2D GetFromClipboard()
    {
        try
        {
            // no image in clipboard
            if (Open() == false)
            {
                return null;
            }

            int width = Width();
            int height = Height();
            int bitsPerPixel = BitsPerPixel();
            int bytesPerPixel = bitsPerPixel / 8;
            IntPtr ptr = Read();
            Texture2D tex = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_SRGB, 0, TextureCreationFlags.None);
            tex.wrapMode = TextureWrapMode.Clamp;

            byte[] bytes = new byte[width * height * bytesPerPixel];

            if (bytesPerPixel == 3)
            {
                // When we have 3 bytes per pixel, it indicates an image formatted differently.
                // The format is BGR, and it additionally skips some bytes at the end of each row.
                // The number of skipped bytes is a function of the width.
                int skippedBytesPerRow = width % 4;
                if (skippedBytesPerRow == 0)
                {
                    skippedBytesPerRow = 4;
                }

                int p = 0;
                bool skipBytes = false;
                int bytesPerRow = width * bytesPerPixel;
                for (int b = 0; b < bytes.Length; b++)
                {
                    if (skipBytes)
                    {
                        p += skippedBytesPerRow;
                    }
                    bytes[b] = Marshal.ReadByte(ptr, p++);
                    skipBytes = (b + 1) % bytesPerRow == 0;
                }
            }
            else
            {
                Marshal.Copy(ptr, bytes, 0, width * height * bytesPerPixel);
            }

            Color32[] colors = new Color32[width * height];

            int c = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    int pos = (y * width + x) * bytesPerPixel;

                    // read in bgra format
                    byte b = bytes[pos];
                    byte g = bytes[pos + 1];
                    byte r = bytes[pos + 2];
                    byte a = bytesPerPixel == 4 ? bytes[pos + 3] : (byte)255;

                    colors[c] = new Color32(r, g, b, a);
                    c++;
                }
            }

            tex.SetPixels32(colors);
            tex.Apply();
            return tex;
        }
        catch (Exception e)
        {
            Debug.LogError($"Can't past image from clipboard {e}");
        }

        return null;
    }
}
#endif