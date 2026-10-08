using System.IO;
using UnityEditor;
using UnityEngine;

namespace SilkTouch.Tools.Textures.Editor
{
    public class NormalMapGeneratorWindow : EditorWindow
    {
        private Texture2D sourceTexture;
        private readonly NormalMapSettings settings = new NormalMapSettings();

        // Raw, color-management-free copy of the source asset's bytes.
        // We deliberately avoid sampling the imported Texture2D asset directly in the shader:
        // Unity auto-linearizes sRGB-flagged textures on sample in Linear color space projects,
        // which skews the math relative to the reference tool (a canvas works on raw bytes only).
        private Texture2D rawSourceTexture;
        private string rawSourcePath;

        private Shader normalShader;
        private Shader blurShader;
        private Material normalMaterial;
        private Material blurMaterial;

        private RenderTexture rtNormal;
        private RenderTexture rtBlurA;
        private RenderTexture rtBlurB;

        // Readback of whatever the final RT contains - used for both the on-screen preview
        // and the saved PNG, so what you see is byte-for-byte what you get.
        private Texture2D resultTexture;

        [MenuItem("Tools/Normal Map Generator")]
        private static void Open()
        {
            GetWindow<NormalMapGeneratorWindow>("Normal Map Generator");
        }

        [MenuItem("Assets/Convert to Normal Map", false, 31)]
        private static void ConvertSelectionToNormalMap()
        {
            Texture2D texture = Selection.activeObject as Texture2D;
            NormalMapGeneratorWindow window = GetWindow<NormalMapGeneratorWindow>("Normal Map Generator");
            window.SetSourceTexture(texture);
        }

        [MenuItem("Assets/Convert to Normal Map", true)]
        private static bool ValidateConvertSelectionToNormalMap()
        {
            return Selection.activeObject is Texture2D;
        }

        private void OnEnable()
        {
            normalShader = Shader.Find("Hidden/NormalMapGenerator/Preview");
            blurShader = Shader.Find("Hidden/NormalMapGenerator/BlurSharp");
        }

        private void OnDisable()
        {
            ReleaseAll();
        }

        private void ReleaseAll()
        {
            ReleaseRT(ref rtNormal);
            ReleaseRT(ref rtBlurA);
            ReleaseRT(ref rtBlurB);

            if (resultTexture != null)
            {
                DestroyImmediate(resultTexture);
                resultTexture = null;
            }
            if (rawSourceTexture != null)
            {
                DestroyImmediate(rawSourceTexture);
                rawSourceTexture = null;
            }
            if (normalMaterial != null)
            {
                DestroyImmediate(normalMaterial);
                normalMaterial = null;
            }
            if (blurMaterial != null)
            {
                DestroyImmediate(blurMaterial);
                blurMaterial = null;
            }
        }

        private static void ReleaseRT(ref RenderTexture rt)
        {
            if (rt != null)
            {
                rt.Release();
                DestroyImmediate(rt);
                rt = null;
            }
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);

            EditorGUILayout.Space(5);
            settings.strength = NormalMapEditorGUI.LogSlider("Strength", settings.strength, 0.05f, 50f);
            settings.level = EditorGUILayout.Slider("Level (detail)", settings.level, 0f, 12f);
            settings.blurSharp = EditorGUILayout.Slider("Blur <-> Sharp", settings.blurSharp, -8f, 8f);
            settings.filterType = (NormalMapFilterType)EditorGUILayout.EnumPopup("Filter", settings.filterType);

            EditorGUILayout.Space(5);
            settings.invertHeight = EditorGUILayout.Toggle("Invert Height", settings.invertHeight);
            settings.invertRed = EditorGUILayout.Toggle("Invert Red", settings.invertRed);
            settings.invertGreen = EditorGUILayout.Toggle("Invert Green", settings.invertGreen);
            settings.seamless = EditorGUILayout.Toggle("Seamless (tileable)", settings.seamless);
            settings.useAlphaAsHeight = EditorGUILayout.Toggle("Use Alpha as Height", settings.useAlphaAsHeight);

            bool changed = EditorGUI.EndChangeCheck();

            if (sourceTexture == null)
            {
                EditorGUILayout.HelpBox("Pick a source texture to preview a normal map.", MessageType.Info);
                return;
            }

            if (changed || resultTexture == null)
            {
                RebuildResult();
            }

            EditorGUILayout.Space(10);

            using (new EditorGUI.DisabledScope(resultTexture == null))
            {
                if (GUILayout.Button("Save Normal Map", GUILayout.Height(30)))
                {
                    Save();
                }
            }

            EditorGUILayout.Space(10);

            if (resultTexture != null)
            {
                Rect previewRect = GUILayoutUtility.GetAspectRect((float)resultTexture.width / resultTexture.height, GUILayout.MaxWidth(512));
                GUI.DrawTexture(previewRect, resultTexture, ScaleMode.ScaleToFit, false);
            }
        }

        public void SetSourceTexture(Texture2D texture)
        {
            sourceTexture = texture;
            Repaint();
        }

