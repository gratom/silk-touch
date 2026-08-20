#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GameStarter : MonoBehaviour
{
    [MenuItem("Game/Start")]
    private static void StartGameFromSpecificScene()
    {
        string scenePath = "Assets/Scenes/boot.unity";
        EditorSceneManager.OpenScene(scenePath);
        EditorApplication.isPlaying = true;
    }
}
#endif