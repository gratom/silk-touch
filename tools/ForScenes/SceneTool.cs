//developer -> gratomov@gmail.com

using System.IO;
using UnityEngine.SceneManagement;

namespace Tools
{
    /// <summary>
    /// Additional tools for working with scenes
    /// </summary>
    public static class SceneTool
    {
        /// <summary>
        /// Return all scenes names in build
        /// </summary>
        public static string[] GetScenesNamesInBuild()
        {
            int sceneNumber = SceneManager.sceneCountInBuildSettings;
            string[] arrayOfNames = new string[sceneNumber];
            for (int i = 0; i < sceneNumber; i++)
            {
                arrayOfNames[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }
            return arrayOfNames;
        }

        /// <summary>
        /// Return all scenes infos in build
        /// </summary>
        public static SceneInfo[] GetSceneInfosInBuild()
        {
            int sceneNumber = SceneManager.sceneCountInBuildSettings;
            SceneInfo[] arrayOfSceneInfos = new SceneInfo[sceneNumber];
            for (int i = 0; i < sceneNumber; i++)
            {
                arrayOfSceneInfos[i] = new SceneInfo(
                    Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)),
                    SceneUtility.GetScenePathByBuildIndex(i),
                    i,
                    CheckSceneExist(SceneUtility.GetScenePathByBuildIndex(i)));
            }
            return arrayOfSceneInfos;
        }

        /// <summary>
        /// checks if the scene file exists on disk, if exists - returns true otherwise - false
        /// </summary>
        /// <param name="path">scene file path</param>
        public static bool CheckSceneExist(string path)
        {
            return File.Exists(path);
        }
    }

    /// <summary>
    /// Structure for storing basic scene information
    /// </summary>
    public struct SceneInfo
    {
        /// <summary>
        /// Scene name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Full path of scene file
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Scene build index
        /// </summary>
        public int BuildIndex { get; private set; }

        /// <summary>
        /// Is scene file still exist on disk
        /// </summary>
        public bool Existing { get; private set; }

        public SceneInfo(string name, string path, int buildIndex, bool existing)
        {
            Name = name;
            Path = path;
            BuildIndex = buildIndex;
            Existing = existing;
        }
    }
}