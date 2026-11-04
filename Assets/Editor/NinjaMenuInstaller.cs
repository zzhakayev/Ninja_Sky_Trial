using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class NinjaMenuInstaller
{
    [MenuItem("Tools/Ninja Midterm/Upgrade Main Menu")]
    private static void Upgrade()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Ninja Menu", "Stop Play mode before upgrading the menu.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        const string path = "Assets/Scenes/Main menu.unity";
        Scene menuScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        int hiddenCanvases = 0;
        foreach (GameObject root in menuScene.GetRootGameObjects())
        {
            foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
            {
                if (canvas.gameObject.activeSelf)
                {
                    canvas.gameObject.SetActive(false);
                    hiddenCanvases++;
                }
            }
        }

        GameObject host = null;
        foreach (GameObject root in menuScene.GetRootGameObjects())
        {
            if (root.name == "Ninja Sky Trial - Modern Menu")
            {
                host = root;
                break;
            }
        }
        if (host == null) host = new GameObject("Ninja Sky Trial - Modern Menu");
        if (host.GetComponent<NinjaModernMenu>() == null) host.AddComponent<NinjaModernMenu>();
        host.SetActive(true);

        EditorSceneManager.MarkSceneDirty(menuScene);
        EditorSceneManager.SaveScene(menuScene);
        Selection.activeGameObject = host;
        Debug.Log("Ninja Midterm: main menu upgraded. Disabled " + hiddenCanvases + " legacy canvases. Press Play to test.");
        EditorUtility.DisplayDialog("Ninja Menu", "Main menu upgraded! Open Game and press Play to test Settings and How to Play.", "OK");
    }
}