        private void EnsureMaterials()
        {
            if (normalShader == null)
            {
                normalShader = Shader.Find("Hidden/NormalMapGenerator/Preview");
            }
            if (blurShader == null)
            {
                blurShader = Shader.Find("Hidden/NormalMapGenerator/BlurSharp");
            }
            if (normalMaterial == null)
            {
                normalMaterial = new Material(normalShader) { hideFlags = HideFlags.HideAndDontSave };
            }
            if (blurMaterial == null)
            {
                blurMaterial = new Material(blurShader) { hideFlags = HideFlags.HideAndDontSave };
            }
        }

        private void EnsureRawSourceTexture()
        {
            string assetPath = AssetDatabase.GetAssetPath(sourceTexture);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            if (rawSourceTexture != null && rawSourcePath == assetPath)
            {
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(assetPath);

            if (rawSourceTexture != null)
            {
                DestroyImmediate(rawSourceTexture);
            }

            // linear:true -> no implicit sRGB->linear conversion when this texture is sampled.
            rawSourceTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false, true)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Point,
            };
            rawSourceTexture.LoadImage(fileBytes);
            rawSourcePath = assetPath;
        }

        private void EnsureRenderTextures(int width, int height)
        {
            if (rtNormal != null && (rtNormal.width != width || rtNormal.height != height))
            {
                ReleaseRT(ref rtNormal);
                ReleaseRT(ref rtBlurA);
                ReleaseRT(ref rtBlurB);
            }

            if (rtNormal == null)
            {
                rtNormal = CreateRT(width, height);
                rtBlurA = CreateRT(width, height);
                rtBlurB = CreateRT(width, height);
            }
        }

        private static RenderTexture CreateRT(int width, int height)
        {
            return new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat,
            };
        }

        private void RebuildResult()
        {
            EnsureMaterials();
            EnsureRawSourceTexture();
            if (rawSourceTexture == null)
            {
                return;
            }

            EnsureRenderTextures(rawSourceTexture.width, rawSourceTexture.height);

            normalMaterial.SetFloat("_Strength", settings.strength);
            normalMaterial.SetFloat("_Level", settings.level);
            normalMaterial.SetFloat("_FilterType", (float)settings.filterType);
            normalMaterial.SetFloat("_InvertHeight", settings.invertHeight ? 1f : 0f);
            normalMaterial.SetFloat("_InvertRed", settings.invertRed ? 1f : 0f);
            normalMaterial.SetFloat("_InvertGreen", settings.invertGreen ? 1f : 0f);
            normalMaterial.SetFloat("_Seamless", settings.seamless ? 1f : 0f);
            normalMaterial.SetFloat("_UseAlpha", settings.useAlphaAsHeight ? 1f : 0f);

            Graphics.Blit(rawSourceTexture, rtNormal, normalMaterial);

            RenderTexture finalRT = rtNormal;

            if (!Mathf.Approximately(settings.blurSharp, 0f))
            {
                blurMaterial.SetFloat("_Seamless", settings.seamless ? 1f : 0f);

                // Vertical pass first, then horizontal - matches the reference tool's pass order.
                blurMaterial.SetFloat("_Axis", 1f);
                blurMaterial.SetFloat("_Amount", settings.blurSharp / rtNormal.height / 5f);
                Graphics.Blit(rtNormal, rtBlurA, blurMaterial);

                blurMaterial.SetFloat("_Axis", 0f);
                blurMaterial.SetFloat("_Amount", settings.blurSharp / rtNormal.width / 5f);
                Graphics.Blit(rtBlurA, rtBlurB, blurMaterial);

                finalRT = rtBlurB;
            }

            ReadBackResult(finalRT);
        }

        private void ReadBackResult(RenderTexture rt)
        {
            if (resultTexture == null || resultTexture.width != rt.width || resultTexture.height != rt.height)
            {
                if (resultTexture != null)
                {
                    DestroyImmediate(resultTexture);
                }
                resultTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };
            }

            RenderTexture previousActive = RenderTexture.active;
            RenderTexture.active = rt;
            resultTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            resultTexture.Apply();
            RenderTexture.active = previousActive;
        }

        private void Save()
        {
            string sourcePath = AssetDatabase.GetAssetPath(sourceTexture);
            if (string.IsNullOrEmpty(sourcePath))
            {
                EditorUtility.DisplayDialog("Normal Map Generator", "Source texture is not a project asset.", "OK");
                return;
            }

            RebuildResult();
            if (resultTexture == null)
            {
                return;
            }

            string directory = Path.GetDirectoryName(sourcePath);
            string fileName = Path.GetFileNameWithoutExtension(sourcePath) + "_Normal.png";
            string targetPath = Path.Combine(directory, fileName).Replace('\\', '/');

            if (File.Exists(targetPath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Normal Map Generator",
                    $"File already exists:\n{targetPath}\n\nOverwrite?",
                    "Overwrite", "Cancel");
                if (!overwrite)
                {
                    return;
                }
            }

            byte[] png = resultTexture.EncodeToPNG();
            File.WriteAllBytes(targetPath, png);

            AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(targetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.convertToNormalmap = false;
                importer.SaveAndReimport();
            }

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath));
            Debug.Log($"Normal map saved to {targetPath}");
        }
    }
}
