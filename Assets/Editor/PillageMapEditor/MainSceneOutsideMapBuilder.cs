#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainSceneOutsideMapBuilder
{
    private const string MainScenePath = "Assets/Scenes/MainScene.unity";

    [MenuItem("Pillage/Map Editor/Rebuild MainScene Outside Map")]
    public static void RebuildMainSceneOutsideMap()
    {
        Scene scene = File.Exists(MainScenePath)
            ? EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single)
            : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        SketchOutsideTransition transition = Object.FindAnyObjectByType<SketchOutsideTransition>();
        if (transition == null)
        {
            GameObject root = new GameObject("MainSceneOutsideMap");
            transition = root.AddComponent<SketchOutsideTransition>();
            root.AddComponent<MainSceneOutsideMapBootstrap>();
            root.AddComponent<OutsideMapRoot>();
        }
        else
        {
            if (transition.GetComponent<MainSceneOutsideMapBootstrap>() == null)
            {
                transition.gameObject.AddComponent<MainSceneOutsideMapBootstrap>();
            }

            if (transition.GetComponent<OutsideMapRoot>() == null)
            {
                transition.gameObject.AddComponent<OutsideMapRoot>();
            }
        }

        if (!Application.isBatchMode
            && transition.transform.Find("GeneratedSketchOutsideContent") != null
            && !EditorUtility.DisplayDialog(
                "Rebuild MainScene Outside Map",
                "This will replace the existing baked outside map hierarchy in MainScene.",
                "Rebuild",
                "Cancel"))
        {
            return;
        }

        transition.RebuildMainSceneOutsideMapForEditor();

        if (!File.Exists(MainScenePath))
        {
            EditorSceneManager.SaveScene(scene, MainScenePath);
        }
        else
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        Selection.activeObject = transition;
        Debug.Log($"Rebuilt MainScene outside map at {MainScenePath}.", transition);
    }
}
#endif
