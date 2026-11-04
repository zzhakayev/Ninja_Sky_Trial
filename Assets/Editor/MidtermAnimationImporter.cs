using UnityEditor;

public class MidtermAnimationImporter : AssetPostprocessor
{
    void OnPreprocessAnimation()
    {
        if (!assetPath.StartsWith("Assets/Animations/")) return;
        bool idle = assetPath.EndsWith("Breathing Idle.fbx");
        bool run = assetPath.EndsWith("Running.fbx");
        bool jump = assetPath.EndsWith("Jumping.fbx");
        if (!idle && !run && !jump) return;
        ModelImporter importer = assetImporter as ModelImporter;
        if (importer == null) return;
        ModelImporterClipAnimation[] clips = importer.defaultClipAnimations;
        if (clips == null || clips.Length == 0) return;
        foreach (ModelImporterClipAnimation clip in clips) clip.loopTime = idle || run;
        importer.clipAnimations = clips;
    }
}
