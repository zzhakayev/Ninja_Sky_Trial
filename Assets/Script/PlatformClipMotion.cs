using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlatformClipMotion : MonoBehaviour
{
    public float distance = 1.5f;
    public float duration = 4f;

    void Start()
    {
        Animation motion = GetComponent<Animation>();
        motion.animatePhysics = true;
        AnimationClip clip = Resources.Load<AnimationClip>("PlatformMotion");
        if (clip == null)
        {
            Vector3 origin = transform.localPosition;
            clip = new AnimationClip();
            clip.legacy = true;
            clip.wrapMode = WrapMode.Loop;
            float cycle = Mathf.Max(1f, duration);
            AnimationCurve x = new AnimationCurve(
                new Keyframe(0f, origin.x),
                new Keyframe(cycle * 0.25f, origin.x + distance),
                new Keyframe(cycle * 0.75f, origin.x - distance),
                new Keyframe(cycle, origin.x));
            clip.SetCurve("", typeof(Transform), "localPosition.x", x);
        }
        motion.AddClip(clip, "PlatformMotion");
        motion.clip = clip;
        motion.Play("PlatformMotion");
    }
}
