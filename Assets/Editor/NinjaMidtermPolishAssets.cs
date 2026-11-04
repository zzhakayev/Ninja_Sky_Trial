using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NinjaMidtermPolishAssets
{
    static NinjaMidtermPolishAssets()
    {
        EditorApplication.delayCall += CreatePlatformClip;
    }

    [MenuItem("Tools/Ninja Midterm/Create Platform Animation Clip")]
    public static void CreatePlatformClip()
    {
        const string path = "Assets/Resources/PlatformMotion.anim";
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null) return;
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AnimationClip clip = new AnimationClip();
        clip.name = "PlatformMotion";
        clip.legacy = true;
        clip.wrapMode = WrapMode.Loop;
        AnimationCurve x = new AnimationCurve(
            new Keyframe(0f, -1f),
            new Keyframe(1f, 0.5f),
            new Keyframe(3f, -2.5f),
            new Keyframe(4f, -1f));
        clip.SetCurve("", typeof(Transform), "localPosition.x", x);
        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Ninja Midterm: created Assets/Resources/PlatformMotion.anim");
    }
}
