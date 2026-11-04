using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NinjaMidtermRepair
{
    [MenuItem("Tools/Ninja Midterm/Fix All 3 Levels")]
    public static void FixAllLevels()
    {
        GameObject ninjaAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Ninja.fbx");
        RuntimeAnimatorController ninjaController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/NInjaAnimator.controller");
        if (ninjaAsset == null || ninjaController == null)
        {
            EditorUtility.DisplayDialog("Ninja Midterm", "Ninja.fbx or NInjaAnimator.controller could not be found. Check Assets/Models and Assets/Animations.", "OK");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        int fixedCount = 0;
        string[] levels = { "Level1", "Level2", "Level3" };
        foreach (string name in levels)
        {
            string path = "Assets/Scenes/" + name + ".unity";
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError("Could not find Player in " + path);
                continue;
            }
            MidtermLevel level = player.GetComponent<MidtermLevel>();
            if (level == null) level = player.AddComponent<MidtermLevel>();
            level.ninjaPrefab = null;
            level.ninjaController = ninjaController;
            Transform existing = player.transform.Find("Ninja Character");
            GameObject model;
            if (existing != null)
            {
                model = existing.gameObject;
            }
            else
            {
                model = PrefabUtility.InstantiatePrefab(ninjaAsset, scene) as GameObject;
                if (model == null)
                {
                    Debug.LogError("Failed to instantiate Ninja in " + path);
                    continue;
                }
                model.transform.SetParent(player.transform, false);
                model.name = "Ninja Character";
            }
            model.transform.localPosition = new Vector3(0f, -0.65f, 0f);
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            foreach (Collider col in model.GetComponentsInChildren<Collider>()) col.enabled = false;
            Animator animator = model.GetComponent<Animator>();
            if (animator == null) animator = model.AddComponent<Animator>();
            animator.runtimeAnimatorController = ninjaController;
            animator.applyRootMotion = false;
            Renderer ballRenderer = player.GetComponent<Renderer>();
            if (ballRenderer != null) ballRenderer.enabled = false;
            Rigidbody rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            EditorUtility.SetDirty(level);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            fixedCount++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Ninja Midterm: repaired " + fixedCount + " levels. Open Main menu, then press Play.");
        EditorUtility.DisplayDialog("Ninja Midterm", "Repaired " + fixedCount + " of 3 levels. Now open Assets/Scenes/Main menu and press Play.", "OK");
    }
}
